using Stripe;

namespace EShopperAngular.Models
{
    public class ChargeCardViewModel
    {
        public string CustomerId { get; set; }
        public string Cardid { get; set; }
        public List<Card> Cards { get; set; }
    }
}
