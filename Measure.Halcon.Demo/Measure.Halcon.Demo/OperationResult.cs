using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    public class OperationResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        public Exception Error { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }


        public static OperationResult CreateSuccessResult()
        {
            return new OperationResult()
            {
                IsSuccess = true,
                ErrorMsg = "Success"
            };
        }

        public static OperationResult CreateFailResult(string errorMsg, Exception error)
        {
            return new OperationResult()
            {
                IsSuccess = false,
                ErrorMsg = errorMsg,
                Error = error
            };
        }

        

    }
}
