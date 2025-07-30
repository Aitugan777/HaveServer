using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AitukServer.Models;
using AitukServer.Data;

namespace AitukServer.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ShopOwnershipAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var dbContext = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Извлекаем идентификатор пользователя из Claims
            var userIdClaim = httpContext.User.FindFirst("SellerId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Проверяем, передано ли тело запроса
            if (!context.ActionArguments.TryGetValue("model", out var model) || model is not IEnumerable<AShop> shops)
            {
                context.Result = new BadRequestObjectResult("Invalid request body.");
                return;
            }

            // Проверяем, принадлежат ли все магазины этому пользователю
            var invalidShop = shops.FirstOrDefault(shop =>
                !dbContext.Shops.Any(s => s.Id == shop.Id && s.SellerId == userId));

            if (invalidShop != null)
            {
                context.Result = new ForbidResult($"Access denied to shop with ID {invalidShop.Id}.");
                return;
            }

            // Если все проверки пройдены, передаём запрос дальше
            await next();
        }
    }
}
