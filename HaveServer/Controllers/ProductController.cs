using AitukCore.Contracts;
using AitukServer.Data;
using AitukServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Http.Cors;

namespace AitukServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ProductContract>> GetProduct(long id)
        {
            var product = await _productRepository.GetProductAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromBody] ProductContract contract)
        {
            await _productRepository.AddProductAsync(contract);
            return Ok(); // можно заменить на Created(...) при необходимости
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductContract contract)
        {
            if (id != contract.Id)
                return BadRequest("ID mismatch");

            await _productRepository.UpdateProductAsync(contract);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpPost("by-shops")]
        public async Task<ActionResult<List<ProductCompactContract>>> GetProductsByShops([FromBody] List<long> shopIds)
        {
            var products = await _productRepository.GetCompactProductsByShopsAsync(shopIds);
            return Ok(products);
        }
        
        [HttpGet("for-this-seller")]
        [Authorize]
        public async Task<ActionResult<List<ProductCompactContract>>> GetAllProductsForSeller()
        {
            var sellerId = ExtractSellerIdFromToken();
            if (sellerId == null)
                return Unauthorized("SellerId not found in token.");

            var shops = await _productRepository.GetCompactProductsBySellerAsync(sellerId.Value);
            return Ok(shops);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductCompactContract>>> GetAllProductsForSeller([FromQuery] ProductFilterContract filter)
        {
            var products = await _productRepository.GetCompactProductsAsync(filter);
            return Ok(products);
        }

        private long? ExtractSellerIdFromToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var sellerIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "SellerId")?.Value;

            if (long.TryParse(sellerIdClaim, out long sellerId))
                return sellerId;

            return null;
        }
    }

}
