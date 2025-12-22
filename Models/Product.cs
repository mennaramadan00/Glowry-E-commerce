using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class Product
    {
        [Key]
        public int ProId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProName { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProSlug { get; set; }

        [MaxLength(1500)]
        public string ProDescription { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool isfeatured { get; set; } = false;




        //fk

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        // Navigation property
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public ICollection<ProductImg> ProductImages { get; set; }
        [ValidateNever]
        public ICollection<ProductOption> ProductOptions { get; set; }



    }
}
