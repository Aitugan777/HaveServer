using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AitukServer.Models
{
    public class APhoto
    {
        [Key]
        public long Id { get; set; }

        public long ParentId { get; set; }

        public EPhotoFor PhotoFor { get; set; }
    }
}
