using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    public class ImageHelper
    {
        //读取本地图像
        public OperationResult ReadImage(string imagePath, ref HTuple WindowHandle, ref HObject hImage)
        {
            try
            {
                HOperatorSet.ReadImage(out hImage, imagePath);
                HOperatorSet.GetImageSize(hImage, out HTuple width, out HTuple height);
                HOperatorSet.SetPart(WindowHandle, 0, 0, height - 1, width - 1);
                HOperatorSet.DispObj(hImage, WindowHandle);
                return OperationResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperationResult()
                {
                    IsSuccess = false,
                    ErrorMsg = ex.Message,
                };
                
            } 
        }

        public OperationResult SaveImage( string path,string format ,HObject hImage)
        {
            try
            {
                HOperatorSet.WriteImage(hImage, format, 0, path);
                return OperationResult.CreateSuccessResult();

            }
            catch (Exception ex )
            {
                return new OperationResult()
                {
                    IsSuccess=false,
                    ErrorMsg=ex.Message,
                };
                
            }
            
        }
    }

}
