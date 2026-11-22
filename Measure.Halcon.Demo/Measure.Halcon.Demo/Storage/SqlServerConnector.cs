using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Measure.Halcon.Demo.Storage
{
    public static class SqlServerConnector
    {
        /// 判断这个错误值得不值得再试一次
        private static bool IsTransient(int number)
        {
            switch (number)
            {
                // ── 超时 ──────────────────────────────────────────
                case -2:      // 登录/连接超时（Connect Timeout 到期）
                              // 也可能是连接池耗尽（消息里带"从池中获取"）
                    return true;

                // ── 找不到服务器 ──────────────────────────────────
                case 2:       // 找不到服务器（共享内存/命名管道连不上）
                case 26:      // 定位实例失败（SQL Browser 没启动、实例名写错）
                case 53:      // 找不到网络路径（IP/主机名错、网段不通）
                case 11001:   // 找不到主机（DNS 解析失败）
                    return true;

                // ── 端口连不上 ────────────────────────────────────
                case 10060:   // TCP 连接超时（防火墙丢包，对方无响应）
                case 10061:   // 目标主动拒绝（服务没启动，端口无人监听）★最常见
                    return true;

                // ── 连上之后又被断开 ──────────────────────────────
                case 233:     // 管道另一端已关闭（服务端主动踢、TLS 协商失败）
                case 10053:   // 本机中止已建立的连接（证书/协议错误、软件主动断）
                case 10054:   // 连接被对端重置（服务重启、进程被杀）
                    return true;

                // ── 库暂时打不开 ──────────────────────────────────
                case 4060:    // 无法打开数据库（不存在/离线/正在崩溃恢复/无权限）
                case 4064:    // 无法打开登录账号的默认数据库
                    return true;

                default:
                    return false;   // 18456 登录失败、229 权限不足、库名拼错等，等也没用
            }
        }


        public static async Task<OperationResult> ConnectWithRetry(Action<string> onProgress)
        {
            int attempts = 3;
            int intervalMs = 2000;
           
            SqlException last = null;

            for (int i = 1; i <= attempts; i++)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(DbConfig.ConnStr))
                    {
                        await conn.OpenAsync();
                    }
                    return OperationResult.CreateSuccessResult("数据库建立成功");
                }
                catch (SqlException ex) when (IsTransient(ex.Number))
                {
                    last = ex;
                    if (onProgress != null)
                        onProgress($"第 {i}/{attempts} 次失败：{ex.Number} {ex.Message}");

                    if (i < attempts)
                        await Task.Delay(intervalMs);
                }
                catch (SqlException ex)
                {

                    return OperationResult.CreateFailResult(
                         $"连接失败({ex.Number})，不再重试：{ex.Message}", ex);
                }
                catch (Exception ex) {

                    return OperationResult.CreateFailResult("连接异常：" + ex.Message, ex);

                }

            }
            return OperationResult.CreateFailResult(
               $"连接失败，已尝试 {attempts} 次", last);
        }
    }
}
