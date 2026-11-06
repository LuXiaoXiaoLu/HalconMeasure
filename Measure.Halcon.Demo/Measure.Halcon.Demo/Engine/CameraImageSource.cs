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
        private readonly ImageHelper _imageHelper;
        public string Name { get { return "相机"; } }

        public int IntervalMs { get { return 0; } }

        public bool StopOnError { get { return true; } }

        public OperationResult GrabImage(out HObject image)
        {
          return  _imageHelper.GrabImage(out image);
        }

        public CameraImageSource(ImageHelper imageHelper)
        {
            _imageHelper = imageHelper;
        }
    }
}
