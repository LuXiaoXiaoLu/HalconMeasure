using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Measure.Halcon.Demo.Storage
{
    /// <summary>
    /// 连接串的唯一来源：从 App.config 的 connectionStrings 读取。
    /// 全进程只解析一次，之后复用同一个字符串，避免连接池被拆成多个。
    /// </summary>
    public static class DbConfig
    {
        private const string SettingName = "MeasureDb";

        private static string _connStr;

        /// <summary>
        /// 规范化后的连接串。第一次访问时读取并校验，之后直接返回缓存。
        /// </summary>
        public static string ConnStr
        {
            get
            {
                if (_connStr == null)
                    _connStr = Load();
                return _connStr;
            }
        }

        private static string Load()
        {
            ConnectionStringSettings setting =
                ConfigurationManager.ConnectionStrings[SettingName];

            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "App.config 中找不到 name=\"" + SettingName + "\" 的连接串配置。");
            }

            // 用 SqlConnectionStringBuilder 解析一遍：
            // 格式或参数名有问题会在这里直接抛出，不用等到第一次查库才发现。
            var builder = new SqlConnectionStringBuilder(setting.ConnectionString);
            return builder.ConnectionString;
        }

        /// <summary>
        /// 返回打码后的连接串，专供写日志用，避免密码明文落盘。
        /// </summary>
        public static string MaskedConnStr()
        {
            var builder = new SqlConnectionStringBuilder(ConnStr);
            if (!string.IsNullOrEmpty(builder.Password))
                builder.Password = "***";
            return builder.ConnectionString;
        }
    }
}
