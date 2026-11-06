using System;

namespace Measure.Halcon.Demo.Helper
{
    /// <summary>
    /// 程序级状态配置（对应 data\config\app.ini），存放「上次用过的痕迹」。
    /// </summary>
    internal class AppSettings
    {
        // section 名和 key 名集中在这里定义：
        // 外部只看到 LastModelName 这个属性，字符串字面量不会散落进 FormMain。
        private const string Section = "界面状态";
        private const string KeyLastModel = "LastModel";

        private readonly IniStore _store;

        /// <summary>
        /// 构造函数。<b>不访问磁盘</b>，要读文件请显式调用 <see cref="Load"/>。
        /// </summary>
        /// <param name="filePath">
        /// app.ini 的完整路径，一般是 <c>Path.Combine(Program.DataRoot, "config", "app.ini")</c>。
        /// </param>
        public AppSettings(string filePath)
        {
            _store = new IniStore(filePath);
        }

        /// <summary>
        /// 上次使用（加载成功）过的模板名。没记录过时返回空字符串。
        /// </summary>
        /// <remarks>
        /// 写入时机放在「读取模板成功之后」，而不是下拉框的 SelectedIndexChanged ——
        /// 前者表示「真的用了」，后者只是「翻看了一眼」，后者会让写盘次数变成前者十倍。
        /// </remarks>
        public string LastModelName
        {
            get { return _store.GetString(Section, KeyLastModel, ""); }
            set { _store.Set(Section, KeyLastModel, value); }
        }

        // ── 以后要加新配置项，照这个形状加一个属性就行，Load / Save 一个字都不用动 ──
        //
        // private const string KeyWindowPos = "WindowPos";
        //
        // /// <summary>主窗口位置，格式 "Left,Top,Width,Height"；未记录时为空串。</summary>
        // public string WindowPos
        // {
        //     get { return _store.GetString(Section, KeyWindowPos, ""); }
        //     set { _store.Set(Section, KeyWindowPos, value); }
        // }

        /// <summary>
        /// 从磁盘读取配置到内存。启动时调用一次即可。
        /// </summary>
        /// <returns>文件不存在也算成功（首次运行属正常状态，各属性走默认值）。</returns>
        public OperationResult Load()
        {
            return _store.Load();
        }

        /// <summary>
        /// 把内存中的配置写回磁盘（整文件覆盖）。
        /// </summary>
        /// <returns>写入结果；失败一般是权限、磁盘满、文件被占用。</returns>
        public OperationResult Save()
        {
            return _store.Save();
        }

        /// <summary>
        /// 设置上次使用的模板名并<b>立即落盘</b>，等价于「赋值 + Save」的组合，省掉调用点的两行。
        /// </summary>
        /// <param name="lastModelName">刚加载成功的模板名。</param>
        /// <returns>落盘结果；内存中的值已更新，即使写盘失败也不影响本次运行。</returns>
        public OperationResult SetLastModelName(string lastModelName)
        {
            LastModelName = lastModelName;
            return _store.Save();
        }

       
    }
}
