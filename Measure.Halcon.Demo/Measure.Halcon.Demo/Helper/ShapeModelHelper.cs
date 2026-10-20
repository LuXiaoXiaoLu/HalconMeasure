using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Helper
{
    internal class ShapeModelHelper
    {
        private HalconHelper  halconHelper =new HalconHelper();
        //创建匹配模板
        public OperationResult CreateShapeModel(ref HTuple HwindowHandle ,ref HObject image, MathParams mathParams)
        {

            try
            {
                halconHelper.disp_message(HwindowHandle, "请绘制模板区域，完成后右击确实", "window", 20, 20, "red", "false");
                //Draw
                HOperatorSet.DrawRectangle2(HwindowHandle, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);

                //gen
                HOperatorSet.GenRectangle2(out mathParams.modelRegion, row, column, phi, length1, length2);
                // ReduceDomain
                HOperatorSet.ReduceDomain(image, mathParams.modelRegion, out HObject imageReduced);

                ClearShapeModel(ref mathParams.modelID);
                // create
                HOperatorSet.CreateShapeModel(imageReduced, "auto", mathParams.startAngle, mathParams.rangAngle, "auto", "auto", "use_polarity", "auto", "auto", out mathParams.modelID);

                HOperatorSet.DispObj(image, HwindowHandle);

                HOperatorSet.DispObj(mathParams.modelRegion, HwindowHandle);



                if (mathParams.modelID != null)
                {
                    halconHelper.disp_message(HwindowHandle, "创建模板成功", "window", 20, 20, "green", "false");
                    return OperationResult.CreateSuccessResult();
                }
                else
                {
                    halconHelper.disp_message(HwindowHandle, "创建模板失败", "window", 20, 20, "red", "false");
                    return new OperationResult
                    {
                        IsSuccess = false,
                      
                    };
                }
            }
            catch (Exception ex)
            {

                
                    halconHelper.disp_message(HwindowHandle, "创建模板失败"+ex.Message, "window", 20, 20, "red", "false");
                    return new OperationResult { 
                        IsSuccess=false,
                        ErrorMsg=ex.Message
                    };
                
            }
               
        }

        public OperationResult FindShapeMode(ref HTuple HwindowHandle, ref HObject image, MathParams mathParams)
        {

            try
            {
                HOperatorSet.FindShapeModel(image, mathParams.modelID, mathParams.startAngle, mathParams.rangAngle, mathParams.score, mathParams.numMatchs,
            mathParams.overlap, "least_squares", 0, mathParams.greediness, out HTuple row, out HTuple column, out HTuple angle, out HTuple score);

                //判断结果
                if (row.Length > 0)
                {
                    //处理

                    //显示

                    //获取仿射变换矩阵
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, column, angle, out HTuple homMat2D);

                    HOperatorSet.GetShapeModelContours(out HObject contours, mathParams.modelID, 1);

                    HOperatorSet.AffineTransContourXld(contours, out HObject contours2, homMat2D);

                    HOperatorSet.GenCrossContourXld(out HObject cross, row, column, 20, new HTuple(45));

                    HOperatorSet.DispObj(image, HwindowHandle);

                    HOperatorSet.DispObj(contours2, HwindowHandle);

                    HOperatorSet.DispObj(cross, HwindowHandle);

                    halconHelper.disp_message(HwindowHandle, "定位成功", "window", 20, 20, "green", "false");

                    return OperationResult.CreateSuccessResult();
                }
                else
                {
                    halconHelper.disp_message(HwindowHandle, "定位失败", "window", 20, 20, "red", "false");

                    return new OperationResult { IsSuccess = false, ErrorMsg = "未找到模板" };
                }
            }
            catch (Exception ex)
            {
                halconHelper.disp_message(HwindowHandle, "查找模板失败" + ex.Message, "window", 20, 20, "red", "false");
                return new OperationResult
                {
                    IsSuccess = false,
                    ErrorMsg = ex.Message
                };
            }




        }

        public void ClearShapeModel(ref HTuple modelId) {
            if (modelId != null)
            {
                HOperatorSet.ClearShapeModel(modelId);
                modelId = null;
            }
        }

        
    }
}
