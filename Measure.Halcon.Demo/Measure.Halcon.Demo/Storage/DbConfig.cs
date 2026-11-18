using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Storage
{
    internal static class DbConfig
    {
        public static string DbPath
        {
            get { return Path.Combine(Program.DataRoot, "db", "measure.db"); }
        }

        public static SQLiteConnection CreateConnection()
        {
            string dbPath = DbPath;

            // SQLite 只建文件、不建目录；Program.cs 里没建 db，这里补上
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = dbPath,
                Version = 3,
                ForeignKeys = true,                        // 每连接必须
                JournalMode = SQLiteJournalModeEnum.Wal,   // 持久化属性，一次即永久
                SyncMode = SynchronizationModes.Normal, // WAL 下减少 fsync
                BusyTimeout = 5000,                        // 遇锁最多等 5 秒
                Pooling = true,
                FailIfMissing = false,                       // 文件不存在就新建
                DateTimeFormat = SQLiteDateFormats.ISO8601,
                DateTimeKind = DateTimeKind.Utc
            };

            var conn = new SQLiteConnection(builder.ToString());
            conn.Open();
            return conn;
        }
    }
}
