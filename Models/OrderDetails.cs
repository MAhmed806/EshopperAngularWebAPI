using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EShopperAngular.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        [Display(Name = "Order")]
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Product Quantity")]
        public int ProductQuantity { get; set; }

        
        //public Order? Order { get; set; }
        [ForeignKey("ProductId")]
        public Products? Product { get; set; }
    }
}
