using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IniParser;        // FileIniDataParser 等解析类
using IniParser.Model;
using System.IO;  // IniData 这个数据模型


namespace Measure.Halcon.Demo.Helper
{
    internal class ShapeModelHelper
    {
        private HalconHelper halconHelper = new HalconHelper();
       private FileIniDataParser parser = new FileIniDataParser();

        //创建匹配模板
        public OperationResult CreateShapeModel(ref HTuple HwindowHandle, ref HObject image, MatchParams mathParams)
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
                HOperatorSet.CreateShapeModel(imageReduced, "auto", mathParams.StartAngleRad, mathParams.RangeAngleRad, "auto", "auto", "use_polarity", "auto", "auto", out mathParams.modelID);

                HOperatorSet.DispObj(image, HwindowHandle);

                HOperatorSet.DispObj(mathParams.modelRegion, HwindowHandle);



                if (mathParams.modelID != null)
                {
                    halconHelper.disp_message(HwindowHandle, "创建模板成功", "window", 20, 20, "green", "false");
                    return OperationResult.CreateSuccessResult("创建模板成功");
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

                HOperatorSet.DispObj(image, HwindowHandle);
                halconHelper.disp_message(HwindowHandle, "创建模板失败", "window", 20, 20, "red", "false");
                return OperationResult.CreateFailResult("创建模板失败", ex);

            }

        }

        public OperationResult FindShapeMode(ref HTuple HwindowHandle, ref HObject image, MatchParams mathParams, out HTuple homMat2d_T)
        {
            homMat2d_T = new HTuple();

            try
            {
                HOperatorSet.FindShapeModel(image, mathParams.modelID, mathParams.StartAngleRad, mathParams.RangeAngleRad, mathParams.score, mathParams.numMatchs,
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
                    HOperatorSet.AreaCenter(mathParams.modelRegion, out HTuple area, out HTuple row1, out HTuple column1);
                    HOperatorSet.VectorAngleToRigid(row1, column1, 0, row, column, angle, out homMat2d_T);


                    HOperatorSet.DispObj(image, HwindowHandle);

                    HOperatorSet.DispObj(contours2, HwindowHandle);

                    HOperatorSet.DispObj(cross, HwindowHandle);

                    halconHelper.disp_message(HwindowHandle, "定位成功", "window", 20, 20, "green", "false");

                    return OperationResult.CreateSuccessResult("查找模板成功");
                }
                else
                {
                    halconHelper.disp_message(HwindowHandle, "定位失败", "window", 20, 20, "red", "false");

                    return new OperationResult { IsSuccess = false, Msg = "未找到模板" };
                }
            }
            catch (Exception ex)
            {
                halconHelper.disp_message(HwindowHandle, "查找模板失败" + ex.Message, "window", 20, 20, "red", "false");
                return OperationResult.CreateFailResult("查找模板失败", ex);
            }




        }

        public void ClearShapeModel(ref HTuple modelId)
        {
            if (modelId != null)
            {
                HOperatorSet.ClearShapeModel(modelId);
                modelId = null;
            }
        }

        /// <summary>
        /// 加载参数（ini 里的数值/文本参数）。
        /// 注意：模板 modelID 和 ROI 还需另外从 model.she / roi_*.tup 读取。
        /// </summary>
        public OperationResult ImportShapeModel(string path, MatchParams matchParams, CircleParams circleParams)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData read = parser.ReadFile(path + "\\param.ini");

                matchParams.startAngle = Convert.ToDouble(read["定位参数"]["起始角度"]);
                matchParams.rangAngle = Convert.ToDouble(read["定位参数"]["角度范围"]);
                matchParams.score = Convert.ToDouble(read["定位参数"]["分数"]);
                matchParams.overlap = Convert.ToDouble(read["定位参数"]["重叠度"]);
                matchParams.numMatchs = Convert.ToInt32(read["定位参数"]["数量"]);

                circleParams.circle_Elements = Convert.ToInt32(read["测量参数"]["搜索点数"]);
                circleParams.circle_Threshold = Convert.ToInt32(read["测量参数"]["边缘阈值"]);
                circleParams.circle_Sigma = Convert.ToDouble(read["测量参数"]["平滑系数"]);
                circleParams.circle_Transition = read["测量参数"]["极性"];
                circleParams.circle_Point_Select = read["测量参数"]["点选择"];

                circleParams.scale = Convert.ToDouble(read["标定参数"]["像素距离"]);

                circleParams.circle_Dis_Min = Convert.ToDouble(read["直径范围"]["最小直径"]);
                circleParams.circle_Dis_Max = Convert.ToDouble(read["直径范围"]["最大直径"]);

