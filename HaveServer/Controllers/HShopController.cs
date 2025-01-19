using HaveServer.ActionFilters;
using HaveServer.Data;
using HaveServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace HaveServer.Controllers
{
    // Контроллеры
    [ApiController]
    [Route("api/[controller]")]
    public class HShopController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ShopOwnership]
        public async Task<IActionResult> AddShop(HShop shop)
        {
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetShopById), new { id = shop.Id }, shop);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
                return NotFound();

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<HShop>>> GetShopsBySellerId(int sellerId)
        {
            return await _context.Shops.Where(s => s.SellerId == sellerId).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HShop>> GetShopById(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
                return NotFound();

            return shop;
        }

        [HttpGet]
        public async Task<IActionResult> GetShopsBySellerId()
        {
            // Извлекаем JWT из заголовка Authorization
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing.");
            }

            // Декодируем JWT и извлекаем SellerId из payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Получаем SellerId из полезной нагрузки (Claims)
            var sellerIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "SellerId")?.Value;

            if (string.IsNullOrEmpty(sellerIdClaim))
            {
                return Unauthorized("SellerId not found in token.");
            }

            // Преобразуем SellerId в нужный тип (например, int)
            if (!int.TryParse(sellerIdClaim, out var sellerId))
            {
                return BadRequest("Invalid SellerId.");
            }

            // Получаем магазины для конкретного SellerId
            var shops = await _context.Shops
                .Where(s => s.SellerId == sellerId)
                .ToListAsync();

            return Ok(shops);
        }
    }
}
