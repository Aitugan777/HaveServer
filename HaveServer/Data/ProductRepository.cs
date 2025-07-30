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
                    Id = ps.Shop.Id,
                    Name = ps.Shop.Name
                }).ToList()
            };
        }

        public async Task<List<ProductCompactContract>> GetCompactProductsByShopsAsync(List<long> shopIds)
        {
            var products = await _dbContext.Products
                .Include(p => p.Photos)
                .Include(p => p.ProductShops)
                .Where(p => p.ProductShops.Any(ps => shopIds.Contains(ps.ShopId)))
                .ToListAsync();

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
                ProductShops = contract.Shops.Select(s => new AProductShop { ShopId = s.Id }).ToList()
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
            product.ProductShops = contract.Shops.Select(s => new AProductShop { ShopId = s.Id }).ToList();

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
