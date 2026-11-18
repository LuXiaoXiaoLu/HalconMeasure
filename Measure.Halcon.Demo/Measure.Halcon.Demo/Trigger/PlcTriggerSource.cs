using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Measure.Halcon.Demo.Trigger
{
    internal class PlcTriggerSource : ITriggerSource
    {

        private readonly BlockingCollection<TriggerInfo> _triggerQueue;

        public PlcTriggerSource(BlockingCollection<TriggerInfo> queue)
        {
            _triggerQueue = queue;
        }

        public string Name { get { return "PLC触发"; } }


        public bool WaitTrigger(CancellationToken token,out TriggerInfo tri)
        {
            try
            {
                tri = _triggerQueue.Take(token);
                return true;
            }
            catch (OperationCanceledException)
            {
                tri=null;
                return false;
            }
        }
    }
}
