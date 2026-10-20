using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo.Helper

{
    //模板参数
    internal class MathParams

    {

        public MathParams ()
        {
            startAngle = 45;
            rangAngle = 90;
            score = 0.5;
            numMatchs = 1;
            overlap = 0.1;
            greediness = 0.9;
            scaleRMin = 0.8;
            scaleRMax = 1.2;
            scaleCMin = 0.8;
            scaleCMax = 1.2;
            HOperatorSet.GenEmptyObj(out modelRegion);
        }

        //起始角度
        public double startAngle;

        //角度范围
        public double rangAngle;

        //最小分数
        public double score;

        //行最小缩放
        public double scaleRMin;

        //行最大缩放
        public double scaleRMax;

        //行最小缩放
        public double scaleCMin;

        //列最大缩放
        public double scaleCMax;

        //贪心算法
        public double greediness;

        //匹配数量
        public int numMatchs;

        //最大重叠度
        public double overlap;

        //极性
        public HTuple metric;

        public HObject modelRegion;

        public HTuple modelID;

        ~MathParams()
        {
            if (modelID != null) { 
                HOperatorSet.ClearShapeModel(modelID);
            }

        }
    }
}
