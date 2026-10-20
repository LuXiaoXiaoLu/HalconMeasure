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

        public bool IsCameraOpen()
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
                        acqHandle=null;
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

        /// <summary>读取曝光时间（微秒）。读不到返回 0。</summary>
        public double GetExposureTime()
        {
            return GetCamParamDouble("ExposureTime");
        }

        /// <summary>设置曝光时间（微秒）。</summary>
        public OperationResult SetExposureTime(double exposureTime)
        {
            return SetCamParam("ExposureTime", exposureTime);
        }

        /// <summary>读取增益。读不到返回 0。</summary>
        public double GetGain()
        {
            return GetCamParamDouble("Gain");
        }

        /// <summary>设置增益。</summary>
        public OperationResult SetGain(double gain)
        {
            return SetCamParam("Gain", gain);
        }

        public static bool IsObjectValid(HObject Hobject)
        {
            if (Hobject == null) return false;
            if (!Hobject.IsInitialized()) return false;
            if (Hobject.CountObj() == 0) return false;
            return true;
        }

        /// <summary>读取触发模式：TriggerMode 为 On 时返回 true。</summary>
        public bool GetTriggerMode()
        {
            try
            {
                if (IsCameraOpen())
                {
                    HOperatorSet.GetFramegrabberParam(acqHandle, "TriggerMode", out HTuple value);

                    if (value != null && value.Length > 0)
                    {
                        // 不区分大小写，兼容相机返回 "On" 或 "on"
                        return string.Equals(value.S, "On", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch (HalconException ex)
            {
                // 参数不存在/相机没打开等：记下来，别静默吞掉
                System.Diagnostics.Debug.WriteLine("读取 TriggerMode 失败：" + ex.Message);
            }
            return false;
        }

        /// <summary>设置触发模式：true = On（触发采集），false = Off（连续采集）。</summary>
        public OperationResult SetTriggerMode(bool triggerOn)
        {
            // TriggerMode 在 SFNC/GenICam 里是【枚举】，取值是字符串 "On" / "Off"（首字母大写）。
            // HTuple 内部只有 int/double/string/handle 四种类型，没有布尔，
            // 所以 new HTuple(true) 只会变成整数 1，不能当枚举值下发。
            return SetCamParam("TriggerMode", triggerOn ? "On" : "Off");
        }

        /// <summary>读取数值型相机参数的公共实现。读不到返回 0。</summary>
        private double GetCamParamDouble(string param)
        {
            try
            {
                if (IsCameraOpen())
                {
                    HOperatorSet.GetFramegrabberParam(acqHandle, param, out HTuple value);

                    if (value != null && value.Length > 0)
                    {
                        // 用 .D 保留小数；对浮点元组用 .I 会抛 HTupleAccessException
                        return value.D;
                    }
                }
            }
            catch (HalconException ex)
            {
                System.Diagnostics.Debug.WriteLine("读取 " + param + " 失败：" + ex.Message);
            }
            return 0;
        }

        /// <summary>设置数值型相机参数（如 ExposureTime / Gain）。</summary>
        private OperationResult SetCamParam(string param, double value)
        {
            return SendCamParam(param, new HTuple(value));
        }

        /// <summary>设置字符串 / 枚举型相机参数（如 TriggerMode）。</summary>
        private OperationResult SetCamParam(string param, string value)
        {
            return SendCamParam(param, new HTuple(value));
        }

        /// <summary>下发相机参数的公共实现。HTuple 只在这一处出现，不对外暴露。</summary>
        private OperationResult SendCamParam(string param, HTuple value)
        {
            if (!IsCameraOpen())
            {
                return new OperationResult()
                {
                    IsSuccess = false,
                    ErrorMsg = "相机未打开，无法设置 " + param,
                };
            }

            try
            {
                HOperatorSet.SetFramegrabberParam(acqHandle, param, value);
                return OperationResult.CreateSuccessResult();
            }
            catch (HalconException ex)
            {
                System.Diagnostics.Debug.WriteLine("设置 " + param + " 失败：" + ex.Message);
                return new OperationResult()
                {
                    IsSuccess = false,
                    // 带上真实原因，别一律说"相机未打开"（可能是值非法/相机不支持）
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
