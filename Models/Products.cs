using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace EShopperAngular.Models
{
    public class Products
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Image { get; set; }

        [DisplayName("Product Color")]
        public string PColor { get; set; }
        [Required]
        [Display(Name = "Available Quantity")]
        public int Availablequantity { get; set; }
        [Required]
        [Display(Name = "Product Quantity")]
        public int Quantity { get; set; }
        [Required]
        [DisplayName("Product Id")]
        public int ProductTypeID { get; set; }

        [ForeignKey("ProductTypeID")]
        public ProductTypes ProductTypes { get; set; }
    }
}
