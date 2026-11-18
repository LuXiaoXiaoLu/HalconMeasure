using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Trigger
{
    /// <summary>
    /// 触发源接口
    /// </summary>
    internal interface ITriggerSource
    {
        string Name { get; }

        bool WaitTrigger(CancellationToken token,out TriggerInfo tri);
    }
}
