using AitukCore.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AitukServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string YandexApiKey = "1ac63f51-a004-4f88-bba3-2e2b4255eec5"; // Замените на ваш API-ключ

        public GeoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("coords")]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest("Адрес не должен быть пустым.");

            string url = $"https://geocode-maps.yandex.ru/1.x/?apikey={YandexApiKey}&geocode={Uri.EscapeDataString(address)}&format=json";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Ошибка запроса к Yandex API");

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                var geoObject = root
                    .GetProperty("response")
                    .GetProperty("GeoObjectCollection")
                    .GetProperty("featureMember")[0]
                    .GetProperty("GeoObject");

                // Получаем строку координат
                var pos = geoObject
                    .GetProperty("Point")
                    .GetProperty("pos")
                    .GetString();

                // Получаем полный адрес
                var fullAddress = geoObject
                    .GetProperty("metaDataProperty")
                    .GetProperty("GeocoderMetaData")
                    .GetProperty("text")
                    .GetString();

                // Удалим название страны (первое слово до запятой)
                var addressParts = fullAddress?.Split(',', 2); // split только на 2 части
                var addressWithoutCountry = addressParts != null && addressParts.Length == 2
                    ? addressParts[1].Trim()
                    : fullAddress;

                var coords = pos.Split(' ');
                var longitude = coords[0];
                var latitude = coords[1];

                if (double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double longitudeDouble) &&
                    double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double latitudeDouble))
                {
                    var positionContract = new PositionContract()
                    {
                        Latitude = latitudeDouble,
                        Longitude = longitudeDouble,
                        FullAddress = addressWithoutCountry
                    };

                    return Ok(positionContract);
                }
                else
                {
                    return BadRequest("Ошибка в преобразовании координат");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка обработки запроса: {ex.Message}");
            }
        }

    }
}
