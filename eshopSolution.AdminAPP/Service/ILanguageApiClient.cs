using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Service
{
    public interface ILanguageApiClient
    {
        public Task<APIResultMessage<List<LanguageVM>>> GetAll();
    }
}