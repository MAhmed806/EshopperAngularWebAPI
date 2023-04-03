using EShopperAngular.Data;
using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using EShopperAngular.Stripe_Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Stripe;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly EShopperAngular.Stripe_Services.StripeService _stripeservice;
        private readonly CardService _cardService;
        private readonly IGenericRepository<ChargeAgainstOrder> _chargeAgainstOrderRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        public PaymentController(StripeService stripeservice, ApplicationDbContext context, UserManager<ApplicationUser> userManager, CardService cardservice,IGenericRepository<ChargeAgainstOrder> chargeagainstorder,IGenericRepository<Order> orderrepo)
        {
            _context = context;
            _stripeservice = stripeservice;
            _userManager = userManager;
            _cardService = cardservice;
            _chargeAgainstOrderRepository= chargeagainstorder;
            _orderRepository= orderrepo;
        }
        [HttpGet("GetCards/{Email}")]
        public IActionResult _Charge(string Email)
        {
            var options = new CustomerListOptions { Email = Email };
            var service = new CustomerService();
            var customer = service.List(options);
            string customerId = null;
            List<Card> cards = new List<Card>();
            foreach (var item in customer)
            {
                cards = _stripeservice.GetCard(item.Id);
                customerId = item.Id;
            }
            var model = new ChargeCardViewModel
            {
                CustomerId = customerId,
                Cards = cards
            };
            return Ok(model);
        }
        public class helloworld
        {
            public int cost { get; set; }
            public string cardid { get; set; }
            public string email { get; set; }
            public string orderid { get; set; }
        }
        [HttpPost("CreateCharge")]
        public IActionResult CreateCharge(JsonValue json)
        {
            helloworld Var = JsonSerializer.Deserialize<helloworld>(json);
            if (Var.cost == 0) { return BadRequest("Cost Cannot Be Zero"); }
            var options = new CustomerListOptions { Email = Var.email };
            var service = new CustomerService();
            var customer = service.List(options).Data[0];
            var stripeCharge = _stripeservice.Charge(Var.cost, customer.Id, Var.cardid);
            ChargeAgainstOrder cao= new ChargeAgainstOrder();
            cao.ChargeId = stripeCharge.Id;
            cao.ChargeStatus = stripeCharge.Status;
            cao.RefundId = "";
            cao.RefundStatus = "";
            cao.OrderId = Convert.ToInt32(Var.orderid);
            _chargeAgainstOrderRepository.Create(cao);
            _chargeAgainstOrderRepository.Save();
            return Ok(stripeCharge);
        }
        [HttpPost("Refund")]
        public IActionResult Refund(JsonValue json)
        {
            var id = JsonSerializer.Deserialize<int>(json);
            var cao = _chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == id) ;
            if (_orderRepository.Details(id).PaymentMethod=="Credit Card")
            {
                var chargeid= _chargeAgainstOrderRepository.Index()?.FirstOrDefault(x=>x.OrderId == id)?.ChargeId;
                if(chargeid!=null)
                {
                    var refund = _stripeservice.RefundCharge(_chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == id).ChargeId);
                    if (refund != null)
                    {
                        cao.ChargeStatus = "refunded";
                        cao.RefundId = refund.Id;
                        cao.RefundStatus = refund.Status;
                        _chargeAgainstOrderRepository.Edit(cao);
                        _chargeAgainstOrderRepository.Save();
                        return Ok(new { message = "Refund SuccessFull" });
                    }
                    else
                    {
                        return Ok(new { message = "Amount Already Refunded" });
                    }
                }
                else
                {
                    return Ok(new { message = "Charge Id K Masail" });
                }
               
            }
            else
            {
                return Ok(new { message = "Cash On Delivery Refunds are Initiated Manually" });
            }
        }
        [HttpGet("RefundStatus/{id}")]
        public IActionResult RefundCheck(string chargeid) 
        {
            return Ok(_stripeservice.RefundStatus(chargeid));
        }
        [HttpGet("ManageCards")]
        public IActionResult _ManageCards()
        {
            var options = new CustomerListOptions { Email = User.Identity.Name };
            var service = new CustomerService();
            var customer = service.List(options);
            string customerId = null;
            List<Card> cards = new List<Card>();
            foreach (var item in customer)
            {
                cards = _stripeservice.GetCard(item.Id);
                customerId = item.Id;
            }
            var model = new ChargeCardViewModel
            {
                CustomerId = customerId,
                Cards = cards
            };
            return Ok();
        }
        public class myself
        {
            public string name { get; set; }
            public string token { get; set; }
            public string email { get; set; }
        }
        [HttpPost("AddCard")]
        public IActionResult _ManageCards(JsonValue json)
        {
            var hehe = JsonSerializer.Deserialize<myself>(json);

            var options = new CustomerListOptions { Email = hehe.email };
            var service = new CustomerService();
            var customer = service.List(options);
            if (customer.Data.Count > 0)
            {
                foreach (var item in customer)
                {
                    var options1 = new CardCreateOptions
                    {
                        Source = hehe.token,
                    };
                    var custcard = _cardService.Create(item.Id, options1);

                    return Ok(custcard);

                }
            }
            else
            {
                var stripeCustomer = _stripeservice.CreateCustomer(hehe.name, hehe.email, hehe.token);
                return Ok(stripeCustomer);
            }

            return BadRequest();
        }

    }
}
