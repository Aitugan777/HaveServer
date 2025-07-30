using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class AProductSize
    {
        [Key]
        public long Id { get; set; }

        public int SizeId { get; set; }

        public long ProductId { get; set; }
    }
}
