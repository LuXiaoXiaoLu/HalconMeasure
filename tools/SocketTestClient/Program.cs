using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketTestClient
{
    /// <summary>
    /// PLC 模拟客户端：主动连接上位机的 Socket 服务端，打印收到的每一段数据。
    /// <para>
    /// 默认连 127.0.0.1:5000，断线后每 2 秒自动重连，方便反复测试。
    /// 用法：SocketTestClient.exe [ip] [port]
    /// </para>
    /// <remarks>
    /// 为什么要同时打印 TEXT 和 HEX：
    /// TXT 看内容对不对，HEX 看有没有隐藏的换行符/结束符、以及粘包时字节是怎么拼起来的。
    /// 帧边界问题光看文本是看不出来的（"\r\n" 在控制台上看不见）。
    /// </remarks>
    /// </summary>
    internal class Program
    {
        /// <summary>故障注入用的随机源。</summary>
        private static readonly Random Rng = new Random();

        /// <summary>是否开启故障注入（--fault 参数）。</summary>
        private static bool faultMode;

        private static void Main(string[] args)
        {
            string ip = args.Length > 0 ? args[0] : "127.0.0.1";
            int port = 5000;
            if (args.Length > 1 && !int.TryParse(args[1], out port))
            {
                Console.WriteLine("Invalid port: " + args[1]);
                return;
            }

            faultMode = args.Length > 2 && string.Equals(args[2], "--fault", StringComparison.OrdinalIgnoreCase);

            Console.WriteLine("=== Socket Test Client ===");
            Console.WriteLine("Target: {0}:{1}", ip, port);
            Console.WriteLine("ACK mode: {0}", faultMode
                ? "FAULT INJECTION (60% ok / 15% drop / 15% wrong seq / 10% bad format)"
                : "normal (always reply correctly)");
            Console.WriteLine("Press Ctrl+C to quit.");
            Console.WriteLine();

            int round = 0;
            while (true)
            {
                round++;
                try
                {
                    using (var client = new TcpClient())
                    {
                        Console.WriteLine("[{0}] (#{1}) Connecting ...", Now(), round);
                        client.Connect(ip, port);
                        Console.WriteLine("[{0}] Connected. Waiting for data ...", Now());
                        Console.WriteLine(new string('-', 64));

                        NetworkStream stream = client.GetStream();
                        var readBuf = new byte[4096];
                        var pending = new StringBuilder();
                        int totalBytes = 0;
                        int readCount = 0;
                        int frameCount = 0;

                        while (true)
                        {
                            int n = stream.Read(readBuf, 0, readBuf.Length);
                            if (n == 0)
                            {
                                Console.WriteLine("[{0}] Remote closed. Total: {1} bytes / {2} reads / {3} frames.",
                                    Now(), totalBytes, readCount, frameCount);
                                break;
                            }

                            totalBytes += n;
                            readCount++;

                            string chunk = Encoding.ASCII.GetString(readBuf, 0, n);
                            pending.Append(chunk);

                            Console.WriteLine("[{0}] read#{1}  {2} byte(s)   RAW: {3}",
                                Now(), readCount, n, ShowInvisible(chunk));

                            frameCount = DrainFrames(pending, frameCount, stream);

                            if (pending.Length > 8192)
                            {
                                Console.WriteLine("        !! pending overflow ({0} bytes), discarding", pending.Length);
                                pending.Clear();
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("[{0}] SocketException: {1} (code {2})", Now(), ex.Message, ex.SocketErrorCode);
                    Console.WriteLine("        Is the server running and listening? Is another client holding the only connection?");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[{0}] {1}: {2}", Now(), ex.GetType().Name, ex.Message);
                }

                Console.WriteLine("[{0}] Reconnecting in 2s ...", Now());
                Console.WriteLine();
                Thread.Sleep(2000);
            }
        }

        private static string Now()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff");
        }

        /// <summary>
        /// 把不可见字符替换成可见标记，否则 \r\n 在控制台上完全看不出来，
        /// 排查帧边界时会被误导。
        /// </summary>
        private static string ShowInvisible(string s)
        {
            if (string.IsNullOrEmpty(s)) return "<empty>";
            return s.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0");
        }

        /// <summary>
        /// 从累积缓冲区里切出所有完整帧（"###" ... "***"），打印后把已消费的部分移除。
        /// <para>
        /// 必须基于累积缓冲区解析，不能只看当前这一次 read 的内容 ——
        /// 一个帧很可能被 TCP 拆到两次 read 里（头在第一次、尾在第二次）。
        /// </para>
        /// <para>
        /// 每解析出一帧，就用帧里的第一个字段（seq）回一个 ACK 帧 "###{seq}***"。
        /// </para>
        /// </summary>
        private static int DrainFrames(StringBuilder pending, int frameCount, NetworkStream stream)
        {
            const string frameStart = "###";
            const string frameEnd = "***";

            while (true)
            {
                string all = pending.ToString();

                int start = all.IndexOf(frameStart, StringComparison.Ordinal);
                if (start < 0)
                {
                    // 帧头还没到。保留末尾 (frameStart.Length - 1) 个字符，
                    // 防止 "##" 和 "#" 被拆在两次 read 里导致漏掉帧头。
                    int keep = frameStart.Length - 1;
                    if (all.Length > keep) pending.Remove(0, all.Length - keep);
                    return frameCount;
                }

                int end = all.IndexOf(frameEnd, start + frameStart.Length, StringComparison.Ordinal);
                if (end < 0)
                {
                    // 帧尾还没到：把头之前的杂数据去掉，保留 "###..." 等下一次 read
                    if (start > 0) pending.Remove(0, start);
                    return frameCount;
                }

                string body = all.Substring(start + frameStart.Length, end - start - frameStart.Length);
                frameCount++;

                Console.WriteLine("        >>> FRAME #{0}: {1}", frameCount, body);

                string[] fields = body.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("            fields({0}): {1}", fields.Length, string.Join(" | ", fields));

                // seq 是帧内容的第一个字段
                if (fields.Length > 0) SendAck(stream, fields[0]);

                pending.Remove(0, end + frameEnd.Length);
            }
        }

        /// <summary>
        /// 回一个 ACK 帧："###{seq}***"，格式与上位机的帧保持一致。
        /// <para>
        /// 正常模式总是正确回复；--fault 模式下按概率注入故障，用来验证上位机的 ACK 校验：
        /// 60% 正常 / 15% 不回 / 15% 序号错 / 10% 格式坏。
        /// </para>
        /// </summary>
        private static void SendAck(NetworkStream stream, string seq)
        {
            if (!faultMode)
            {
                WriteFrame(stream, "###" + seq + "***", "ACK");
                return;
            }

            int dice = Rng.Next(100);

            if (dice < 60)
            {
                WriteFrame(stream, "###" + seq + "***", "ACK (normal)");
            }
            else if (dice < 75)
            {
                Console.WriteLine("        [FAULT] drop ACK on purpose (simulate lost packet)");
            }
            else if (dice < 90)
            {
                int seqNo;
                int wrong = int.TryParse(seq, out seqNo) ? seqNo + 1 : 99999;
                WriteFrame(stream, "###" + wrong + "***", "[FAULT] ACK with WRONG seq");
            }
            else
            {
                WriteFrame(stream, "##" + seq + "**", "[FAULT] ACK with BAD format");
            }
        }

        /// <summary>写一帧并打印。失败只提示不抛，让外层 Read 自然暴露断线。</summary>
        private static void WriteFrame(NetworkStream stream, string text, string label)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                Console.WriteLine("        <-- {0}: {1}", label, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("        <-- {0} send failed: {1}", label, ex.Message);
            }
        }
    }
}
