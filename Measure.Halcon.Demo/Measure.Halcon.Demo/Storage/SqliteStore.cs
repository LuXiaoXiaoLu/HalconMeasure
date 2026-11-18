using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Storage
{
    internal  class SqliteStore
    {
       

       

        /// <summary>数据库文件校验 Program.DataRoot</summary>
        public static bool InitialSqlite()
        {
            try
            {
                using (var conn = DbConfig.CreateConnection())
                {
                    // ② 打开成功 → 立刻验证活性 + 记日志（排查"连错库"的第一手证据）
                    string ver;
                    using (var cmd = new SQLiteCommand("SELECT sqlite_version()", conn))
                        ver = Convert.ToString(cmd.ExecuteScalar());

                    Log.Information("数据库已打开：{Path}（SQLite {Ver}）", DbConfig.DbPath, ver);

                    // ③ 查 SchemaVersion 表是否存在
                    bool hasSchemaVersion;
                    using (var cmd = new SQLiteCommand(
                        "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='SchemaVersion'", conn))
                        hasSchemaVersion = Convert.ToInt32(cmd.ExecuteScalar()) > 0;

                    Log.Information("SchemaVersion 表存在：{Has}", hasSchemaVersion);
                }
                return true;
            }
            catch (SQLiteException ex)
            {
                if (ex.ResultCode == SQLiteErrorCode.CantOpen)      // 14
                    Log.Error("无法打开数据库，请检查目录是否存在且可写：{Path}", DbConfig.DbPath);
                else if (ex.ResultCode == SQLiteErrorCode.NotADb)   // 26
                    Log.Error("文件不是有效的 SQLite 数据库：{Path}", DbConfig.DbPath);
                else
                    Log.Error(ex, "数据库打开失败：{Path}", DbConfig.DbPath);
                return false;
            }
        }
    }
}
