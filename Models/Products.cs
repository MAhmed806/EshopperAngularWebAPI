using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

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
        [DisplayName("Available Colors")]
        [NotMapped]
        public List<ProductColors> ProductColors { get; set; }
        [NotMapped]
        public List<Color> AllColors { get; set; }
        [Required]
        [Display(Name = "Available Quantity")]
        public int Availablequantity { get; set; }
        [Required]
        [Display(Name = "Product Quantity")]
        public int Quantity { get; set; }
        public string Description { get; set; }
        [Required]
        [DisplayName("Product Id")]
        public int ProductTypeID { get; set; }

        [ForeignKey("ProductTypeID")]
        public ProductTypes ProductTypes { get; set; }
    }
}
