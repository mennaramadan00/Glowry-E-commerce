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

        [Required]
        [ForeignKey("Product")]
        public int ProId { get; set; }
        public Product Product { get; set; }

        public int StockQuantity { get; set; }

        public ICollection<ImageMap> ImageMaps { get; set; }= new List<ImageMap>();
        public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
        public ICollection<WishListItem> WishlistItems { get; set; }= new List<WishListItem>();
    }
}
