using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Serilog;

namespace Measure.Halcon.Demo
{
    internal static class Program
    {
        /// <summary>
        /// 运行时数据根目录（模板 model / 配置 config / 日志 logs 的父目录）。
        /// Debug：放在项目目录下（清理 bin 不丢、Debug 与 Release 共用一份）；
        /// Release：放在 exe 旁边。
        /// </summary>
        public static readonly string DataRoot = ComputeDataRoot();

        private static string ComputeDataRoot()
        {
#if DEBUG
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "data");
#else
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
#endif
            return Path.GetFullPath(root);   // 把 ..\.. 归一化成绝对路径，方便日志里看得清
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 日志写到数据根目录的 logs\ 下（不再写进 bin\Debug）
            string logPath = Path.Combine(DataRoot, "logs", "app-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,        // 每天一个文件
                    fileSizeLimitBytes: 10 * 1024 * 1024,        // 单个文件 10MB
                    rollOnFileSizeLimit: true,                   // 超了立刻换新文件
                    retainedFileCountLimit: 20,                  // 最多留 20 个，多的删最旧
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    encoding: Encoding.UTF8,
                    flushToDiskInterval: TimeSpan.FromSeconds(1)) // 崩溃最多丢 1 秒
                .CreateLogger();

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "程序未捕获的异常");
                MessageBox.Show("程序异常退出：" + ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();   // 必须，否则缓冲区最后几行可能不落盘
            }
        }
    }
}
