using HalconDotNet;
using Measure.Halcon.Demo.Trigger;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;


namespace Measure.Halcon.Demo.Helper
{
    internal class SocketHeler
    {

        private Socket _socket;

        private LogHelper _log;

        /// <summary>
        /// 结果消息队列
        /// </summary>
        private readonly BlockingCollection<IResultInfo> _resultQueue = new BlockingCollection<IResultInfo>(5);

        /// <summary>
        /// 触发消息队列
        /// </summary>
        private readonly BlockingCollection<TriggerInfo> _triggerQueue;

        private CancellationTokenSource _cts;

        private readonly Func<bool> _canAcceptTrigger;

        private DateTime _lastSendTime;

        public SocketHeler(Socket socket, LogHelper log, BlockingCollection<TriggerInfo> triggerQueue, Func<bool> func)
        {
            _socket = socket;
            this._log = log;
            _triggerQueue = triggerQueue;
            _canAcceptTrigger = func;
        }

        public OperationResult StartSockeServer()
        {
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, 5000));
                _socket.Listen(1);
                _cts = new CancellationTokenSource();
                var token = _cts.Token;
                Task.Factory.StartNew(
                    () => SocketPross(token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                return OperationResult.CreateSuccessResult("通信初始化成功");
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("通信初始化失败", ex);
            }


        }



        public void SendData(IResultInfo resultInfo)
        {

            _resultQueue.Add(resultInfo);
            _log.Info("待发送:" + resultInfo.ToString());

        }


        /// <summary>
        /// 上位机自动发送
        /// </summary>
        //private void SocketPross(CancellationToken token)
        //{
        //    try
        //    {
        //        while (!token.IsCancellationRequested)
        //        {
        //            Socket conn = null;
        //            try
        //            {
        //                conn = _socket.Accept();
        //                _log.Info("PLC 已连接：" + conn.RemoteEndPoint);
        //                while (!token.IsCancellationRequested)
        //                {
        //                    List<string> data = _triggerQueue.Take(token);   // 无数据就挂起；取消时抛 OperationCanceledException
        //                    string sendData = "###"  + string.Join(",", data) + "***";

        //                    int retry = 0;

        //                    while (retry < _maxRetryCount)
        //                    {
        //                        conn.Send(Encoding.ASCII.GetBytes(sendData));
        //                        _log.Info("发送数据：" + sendData);
        //                        try
        //                        {
        //                            byte[] receiveData = new byte[1024];
        //                            conn.ReceiveTimeout = 500;
        //                            int n= conn.Receive(receiveData);
        //                            if(n == 0) throw new SocketException((int)SocketError.ConnectionReset); // 对端断开
        //                            string ack = Encoding.ASCII.GetString(receiveData, 0, n);
        //                            if (!ack.StartsWith("###") || !ack.EndsWith("***")) {
        //                                retry++; continue;
        //                            }   
        //                            if (ack.Trim('#','*').Equals(data[0]))
        //                            {
        //                                _log.Info("发送成功：" + sendData);
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                _log.Warn("Ack错误：" + data[0] + "!=" + ack);
        //                                retry++;
        //                                continue;
        //                            }

        //                        }
        //                        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        //                        {

        //                            retry++;
        //                            _log.Warn("响应超时" + retry + "次");  // 超时，也重发
        //                        }
        //                        catch (SocketException)
        //                        {
        //                            throw;                                          // 真断线，交给外层重连
        //                        }


        //                    }

        //                    if (retry >= _maxRetryCount) _log.Warn("重发 3 次仍无应答");

        //                    //conn.Send(Encoding.ASCII.GetBytes(sendData));

        //                    //_log.Info("已发送 " + sendData);

        //                    //string Ack = string.Empty;
        //                    //int i = 0;
        //                    //while (!Ack.Equals(_seq.ToString()))
        //                    //{

        //                    //    try
        //                    //    {
        //                    //        //接收ASK
        //                    //        byte[] buffer = new byte[1024];
        //                    //        try
        //                    //        {
        //                    //            conn.ReceiveTimeout = 3000;
        //                    //        }
        //                    //        catch (Exception ex)
        //                    //        {
        //                    //            i++;
        //                    //            _log.Warn("通信超时：尝试重发" + i + "次");
        //                    //            _log.Result(OperationResult.CreateFailResult("ACK 超时", ex));
        //                    //            return;
        //                    //        }

