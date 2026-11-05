using IniParser;        // FileIniDataParser
using IniParser.Model;  // IniData / SectionData
using System;
using System.IO;

namespace Measure.Halcon.Demo.Helper
{
    /// <summary>
    /// 单个 ini 文件的薄封装：只负责「路径、磁盘读写、类型转换」三件事。
    /// <para>
    /// 分层位置：IniStore（文件层） ← AppSettings / DeviceSettings（业务层）。<br/>
    /// 业务层负责「某个 key 是什么意思、默认值多少」，本类只负责「把这些键值安全地搬进搬出磁盘」。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 三条约定：<br/>
    /// ① 本类不记日志 —— 失败统一返回 <see cref="OperationResult"/>，由调用方（边界层）决定记 Warn 还是 Error。<br/>
    /// ② Get 系列永不抛异常、永不返回 null —— 「键不存在」是高频正常情况，用默认值兜底而不是当失败处理。<br/>
    /// ③ 仅限 UI 线程调用。内部不加锁，因为配置只在按钮/下拉框事件里读写。
    /// </remarks>
    internal class IniStore
    {
        /// <summary>ini 文件的绝对路径，构造后不再变化。</summary>
        private readonly string _filePath;

        /// <summary>ini 解析器。本身无状态，整个生命周期复用同一个实例即可。</summary>
        private readonly FileIniDataParser _parser = new FileIniDataParser();

        /// <summary>
        /// 内存中的整份配置（section → key → value）。
        /// Load 之前是空表，此时所有 Get 返回各自的默认值。
        /// </summary>
        private IniData _data = new IniData();

