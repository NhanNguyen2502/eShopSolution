using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Product;
using eShopSolution.ViewModels.Catalog.ProductImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eShopSolutionBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class productsController : ControllerBase
    {
        private IProductService _IProductService;

        public productsController(IProductService iproductService)
        {
            _IProductService = iproductService;
        }

        [HttpGet("public-paging/{languageid}")]
        public async Task<IActionResult> GetAllPaging(string languageid, [FromQuery] GetPublicProductPagingRequest request)
        {
            try
            {
                var product = await _IProductService.GetAllCategory(languageid, request);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getproductpaging")]
        public async Task<IActionResult> GetProductPaging([FromQuery] GetManageProductPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _IProductService.GetAllPaging(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{productid}/{LanguageId}")]
        public async Task<IActionResult> ProductByID(int productid, string LanguageId)
        {
            try
            {
                var product = await _IProductService.GetProductId(productid, LanguageId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _IProductService.Create(request);
            if (!product.IsSuccessed)
                return Ok(product);
            //var product = await _IProductService.GetProductId((int)productID.Data, request.LanguageId);
            //return CreatedAtAction(nameof(ProductByID), productID.Data, product);
            return Ok(product);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] ProductUpdateRequest request)
        {
            var affectedResult = await _IProductService.Update(request);
            if (affectedResult.IsSuccessed)
                return Ok(affectedResult);
            return Ok(affectedResult);
        }

        [HttpGet("ListImage/{productid}")]
        public async Task<IActionResult> getlistimage(int productid)
        {
            var ListImage = await _IProductService.GetListImage(productid);
            if (ListImage?.Count > 0)
                return Ok(ListImage);
            return BadRequest("Data can not found");
        }

        [HttpDelete("{productid}")]
        public async Task<IActionResult> Delete(int productid)
        {
            var affectedResult = await _IProductService.Delete(productid);
            return Ok(affectedResult);
        }

        [HttpPut("updatePrice")]
        public async Task<IActionResult> UpdatePrice([FromForm] ProductUpdateRequest request)
        {
            var result = await _IProductService.UpdatePrice(request);
            if (result.IsSuccessed)
                return Ok(result);
            return Ok(result);
        }

        [HttpPost("AddImage/{productid}")]
        public async Task<IActionResult> Addimage(int productid, [FromForm] ProductImageCreateRequest request)
        {
            var productimage = await _IProductService.AddIamges(productid, request);
            if (productimage == 0)
                return BadRequest("Add Fail");
            return Ok("Add successfully");
        }

        [HttpPut("UpdateImage/{imageid}")]
        public async Task<IActionResult> UpdateImage(int imageid, int productid, [FromForm] ProductImageUpdateRequest request)
        {
            var issuccessfull = await _IProductService.UpdateImage(imageid, productid, request);
            if (issuccessfull == 0)
                return BadRequest("Update Fail!");
            return Ok("Update Successfully!");
        }

        [HttpDelete("delete/{imageid}")]
        public async Task<IActionResult> RemoveImage(int imageid)
        {
            var affectedResult = await _IProductService.RemoveImages(imageid);
            return Ok(affectedResult);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword, string LanguageId)
        {
            try
            {
                var names = await _IProductService.Search(keyword, LanguageId);
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("categories/{Id}/category")]
        public async Task<IActionResult> CategoriesAssign(int Id, [FromBody] CategoryAssignRequest request)
        {
            var result = await _IProductService.CategoryAssign(Id, request);
            if (result.IsSuccessed)
                return Ok(result);
            return Ok(result);
        }
    }
}