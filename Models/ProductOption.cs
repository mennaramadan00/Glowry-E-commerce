using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class ProductOption
    {
        [Key]
        public int OptionId { get; set; }

        [MaxLength(50)]
        public string AttributeName { get; set; }

        [MaxLength(50)]
        public string AttributeValue { get; set; }

        public bool isdefault { get; set; } = false;

        [Required]
        [ForeignKey("Product")]
        public int ProId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }

        public int StockQuantity { get; set; }
        [ValidateNever]
        public ICollection<ImageMap> ImageMaps { get; set; }= new List<ImageMap>();
        [ValidateNever]
        public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
        [ValidateNever]
        public ICollection<WishListItem> WishlistItems { get; set; }= new List<WishListItem>();
    }
}
