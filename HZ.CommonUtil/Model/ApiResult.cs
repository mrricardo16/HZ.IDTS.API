using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HZ.CommonUtil.Model
{
    /// <summary>
    /// 统一接口返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult
    {
        public ApiResult()
        {
            StatusCode = 0;
        }

        /// <summary>
        /// 请求状态
        /// </summary>
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }
        public int ErrCode { get; set; }

        public Object Data { get; set; }
        /// <summary>
        /// 返回时间戳
        /// </summary>
        public string Timestamp { get; set; } = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();

        public static ApiResult Success(string msg = "",object data=null)
        {
            return new ApiResult()
            {
                StatusCode = (int)StatusCodeType.Success,
                IsSuccess = true,
                Message = msg,
                Data=data
            };
        }
        public static ApiResult Error(string msg = "",int errCode=0)
        {
            return new ApiResult()
            {
                StatusCode = (int)StatusCodeType.BussError,
                Message = msg,
                ErrCode = errCode
            };
        }
        public static ApiResult ErrorTip(string msg = "", int errCode = 0)
        {
            return new ApiResult()
            {
                StatusCode = (int)StatusCodeType.ErrTipCode,
                Message = msg,
                ErrCode = errCode
            };
        }
        public static ApiResult Error(Exception ex, int errCode = 0)
        {
            return new ApiResult()
            {
                StatusCode = (int)StatusCodeType.Error,
                Message = ex.Message,
                ErrCode = errCode
            };
        }
    

    }

    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 接口返回值
        /// </summary>
        public T Data;

        public static ApiResult<T> Success(T data, string msg = "")
        {
            return new ApiResult<T>()
            {
                Data = data,
                StatusCode = (int)StatusCodeType.Success,
                 IsSuccess=true,
                Message = msg,
            };
        }
    }
}
