using AitukServer.Models;

namespace AitukServer.Data
{
    public class ImageRepository
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Photos");

        public async Task<List<byte[]>> GetPhotosAsync(EPhotoFor photoFor, long entityId)
        {
            var path = Path.Combine(_basePath, photoFor.ToString(), entityId.ToString());

            if (!Directory.Exists(path))
                return new List<byte[]>();

            var files = Directory.GetFiles(path);
            var photos = new List<byte[]>();

            foreach (var file in files)
            {
                var data = await File.ReadAllBytesAsync(file);
                photos.Add(data);
            }

            return photos;
        }

        public async Task SavePhotosAsync(EPhotoFor photoFor, long entityId, Dictionary<long, List<byte[]>> photosById)
        {
            var baseDir = Path.Combine(_basePath, photoFor.ToString(), entityId.ToString());

            // Удаляем существующие файлы (опционально)
            if (Directory.Exists(baseDir))
                Directory.Delete(baseDir, true);

            Directory.CreateDirectory(baseDir);

            foreach (var pair in photosById)
            {
                long photoId = pair.Key;
                foreach (var photoBytes in pair.Value)
                {
                    string ext = GetImageExtension(photoBytes);
                    if (ext == null) continue;

                    string filePath = Path.Combine(baseDir, $"{photoId}.{ext}");
                    await File.WriteAllBytesAsync(filePath, photoBytes);
                }
            }
        }

        private string GetImageExtension(byte[] bytes)
        {
            // JPEG
            if (bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "jpg";

            // PNG
            if (bytes.Length > 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 &&
                bytes[2] == 0x4E && bytes[3] == 0x47)
                return "png";

            // GIF
            if (bytes.Length > 6 &&
                bytes[0] == 0x47 && bytes[1] == 0x49 &&
                bytes[2] == 0x46)
                return "gif";

            // BMP
            if (bytes.Length > 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
                return "bmp";

            // WEBP
            if (bytes.Length > 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 &&
                bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 &&
                bytes[10] == 0x42 && bytes[11] == 0x50)
                return "webp";

            return null;
        }
    }
}
