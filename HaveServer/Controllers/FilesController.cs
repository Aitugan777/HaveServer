using AitukServer.Data;
using AitukServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AitukServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Photos");
        private const long MaxFileSize = 3 * 1024 * 1024; // 3MB

        public FilesController(ApplicationDbContext context)
        {
            _context = context;

            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] EPhotoFor type, [FromForm] long parentId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "Файл не загружен" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { Message = "Файл превышает 3 МБ" });

            // Сохраняем запись в БД
            var photo = new APhoto
            {
                ParentId = parentId,
                PhotoFor = type
            };

            _context.Add(photo);
            await _context.SaveChangesAsync();

            string extension = Path.GetExtension(file.FileName);
            string typeFolder = Path.Combine(_baseDirectory, type.ToString());
            string parentFolder = Path.Combine(typeFolder, parentId.ToString());

            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder);

            string filePath = Path.Combine(parentFolder, $"{photo.Id}{extension}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                Message = "Файл успешно загружен",
                PhotoId = photo.Id,
                Path = $"Photos/{type}/{parentId}/{photo.Id}{extension}"
            });
        }

        [HttpGet("{type}/{parentId}/{photoId}")]
        public IActionResult GetFile(EPhotoFor type, long parentId, long photoId)
        {
            string directory = Path.Combine(_baseDirectory, type.ToString(), parentId.ToString());

            if (!Directory.Exists(directory))
                return NotFound(new { Message = "Папка не найдена" });

            string file = Directory.GetFiles(directory, $"{photoId}.*").FirstOrDefault();
            if (file == null)
                return NotFound(new { Message = "Файл не найден" });

            string contentType = GetContentType(file);
            return PhysicalFile(file, contentType);
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }

}
