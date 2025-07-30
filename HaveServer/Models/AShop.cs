using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AitukServer.Models
{
    public class AShop
    {
        public long Id { get; set; }
        public long SellerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public ICollection<AShopPhoto> Photos { get; set; }
        public ICollection<AProductShop> ProductShops { get; set; }
    }
}