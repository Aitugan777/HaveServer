using AitukCore.Contracts;
using AitukServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AitukServer.Data
{
    public class ShopRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ImageRepository _imageRepository;

        public ShopRepository(ApplicationDbContext dbContext, ImageRepository imageRepository)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
        }

        public async Task<ShopContract> GetShopAsync(long id)
        {
            var shop = await _dbContext.Shops
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null) return null;

            return new ShopContract
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                Photos = await _imageRepository.GetPhotosAsync(EPhotoFor.Shop, shop.Id)
            };
        }

        public async Task<List<ShopCompactContract>> GetAllCompactShopsAsync(long sellerId)
        {
            var shops = await _dbContext.Shops
                .Where(x => x.SellerId == sellerId)
                .Include(s => s.ProductShops)
                .ToListAsync();

            return shops.Select(shop => new ShopCompactContract
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                Address = shop.Address,
                ProductCount = shop.ProductShops.Count
            }).ToList();
        }

        public async Task AddShopAsync(ShopContract contract, long sellerId)
        {
            var shop = new AShop
            {
                Name = contract.Name,
                Description = contract.Description,
                Address = contract.Address,
                Latitude = contract.Latitude,
                Longitude = contract.Longitude,
                SellerId = sellerId
                // Photos будут добавлены ниже после сохранения
            };

            _dbContext.Shops.Add(shop);
            await _dbContext.SaveChangesAsync(); // теперь shop.Id доступен

            // Создаём записи для AShopPhoto без хранения фото в БД
            var photoEntities = contract.Photos.Select(_ => new AShopPhoto
            {
                ShopId = shop.Id
            }).ToList();

            _dbContext.ShopPhotos.AddRange(photoEntities);
            await _dbContext.SaveChangesAsync(); // получаем Id для фото

            // Готовим структуру для ImageRepository
            var photoDict = photoEntities
                .Select((entity, index) => new { entity.Id, Bytes = contract.Photos[index] })
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Bytes).ToList());

            await _imageRepository.SavePhotosAsync(EPhotoFor.Shop, shop.Id, photoDict);
        }

        public async Task UpdateShopAsync(ShopContract contract)
        {
            var shop = await _dbContext.Shops
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == contract.Id);

            if (shop == null) return;

            // Обновляем основные поля
            shop.Name = contract.Name;
            shop.Description = contract.Description;
            shop.Address = contract.Address;
            shop.Latitude = contract.Latitude;
            shop.Longitude = contract.Longitude;

            // Удаляем старые фото из БД
            _dbContext.ShopPhotos.RemoveRange(shop.Photos);
            await _dbContext.SaveChangesAsync();

            // Добавляем новые записи AShopPhoto (без байтов)
            var newPhotos = contract.Photos.Select(_ => new AShopPhoto
            {
                ShopId = shop.Id
            }).ToList();

            _dbContext.ShopPhotos.AddRange(newPhotos);
            await _dbContext.SaveChangesAsync(); // здесь получим Id

            // Готовим структуру для ImageRepository
            var photoDict = newPhotos
                .Select((entity, index) => new { entity.Id, Bytes = contract.Photos[index] })
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Bytes).ToList());

            await _imageRepository.SavePhotosAsync(EPhotoFor.Shop, shop.Id, photoDict);
        }


        public async Task DeleteShopAsync(long id)
        {
            var shop = await _dbContext.Shops
                .Include(s => s.Photos)
                .Include(s => s.ProductShops)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null) return;

            _dbContext.Shops.Remove(shop);
            await _dbContext.SaveChangesAsync();
        }
    }
}
