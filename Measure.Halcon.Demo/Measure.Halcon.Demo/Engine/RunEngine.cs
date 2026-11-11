using HalconDotNet;
using Measure.Halcon.Demo.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Engine
{
    /// <summary>引擎状态。</summary>
    internal enum EngineState
    {
        Idle,       // 空闲：没有循环在跑
        Running,    // 运行中
        Stopping    // 已请求停止，正在等线程退出
    }

    /// <summary>
    /// 运行引擎：唯一的一个「取图 → 处理 → 上报」循环。
    /// 图像源IImageSource， 处理方法process 委托决定。
    /// </summary>  
    internal sealed class RunEngine
    {
        private readonly LogHelper log;
        private CancellationTokenSource cts;
        private Task task;

        private volatile EngineState state = EngineState.Idle;

        /// <summary>当前正在跑的流程名（Start 传入的 name）；空闲时为 null。</summary>
        private volatile string currentName;

        public RunEngine(LogHelper log)
        {
            this.log = log;
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public EngineState State { get { return state; } }

        /// <summary>
        /// 当前正在跑的是哪个流程（Start 传入的 name），空闲时为 null。<br/>
        /// UI 用它区分「点的是自己（该停止）」还是「点的是别人（该拒绝）」，避免互相顶掉。
        /// </summary>
        public string CurrentName { get { return currentName; } }

        /// <summary>
        /// 启动循环（一个引擎同时只跑一个流程）。
        /// </summary>
        /// <returns>true = 已启动；false = 被拒绝（原因已记日志）。</returns>
        public bool Start(string name,
                          IImageSource source,
                          Func<HObject, OperationResult> process,
                          Action<OperationResult> report)
        {
            if (state != EngineState.Idle)
            {
                log.Warn(name + "已在运行请勿重复启动");
                return false;
            }

            if (source == null || process == null)
            {
                log.Warn(name + " 启动失败：source 或 process 为空");
                return false;
            }

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            state = EngineState.Running;
            currentName = name;

            task = Task.Factory.StartNew(
                () => Loop(name, token, source, process, report),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            log.Info(name + " 启动（来源：" + source.Name + "）");
            return true;
        }

        /// <summary>循环本体，跑在独立线程上。</summary>
        private void Loop(string name,
                          CancellationToken token,
                          IImageSource source,
                          Func<HObject, OperationResult> process,
                          Action<OperationResult> report)
        {


            try
            {
                while (!token.IsCancellationRequested)
                {
                    OperationResult garbImageResult = source.GrabImage(out HObject image);
                    if (garbImageResult.IsSuccess)
                    {
                        try
                        {
                            OperationResult processResult = process(image);
                            if (report != null) report(processResult);
                        }
                        finally
                        {
                            if (image != null) image.Dispose();
                        }

                    }
                    else
                    {
                        if (report!=null)
                        {
                            report(garbImageResult);
                        }
                        if (source.StopOnError) break;
                    }
                    if (source.IntervalMs > 0 && token.WaitHandle.WaitOne(source.IntervalMs)) break;
                }
            }
            catch (Exception ex)
            {

                log.Fail(OperationResult.CreateFailResult(name + "引擎异常", ex));
            }
            finally
            {
                // 自然结束（StopOnError / 异常）要自己收回 Idle 并清掉名字；
                // 如果是 Stop() 触发的退出，由 Stop 负责收尾，这里别抢
                if (state != EngineState.Stopping)
                {
                    state = EngineState.Idle;
                    currentName = null;
                }
                log.Info(name + " 结束");
            }



        }

        /// <summary>
        /// 请求停止并等待线程退出。
        /// </summary>
        /// <returns>true = 已确认停止；false = 超时仍未退出（状态保留 Running，防止重复启动）。</returns>
        public bool Stop(int timeoutMs)
        {
            if (state == EngineState.Idle) return true;

            state = EngineState.Stopping;

            cts.Cancel();

            bool exited;
            try
            {
                exited = task.Wait(timeoutMs);
            }
            catch (AggregateException ex)
            {

                log.Fail(OperationResult.CreateFailResult("引擎线程异常结束", ex.InnerException ?? ex));
                exited = true;
            }

            if (!exited)
            {
                log.Warn("引擎线程未在 " + timeoutMs + " ms 内退出");
                state = EngineState.Running;
                return false;
            }

            cts.Dispose();
            cts = null;
            state = EngineState.Idle;
            currentName = null;
            return true;
        }
    }
}
