using HalconDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Helper
{
   //圆参数
    internal class CircleParams
    {
        public CircleParams() {
            circle_Elements = 60;
            circle_Caliper_Height = 60;
            circle_Caliper_Width = 15;
            circle_Min_Points_Num = 5;
            circle_Threshold = 15;
            circle_Sigma = 1;
            circle_Transition = "all";
            circle_Point_Select = "max";
            circle_Dis_Min = 1;
            circle_Dis_Max = 99;
            scale = 1.0;

        }


        public override string ToString()
        {
            return $"卡尺[数={circle_Elements}, 高={circle_Caliper_Height}, 宽={circle_Caliper_Width}], " +
                   $"边缘[最小点数={circle_Min_Points_Num}, 阈值={circle_Threshold}, 滤波={circle_Sigma}, " +
                   $"极性={circle_Transition}, 点选择={circle_Point_Select}], " +
                   $"直径=[{circle_Dis_Min},{circle_Dis_Max}], 标定={scale}, " +
                   $"ROI点数={TupleLen(rOI_X)}, 圆心点数={TupleLen(circleCenter_X)}, " +
                   $"半径点数={TupleLen(circleRadius)}, 距离={distance}";
        }

        private static int TupleLen(HTuple t)
        {
            return (t == null) ? 0 : t.Length;
        }

        //卡尺个数
        /// <summary>
        /// 卡尺数量
        /// </summary>
        public int circle_Elements;


        //圆卡尺工具宽度
        public int circle_Caliper_Width;

        //圆卡尺工具高度
        public int circle_Caliper_Height;


        //拟合圆最小点数
        public int circle_Min_Points_Num;

        //圆边缘点阈值
        public int circle_Threshold;

        //圆边缘点滤波系数
        public double circle_Sigma;

        //圆边缘点极性
        public string circle_Transition;

        //圆边缘点选择
        public string circle_Point_Select;


        //圆最小直径
        public double circle_Dis_Min;


        //圆最大直径
        public double circle_Dis_Max;

        //像素标定系数

        public double scale;

        public HTuple rOI_X;

        public HTuple rOI_Y;

        public HTuple circle_Direct;

        //输出圆数据
        public HTuple circleCenter_X;

        public HTuple circleCenter_Y;

        public HTuple circleRadius;

        //输出结果
        public double distance = 0.0;

    }
}
