using Microsoft.AspNetCore.Mvc;

namespace HaveServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly string _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

        public FilesController()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            string filePath = Path.Combine(_baseDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "Файл не найден" });
            }

            string contentType = GetContentType(filePath);

            // Возвращаем файл с правильным Content-Type
            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Файл не загружен" });
            }

            string filePath = Path.Combine(_baseDirectory, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Message = "Файл успешно загружен", FileName = file.FileName });
        }

        private string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream" // На случай неизвестного типа
            };
        }
    }
}
