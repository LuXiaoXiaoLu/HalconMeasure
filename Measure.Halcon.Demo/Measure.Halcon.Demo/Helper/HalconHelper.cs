using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Measure.Halcon.Demo.Helper
{
    internal class HalconHelper
    {
        
        public OperationResult HalconInitial(HWindowControl hWindowControl,out HTuple hwindowHandle)
        {
            
            try
            {
                HOperatorSet.SetSystem("width", 4000);
                HOperatorSet.SetSystem("height", 3000);
                hwindowHandle = hWindowControl.HalconWindow;
                HOperatorSet.SetDraw(hwindowHandle, "margin");
                HOperatorSet.SetColor(hwindowHandle, "red");
                HOperatorSet.SetColored(hwindowHandle, 12);
                return  OperationResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                hwindowHandle = null;
                return OperationResult.CreateFailResult("Halcon 初始化失败", ex);
            }
        }
        //判断HObject对象是否有效
        public static bool  IsObjectValid(HObject Hobject) {
            if (Hobject == null) return false;
            if(!Hobject.IsInitialized()) return false;
            if(Hobject.CountObj()==0) return false;
            return true;
        }

        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
        HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed.
            //String: A tuple of strings containing the text messages to be displayed.
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'imageHelper', imageHelper coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Column: The column coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Color: defines the color of the text as string.
            //   If set to [] or '' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for every text position defined by Row and Column,
            //   or every new text line in case of |Row| == |Column| == 1.
            //Box: A tuple controlling a possible box surrounding the text.
            //   Its entries:
            //   - Box[0]: Controls the box and its color. Possible values:
            //     -- 'true' (Default): An orange box is displayed.
            //     -- 'false': No box is displayed.
            //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
            //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
            //     -- 'true' (Default): A shadow is displayed in
            //               darker orange if Box[0] is not a color and in 'white' otherwise.
            //     -- 'false': No shadow is displayed.
            //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply on the
            //parameters String, Row, and Column:
            //They can only have either 1 entry or n entries.
            //Behavior in the different cases:
            //   - Multiple text positions are specified, i.e.,
            //       - |Row| == n, |Column| == n
            //       - |Row| == n, |Column| == 1
            //       - |Row| == 1, |Column| == n
            //     In this case we distinguish:
            //       - |String| == n: Each element of String is displayed
            //                        at the corresponding position.
            //       - |String| == 1: String is displayed n times
            //                        at the corresponding positions.
            //   - Exactly one text position is specified,
            //      i.e., |Row| == |Column| == 1:
            //      Each element of String is display in a new textline.
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName.Dispose();
            hv_GenParamName = new HTuple();
            hv_GenParamValue.Dispose();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP = "imageHelper";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            hv_Color_COPY_INP_TMP.Dispose();
            hv_Column_COPY_INP_TMP.Dispose();
            hv_CoordSystem_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_GenParamName.Dispose();
            hv_GenParamValue.Dispose();

            return;
        }


        // Procedures 
        public void draw_spoke3(HObject ho_Image, out HObject ho_Regions, HTuple hv_WindowHandle,
     HTuple hv_Elements, HTuple hv_DetectHeight, HTuple hv_DetectWidth, out HTuple hv_ROIRows,
     out HTuple hv_ROICols, out HTuple hv_Direct)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_objArrow = null, ho_Arrow = null, ho_Contour;
            HObject ho_ContCircle, ho_Cross, ho_Rectangle1 = null, ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Rows = new HTuple(), hv_Cols = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Button = new HTuple(), hv_i = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_ExceptionT = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_RowC = new HTuple(), hv_ColumnC = new HTuple();
            HTuple hv_Radius = new HTuple(), hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_RowXLD = new HTuple(), hv_ColXLD = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_DistanceStart = new HTuple(), hv_DistanceEnd = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_j = new HTuple();
            HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_objArrow);
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            hv_ROIRows = new HTuple();
            hv_ROICols = new HTuple();
            hv_Direct = new HTuple();
            try
            {
                //提示
                disp_message(hv_WindowHandle, new HTuple("1、画4个以上点确定一个圆弧,点击右键确认"),
                    "window", 12, 12, "red", "false");
                try
                {
                    hv_Rows.Dispose();
                    hv_Rows = new HTuple();
                    hv_Cols.Dispose();
                    hv_Cols = new HTuple();
                    while ((int)(1) != 0)
                    {
                        hv_Row.Dispose(); hv_Column.Dispose(); hv_Button.Dispose();
                        HOperatorSet.GetMbutton(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Button);
                        if ((int)(new HTuple(hv_Button.TupleEqual(1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Rows = hv_Rows.TupleConcat(
                                        hv_Row);
                                    hv_Rows.Dispose();
                                    hv_Rows = ExpTmpLocalVar_Rows;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Cols = hv_Cols.TupleConcat(
                                        hv_Column);
                                    hv_Cols.Dispose();
                                    hv_Cols = ExpTmpLocalVar_Cols;
                                }
                            }
                            //产生一个空显示对象，用于显示
                            ho_objArrow.Dispose();
                            HOperatorSet.GenEmptyObj(out ho_objArrow);
                            if ((int)(new HTuple((new HTuple(hv_Rows.TupleLength())).TupleGreater(
                                1))) != 0)
                            {
                                for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Rows.TupleLength())) - 2); hv_i = (int)hv_i + 1)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        ho_Arrow.Dispose();
                                        gen_arrow_contour_xld(out ho_Arrow, hv_Rows.TupleSelect(hv_i), hv_Cols.TupleSelect(
                                            hv_i), hv_Rows.TupleSelect(hv_i + 1), hv_Cols.TupleSelect(hv_i + 1),
                                            25, 25);
                                    }
                                    {
                                        HObject ExpTmpOutVar_0;
                                        HOperatorSet.ConcatObj(ho_objArrow, ho_Arrow, out ExpTmpOutVar_0);
                                        ho_objArrow.Dispose();
                                        ho_objArrow = ExpTmpOutVar_0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((int)(new HTuple(hv_Button.TupleEqual(4))) != 0)
                            {
                                if ((int)(new HTuple((new HTuple(hv_Rows.TupleLength())).TupleGreater(
                                    1))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_Distance.Dispose();
                                        HOperatorSet.DistancePp(hv_Rows.TupleSelect(0), hv_Cols.TupleSelect(
                                            0), hv_Rows.TupleSelect((new HTuple(hv_Rows.TupleLength())) - 1),
                                            hv_Cols.TupleSelect((new HTuple(hv_Rows.TupleLength())) - 1), out hv_Distance);
                                    }
                                    if ((int)(new HTuple(hv_Distance.TupleLess(20))) != 0)
                                    {
                                        if ((int)((new HTuple(((hv_Rows.TupleSelect((new HTuple(hv_Rows.TupleLength()
                                            )) - 1))).TupleNotEqual(hv_Rows.TupleSelect(0)))).TupleOr(new HTuple(((hv_Cols.TupleSelect(
                                            (new HTuple(hv_Cols.TupleLength())) - 1))).TupleNotEqual(hv_Cols.TupleSelect(
                                            0))))) != 0)
                                        {
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_Rows = hv_Rows.TupleConcat(
                                                        hv_Rows.TupleSelect(0));
                                                    hv_Rows.Dispose();
                                                    hv_Rows = ExpTmpLocalVar_Rows;
                                                }
                                            }
                                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                            {
                                                {
                                                    HTuple
                                                      ExpTmpLocalVar_Cols = hv_Cols.TupleConcat(
                                                        hv_Cols.TupleSelect(0));
                                                    hv_Cols.Dispose();
                                                    hv_Cols = ExpTmpLocalVar_Cols;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        HOperatorSet.DispObj(ho_objArrow, hv_WindowHandle);
                    }
                }
                // catch (ExceptionT) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_ExceptionT);
                }
                HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                //产生一个空显示对象，用于显示
                ho_Regions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Regions);
                //至少要4个点
                hv_Length1.Dispose();
                HOperatorSet.TupleLength(hv_Rows, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(4))) != 0)
                {
                    disp_message(hv_WindowHandle, "提示：点数太少，请重画", "window", 32, 12,
                        "red", "false");
                    hv_ROIRows.Dispose();
                    hv_ROIRows = new HTuple();
                    hv_ROICols.Dispose();
                    hv_ROICols = new HTuple();
                    ho_objArrow.Dispose();
                    ho_Arrow.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    hv_Rows.Dispose();
                    hv_Cols.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Button.Dispose();
                    hv_i.Dispose();
                    hv_Distance.Dispose();
                    hv_ExceptionT.Dispose();
                    hv_Length1.Dispose();
                    hv_RowC.Dispose();
                    hv_ColumnC.Dispose();
                    hv_Radius.Dispose();
                    hv_StartPhi.Dispose();
                    hv_EndPhi.Dispose();
                    hv_PointOrder.Dispose();
                    hv_RowXLD.Dispose();
                    hv_ColXLD.Dispose();
                    hv_Row1.Dispose();
                    hv_Column1.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_DistanceStart.Dispose();
                    hv_DistanceEnd.Dispose();
                    hv_Length2.Dispose();
                    hv_j.Dispose();
                    hv_RowE.Dispose();
                    hv_ColE.Dispose();
                    hv_ATan.Dispose();
                    hv_RowL2.Dispose();
                    hv_RowL1.Dispose();
                    hv_ColL2.Dispose();
                    hv_ColL1.Dispose();

                    return;
                }
                //获取点
                hv_ROIRows.Dispose();
                hv_ROIRows = new HTuple(hv_Rows);
                hv_ROICols.Dispose();
                hv_ROICols = new HTuple(hv_Cols);
                //产生xld
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
                //用回归线法（不抛出异常点，所有点权重一样）拟合圆
                hv_RowC.Dispose(); hv_ColumnC.Dispose(); hv_Radius.Dispose(); hv_StartPhi.Dispose(); hv_EndPhi.Dispose(); hv_PointOrder.Dispose();
                HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                    out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                //根据拟合结果产生xld，并保持到显示对象
                ho_ContCircle.Dispose();
                HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                    hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_ContCircle, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }

                //获取圆或圆弧xld上的点坐标
                hv_RowXLD.Dispose(); hv_ColXLD.Dispose();
                HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);
                //显示图像和圆弧
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ContCircle, HDevWindowStack.GetActive());
                }
                //产生并显示圆心
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_RowC, hv_ColumnC, 60, 0.785398);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Cross, HDevWindowStack.GetActive());
                }
                //提示
                disp_message(hv_WindowHandle, "2、远离圆心，画箭头确定边缘检测方向，点击右键确认",
                    "window", 12, 12, "red", "false");
                //画线，确定检测方向
                hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                HOperatorSet.DrawLine(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                    out hv_Column2);
                //求圆心到检测方向直线起点的距离
                hv_DistanceStart.Dispose();
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row1, hv_Column1, out hv_DistanceStart);
                //求圆心到检测方向直线终点的距离
                hv_DistanceEnd.Dispose();
                HOperatorSet.DistancePp(hv_RowC, hv_ColumnC, hv_Row2, hv_Column2, out hv_DistanceEnd);

                //求圆或圆弧xld上的点的数量
                hv_Length2.Dispose();
                HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
                //判断检测的边缘数量是否过少
                if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
                {
                    hv_ROIRows.Dispose();
                    hv_ROIRows = new HTuple();
                    hv_ROICols.Dispose();
                    hv_ROICols = new HTuple();
                    disp_message(hv_WindowHandle, "检测的边缘数量太少，请重新设置!", "window",
                        52, 12, "red", "false");
                    ho_objArrow.Dispose();
                    ho_Arrow.Dispose();
                    ho_Contour.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Cross.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Arrow1.Dispose();

                    hv_Rows.Dispose();
                    hv_Cols.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Button.Dispose();
                    hv_i.Dispose();
                    hv_Distance.Dispose();
                    hv_ExceptionT.Dispose();
                    hv_Length1.Dispose();
                    hv_RowC.Dispose();
                    hv_ColumnC.Dispose();
                    hv_Radius.Dispose();
                    hv_StartPhi.Dispose();
                    hv_EndPhi.Dispose();
                    hv_PointOrder.Dispose();
                    hv_RowXLD.Dispose();
                    hv_ColXLD.Dispose();
                    hv_Row1.Dispose();
                    hv_Column1.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_DistanceStart.Dispose();
                    hv_DistanceEnd.Dispose();
                    hv_Length2.Dispose();
                    hv_j.Dispose();
                    hv_RowE.Dispose();
                    hv_ColE.Dispose();
                    hv_ATan.Dispose();
                    hv_RowL2.Dispose();
                    hv_RowL1.Dispose();
                    hv_ColL2.Dispose();
                    hv_ColL1.Dispose();

                    return;
                }
                //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
                //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
                HTuple end_val86 = hv_Elements - 1;
                HTuple step_val86 = 1;
                for (hv_i = 0; hv_i.Continue(end_val86, step_val86); hv_i = hv_i.TupleAdd(step_val86))
                {

                    if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                        hv_Length2 - 1)))) != 0)
                    {
                        //xld的起点和终点坐标相对，为圆
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_j.Dispose();
                            HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                        }

                    }
                    else
                    {
                        //否则为圆弧
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_j.Dispose();
                            HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                        }
                    }
                    //索引越界，强制赋值为最后一个索引
                    if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                    {
                        hv_j.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_j = hv_Length2 - 1;
                        }
                        //continue
                    }
                    //获取卡尺工具中心
                    hv_RowE.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowE = hv_RowXLD.TupleSelect(
                            hv_j);
                    }
                    hv_ColE.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColE = hv_ColXLD.TupleSelect(
                            hv_j);
                    }

                    //如果圆心到检测方向直线的起点的距离大于圆心到检测方向直线的终点的距离，搜索方向由圆外指向圆心
                    //如果圆心到检测方向直线的起点的距离不大于圆心到检测方向直线的终点的距离，搜索方向由圆心指向圆外
                    if ((int)(new HTuple(hv_DistanceStart.TupleGreater(hv_DistanceEnd))) != 0)
                    {
                        //求卡尺工具的边缘搜索方向
                        //求圆心指向边缘的矢量的角度
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ATan.Dispose();
                            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        }
                        //角度反向
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ATan = ((new HTuple(180)).TupleRad()
                                    ) + hv_ATan;
                                hv_ATan.Dispose();
                                hv_ATan = ExpTmpLocalVar_ATan;
                            }
                        }
                        //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                        hv_Direct.Dispose();
                        hv_Direct = "inner";
                    }
                    else
                    {
                        //求卡尺工具的边缘搜索方向
                        //求圆心指向边缘的矢量的角度
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ATan.Dispose();
                            HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                        }
                        //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                        hv_Direct.Dispose();
                        hv_Direct = "outer";
                    }

                    //产生卡尺xld，并保持到显示对象
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE,
                            hv_ATan, hv_DetectHeight / 2, hv_DetectWidth / 2);
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }

                    //用箭头xld指示边缘搜索方向，并保持到显示对象
                    if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                    {
                        hv_RowL2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()
                                ));
                        }
                        hv_RowL1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()
                                ));
                        }
                        hv_ColL2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()
                                ));
                        }
                        hv_ColL1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()
                                ));
                        }
                        ho_Arrow1.Dispose();
                        gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                            25, 25);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                            ho_Regions.Dispose();
                            ho_Regions = ExpTmpOutVar_0;
                        }
                    }
                }

                ho_objArrow.Dispose();
                ho_Arrow.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                hv_Rows.Dispose();
                hv_Cols.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Button.Dispose();
                hv_i.Dispose();
                hv_Distance.Dispose();
                hv_ExceptionT.Dispose();
                hv_Length1.Dispose();
                hv_RowC.Dispose();
                hv_ColumnC.Dispose();
                hv_Radius.Dispose();
                hv_StartPhi.Dispose();
                hv_EndPhi.Dispose();
                hv_PointOrder.Dispose();
                hv_RowXLD.Dispose();
                hv_ColXLD.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_DistanceStart.Dispose();
                hv_DistanceEnd.Dispose();
                hv_Length2.Dispose();
                hv_j.Dispose();
                hv_RowE.Dispose();
                hv_ColE.Dispose();
                hv_ATan.Dispose();
                hv_RowL2.Dispose();
                hv_RowL1.Dispose();
                hv_ColL2.Dispose();
                hv_ColL1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_objArrow.Dispose();
                ho_Arrow.Dispose();
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Cross.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                hv_Rows.Dispose();
                hv_Cols.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Button.Dispose();
                hv_i.Dispose();
                hv_Distance.Dispose();
                hv_ExceptionT.Dispose();
                hv_Length1.Dispose();
                hv_RowC.Dispose();
                hv_ColumnC.Dispose();
                hv_Radius.Dispose();
                hv_StartPhi.Dispose();
                hv_EndPhi.Dispose();
                hv_PointOrder.Dispose();
                hv_RowXLD.Dispose();
                hv_ColXLD.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_DistanceStart.Dispose();
                hv_DistanceEnd.Dispose();
                hv_Length2.Dispose();
                hv_j.Dispose();
                hv_RowE.Dispose();
                hv_ColE.Dispose();
                hv_ATan.Dispose();
                hv_RowL2.Dispose();
                hv_RowL1.Dispose();
                hv_ColL2.Dispose();
                hv_ColL1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void spoke(HObject ho_Image, out HObject ho_Regions, HTuple hv_Elements,
    HTuple hv_DetectHeight, HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold,
    HTuple hv_Transition, HTuple hv_Select, HTuple hv_ROIRows, HTuple hv_ROICols,
    HTuple hv_Direct, out HTuple hv_ResultRow, out HTuple hv_ResultColumn, out HTuple hv_ArcType)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Contour, ho_ContCircle, ho_Rectangle1 = null;
            HObject ho_Arrow1 = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_RowC = new HTuple(), hv_ColumnC = new HTuple();
            HTuple hv_Radius = new HTuple(), hv_StartPhi = new HTuple();
            HTuple hv_EndPhi = new HTuple(), hv_PointOrder = new HTuple();
            HTuple hv_RowXLD = new HTuple(), hv_ColXLD = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_i = new HTuple();
            HTuple hv_j = new HTuple(), hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
            HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_k = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = new HTuple(hv_Select);
            HTuple hv_Transition_COPY_INP_TMP = new HTuple(hv_Transition);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();
            hv_ArcType = new HTuple();
            //获取图像尺寸
            hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //产生一个空显示对象，用于显示
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            //初始化边缘坐标数组
            hv_ResultRow.Dispose();
            hv_ResultRow = new HTuple();
            hv_ResultColumn.Dispose();
            hv_ResultColumn = new HTuple();

            //产生xld
            ho_Contour.Dispose();
            HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ROIRows, hv_ROICols);
            //用回归线法（不抛出异常点，所有点权重一样）拟合圆
            hv_RowC.Dispose(); hv_ColumnC.Dispose(); hv_Radius.Dispose(); hv_StartPhi.Dispose(); hv_EndPhi.Dispose(); hv_PointOrder.Dispose();
            HOperatorSet.FitCircleContourXld(ho_Contour, "algebraic", -1, 0, 0, 1, 2, out hv_RowC,
                out hv_ColumnC, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
            //根据拟合结果产生xld，并保持到显示对象
            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, hv_RowC, hv_ColumnC, hv_Radius,
                hv_StartPhi, hv_EndPhi, hv_PointOrder, 3);
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConcatObj(ho_Regions, ho_ContCircle, out ExpTmpOutVar_0);
                ho_Regions.Dispose();
                ho_Regions = ExpTmpOutVar_0;
            }

            //获取圆或圆弧xld上的点坐标
            hv_RowXLD.Dispose(); hv_ColXLD.Dispose();
            HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);

            //求圆或圆弧xld上的点的数量
            hv_Length2.Dispose();
            HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
            if ((int)(new HTuple(hv_Elements.TupleLess(3))) != 0)
            {
                //    disp_message (WindowHandle, '检测的边缘数量太少，请重新设置!', 'window', 52, 12, 'red', 'false')
                ho_Contour.Dispose();
                ho_ContCircle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Arrow1.Dispose();

                hv_Select_COPY_INP_TMP.Dispose();
                hv_Transition_COPY_INP_TMP.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_RowC.Dispose();
                hv_ColumnC.Dispose();
                hv_Radius.Dispose();
                hv_StartPhi.Dispose();
                hv_EndPhi.Dispose();
                hv_PointOrder.Dispose();
                hv_RowXLD.Dispose();
                hv_ColXLD.Dispose();
                hv_Length2.Dispose();
                hv_i.Dispose();
                hv_j.Dispose();
                hv_RowE.Dispose();
                hv_ColE.Dispose();
                hv_ATan.Dispose();
                hv_RowL2.Dispose();
                hv_RowL1.Dispose();
                hv_ColL2.Dispose();
                hv_ColL1.Dispose();
                hv_MsrHandle_Measure.Dispose();
                hv_RowEdge.Dispose();
                hv_ColEdge.Dispose();
                hv_Amplitude.Dispose();
                hv_Distance.Dispose();
                hv_tRow.Dispose();
                hv_tCol.Dispose();
                hv_t.Dispose();
                hv_Number.Dispose();
                hv_k.Dispose();

                return;
            }
            //如果xld是圆弧，有Length2个点，从起点开始，等间距（间距为Length2/(Elements-1)）取Elements个点，作为卡尺工具的中点
            //如果xld是圆，有Length2个点，以0°为起点，从起点开始，等间距（间距为Length2/(Elements)）取Elements个点，作为卡尺工具的中点
            HTuple end_val27 = hv_Elements - 1;
            HTuple step_val27 = 1;
            for (hv_i = 0; hv_i.Continue(end_val27, step_val27); hv_i = hv_i.TupleAdd(step_val27))
            {

                if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                    hv_Length2 - 1)))) != 0)
                {
                    //xld的起点和终点坐标相对，为圆
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_j.Dispose();
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / hv_Elements) * hv_i, out hv_j);
                    }
                    hv_ArcType.Dispose();
                    hv_ArcType = "circle";
                }
                else
                {
                    //否则为圆弧
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_j.Dispose();
                        HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements - 1)) * hv_i, out hv_j);
                    }
                    hv_ArcType.Dispose();
                    hv_ArcType = "arc";
                }
                //索引越界，强制赋值为最后一个索引
                if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                {
                    hv_j.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_j = hv_Length2 - 1;
                    }
                    //continue
                }
                //获取卡尺工具中心
                hv_RowE.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowE = hv_RowXLD.TupleSelect(
                        hv_j);
                }
                hv_ColE.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColE = hv_ColXLD.TupleSelect(
                        hv_j);
                }

                //超出图像区域，不检测，否则容易报异常
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                {
                    continue;
                }
                //边缘搜索方向类型：'inner'搜索方向由圆外指向圆心；'outer'搜索方向由圆心指向圆外
                if ((int)(new HTuple(hv_Direct.TupleEqual("inner"))) != 0)
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ATan.Dispose();
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                    }
                    //角度反向
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ATan = ((new HTuple(180)).TupleRad()
                                ) + hv_ATan;
                            hv_ATan.Dispose();
                            hv_ATan = ExpTmpLocalVar_ATan;
                        }
                    }
                }
                else
                {
                    //求卡尺工具的边缘搜索方向
                    //求圆心指向边缘的矢量的角度
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ATan.Dispose();
                        HOperatorSet.TupleAtan2((-hv_RowE) + hv_RowC, hv_ColE - hv_ColumnC, out hv_ATan);
                    }
                }


                //产生卡尺xld，并保持到显示对象
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle1, hv_RowE, hv_ColE, hv_ATan,
                        hv_DetectHeight / 2, hv_DetectWidth / 2);
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle1, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                //用箭头xld指示边缘搜索方向，并保持到显示对象
                if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                {
                    hv_RowL2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowL2 = hv_RowE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()
                            ));
                    }
                    hv_RowL1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowL1 = hv_RowE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleSin()
                            ));
                    }
                    hv_ColL2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColL2 = hv_ColE + ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()
                            ));
                    }
                    hv_ColL1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColL1 = hv_ColE - ((hv_DetectHeight / 2) * (((-hv_ATan)).TupleCos()
                            ));
                    }
                    ho_Arrow1.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                        25, 25);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                }


                //产生测量对象句柄
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MsrHandle_Measure.Dispose();
                    HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, hv_DetectHeight / 2,
                        hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);
                }

                //设置极性
                if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                {
                    hv_Transition_COPY_INP_TMP.Dispose();
                    hv_Transition_COPY_INP_TMP = "negative";
                }
                else
                {
                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                    {

                        hv_Transition_COPY_INP_TMP.Dispose();
                        hv_Transition_COPY_INP_TMP = "positive";
                    }
                    else
                    {
                        hv_Transition_COPY_INP_TMP.Dispose();
                        hv_Transition_COPY_INP_TMP = "all";
                    }
                }
                //设置边缘位置。最强点是从所有边缘中选择幅度绝对值最大点，需要设置为'all'
                if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                {
                    hv_Select_COPY_INP_TMP.Dispose();
                    hv_Select_COPY_INP_TMP = "first";
                }
                else
                {
                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                    {

                        hv_Select_COPY_INP_TMP.Dispose();
                        hv_Select_COPY_INP_TMP = "last";
                    }
                    else
                    {
                        hv_Select_COPY_INP_TMP.Dispose();
                        hv_Select_COPY_INP_TMP = "all";
                    }
                }
                //检测边缘
                hv_RowEdge.Dispose(); hv_ColEdge.Dispose(); hv_Amplitude.Dispose(); hv_Distance.Dispose();
                HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure, hv_Sigma, hv_Threshold,
                    hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                    out hv_Amplitude, out hv_Distance);
                //清除测量对象句柄
                HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                //临时变量初始化
                //tRow，tCol保存找到指定边缘的坐标
                hv_tRow.Dispose();
                hv_tRow = 0;
                hv_tCol.Dispose();
                hv_tCol = 0;
                //t保存边缘的幅度绝对值
                hv_t.Dispose();
                hv_t = 0;
                hv_Number.Dispose();
                HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                //找到的边缘必须至少为1个
                if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                {
                    continue;
                }
                //有多个边缘时，选择幅度绝对值最大的边缘
                HTuple end_val120 = hv_Number - 1;
                HTuple step_val120 = 1;
                for (hv_k = 0; hv_k.Continue(end_val120, step_val120); hv_k = hv_k.TupleAdd(step_val120))
                {
                    if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                        hv_t))) != 0)
                    {

                        hv_tRow.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_tRow = hv_RowEdge.TupleSelect(
                                hv_k);
                        }
                        hv_tCol.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_tCol = hv_ColEdge.TupleSelect(
                                hv_k);
                        }
                        hv_t.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_t = ((hv_Amplitude.TupleSelect(
                                hv_k))).TupleAbs();
                        }
                    }
                }
                //把找到的边缘保存在输出数组
                if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                {

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ResultRow = hv_ResultRow.TupleConcat(
                                hv_tRow);
                            hv_ResultRow.Dispose();
                            hv_ResultRow = ExpTmpLocalVar_ResultRow;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ResultColumn = hv_ResultColumn.TupleConcat(
                                hv_tCol);
                            hv_ResultColumn.Dispose();
                            hv_ResultColumn = ExpTmpLocalVar_ResultColumn;
                        }
                    }
                }
            }


            ho_Contour.Dispose();
            ho_ContCircle.Dispose();
            ho_Rectangle1.Dispose();
            ho_Arrow1.Dispose();

            hv_Select_COPY_INP_TMP.Dispose();
            hv_Transition_COPY_INP_TMP.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_RowC.Dispose();
            hv_ColumnC.Dispose();
            hv_Radius.Dispose();
            hv_StartPhi.Dispose();
            hv_EndPhi.Dispose();
            hv_PointOrder.Dispose();
            hv_RowXLD.Dispose();
            hv_ColXLD.Dispose();
            hv_Length2.Dispose();
            hv_i.Dispose();
            hv_j.Dispose();
            hv_RowE.Dispose();
            hv_ColE.Dispose();
            hv_ATan.Dispose();
            hv_RowL2.Dispose();
            hv_RowL1.Dispose();
            hv_ColL2.Dispose();
            hv_ColL1.Dispose();
            hv_MsrHandle_Measure.Dispose();
            hv_RowEdge.Dispose();
            hv_ColEdge.Dispose();
            hv_Amplitude.Dispose();
            hv_Distance.Dispose();
            hv_tRow.Dispose();
            hv_tCol.Dispose();
            hv_t.Dispose();
            hv_Number.Dispose();
            hv_k.Dispose();

            return;
        }

        public void pts_to_best_circle(out HObject ho_Circle, HTuple hv_Rows, HTuple hv_Cols,
    HTuple hv_ActiveNum, HTuple hv_ArcType, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
    out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
    out HTuple hv_ArcAngle)
        {



            // Local iconic variables 

            HObject ho_Contour = null;

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_CircleLength = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            hv_RowCenter = new HTuple();
            hv_ColCenter = new HTuple();
            hv_Radius = new HTuple();
            hv_StartPhi = new HTuple();
            hv_EndPhi = new HTuple();
            hv_PointOrder = new HTuple();
            hv_ArcAngle = new HTuple();
            //初始化
            hv_RowCenter.Dispose();
            hv_RowCenter = 0;
            hv_ColCenter.Dispose();
            hv_ColCenter = 0;
            hv_Radius.Dispose();
            hv_Radius = 0;
            //产生一个空的直线对象，用于保存拟合后的圆
            ho_Circle.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Circle);
            //计算边缘数量
            hv_Length.Dispose();
            HOperatorSet.TupleLength(hv_Cols, out hv_Length);
            //当边缘数量不小于有效点数时进行拟合
            if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_ActiveNum))).TupleAnd(new HTuple(hv_ActiveNum.TupleGreater(
                2)))) != 0)
            {
                //halcon的拟合是基于xld的，需要把边缘连接成xld
                if ((int)(new HTuple(hv_ArcType.TupleEqual("circle"))) != 0)
                {
                    //如果是闭合的圆，轮廓需要首尾相连
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Contour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows.TupleConcat(hv_Rows.TupleSelect(
                            0)), hv_Cols.TupleConcat(hv_Cols.TupleSelect(0)));
                    }
                }
                else
                {
                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_Rows, hv_Cols);
                }
                //拟合圆。使用的算法是''geotukey''，其他算法请参考fit_circle_contour_xld的描述部分。
                hv_RowCenter.Dispose(); hv_ColCenter.Dispose(); hv_Radius.Dispose(); hv_StartPhi.Dispose(); hv_EndPhi.Dispose(); hv_PointOrder.Dispose();
                HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2, out hv_RowCenter,
                    out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                //判断拟合结果是否有效：如果拟合成功，数组中元素的数量大于0
                hv_Length1.Dispose();
                HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                {
                    ho_Contour.Dispose();

                    hv_Length.Dispose();
                    hv_Length1.Dispose();
                    hv_CircleLength.Dispose();

                    return;
                }
                //根据拟合结果，产生直线xld
                if ((int)(new HTuple(hv_ArcType.TupleEqual("arc"))) != 0)
                {
                    //判断圆弧的方向：顺时针还是逆时针
                    //halcon求圆弧会出现方向混乱的问题
                    //tuple_mean (Rows, RowsMean)
                    //tuple_mean (Cols, ColsMean)
                    //gen_cross_contour_xld (Cross, RowsMean, ColsMean, 6, 0.785398)
                    //gen_circle_contour_xld (Circle1, RowCenter, ColCenter, Radius, StartPhi, EndPhi, 'positive', 1)
                    //求轮廓1中心
                    //area_center_points_xld (Circle1, Area, Row1, Column1)
                    //gen_circle_contour_xld (Circle2, RowCenter, ColCenter, Radius, StartPhi, EndPhi, 'negative', 1)
                    //求轮廓2中心
                    //area_center_points_xld (Circle2, Area, Row2, Column2)
                    //distance_pp (RowsMean, ColsMean, Row1, Column1, Distance1)
                    //distance_pp (RowsMean, ColsMean, Row2, Column2, Distance2)
                    //ArcAngle := EndPhi-StartPhi
                    //if (Distance1<Distance2)

                    //PointOrder := 'positive'
                    //copy_obj (Circle1, Circle, 1, 1)
                    //else

                    //PointOrder := 'negative'
                    //if (abs(ArcAngle)>3.1415926)
                    //ArcAngle := ArcAngle-2.0*3.1415926
                    //endif
                    //copy_obj (Circle2, Circle, 1, 1)
                    //endif
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);

                    hv_CircleLength.Dispose();
                    HOperatorSet.LengthXld(ho_Circle, out hv_CircleLength);
                    hv_ArcAngle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ArcAngle = hv_EndPhi - hv_StartPhi;
                    }
                    if ((int)(new HTuple(hv_CircleLength.TupleGreater(((new HTuple(180)).TupleRad()
                        ) * hv_Radius))) != 0)
                    {
                        if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleLess((new HTuple(180)).TupleRad()
                            ))) != 0)
                        {
                            if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_ArcAngle = ((new HTuple(360)).TupleRad()
                                            ) - hv_ArcAngle;
                                        hv_ArcAngle.Dispose();
                                        hv_ArcAngle = ExpTmpLocalVar_ArcAngle;
                                    }
                                }
                            }
                            else
                            {

                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_ArcAngle = ((new HTuple(360)).TupleRad()
                                            ) + hv_ArcAngle;
                                        hv_ArcAngle.Dispose();
                                        hv_ArcAngle = ExpTmpLocalVar_ArcAngle;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((int)(new HTuple(hv_CircleLength.TupleLess(((new HTuple(180)).TupleRad()
                            ) * hv_Radius))) != 0)
                        {
                            if ((int)(new HTuple(((hv_ArcAngle.TupleAbs())).TupleGreater((new HTuple(180)).TupleRad()
                                ))) != 0)
                            {
                                if ((int)(new HTuple(hv_ArcAngle.TupleGreater(0))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ArcAngle = hv_ArcAngle - ((new HTuple(360)).TupleRad()
                                                );
                                            hv_ArcAngle.Dispose();
                                            hv_ArcAngle = ExpTmpLocalVar_ArcAngle;
                                        }
                                    }

                                }
                                else
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ArcAngle = ((new HTuple(360)).TupleRad()
                                                ) + hv_ArcAngle;
                                            hv_ArcAngle.Dispose();
                                            hv_ArcAngle = ExpTmpLocalVar_ArcAngle;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                else
                {
                    hv_StartPhi.Dispose();
                    hv_StartPhi = 0;
                    hv_EndPhi.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_EndPhi = (new HTuple(360)).TupleRad()
                            ;
                    }
                    hv_ArcAngle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ArcAngle = (new HTuple(360)).TupleRad()
                            ;
                    }
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                }
            }

            ho_Contour.Dispose();

            hv_Length.Dispose();
            hv_Length1.Dispose();
            hv_CircleLength.Dispose();

            return;
        }


        // Procedures 
        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
           HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
            HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
            HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
            HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            hv_Length.Dispose();
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZeroLengthIndices = hv_Length.TupleFind(
                    0);
            }
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            }
            hv_DC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            }
            hv_HalfHeadWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            }
            //
            //Calculate end points of the arrow head.
            hv_RowP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            }
            hv_RowP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            }
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                            hv_Column1.TupleSelect(hv_Index));
                    }
                }
                else
                {
                    //Create arrow contour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            hv_Length.Dispose();
            hv_ZeroLengthIndices.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_HalfHeadWidth.Dispose();
            hv_RowP1.Dispose();
            hv_ColP1.Dispose();
            hv_RowP2.Dispose();
            hv_ColP2.Dispose();
            hv_Index.Dispose();

            return;
        }

        public OperationResult Fit_Circle(HObject image,CircleParams circle,out HObject hRegion,out HTuple resultRow,
            out HObject result_Circle, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
            out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
            out HTuple hv_ArcAngle)
        {
            hv_RowCenter = new HTuple();
            hv_RowCenter = new HTuple();
            hv_ColCenter = new HTuple();
            hv_Radius = new HTuple();
            hv_StartPhi = new HTuple();
            hv_EndPhi = new HTuple();
            hv_PointOrder = new HTuple();
            hv_ArcAngle = new HTuple();
            result_Circle = null;
            resultRow=null;
            hRegion=null;
            try
            {
                spoke(image, out hRegion, circle.circle_Elements, circle.circle_Caliper_Height,circle.circle_Caliper_Width
                    , circle.circle_Sigma, circle.circle_Threshold,circle.circle_Transition ,circle.circle_Point_Select, circle.rOI_X, circle.rOI_Y, 
                    circle.circle_Direct,out resultRow, out HTuple resultCol, out HTuple resultArcType);

                if (resultRow.Length > circle.circle_Min_Points_Num)
                {
                    HOperatorSet.GenCrossContourXld(out HObject cross, resultRow, resultCol, 20, new HTuple(45));
                    pts_to_best_circle(out result_Circle, resultRow, resultCol,circle.circle_Min_Points_Num,resultArcType, out  hv_RowCenter, out  hv_ColCenter,
                    out  hv_Radius, out  hv_StartPhi, out  hv_EndPhi, out hv_PointOrder,out  hv_ArcAngle);
                    return OperationResult.CreateSuccessResult();
                }
                else
                {
                    result_Circle = null;
                    return OperationResult.CreateFailResult("拟合圆找到边缘点数太少");
                }
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("拟合圆失败", ex);
            }

        }
    }
}
