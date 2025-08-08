using AitukCore.Contracts;
using AitukServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AitukServer.Data
{
    public class ClothPropertiesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private static bool _initialized = false;

        public ClothPropertiesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            if (await _dbContext.Categories.AnyAsync()) return; 

            // Размеры
            if (!await _dbContext.Sizes.AnyAsync())
            {
                var sizes = new List<ASize>
                {
                    new ASize { Name = "XS" },
                    new ASize { Name = "S" },
                    new ASize { Name = "M" },
                    new ASize { Name = "L" },
                    new ASize { Name = "XL" },
                    new ASize { Name = "XXL" },
                    new ASize { Name = "XXXL" }
                };
                _dbContext.Sizes.AddRange(sizes);
            }

            // Цвета
            if (!await _dbContext.Colors.AnyAsync())
            {
                var colors = new List<AColor>
                {
                    new AColor { Name = "Чёрный" },
                    new AColor { Name = "Белый" },
                    new AColor { Name = "Серый" },
                    new AColor { Name = "Красный" },
                    new AColor { Name = "Синий" },
                    new AColor { Name = "Зелёный" },
                    new AColor { Name = "Жёлтый" },
                    new AColor { Name = "Розовый" },
                    new AColor { Name = "Коричневый" },
                    new AColor { Name = "Бежевый" },
                    new AColor { Name = "Фиолетовый" },
                    new AColor { Name = "Оранжевый" },
                    new AColor { Name = "Бордовый" },
                    new AColor { Name = "Голубой" }
                };
                _dbContext.Colors.AddRange(colors);
            }

            // Категории
            if (!await _dbContext.Categories.AnyAsync())
            {
                var categories = new List<ACategory>
                {
                    new ACategory { Name = "Футболки" },
                    new ACategory { Name = "Рубашки" },
                    new ACategory { Name = "Толстовки" },
                    new ACategory { Name = "Худи" },
                    new ACategory { Name = "Кофты" },
                    new ACategory { Name = "Свитера" },
                    new ACategory { Name = "Пиджаки" },
                    new ACategory { Name = "Пальто" },
                    new ACategory { Name = "Куртки" },
                    new ACategory { Name = "Штаны" },
                    new ACategory { Name = "Джинсы" },
                    new ACategory { Name = "Шорты" },
                    new ACategory { Name = "Юбки" },
                    new ACategory { Name = "Платья" },
                    new ACategory { Name = "Комбинезоны" },
                    new ACategory { Name = "Обувь" },
                    new ACategory { Name = "Аксессуары" },
                    new ACategory { Name = "Нижнее белье" }
                };
                _dbContext.Categories.AddRange(categories);
            }

            // Пол
            if (!await _dbContext.Genders.AnyAsync())
            {
                var genders = new List<AGender>
                {
                    new AGender { Name = "Мужской" },
                    new AGender { Name = "Женский" },
                    new AGender { Name = "Универсальный" }
                };
                _dbContext.Genders.AddRange(genders);
            }

            await _dbContext.SaveChangesAsync();
            _initialized = true;
        }

        public async Task<List<SizeContract>> GetSizesAsync()
        {
            await InitializeAsync();
            var sizes = await _dbContext.Sizes.ToListAsync();
            return sizes.Select(x => new SizeContract { Name = x.Name, Id = x.Id }).ToList();
        }

        public async Task<List<CategoryContract>> GetCategoriesAsync()
        {
            await InitializeAsync();
            var categories = await _dbContext.Categories.ToListAsync();
            return categories.Select(x => new CategoryContract { Name = x.Name, Id = x.Id }).ToList();
        }

        public async Task<List<GenderContract>> GetGendersAsync()
        {
            await InitializeAsync();
            var genders = await _dbContext.Genders.ToListAsync();
            return genders.Select(x => new GenderContract { Name = x.Name, Id = x.Id }).ToList();
        }

        public async Task<List<ColorContract>> GetColorsAsync()
        {
            await InitializeAsync();
            var colors = await _dbContext.Colors.ToListAsync();
            return colors.Select(x => new ColorContract { Name = x.Name, Id = x.Id }).ToList();
        }
    }

}
