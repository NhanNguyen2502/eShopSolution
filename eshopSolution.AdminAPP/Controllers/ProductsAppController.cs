using eshopSolution.AdminAPP.Service;
using eShopSolution.ViewModels.Catalog.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace eshopSolution.AdminAPP.Controllers
{
    public class productsAppController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IProductApiClient _productApiClient;

        public productsAppController(IProductApiClient productApiClient, IConfiguration configuration)
        {
            _productApiClient = productApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int pageindex = 1, int pagesize = 10)
        {
            try
            {
                var request = new GetManageProductPagingRequest()
                {
                    Keyword = keyword,
                    PageIndex = pageindex,
                    PageSize = pagesize,
                    LanguageId = HttpContext.Session.GetString("DefaultLanguageId"),
                };

                var result = await _productApiClient.GetAll(request);
                ViewBag.Keyword = keyword;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            var result = await _productApiClient.Created(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Created successfully!";
                return RedirectToAction("Index", "ProductsApp");
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
        [HttpPost]
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
    }
}