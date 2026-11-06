using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Engine
{
    /// <summary>
    /// 图像源接口，不关心图来自相机还是文件。
    /// </summary>
    internal interface IImageSource
    {
        /// <summary>日志里用的名字，如 "文件:连接件" / "相机:GigEVision2"。仅用于展示。</summary>
        string Name { get; }

        /// <summary>
        /// 两次取图之间的停顿（毫秒）。<br/>
        /// 0 = 不额外等待，节奏由取图本身决定（相机 GrabImage 阻塞到帧回来）；<br/>
        /// 2000 = 文件源，每张停 2 秒，方便人眼看。
        /// </summary>
        int IntervalMs { get; }

        /// <summary>
        /// 取图失败时是否终止整个流程。<br/>
        /// true  = 相机掉线，继续跑没有意义；<br/>
        /// false = 文件源，某张图损坏应跳过继续。
        /// </summary>
        bool StopOnError { get; }

        OperationResult GrabImage(out HObject image);
    }
}
