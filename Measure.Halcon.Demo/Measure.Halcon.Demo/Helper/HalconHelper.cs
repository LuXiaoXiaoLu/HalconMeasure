using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            catch (Exception ex )
            {
                hwindowHandle = null;
                return new OperationResult()
                {
                    IsSuccess = false,
                    ErrorMsg = ex.Message
                };
            }
        }
        //判断HObject对象是否有效
        public bool IsObjectValid(HObject Hobject) {
            if (Hobject == null) return false;
            if(!Hobject.IsInitialized()) return false;
            if(Hobject.CountObj()==0) return false;
            return true;
        }
    }
}
