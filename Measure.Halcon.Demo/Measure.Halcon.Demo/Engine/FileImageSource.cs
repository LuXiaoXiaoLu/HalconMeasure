using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Engine
{
    internal class FileImageSource : IImageSource
    {
        private readonly string _filePath;
        public string Name { get { return "相机"; } }

       public int IntervalMs { get { return 2000; } }
      
       public bool StopOnError { get { return true; } }

        public OperationResult GrabImage(out HObject image)
        {
            try
            {
                HOperatorSet.ReadImage(out image, _filePath);
                return OperationResult.CreateSuccessResult("读取图像成功");
            }
            catch (Exception ex)
            {
                image = null;
                return OperationResult.CreateFailResult("读取图像失败", ex);
            }
        }

        public FileImageSource(string path)
        {
            _filePath = path;
        }
    }
}
