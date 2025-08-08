using AitukCore.Contracts;
using AitukServer.Data;
using AitukServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AitukServer.Data
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ImageRepository _imageRepository;

        public ProductRepository(ApplicationDbContext dbContext, ImageRepository imageRepository)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
        }

        public async Task<ProductContract> GetProductAsync(long productId)
        {
            var product = await _dbContext.Products
                .Include(p => p.Photos)
                .Include(p => p.Sizes)
                .Include(p => p.ProductShops)
                    .ThenInclude(ps => ps.Shop)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return null;

            return new ProductContract
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Cost = product.Cost,
                Brand = product.Brand,
                Code = product.Code,
                KeyWords = product.KeyWords,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                GenderId = product.GenderId,
                Sizes = product.Sizes.Select(s => s.SizeId).ToList(),
                Photos = await _imageRepository.GetPhotosAsync(EPhotoFor.Product, product.Id),
                Shops = product.ProductShops.Select(ps => new ShopCompactContract
                {
                    Id = ps.ShopId,
                    ProductCount = ps.ProductCount
                }).ToList()
            };
        }
        /// <summary>
        /// Получить все продукты магазинов
        /// </summary>
        /// <param name="shopIds"></param>
        /// <returns></returns>
        public async Task<List<ProductCompactContract>> GetCompactProductsByShopsAsync(List<long> shopIds)
        {
            var query = _dbContext.Products
                .Where(p => p.ProductShops.Any(ps => shopIds.Contains(ps.ShopId)))
                .Include(p => p.Photos);

            return await GetCompactProducts(query);
        }

        /// <summary>
        /// Получить все продукты продавца
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public async Task<List<ProductCompactContract>> GetCompactProductsBySellerAsync(long sellerId)
        {
            var shopIds = await _dbContext.Shops
                .Where(x => x.SellerId == sellerId)
                .Select(x => x.Id)
                .ToListAsync();

            var query = _dbContext.Products
                .Where(p => p.ProductShops.Any(ps => shopIds.Contains(ps.ShopId)))
                .Include(p => p.Photos)
                .Include(p => p.ProductShops); // <- включаем ProductShops по запросу

            return await GetCompactProducts(query);
        }

        /// <summary>
        /// Получить все продукты
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductCompactContract>> GetCompactProductsAsync(ProductFilterContract filter)
        {
            var query = _dbContext.Products
                .Include(p => p.Photos)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var lowered = filter.SearchText.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(lowered) ||
                    p.KeyWords.ToLower().Contains(lowered));
            }

            if (filter.SizeIds != null && filter.SizeIds.Any())
            {
                query = query.Where(p => p.Sizes.Any(s => filter.SizeIds.Contains(s.SizeId)));
            }

            if (filter.ColorIds != null && filter.ColorIds.Any())
            {
                query = query.Where(p => filter.ColorIds.Contains(p.ColorId));
            }

            if (filter.CategoryIds != null && filter.CategoryIds.Any())
            {
                query = query.Where(p => filter.CategoryIds.Contains(p.CategoryId));
            }

            if (filter.GenderId.HasValue)
            {
                query = query.Where(p => p.GenderId == filter.GenderId);
            }

            if (filter.MinCost.HasValue)
            {
                query = query.Where(p => p.Cost >= filter.MinCost.Value);
            }

            if (filter.MaxCost.HasValue)
            {
                query = query.Where(p => p.Cost <= filter.MaxCost.Value);
            }

            return await GetCompactProducts(query);
        }


        /// <summary>
        /// Получить все продукты по названию и ключевым словам
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<List<ProductCompactContract>> GetCompactProductsAsync(string searchText)
        {
            var loweredText = searchText.ToLower();

            var query = _dbContext.Products
                .Where(x => x.Name.ToLower().Contains(loweredText) || x.KeyWords.ToLower().Contains(loweredText))
                .Include(p => p.Photos);

            return await GetCompactProducts(query);
        }

        /// <summary>
        /// Общий метод для получения продуктов
        /// </summary>
        /// <param name="query">Фильтрованный запрос или null для всех</param>
        /// <returns></returns>
        private async Task<List<ProductCompactContract>> GetCompactProducts(IQueryable<AProduct> query)
        {
            var productsQuery = query ?? _dbContext.Products.Include(p => p.Photos);

            var products = await productsQuery.ToListAsync();

            var result = await Task.WhenAll(products.Select(async p =>
            {
                var photos = await _imageRepository.GetPhotosAsync(EPhotoFor.Product, p.Id);

                return new ProductCompactContract
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Cost = p.Cost,
                    MainPhoto = photos.FirstOrDefault(),
                };
            }));

            return result.ToList();
        }


        public async Task AddProductAsync(ProductContract contract)
        {
            var product = new AProduct
            {
                Name = contract.Name,
                Description = contract.Description,
                Cost = contract.Cost,
                Brand = contract.Brand,
                Code = contract.Code,
                KeyWords = contract.KeyWords,
                CategoryId = contract.CategoryId,
                ColorId = contract.ColorId,
                GenderId = contract.GenderId,
                Sizes = contract.Sizes.Select(s => new AProductSize { SizeId = s }).ToList(),
                ProductShops = contract.Shops.Select(s => new AProductShop { ShopId = s.Id, ProductCount = s.ProductCount }).ToList()
            };

            // Добавляем продукт в базу
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(); // После этого product.Id уже присвоен

            // Формируем записи для таблицы Photo
            var photoEntities = contract.Photos.Select((photoBytes, index) => new AProductPhoto
            {
                ProductId = product.Id
            }).ToList();

            product.Photos = photoEntities;
            await _dbContext.SaveChangesAsync();

            // Подготавливаем структуру фото для файловой системы
            var photoDict = photoEntities
                .Select((entity, index) => new { PhotoId = entity.Id, Bytes = contract.Photos[index] })
                .GroupBy(x => x.PhotoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Bytes).ToList()
                );

            // Сохраняем фотографии физически
            await _imageRepository.SavePhotosAsync(EPhotoFor.Product, product.Id, photoDict);
        }


        public async Task UpdateProductAsync(ProductContract contract)
        {
            var product = await _dbContext.Products
                .Include(p => p.Photos)
                .Include(p => p.Sizes)
                .Include(p => p.ProductShops)
                .FirstOrDefaultAsync(p => p.Id == contract.Id);

            if (product == null) return;

            // Обновление основных полей
            product.Name = contract.Name;
            product.Description = contract.Description;
            product.Cost = contract.Cost;
            product.Brand = contract.Brand;
            product.Code = contract.Code;
            product.KeyWords = contract.KeyWords;
            product.CategoryId = contract.CategoryId;
            product.ColorId = contract.ColorId;
            product.GenderId = contract.GenderId;

            // Удаляем старые связанные записи
            _dbContext.ProductSizes.RemoveRange(product.Sizes);
            _dbContext.ProductPhotos.RemoveRange(product.Photos);
            _dbContext.ProductShops.RemoveRange(product.ProductShops);
            await _dbContext.SaveChangesAsync();

            // Добавляем новые связанные записи
            product.Sizes = contract.Sizes.Select(s => new AProductSize { SizeId = s }).ToList();
            product.ProductShops = contract.Shops.Select(s => new AProductShop { ShopId = s.Id, ProductCount = s.ProductCount }).ToList();

            // Фото — только сохраняем записи, без байтов
            var photoEntities = contract.Photos.Select((p, i) => new AProductPhoto { ProductId = product.Id }).ToList();
            product.Photos = photoEntities;
            await _dbContext.SaveChangesAsync(); // после этого получим Id для фото

            // Сохраняем физически фото по их ID
            var photoDict = photoEntities
                .Select((entity, index) => new { entity.Id, Bytes = contract.Photos[index] })
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Bytes).ToList());

            await _imageRepository.SavePhotosAsync(EPhotoFor.Product, product.Id, photoDict);
        }

        public async Task DeleteProductAsync(long productId)
        {
            var product = await _dbContext.Products
                .Include(p => p.Photos)
                .Include(p => p.Sizes)
                .Include(p => p.ProductShops)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
