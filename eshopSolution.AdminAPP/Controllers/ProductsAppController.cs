using eShopSolutio.Utilities.Contains;
using eshopSolution.AdminAPP.Service;
using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopSolution.AdminAPP.Controllers
{
    public class productsAppController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public productsAppController(IProductApiClient productApiClient, IConfiguration configuration,
            ICategoryApiClient categoryApiClient)
        {
            _productApiClient = productApiClient;
            _configuration = configuration;
            _categoryApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int? categoryID, int pageindex = 1, int pagesize = 10)
        {
            try
            {
                var languageId = HttpContext.Session.GetString(SystemConstains.AppSetting.DefaultLanguageIdsys);
                var request = new GetManageProductPagingRequest()
                {
                    Keyword = keyword,
                    PageIndex = pageindex,
                    PageSize = pagesize,
                    LanguageId = HttpContext.Session.GetString("DefaultLanguageId"),
                    CategoryId = categoryID,
                };

                var result = await _productApiClient.GetAll(request);
                ViewBag.Keyword = keyword;
                var categories = await _categoryApiClient.GetAll(languageId);
                ViewBag.categories = categories.ResultObj.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = categoryID.HasValue && categoryID.Value == x.Id
                });
                if (TempData["result"] != null)
                {
                    ViewBag.SuccessMsg = TempData["result"];
                }
                return View(result.ResultObj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SuggestSearch()
        {
            var keyword = HttpContext.Request.Query["term"].ToString();
            var result = await _productApiClient.SuggestSearch(keyword);
            if (result.IsSuccessed)
                return Ok(result.ResultObj);
            return Ok(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var result = await _productApiClient.GetProductId(id);
            return View(result.ResultObj);
        }

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(string name, decimal price, decimal orOriginalPrice,
            int stock, string desDescription, string details, string seoDescription, string seoTitle, string seoAlias, IFormFile thumbnailImage)
        {
            var request = new ProductCreateRequest()
            {
                Name = name,
                Price = price,
                OrOriginalPrice = orOriginalPrice,
                Stock = stock,
                DesDescription = desDescription,
                Details = details,
                SeoDescription = seoDescription,
                SeoTitle = seoTitle,
                SeoAlias = seoAlias,
                ThumbnailImage = thumbnailImage,
            };
            var result = await _productApiClient.Created(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Created successfully!";
                //return RedirectToAction("Index", "ProductsApp");
                return Json(TempData["result"].ToString());
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var productbyid = await _productApiClient.GetProductId(id);
            if (productbyid.IsSuccessed)
            {
                var product = productbyid.ResultObj;
                var updaterequest = new ProductUpdateRequest()
                {
                    Id = product.Id,
                    Name = product.Name,
                    NewPrice = product.Price,
                    NeworOriginalPrice = product.OrOriginalPrice,
                    Stock = product.Stock,
                    Description = product.DesDescription,
                    SeoTitle = product.SeoTitle,
                };
                return View(updaterequest);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var result = await _productApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Update successfully!";
                return RedirectToAction("Index", "ProductsApp");
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        //[HttpGet]
        //public IActionResult Delete(int Id)
        //{
        //    var delete = new ProductDeleteRequest()
        //    {
        //        Id = Id,
        //    };
        //    return PartialView("_DeleteProductModelPartial", delete);
        //}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productApiClient.Delete(id);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Delete successfully!";
                RedirectToAction("Index", "ProductsApp");
                return Json(TempData["result"].ToString());
            }
            ModelState.AddModelError("", result.Message);
            return View();
        }

        private async Task<CategoryAssignRequest> GetCategory(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstains.AppSetting.DefaultLanguageIdsys);
            var productObj = await _productApiClient.GetProductId(id);
            var Categories = await _categoryApiClient.GetAll(languageId);
            var categoryAssign = new CategoryAssignRequest();
            foreach (var category in Categories.ResultObj)
            {
                categoryAssign.Categories.Add(new SelectItem()
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Selected = productObj.ResultObj.Categories.Contains(category.Name),
                });
            }
            return categoryAssign;
        }

        [HttpGet]
        public async Task<IActionResult> AssignCategory(int id)
        {
            var getcategaries = await GetCategory(id);
            return View(getcategaries);
        }

        [HttpPost]
        public async Task<IActionResult> AssignCategory(CategoryAssignRequest request)
        {
            var result = await _productApiClient.CategoryAssign(request.id, request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Assign Successfully!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            var roleAssignRequst = await GetCategory(request.id);
            return View(roleAssignRequst);
        }
    }
}