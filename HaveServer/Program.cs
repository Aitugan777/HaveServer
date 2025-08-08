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
                options.Listen(IPAddress.Any, 5070);  // ��������� ������������� �� ��� IP
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
            });

            // ���������� �������� � ���������
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

            // ���������� CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()   // ��������� ������� � ����� �������
                          .AllowAnyHeader()   // ��������� ����� ���������
                          .AllowAnyMethod();  // ��������� ����� ������ (GET, POST, PUT � �.�.)
                });
            });

            // ��� ������ � Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // ������������ HTTP-���������
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ��������� UseRouting �� ������������� �������������� � �����������
            app.UseRouting();

            // ��������� CORS
            app.UseCors();

            // �������� �������������� � �����������
            app.UseAuthentication();
            app.UseAuthorization();

            // �������� ��� ������������
            app.MapControllers();

            app.Run();
        }
    }
}
