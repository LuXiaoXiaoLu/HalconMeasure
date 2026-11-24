using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Storage
{
    /// <summary>
    /// SQL 脚本仓库：管理 Storage\sql 下的迁移脚本。
    /// 职责：扫描目录建立「版本号 → 文件名」索引，并按文件名执行脚本。
    /// </summary>
    public static class SqlScriptRepository
    {
        private static SortedDictionary<int, string> _scriptMap;

        /// <summary>版本号 → 脚本文件名，按版本号升序。</summary>
        public static SortedDictionary<int, string> ScriptMap
        {
            get
            {
                if (_scriptMap == null)
                    _scriptMap = LoadScriptMap();
                return _scriptMap;
            }
        }

        /// <summary>扫描脚本目录，建立版本号到文件名的索引。</summary>
        private static SortedDictionary<int, string> LoadScriptMap()
        {
            string dir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Storage", "sql");

            var dict = new SortedDictionary<int, string>();

            foreach (string path in Directory.GetFiles(dir, "*.sql"))
            {
                string name = Path.GetFileName(path);
                Match m = Regex.Match(name, @"^(\d{3})_");       // 取前三位数字
                if (!m.Success) continue;                        // 命名不规范的忽略

                int version = int.Parse(m.Groups[1].Value);

                if (dict.ContainsKey(version))
                    throw new InvalidOperationException(
                        $"脚本版本号重复：{version:D3}（{dict[version]} 和 {name}）");

                dict.Add(version, name);
            }
            return dict;
        }

        /// <summary>执行指定文件名的脚本。</summary>
        public static async Task<OperationResult> ExecuteAsync(string scriptFileName)
        {
            string fullPath = Path.Combine(
               AppDomain.CurrentDomain.BaseDirectory,
               "Storage", "sql", scriptFileName);
            if (!File.Exists(fullPath)) {
                return OperationResult.CreateFailResult("找不到脚本：" + fullPath);
               
            }
            string sql = File.ReadAllText(fullPath, Encoding.UTF8);
            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.ConnStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                       await cmd.ExecuteNonQueryAsync();
                    return OperationResult.CreateSuccessResult("执行成功："+ scriptFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailResult("执行失败：" + scriptFileName, ex);
                
            }
        }


    }
}
