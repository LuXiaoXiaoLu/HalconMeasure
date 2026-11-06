using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Engine
{
    internal class CameraImageSource : IImageSource
    {
        public string Name { get { return "相机"; } }

        public int IntervalMs { get { return 0; } }

        public bool StopOnError { get { return true; } }

        public OperationResult GrabImage(out HObject image)
        {
            throw new NotImplementedException();
        }
    }
}
