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
        private IPublicProductService _PublicProductService;
        private IManageProductService _ManageProductService;

        public productsController(IPublicProductService PublicProductService, IManageProductService ManageProductService)
        {
            _PublicProductService = PublicProductService;
            _ManageProductService = ManageProductService;
        }

        [HttpGet("public-paging/{languageid}")]
        public async Task<IActionResult> GetAllPaging(string languageid, [FromQuery] GetPublicProductPagingRequest request)
        {
            try
            {
                var product = await _PublicProductService.GetAllCategory(languageid, request);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetProductID")]
        public async Task<IActionResult> GetProductID(int productid, string LanguageId)
        {
            try
            {
                var product = await _ManageProductService.GetProductId(productid, LanguageId);
                if (product == null)
                    return BadRequest("Cannot Find Product!");
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var productID = await _ManageProductService.Create(request);
            if (productID.Data == null)
                return BadRequest("Create Fail");
            var product = await _ManageProductService.GetProductId((int)productID.Data, request.LanguageId);
            return CreatedAtAction(nameof(GetProductID), productID.Data, product);
            //return Ok(productID);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] ProductUpdateRequest request)
        {
            var affectedResult = await _ManageProductService.Update(request);
            if (affectedResult == 0)
                return BadRequest("Update Fail");
            return Ok();
        }

        [HttpGet("ListImage/{productid}")]
        public async Task<IActionResult> getlistimage(int productid)
        {
            var ListImage = await _ManageProductService.GetListImage(productid);
            if (ListImage?.Count > 0)
                return Ok(ListImage);
            return BadRequest("Data can not found");
        }

        [HttpDelete("Delete/{productid}")]
        public async Task<IActionResult> Delete(int productid)
        {
            var affectedResult = await _ManageProductService.Delete(productid);
            return Ok();
        }

        [HttpPut("UpdatePrice/{productid}")]
        public async Task<IActionResult> UpdatePrice(int productid, decimal newPrice)
        {
            var isSuccessful = await _ManageProductService.UpdatePrice(productid, newPrice);
            if (isSuccessful)
                return Ok();
            return BadRequest();
        }

        [HttpPost("AddImage/{productid}")]
        public async Task<IActionResult> Addimage(int productid, [FromForm] ProductImageCreateRequest request)
        {
            var productimage = await _ManageProductService.AddIamges(productid, request);
            if (productimage == 0)
                return BadRequest("Add Fail");
            return Ok("Add successfully");
        }

        [HttpPut("UpdateImage/{imageid}")]
        public async Task<IActionResult> UpdateImage(int imageid, int productid, [FromForm] ProductImageUpdateRequest request)
        {
            var issuccessfull = await _ManageProductService.UpdateImage(imageid, productid, request);
            if (issuccessfull == 0)
                return BadRequest("Update Fail!");
            return Ok("Update Successfully!");
        }

        [HttpDelete("delete/{imageid}")]
        public async Task<IActionResult> RemoveImage(int imageid)
        {
            var affectedResult = await _ManageProductService.RemoveImages(imageid);
            return Ok(affectedResult);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                //string term1 = HttpContext.Request.Query["term"].ToString();
                var names = await _ManageProductService.Search(term);
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}