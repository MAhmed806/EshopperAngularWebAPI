using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EShopperAngular.Stripe_Services
{
    public class StripeService
    {

        public StripeService()
        {
            StripeConfiguration.SetApiKey("sk_test_51MKM41BV4fcsoG0jx6BaLZtyH22qZeNe5jKgzx5gAZ40zG2sVc00946zFYsohLwGOwwUgyh6di8DprCUBWgRIsVZ00Pbtll7rR");
        }

        public Charge Charge(decimal amount, string custid, string cardid)
        {
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = (long)(amount*100),
                Currency = "pkr",
                Description = "EShopper Charge Receipt",
                Source = cardid,
                Customer = custid
            };
            var chargeService = new ChargeService();
            var stripeCharge = chargeService.Create(chargeOptions);
            return stripeCharge;
        }

        public Customer CreateCustomer(string Name, string stripeEmail, string StripeToken)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Name = Name,
                Email = stripeEmail,
                Source = StripeToken
            };

            var customerService = new CustomerService();
            var stripeCustomer = customerService.Create(customerOptions);
            return stripeCustomer;
        }
        public List<Card> GetCard(string customerid)
        {
            var cardService = new CardService();
            var cards = new List<Card>();
            var cardList = cardService.List(customerid);
            cards.AddRange(cardList.Data);
            return cards;
        }
        public string GetCurrentUserId(string Email)
        {
            var options = new CustomerListOptions { Email = Email };
            var service = new CustomerService();
            var customer = service.List(options);
            string custid = "";
            if (customer.Data.Count > 0)
            {
                foreach (var item in customer)
                {
                    custid = item.Id;
                }
            }
            return custid;
        }
        public Refund RefundCharge(string chargeid)
        {
            var chargeservice = new ChargeService();
            var charge=  chargeservice.Get(chargeid);
            if (!charge.Refunded)
            {
                var service = new RefundService();
                var refundoptions = new RefundCreateOptions
                {
                    Charge = chargeid,
                    Reason = RefundReasons.RequestedByCustomer
                };
                var refund = service.Create(refundoptions);
                return refund;
            }

            return null;
           
            
        }
        public bool RefundStatus(string chargeid)
        {
            var service = new ChargeService();
            var charge=service.Get(chargeid);
            return charge.Refunded;
        }
    }
}
