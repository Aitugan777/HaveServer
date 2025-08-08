using AitukCore.Contracts;
using AitukServer.ActionFilters;
using AitukServer.Data;
using AitukServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace AitukServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ShopRepository _shopRepository;

        public ShopController(ShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ShopContract>> GetShopById(long id)
        {
            var shop = await _shopRepository.GetShopAsync(id);
            if (shop == null)
                return NotFound();
            return Ok(shop);
        }

        [HttpGet("compact")]
        [Authorize]
        public async Task<ActionResult<List<ShopCompactContract>>> GetAllCompactShops()
        {
            var sellerId = ExtractSellerIdFromToken();
            if (sellerId == null)
                return Unauthorized("SellerId not found in token.");

            var shops = await _shopRepository.GetAllCompactShopsAsync(sellerId.Value);
            return Ok(shops);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddShop([FromBody] ShopContract contract)
        {
            var sellerId = ExtractSellerIdFromToken();
            if (sellerId == null)
                return Unauthorized("SellerId not found in token.");

            await _shopRepository.AddShopAsync(contract, sellerId.Value);
            return Ok();
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> UpdateShop(long id, [FromBody] ShopContract contract)
        {
            if (id != contract.Id)
                return BadRequest("ID mismatch");

            var sellerId = ExtractSellerIdFromToken();
            if (sellerId == null)
                return Unauthorized("SellerId not found in token.");

            await _shopRepository.UpdateShopAsync(contract);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteShop(long id)
        {
            await _shopRepository.DeleteShopAsync(id);
            return NoContent();
        }


        [HttpGet("contactTypes")]
        [Authorize]
        public async Task<ActionResult<List<ContactTypeContract>>> GetContactTypes()
        {
            var sellerId = ExtractSellerIdFromToken();
            if (sellerId == null)
                return Unauthorized("SellerId not found in token.");

            var contactTypes = await _shopRepository.GetContactTypeContracts();
            return Ok(contactTypes);
        }

        // ====== Вспомогательный метод ======
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
