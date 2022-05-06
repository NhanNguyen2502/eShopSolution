using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Result
{
    public class APIResultMessage<T>
    {
        public string Message { get; set; }
        public bool IsSuccessed { get; set; }
        public T ResultObj { get; set; }
    }
}