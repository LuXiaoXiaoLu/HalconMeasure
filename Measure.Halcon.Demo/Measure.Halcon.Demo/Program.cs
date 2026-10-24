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
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 锚定到 exe 同级 logs\ 目录
            string logPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log");

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
