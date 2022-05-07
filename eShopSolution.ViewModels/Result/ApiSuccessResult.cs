using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Result
{
    public class ApiSuccessResult<T> : APIResultMessage<T>
    {
        public ApiSuccessResult()
        {
            IsSuccessed = true;
        }

        public ApiSuccessResult(T resultObj)
        {
            IsSuccessed = true;
            ResultObj = resultObj;
        }
    }
}