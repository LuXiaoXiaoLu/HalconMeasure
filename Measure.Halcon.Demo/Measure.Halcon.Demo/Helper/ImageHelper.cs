using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    public class ImageHelper
    {
        //相机对象句柄
        private HTuple acqHandle;
        /// <summary>
        /// 后缀名 -> HALCON 支持的格式名
        /// </summary>
        private static readonly Dictionary<string, string> SaveFormats =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".bmp",  "bmp"  },
                { ".png",  "png"  },
                { ".jpg",  "jpeg" },
                { ".jpeg", "jpeg" },
                { ".tif",  "tiff" },
                { ".tiff", "tiff" },
            };
        //打开相机
        public OperationResult OpenCam(string Drvname,string CamName)
        {
            try
            {
                CloseCam();
                HOperatorSet.OpenFramegrabber(Drvname, 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default",
                CamName, 0, -1, out acqHandle);
                return  OperationResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {

                return new OperationResult()
                {
                    IsSuccess = false,
                    ErrorMsg = ex.Message,
                }
                ;
            }
            
        }

        private bool IsCameraOpen()
        {
            if (acqHandle != null && acqHandle.Length > 0)
            {
                HTuple isValid;
                HOperatorSet.TupleIsValidHandle(acqHandle, out isValid);
                if (isValid.I == 1)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        //关闭相机
        public OperationResult CloseCam( )
        {
            if(IsCameraOpen())
            {
                    try
                    {
                        HOperatorSet.CloseFramegrabber(acqHandle);
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

            return OperationResult.CreateSuccessResult();
        }

        //相机读取图像
        public OperationResult GrabImage( ref HObject hImage, ref HTuple WindowHandle)
        {
            try
            {
                HOperatorSet.GrabImage(out hImage, acqHandle);
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

      

        /// <summary>
        /// </summary>
        /// <param name="path">完整的文件路径，如 D:\img\1.bmp</param>
        /// <param name="hImage">要保存的图像</param>
        public OperationResult SaveImage(string path, HObject hImage)
        {
            try
            {
                // 从路径里取后缀，例如 ".bmp"（含点）
                string ext = Path.GetExtension(path);
                string format;
                if (!SaveFormats.TryGetValue(ext, out format))
                {
                    return new OperationResult()
                    {
                        IsSuccess = false,
                        ErrorMsg = string.IsNullOrEmpty(ext)
                            ? "文件路径没有扩展名，无法判断保存格式"
                            : "不支持保存为 \"" + ext + "\" 格式，请使用 bmp / png / jpg / jpeg / tif / tiff",
                    };
                }

                // 目录不存在就建，避免 HALCON 直接抛异常
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                HOperatorSet.WriteImage(hImage, format, 0, path);
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
    }

}
