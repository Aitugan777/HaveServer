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
                .Include(s => s.Contacts)
                .Include(s => s.WorkSheldure)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null) return null;

            WorkDayContract MapWorkDay(AWorkDay? day)
            {
                if (day == null) return null;

                return new WorkDayContract
                {
                    StartTime = day.StartTime,
                    EndTime = day.EndTime,
                    IsWorkingDay = day.IsWorkingDay
                };
            }

            var contract = new ShopContract
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                Photos = await _imageRepository.GetPhotosAsync(EPhotoFor.Shop, shop.Id),
                Contacts = shop.Contacts?.Select(c => new ContactContract
                {
                    ContactTypeId = c.ContactTypeId,
                    Value = c.Contact
                }).ToList(),
                WorkSheldure = shop.WorkSheldure != null
                    ? new WorkSheldureContract
                    {
                        Monday = MapWorkDay(shop.WorkSheldure.Monday),
                        Tuesday = MapWorkDay(shop.WorkSheldure.Tuesday),
                        Wednesday = MapWorkDay(shop.WorkSheldure.Wednesday),
                        Thursday = MapWorkDay(shop.WorkSheldure.Thursday),
                        Friday = MapWorkDay(shop.WorkSheldure.Friday),
                        Saturday = MapWorkDay(shop.WorkSheldure.Saturday),
                        Sunday = MapWorkDay(shop.WorkSheldure.Sunday)
                    }
                    : null
            };

            return contract;
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

            // Добавляем рабочее расписание
            var workSheldure = new AWorkSheldure { ShopId = shop.Id };

            UpdateWorkDay(workSheldure.Monday, contract.WorkSheldure.Monday);
            UpdateWorkDay(workSheldure.Tuesday, contract.WorkSheldure.Tuesday);
            UpdateWorkDay(workSheldure.Wednesday, contract.WorkSheldure.Wednesday);
            UpdateWorkDay(workSheldure.Thursday, contract.WorkSheldure.Thursday);
            UpdateWorkDay(workSheldure.Friday, contract.WorkSheldure.Friday);
            UpdateWorkDay(workSheldure.Saturday, contract.WorkSheldure.Saturday);
            UpdateWorkDay(workSheldure.Sunday, contract.WorkSheldure.Sunday);
            _dbContext.WorkSheldures.Add(workSheldure);

            // Добавляем контакты
            shop.Contacts = contract.Contacts.Select(x => new AShopContact
            {
                ShopId = shop.Id,
                ContactTypeId = x.ContactTypeId,
                Contact = x.Value
            }).ToList();

            // Добавляем фото
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
                .Include(s => s.Contacts)
                .Include(s => s.WorkSheldure) // Подгружаем расписание
                .FirstOrDefaultAsync(s => s.Id == contract.Id);

            if (shop == null) return;

            // Обновляем основные поля
            shop.Name = contract.Name;
            shop.Description = contract.Description;
            shop.Address = contract.Address;
            shop.Latitude = contract.Latitude;
            shop.Longitude = contract.Longitude;

            // Обновляем расписание
            if (shop.WorkSheldure == null)
            {
                shop.WorkSheldure = new AWorkSheldure { ShopId = shop.Id };
                _dbContext.WorkSheldures.Add(shop.WorkSheldure);
            }

            UpdateWorkDay(shop.WorkSheldure.Monday, contract.WorkSheldure.Monday);
            UpdateWorkDay(shop.WorkSheldure.Tuesday, contract.WorkSheldure.Tuesday);
            UpdateWorkDay(shop.WorkSheldure.Wednesday, contract.WorkSheldure.Wednesday);
            UpdateWorkDay(shop.WorkSheldure.Thursday, contract.WorkSheldure.Thursday);
            UpdateWorkDay(shop.WorkSheldure.Friday, contract.WorkSheldure.Friday);
            UpdateWorkDay(shop.WorkSheldure.Saturday, contract.WorkSheldure.Saturday);
            UpdateWorkDay(shop.WorkSheldure.Sunday, contract.WorkSheldure.Sunday);

            // Удаляем старые фото из БД
            _dbContext.ShopPhotos.RemoveRange(shop.Photos);
            await _dbContext.SaveChangesAsync();

            // Обновляем контакты
            _dbContext.ShopContacts.RemoveRange(shop.Contacts);
            shop.Contacts = contract.Contacts.Select(x => new AShopContact
            {
                ShopId = shop.Id,
                ContactTypeId = x.ContactTypeId,
                Contact = x.Value
            }).ToList();

            // Добавляем новые фото (без байтов)
            var newPhotos = contract.Photos.Select(_ => new AShopPhoto
            {
                ShopId = shop.Id
            }).ToList();

            _dbContext.ShopPhotos.AddRange(newPhotos);
            await _dbContext.SaveChangesAsync(); // получим Id

            // Сохраняем изображения
            var photoDict = newPhotos
                .Select((entity, index) => new { entity.Id, Bytes = contract.Photos[index] })
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Bytes).ToList());

            await _imageRepository.SavePhotosAsync(EPhotoFor.Shop, shop.Id, photoDict);
        }
        private void UpdateWorkDay(AWorkDay? target, WorkDayContract? source)
        {
            if (source == null)
            {
                target = null;
                return;
            }

            if (target == null)
                target = new AWorkDay();

            target.StartTime = source.StartTime;
            target.EndTime = source.EndTime;
            target.IsWorkingDay = source.IsWorkingDay;
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

        public async Task<List<ContactTypeContract>> GetContactTypeContracts()
        {
            if (!_dbContext.ContactTypes.Any())
                await InitialContactTypesAsync();

            var contacts = _dbContext.ContactTypes.Select(x => new ContactTypeContract() { Id = x.Id, Name = x.Name }).ToList();
            return contacts;
        }

        private async Task InitialContactTypesAsync()
        {
            _dbContext.ContactTypes.Add(new AContactType() { Name = "Телефон" });
            _dbContext.ContactTypes.Add(new AContactType() { Name = "Почта" });
            _dbContext.ContactTypes.Add(new AContactType() { Name = "Telegram" });
            _dbContext.ContactTypes.Add(new AContactType() { Name = "WhatsApp" });
            await _dbContext.SaveChangesAsync();
        }
    }
}
