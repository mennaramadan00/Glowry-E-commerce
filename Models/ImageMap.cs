using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glowry.Models
{
    public class ImageMap
    {
        [Key]
        public int ImgMapId { get; set; }

        [ForeignKey("ProductImg")]
        public int ImgId { get; set; }
        public ProductImg ProductImg { get; set; }

        [ForeignKey("ProductOption")]
        public int OptionId { get; set; }
        public ProductOption ProductOption { get; set; }
    }
}
