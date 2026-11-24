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
    /// <summary>
    /// 数据库初始化器：启动时把库弄到「能连、结构是最新版本」的状态。
    /// 职责链：连接（带重试）→ 读当前版本 → 校验 → 按需逐级升级。
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>程序期望的库结构版本；每新增一个迁移脚本就 +1。</summary>
        private int _targetVersion = 0;

        /// <summary>判断这个错误值不值得再试一次</summary>
        private bool IsRetryableError(int number)
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


        /// <summary>确保能连上数据库；瞬时故障自动重试。</summary>
        public async Task<OperationResult> EnsureConnectedAsync(Action<string> onProgress)
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
                    return OperationResult.CreateSuccessResult("数据库链接成功");
                }
                catch (SqlException ex) when (IsRetryableError(ex.Number))
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
                catch (Exception ex)
                {

                    return OperationResult.CreateFailResult("连接异常：" + ex.Message, ex);

                }

            }
            return OperationResult.CreateFailResult(
               $"连接失败，已尝试 {attempts} 次", last);
        }


        /// <summary>读取数据库当前的结构版本号。</summary>
        public async Task<OperationResult<int>> GetCurrentVersionAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DbConfig.ConnStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("Select MAX(currentVersion) FROM dbo.SchemaVersion", connection))
                    {
                        var values = await cmd.ExecuteScalarAsync();
                        if (values == null || values == DBNull.Value)
                        {
                            return OperationResult<int>.CreateFailResult("SchemaVersion 表为空");
                        }
                        return OperationResult<int>.CreateSuccessResult((int)values);
                    }

                }
            }
            catch (SqlException ex) when (ex.Number == 208)
            {

                return OperationResult<int>.CreateFailResult("SchemaVersion 表不存在，需要执行初始化脚本", ex);
            }
            catch (Exception ex)
            {
                {
                    return OperationResult<int>.CreateFailResult("读取结构版本失败：" + ex.Message, ex);
                }
            }
        }


        /// <summary>校验库版本，落后则升级到目标版本。</summary>
        public async Task<OperationResult> EnsureUpToDateAsync(int dbVersion)
        {
            if (dbVersion == _targetVersion)
            {
                return OperationResult.CreateSuccessResult("数据库版本校验成功无需更新");
            }

            if (dbVersion < _targetVersion)
            {
                return await UpgradeSchemaAsync(dbVersion);
            }


            return OperationResult.CreateFailResult("数据库版本异常");

        }

        /// <summary>按差额逐级执行迁移脚本，把库升到目标版本。</summary>
        public async Task<OperationResult> UpgradeSchemaAsync(int dbVersion)
        {

            for (int i = dbVersion; i <= _targetVersion; i++)
            {
                string fileName;
                if (!SqlScriptRepository.ScriptMap.TryGetValue(i, out fileName))
                {
                    return OperationResult.CreateFailResult($"数据库版本更新失败：缺少版本 {i:D3} 的脚本文件");
                }
                OperationResult result = await SqlScriptRepository.ExecuteAsync(fileName);
                if (!result.IsSuccess)
                {
                    return OperationResult.CreateFailResult("数据库版本更新失败" + result.Msg, result.Error);
                }
            }

            return OperationResult.CreateSuccessResult("数据库版本更新成功");
        }
    }
}