        //                    //        int bufferLength = conn.Receive(buffer);
        //                    //        string ReceiveData = Encoding.ASCII.GetString(buffer, 0, bufferLength);
        //                    //        if (!ReceiveData.StartsWith("###") || !ReceiveData.EndsWith("***"))
        //                    //        {
        //                    //            _log.Warn("ACK 格式不完整，忽略：" + ReceiveData);
        //                    //            return;
        //                    //        }
        //                    //        Ack = ReceiveData.Trim('#', '*');

        //                    //        if (Ack.Equals(_seq.ToString()))
        //                    //        {
        //                    //            _log.Info("数据发送成功：" + sendData);
        //                    //            break;
        //                    //        }
        //                    //        _log.Warn("通信超时：尝试重发" + i + "次");
        //                    //        if (i > _maxRetryCount)
        //                    //        {
        //                    //            _log.Warn("通信超时：重发失败");
        //                    //            break;
        //                    //        }
        //                    //        i++;

        //                    //    }
        //                    //    catch (Exception)
        //                    //    {
        //                    //        _log.Warn("通信超时结束");
        //                    //        break;
        //                    //    }
        //                    //}
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (token.IsCancellationRequested)
        //                {
        //                    _log.Info("通信线程已停止");
        //                }
        //                else
        //                {
        //                    _log.Result(OperationResult.CreateFailResult("PLC 连接断开，等待重连", ex));
        //                }
        //            }
        //            finally
        //            {
        //                conn?.Close();

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Fail(OperationResult.CreateFailResult("通信线程已终止", ex));
        //    }
        //}


        private void SocketPross(CancellationToken token)
        {

            while (!token.IsCancellationRequested)
            {
                Socket conn = null;
                try
                {
                    conn = _socket.Accept();
                    _log.Info("PLC 已连接：" + conn.RemoteEndPoint);
                    _lastSendTime = DateTime.Now;
                    DateTime now;
                    int state = 0;

                    while (!token.IsCancellationRequested)
                    {

                        byte[] data = new byte[1024];

                        if (conn.Available > 0)
                        {
                            int n = conn.Receive(data);
                            if (n == 0) {
                                _log.Warn("PLC断开链接");
                            
                            }
                            TriggerInfo triggerInfo = TriggerInfo.CreateTriggerInfo(Encoding.ASCII.GetString(data, 0, n));
                            if (_canAcceptTrigger != null && !_canAcceptTrigger())
                            {
                                _log.Warn("引擎未运行，丢弃触发：" + triggerInfo.WorkpieceId);
                            }
                            else
                            {
                                //添加入触发消息队列
                                if (triggerInfo != null)
                                {
                                    _triggerQueue.Add(triggerInfo);
                                }
                            }
                            
                           
                            
                        }


                        //发送数据
                        if (_resultQueue.TryTake(out IResultInfo resul))
                        {
                            if (resul != null)
                            {
                                conn.Send(Encoding.ASCII.GetBytes(resul.ToString()));
                            }
                        }
                        now = DateTime.Now;
                        if ((now - _lastSendTime).TotalMilliseconds > 500)
                        {
                            
                            conn.Send(Encoding.ASCII.GetBytes("[H," + state + "]"));
                            if (state == 0)
                            {
                                state=1;
                            }
                            else
                            {
                                state = 0;
                            }
                            _lastSendTime = now;
                        }

                        Thread.Sleep(1);

                    }
                }
                catch (SocketException ex) when (
                    ex.SocketErrorCode == SocketError.ConnectionReset ||      // 10054 对端 RST
                    ex.SocketErrorCode == SocketError.ConnectionAborted ||    // 10053 本地中止
                    ex.SocketErrorCode == SocketError.Shutdown ||             // 10058 本端已 shutdown
                    ex.SocketErrorCode == SocketError.NetworkReset ||         // 10052 网络中断
                    ex.SocketErrorCode == SocketError.NotConnected)           // 10057 未连接
                {
                    _log.Warn("PLC 断开连接（" + ex.SocketErrorCode + "），等待重连");
                }
                catch (Exception ex)
                {
                    _log.Result(OperationResult.CreateFailResult("通信异常关闭", ex));
                }
                finally
                {
                    conn?.Close();
                }
            }
        }

        public void Stop()
        {
            if (_cts != null) _cts.Cancel();

            _socket.Close();          // 打断阻塞中的 Accept
        }


    }
}