        /// <summary>
        /// 构造函数。<b>不访问磁盘</b>，只记住路径。
        /// </summary>
        /// <param name="filePath">
        /// ini 文件的完整路径。<b>必须传绝对路径</b>；相对路径会依赖「当前工作目录」，
        /// 而工作目录可能被 OpenFileDialog 之类的操作改掉，导致文件写到意想不到的地方。
        /// </param>
        public IniStore(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>ini 文件的绝对路径，供日志和排错时回显。</summary>
        public string FilePath { get { return _filePath; } }

        /// <summary>
        /// 把磁盘上的 ini 读进内存，<b>覆盖</b>当前内存数据。
        /// </summary>
        /// <returns>
        /// 文件不存在 → 成功（首次运行是正常状态，内存给空表，后续 Get 全部走默认值）；<br/>
        /// 文件损坏/无权限 → 失败（带异常），同时内存重置为空表，让程序以默认值继续运行而不是崩掉。
        /// </returns>
        public OperationResult Load()
        {
            try
            {
                // 文件不存在算成功：装新机器时第一次启动就是这种情况，
                // 如果算失败，每台机器开机都会刷一条红色错误日志，操作员会以为程序坏了。
                if (!File.Exists(_filePath))
                {
                    _data = new IniData();
                    return OperationResult.CreateSuccessResult("配置文件不存在，使用默认值：" + _filePath);
                }

                _data = _parser.ReadFile(_filePath);
                return OperationResult.CreateSuccessResult("读取配置成功：" + _filePath);
            }
            catch (Exception ex)
            {
                // 读坏了也降级为「全默认」，不能因为配置文件有问题就让程序起不来。
                // 这里带 ex 返回 → 日志会被记成 Error（技术故障），而不是 Warn（业务失败）。
                _data = new IniData();
                return OperationResult.CreateFailResult("读取配置失败：" + _filePath, ex);
            }
        }

        /// <summary>
        /// 把内存数据写回磁盘，<b>整文件覆盖</b>（ini-parser 不支持只改一行）。
        /// </summary>
        /// <returns>成功 / 失败（磁盘满、无写权限、文件被其它进程占用等）。</returns>
        /// <remarks>
        /// 覆盖前会自动创建父目录：ini-parser 的 WriteFile 不会建目录，
        /// 目录不存在会抛 DirectoryNotFoundException，而这个异常很容易被误判成「权限问题」。
        /// </remarks>
        public OperationResult Save()
        {
            try
            {
                // 对已存在的目录调用 CreateDirectory 是幂等的，不会报错。
                string dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                _parser.WriteFile(_filePath, _data);
                return OperationResult.CreateSuccessResult("保存配置成功：" + _filePath);
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailResult("保存配置失败：" + _filePath, ex);
            }
        }

        /// <summary>
        /// 读取字符串。四种情况都返回 <paramref name="defaultValue"/>：
        /// section 不存在、key 不存在、值为空串、ini 尚未 Load。
        /// </summary>
        /// <param name="section">节名，<b>不带方括号</b>，如 "Camera1" 对应 [Camera1]。</param>
        /// <param name="key">键名，<b>不带等号</b>，如 "Ip" 对应 Ip = 192.168.1.10。</param>
        /// <param name="defaultValue">读不到时返回的兜底值。</param>
        /// <returns>读到的值；读不到则返回 defaultValue。永不为 null。</returns>
        public string GetString(string section, string key, string defaultValue = "")
        {
            // SectionDataCollection 的索引器返回的就是「键集合」（KeyDataCollection），
            // 所以这里直接拿到 keys，不需要再 .Keys 转一层。
            if (!_data.Sections.ContainsSection(section)) return defaultValue;

            KeyDataCollection keys = _data.Sections[section];
            if (keys == null || !keys.ContainsKey(key)) return defaultValue;

            string value = keys[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>读取整数。文本不是合法整数时返回 <paramref name="defaultValue"/>，不抛异常。</summary>
        /// <param name="section">节名，同 <see cref="GetString"/>。</param>
        /// <param name="key">键名，同 <see cref="GetString"/>。</param>
        /// <param name="defaultValue">解析失败或读不到时的兜底值。</param>
        public int GetInt(string section, string key, int defaultValue = 0)
        {
            int v;
            return int.TryParse(GetString(section, key, ""), out v) ? v : defaultValue;
        }

        /// <summary>读取小数。文本不是合法数字时返回 <paramref name="defaultValue"/>，不抛异常。</summary>
        /// <param name="section">节名，同 <see cref="GetString"/>。</param>
        /// <param name="key">键名，同 <see cref="GetString"/>。</param>
        /// <param name="defaultValue">解析失败或读不到时的兜底值。</param>
        public double GetDouble(string section, string key, double defaultValue = 0)
        {
            double v;
            return double.TryParse(GetString(section, key, ""), out v) ? v : defaultValue;
        }

        /// <summary>
        /// 读取布尔。只认 "true"/"false"（大小写不敏感）；写成 1/0 会被当成读不到。
        /// </summary>
        /// <param name="section">节名，同 <see cref="GetString"/>。</param>
        /// <param name="key">键名，同 <see cref="GetString"/>。</param>
        /// <param name="defaultValue">解析失败或读不到时的兜底值。</param>
        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            bool v;
            return bool.TryParse(GetString(section, key, ""), out v) ? v : defaultValue;
        }

        /// <summary>
        /// 写入一个字符串值，<b>只改内存，不落盘</b>。需要持久化时再调 <see cref="Save"/>。
        /// </summary>
        /// <param name="section">节名；不存在会自动创建。</param>
        /// <param name="key">键名；不存在会自动创建。</param>
        /// <param name="value">要写入的值；传 null 会被写成空串（ini 里不能写 null）。</param>
        public void Set(string section, string key, string value)
        {
            if (!_data.Sections.ContainsSection(section))
                _data.Sections.AddSection(section);

            // 索引器的 setter 在 key 不存在时会自动新增，和 ShapeModelHelper 里的写法一致
            _data.Sections[section][key] = value ?? string.Empty;
        }

        /// <summary>写入整数。<b>只改内存</b>，需再调 <see cref="Save"/>。</summary>
        public void Set(string section, string key, int value)
        {
            Set(section, key, value.ToString());
        }

        /// <summary>写入小数。<b>只改内存</b>，需再调 <see cref="Save"/>。</summary>
        public void Set(string section, string key, double value)
        {
            Set(section, key, value.ToString());
        }

        /// <summary>写入布尔（存成 "true"/"false"）。<b>只改内存</b>，需再调 <see cref="Save"/>。</summary>
        public void Set(string section, string key, bool value)
        {
            Set(section, key, value ? "true" : "false");
        }
    }
}
