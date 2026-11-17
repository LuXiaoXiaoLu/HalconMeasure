using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Trigger
{
    /// <summary>
    /// 触发数据类
    /// </summary>
    internal class TriggerInfo
    {
        /// <summary>工件 ID</summary>
        public string WorkpieceId { get; set; }

        /// <summary>收到触发的时刻</summary>
        public DateTime ReceivedAt { get; set; }

        /// <summary>原始帧内容。</summary>
        public string Raw { get; set; }

        /// <summary>把一帧 [T,2] 解析成 TriggerInfo；格式不对返回 null（不抛异常）。</summary>
        public static TriggerInfo CreateTriggerInfo(string frame)
        {
            if (string.IsNullOrEmpty(frame)) return null;

            string s = frame.Trim();

            if (!s.StartsWith("[") || !s.EndsWith("]")) return null;

            string body = s.Substring(1, s.Length - 2);

            string[] parts = body.Split(',');
            if (parts.Length < 2) return null;

            if (!parts[0].Trim().Equals("T", StringComparison.OrdinalIgnoreCase)) return null;

            string id = parts[1].Trim();
            if (id.Length == 0) return null;

            return new TriggerInfo
            {
                WorkpieceId = id,
                ReceivedAt = DateTime.Now,
                Raw = frame
            };
        }

    }
}
