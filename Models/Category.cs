using System.ComponentModel.DataAnnotations;

namespace Glowry.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public byte[] Img { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategName { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategSlug { get; set; }

        // Navigation property
        
        public ICollection<Product> Products { get; set; }
    }
}
