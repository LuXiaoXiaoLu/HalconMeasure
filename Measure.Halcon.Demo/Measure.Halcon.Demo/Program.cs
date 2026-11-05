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
        public static readonly string DataRoot = ResolveDataRoot();

        private static string ResolveDataRoot()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string besideExe = Path.Combine(exeDir, "data");

            // ① 部署形态：exe 旁边有 data，就用它
            if (Directory.Exists(besideExe)) return Path.GetFullPath(besideExe);

            // ② 开发形态：项目目录下有 data（bin\Debug / bin\Release 上两级 = 项目目录）
            string projectLevel = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "data"));
            if (Directory.Exists(projectLevel)) return projectLevel;

            // ③ 兜底：都没找到，就在 exe 旁建一个
            return Path.GetFullPath(besideExe);
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
            Directory.CreateDirectory(DataRoot);
            Directory.CreateDirectory(Path.Combine(DataRoot, "model"));
            Directory.CreateDirectory(Path.Combine(DataRoot, "config"));
            Directory.CreateDirectory(Path.Combine(DataRoot, "logs"));

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
