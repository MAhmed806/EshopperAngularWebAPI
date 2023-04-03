using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EShopperAngular.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }
        [Display(Name = "Order Id")]
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        public string OrderNum { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Phone#")]
        public string CustomerPhone { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; }
        [Required]
        [Display(Name = "Postal Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Order Cost")]
        public int OrderCost { get; set; }

        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public  ChargeAgainstOrder PaymentDetails { get; set; }
    }
}
