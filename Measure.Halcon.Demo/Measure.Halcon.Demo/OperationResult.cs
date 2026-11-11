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
        public string Msg { get; set; }


        public static OperationResult CreateSuccessResult()
        {
            return new OperationResult()
            {
                IsSuccess = true,
                Msg = "Success"
            };
        }

        public static OperationResult CreateSuccessResult(string msg)
        {
            return new OperationResult()
            {
                IsSuccess = true,
                Msg = msg
            };
        }

        public static OperationResult CreateFailResult(string errorMsg, Exception error)
        {
            return new OperationResult()
            {
                IsSuccess = false,
                Msg = errorMsg,
                Error = error
            };
        }

        public static OperationResult CreateFailResult(string errorMsg)
        {
            return new OperationResult()
            {
                IsSuccess = false,
                Msg = errorMsg,
                Error = null
            };
        }



    }

    public class OperationResult<T> : OperationResult
    {
        public T Data { get; set; }

        public static OperationResult<T> CreateSuccessResult(T data, string msg = "成功")
            => new OperationResult<T> { IsSuccess = true, Msg = msg, Data = data };

        public static new OperationResult<T> CreateFailResult(string msg)
            => new OperationResult<T> { IsSuccess = false, Msg = msg };

        public static new OperationResult<T> CreateFailResult(string msg, Exception ex)
            => new OperationResult<T> { IsSuccess = false, Msg = msg, Error = ex };
    }



}
