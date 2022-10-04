using System.Threading.Tasks;
using Likya.Business.Services;
using Likya.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Likya.Api.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        IProductService _productService;
        ILogger<ProductController> _logger;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNameOrdered()
        {
            try
            {
                return Ok(await _productService.GetAllNameOrdered());
            }
            catch (System.Exception ex)
            {
                _logger.LogError("GetAllNameOrdered method error in ProductController", ex);
                return BadRequest("GetAllNameOrdered method error in ProductController");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(Product product)
        {
            try
            {
                await _productService.Save(product);
                return Ok("Product save.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Save method error in ProductController", ex);
                return BadRequest("Save method error in ProductController");
            }
        }
    }
}
