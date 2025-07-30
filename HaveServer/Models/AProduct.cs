using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AitukServer.Models
{
    public class AProduct
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string Brand { get; set; }
        public string Code { get; set; }
        public string KeyWords { get; set; }

        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public int GenderId { get; set; }

        public ICollection<AProductShop> ProductShops { get; set; }
        public ICollection<AProductPhoto> Photos { get; set; }
        public ICollection<AProductSize> Sizes { get; set; }
    }
}
