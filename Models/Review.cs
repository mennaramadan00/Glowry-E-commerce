using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class Review
    {
        [Key]
        public int ReviewsId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public ApplicationUser AppUser { get; set; }

        public int RatingValue { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
