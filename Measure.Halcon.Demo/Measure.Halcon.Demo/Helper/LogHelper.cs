using Serilog;
using System;
using System.Windows.Forms;

namespace Measure.Halcon.Demo.Helper
{
    /// <summary>
    /// 日志助手：把日志写到文件（Serilog），并更新界面上的日志列表。
    /// 线程安全，可在任意线程调用（写文件走 Serilog，刷界面走 BeginInvoke 封送）。
    /// </summary>
    public class LogHelper
    {
        private readonly ListView _listView;

        /// <summary>界面上最多保留的日志条数。完整历史在 exe\logs\ 的文件里。</summary>
        private const int MaxLogItems = 500;

        // 日志级别，数值同时也是 imageList1 里的图标序号：0 = info，1 = warning，2 = error
        private const int LogInfo = 0;
        private const int LogWarn = 1;
        private const int LogError = 2;

        public LogHelper(ListView listView)
        {
            _listView = listView;
        }

        /// <summary>记一条普通信息。</summary>
        public void Info(string log) { AddLog(LogInfo, log); }

        /// <summary>记一条警告。</summary>
        public void Warn(string log) { AddLog(LogWarn, log); }

        /// <summary>记一条错误。</summary>
        public void Error(string log) { AddLog(LogError, log); }

        /// <summary>记一个失败的操作结果（自动带上完整异常栈）。</summary>
        public void Warn(OperationResult result) { AddLog(LogWarn, result); }

        /// <summary>记一个失败的操作结果（自动带上完整异常栈）。</summary>
        public void Error(OperationResult result) { AddLog(LogError, result); }

        private void AddLog(int index, string log)
        {
            // ① 先写文件。这一步线程安全、也不依赖窗体状态，
            //    所以必须放在 return 之前 —— 否则关窗那一刻的日志会被丢掉。
            WriteLogToFile(index, log);

            // ② 再更新界面
            AppendLogToUi(index, log);
        }

        private void AddLog(int index, OperationResult result)
        {
            if (result == null)
            {
                WriteLogToFile(index, "(result 为 null)");
                return;
            }

            WriteLogToFile(index, result);
            AppendLogToUi(index, result.Msg);
        }

        /// <summary>把日志写进文件。线程安全，可在任意线程调用。</summary>
        private static void WriteLogToFile(int index, OperationResult result)
        {
            // 用 {LogText} 占位符，把正文当「参数值」而不是「消息模板」：
            // 正文里若出现 {{ 会被模板语法当转义吃掉；走参数则原样保留
            switch (index)
            {
                case LogError: Log.Error(result.Error, "{LogText}", result.Msg); break;
                case LogWarn: Log.Warning(result.Error, "{LogText}", result.Msg); break;
                default: Log.Information(result.Error, "{LogText}", result.Msg); break;
            }
        }

        /// <summary>把日志写进文件。线程安全，可在任意线程调用。</summary>
        private static void WriteLogToFile(int index, string log)
        {
            // 用 {LogText} 占位符，把正文当「参数值」而不是「消息模板」：
            // 正文里若出现 {{ 会被模板语法当转义吃掉；走参数则原样保留
            switch (index)
            {
                case LogError: Log.Error("{LogText}", log); break;
                case LogWarn: Log.Warning("{LogText}", log); break;
                default: Log.Information("{LogText}", log); break;
            }
        }

        /// <summary>更新界面（含跨线程封送）。可在任意线程调用。</summary>
        private void AppendLogToUi(int index, string text)
        {
            if (_listView.IsDisposed) return;

            if (_listView.InvokeRequired)
            {
                // 后台线程：用 BeginInvoke 异步投递。
                // 若用 Invoke（同步等待），UI 被模态对话框挡住时会把后台线程一起卡死。
                try
                {
                    _listView.BeginInvoke(new Action(() => AppendLogItem(index, text)));
                }
                catch (InvalidOperationException)
                {
                    // 关窗瞬间控件句柄已销毁（ObjectDisposedException 也是它的子类），
                    // 这条日志已经落盘了，界面上不显示没关系
                }
                return;
            }

            AppendLogItem(index, text);
        }

        /// <summary>往 ListView 顶部插一条。只能在 UI 线程调用。</summary>
        private void AppendLogItem(int index, string log)
        {
            if (_listView.IsDisposed) return;

            ListViewItem item = new ListViewItem("   " + CurrentTime, index);
            item.SubItems.Add(log);
            _listView.Items.Insert(0, item);

            // 只留最近若干条，否则连续采集跑一晚上会堆出几万行，界面越来越卡
            while (_listView.Items.Count > MaxLogItems)
            {
                _listView.Items.RemoveAt(_listView.Items.Count - 1);
            }
        }

        private string CurrentTime
        {
            get { return DateTime.Now.ToString("T"); }
        }
    }
}
