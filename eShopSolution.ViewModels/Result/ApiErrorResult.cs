using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Result
{
    public class ApiErrorResult<T> : APIResultMessage<T>
    {
        public string[] validationErrors { get; set; }

        public ApiErrorResult()
        {
            IsSuccessed = false;
        }

        public ApiErrorResult(string message)
        {
            IsSuccessed = false;
            Message = message;
        }

        public ApiErrorResult(string[] message)
        {
            IsSuccessed = false;
            validationErrors = message;
        }
    }
}