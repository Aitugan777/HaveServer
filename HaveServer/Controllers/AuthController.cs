using HaveServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HaveServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Здесь проверяем данные пользователя (из базы данных)
            var seller = _context.Sellers.Where(x => x.Email == login.Email && x.Password == login.Password).FirstOrDefault();
            if (seller != null)
            {
                var token = GenerateJwtToken(seller.Id.ToString());
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string sellerId)
        {
            // Добавляем claims, включая SellerId
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, sellerId), // ID пользователя
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Уникальный идентификатор токена
                new Claim("SellerId", sellerId) // Добавляем SellerId в токен
            };

            // Генерируем ключ
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Создаем JWT токен
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"], // Издатель токена
                audience: _configuration["JwtSettings:Audience"], // Целевая аудитория
                claims: claims, // Список claims
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationMinutes"])), // Время истечения
                signingCredentials: creds); // Подпись токена

            // Генерация строки токена
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
