using eShopSolution.ViewModels.System.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Models
{
    public class NavigationViewModel
    {
        public List<LanguageVM> Langguages { get; set; }
        public string CurrentLanguageID { get; set; }
    }
}