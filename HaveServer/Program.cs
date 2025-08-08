using AitukServer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace AitukServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5070);  // Настроить прослушивание на все IP
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
            });

            // Добавление сервисов в контейнер
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("AitukServerDB")));

            builder.Services.AddScoped<ShopRepository>();
            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<ClothPropertiesRepository>();
            builder.Services.AddScoped<ImageRepository>();

            builder.Services.AddHttpClient();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    { 
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
                    };
                });

            builder.Services.AddControllers();

            // Добавление CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()   // Разрешить запросы с любых доменов
                          .AllowAnyHeader()   // Разрешить любые заголовки
                          .AllowAnyMethod();  // Разрешить любые методы (GET, POST, PUT и т.д.)
                });
            });

            // Для работы с Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Конфигурация HTTP-пайплайна
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Добавляем UseRouting до использования аутентификации и авторизации
            app.UseRouting();

            // Включение CORS
            app.UseCors();

            // Включаем аутентификацию и авторизацию
            app.UseAuthentication();
            app.UseAuthorization();

            // Маршруты для контроллеров
            app.MapControllers();

            app.Run();
        }
    }
}
