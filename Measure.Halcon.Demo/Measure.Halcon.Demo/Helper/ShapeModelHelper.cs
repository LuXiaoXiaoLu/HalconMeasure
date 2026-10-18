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
        public OperationResult CreateShapeModel(ref HTuple HwindowHandle ,ref HObject image, MathParams mathParams)
        {

            //Draw
            HOperatorSet.DrawRectangle2(HwindowHandle, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);

            //gen
            HOperatorSet.GenRectangle2(out mathParams.modelRegion, row, column, phi, length1, length2);
            // ReduceDomain
            HOperatorSet.ReduceDomain(image, mathParams.modelRegion, out HObject imageReduced);
            // create
            HOperatorSet.CreateShapeModel(imageReduced,"auto",mathParams.startAngle,mathParams.rangAngle,"auto","auto", "use_polarity", "auto", "auto", out mathParams.modelID);

            HOperatorSet.DispObj(mathParams.modelRegion,HwindowHandle);
            if (mathParams.modelID == null) {
                return OperationResult.CreateSuccessResult();
            }
            else
            {
                return OperationResult.CreateFailResult();
            }
        }
    }
}
