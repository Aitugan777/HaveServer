using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HaveServer.Models
{
    public class HShop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        [ForeignKey("HSeller")]
        public int SellerId { get; set; }

        [JsonIgnore]
        public HSeller Seller { get; set; }

        [JsonIgnore]
        public List<HProduct> Products { get; set; }
    }
}