                // 模板句柄：先释放旧模板，避免反复加载导致 HALCON 句柄泄漏
                ClearShapeModel(ref matchParams.modelID);
                HOperatorSet.ReadShapeModel(path + "\\model.she", out HTuple modelId);
                matchParams.modelID = modelId;

                // 模板区域
                if (matchParams.modelRegion != null) matchParams.modelRegion.Dispose();
                HOperatorSet.ReadRegion(out HObject modelRegion, path + "\\model_region.tif");
                matchParams.modelRegion = modelRegion;

                // ROI 卡尺点：roi_row → rOI_Y（行），roi_col → rOI_X（列）
                HOperatorSet.ReadTuple(path + "\\roi_row.tup", out HTuple roiRows);
                HOperatorSet.ReadTuple(path + "\\roi_col.tup", out HTuple roiCols);
                HOperatorSet.ReadTuple(path + "\\roi_dir.tup", out HTuple roiDir);
                circleParams.rOI_Y = roiRows;
                circleParams.rOI_X = roiCols;
                circleParams.circle_Direct = roiDir;

                return OperationResult.CreateSuccessResult("参数加载成功");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailResult("参数加载失败", ex);
            }
        }


        /// <summary>
        /// 模板存储
        /// </summary>
        /// <param name="path">模板存储文件夹</param>
        /// <param name="matchParams">模板参数</param>
        /// <param name="circleParams">圆拟合参数</param>
        /// <returns></returns>

        public OperationResult SaveShapeModel(string path, MatchParams matchParams, CircleParams circleParams)
        {
            try
            {
                if (matchParams.modelID == null && matchParams.modelRegion == null && circleParams.rOI_X.Length == 0 && circleParams.rOI_Y == 0)
                {
                    if (matchParams.modelID != null && circleParams.rOI_X.Length == 0)
                    {
                        return OperationResult.CreateFailResult("定位模板和圆拟合模板未创建");
                    }
                    else if (matchParams.modelID == null)
                    {
                        return OperationResult.CreateFailResult("定位模板未创建");
                    }
                    else if (circleParams.rOI_X.Length > 0)
                    {
                        return OperationResult.CreateFailResult("圆拟合模板未创建");
                    }
                }
                HOperatorSet.WriteShapeModel(matchParams.modelID, path + "\\model.she");
                HOperatorSet.WriteRegion(matchParams.modelRegion, path + "\\model_region.tif");
                HOperatorSet.WriteTuple(circleParams.rOI_Y, path + "\\roi_row.tup");
                HOperatorSet.WriteTuple(circleParams.rOI_X, path + "\\roi_col.tup");
                HOperatorSet.WriteTuple(circleParams.circle_Direct, path + "\\roi_dir.tup");

                IniData iniData = new IniData();
                iniData["定位参数"]["起始角度"] = matchParams.startAngle.ToString();
                iniData["定位参数"]["角度范围"] = matchParams.rangAngle.ToString();
                iniData["定位参数"]["分数"] = matchParams.score.ToString();
                iniData["定位参数"]["重叠度"] = matchParams.overlap.ToString();
                iniData["定位参数"]["数量"] = matchParams.numMatchs.ToString();

                iniData["测量参数"]["搜索点数"] = circleParams.circle_Elements.ToString();
                iniData["测量参数"]["边缘阈值"] = circleParams.circle_Threshold.ToString();
                iniData["测量参数"]["平滑系数"] = circleParams.circle_Sigma.ToString();
                iniData["测量参数"]["极性"] = circleParams.circle_Transition;
                iniData["测量参数"]["点选择"] = circleParams.circle_Point_Select;

                iniData["标定参数"]["像素距离"] = circleParams.scale.ToString();

                iniData["直径范围"]["最小直径"] = circleParams.circle_Dis_Min.ToString();
                iniData["直径范围"]["最大直径"] = circleParams.circle_Dis_Max.ToString();

                var parser = new FileIniDataParser();
                parser.WriteFile(path + "\\param.ini", iniData);
                return OperationResult.CreateSuccessResult("参数保存成功");
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("参数保存失败", ex);
            }
        }


        public OperationResult GetAllShapeModel(string basePath,out List<string> list)
        {
            try
            {
                list= new List<string>();
                DirectoryInfo directory = new DirectoryInfo(basePath);
                foreach (var item in directory.GetDirectories())
                {
                  list.Add(item.Name);
                }
                return OperationResult.CreateSuccessResult("读取模板列表成功");
            }
            catch (Exception ex )
            {
                list = null;
                return OperationResult.CreateFailResult("读取模板列表失败", ex);
            }
        }
       
    }
}
