using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class ACategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AProduct> Products { get; set; }
    }
}
