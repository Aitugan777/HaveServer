using HaveServer.Data;
using HaveServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web.Http.Cors;

namespace HaveServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct(HProduct product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductsByName), new { name = product.Name }, product);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("shop/{shopId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<HProduct>>> GetProductsByShopId(int shopId)
        {
            var result = await _context.Products.Where(p => p.ShopId == shopId).ToListAsync();
            if (result.Count == 0)
                return NotFound();
            return result;
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<HProduct>>> GetProductsByCategoryId(int categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HProduct>>> GetProductsByName(string name)
        {
            var result = await _context.Products
                                      .Where(p => p.Name.Contains(name))
                                      .Include(p => p.Shop)  // Включаем связанные данные о магазине
                                      .ToListAsync();
            if (result.Count == 0)
                return NotFound();
            return result;
        }
    }
}
