using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class AProductShop
    {
        [Key]
        public long Id { get; set; }

        public long ProductId { get; set; }
        public AProduct Product { get; set; }

        public long ProductCount { get; set; }

        public long ShopId { get; set; }
        public AShop Shop { get; set; }
    }
}
