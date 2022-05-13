using eshopSolution.AdminAPP.Models;
using eshopSolution.AdminAPP.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Controllers.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ILanguageApiClient _languageApiClient;

        public NavigationViewComponent(ILanguageApiClient languageApiClient)
        {
            _languageApiClient = languageApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var languages = await _languageApiClient.GetAll();
            var navigationVM = new NavigationViewModel()
            {
                CurrentLanguageID = HttpContext.Session
                .GetString("DefaultLanguageId"),
                Langguages = languages.ResultObj,
            };

            return View("Default", navigationVM);
        }
    }
}