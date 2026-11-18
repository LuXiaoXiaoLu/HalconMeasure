using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Trigger
{
    internal class FreeRunTriggerSource : ITriggerSource
    {
        public string Name { get { return "自由运行"; } }

        public bool WaitTrigger(CancellationToken token)
        {
            return !token.IsCancellationRequested;
        }

        public bool WaitTrigger(CancellationToken token, out TriggerInfo tri)
        {
            throw new NotImplementedException();
        }
    }
}
