using eShopSolution.Application.Catalog.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolutionBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IPublicProductService _PublicProductService;
        public ProductController(IPublicProductService PublicProductService)
        {
            _PublicProductService = PublicProductService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var product = await _PublicProductService.GetALL(); 
            return Ok(product);
        }
    }
}
