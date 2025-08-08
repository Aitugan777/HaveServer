using AitukCore.Contracts;
using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class AWorkSheldure
    {
        [Key]
        public long Id { get; set; }

        public long ShopId { get; set; }
        public AShop Shop { get; set; }

        public AWorkDay? Monday { get; set; }
        public AWorkDay? Tuesday { get; set; }
        public AWorkDay? Wednesday { get; set; }
        public AWorkDay? Thursday { get; set; }
        public AWorkDay? Friday { get; set; }
        public AWorkDay? Saturday { get; set; }
        public AWorkDay? Sunday { get; set; }
    }

}
