using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Manager
{
    [Route("/api/Manager/")]
    public class ManageOrders : Controller
    {
        private IGenericRepository<Order> _orderRepository;
        private IGenericRepository<OrderDetails> _orderDetailsRepository;
        private IGenericRepository<Products> _productRepository;
        private IGenericRepository<ProductTypes> _productTypesRepository;
        private IGenericRepository<ChargeAgainstOrder> _chargeAgainstOrderRepository;
        public ManageOrders(IGenericRepository<Order> orderRepository, IGenericRepository<OrderDetails> orderDetailsRepository, IGenericRepository<Products> genericRepository, IGenericRepository<ProductTypes> productTypesRepository, IGenericRepository<ChargeAgainstOrder> generic)
        {
            _orderRepository = orderRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _productRepository = genericRepository;
            _productTypesRepository = productTypesRepository;
            _chargeAgainstOrderRepository = generic;
        }
        [HttpGet("Orderslist")]
        public ActionResult<IEnumerable<Order>> Get()
        {

            var orders = _orderRepository.Index();
            if (orders.Count() <= 0)
            {
                return NoContent();
            }
            foreach (var order in orders)
            {
                order.PaymentDetails = _chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == order.Id);

                if (order.PaymentDetails == null)
                {
                    ChargeAgainstOrder hello = new ChargeAgainstOrder();
                    hello.ChargeStatus = "";
                    order.PaymentDetails = hello;
                }
            }
            return Ok(orders);
        }
        [HttpGet("OrderDetails/{id}")]
        public ActionResult<Order> Get(int id)
        {
            Order order = _orderRepository.Details(id);
            if (order != null)
            {
                List<OrderDetails> ODs = _orderDetailsRepository.Index().Where(x => x.OrderId == id).ToList();
                foreach (var ods in ODs)
                {
                    ods.Product = _productRepository.Details(ods.ProductId);
                    ods.Product.ProductTypes = _productTypesRepository.Details(ods.Product.ProductTypeID);
                }
                order.PaymentDetails = _chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == id);
                return order;
            }
            return NotFound();
        }
        [HttpPut("UpdateOrder")]
        public ActionResult Put([FromBody] JsonObject json)
        {
            try
            {
                var order = JsonSerializer.Deserialize<Order>(json);
                var paymentstatus = json["PaymentStatus"]?.ToString();
                var refundstatus = json["RefundStatus"]?.ToString();
                order.OrderDetails = _orderDetailsRepository.Index().Where(x => x.OrderId == order.Id).ToList();
                if (paymentstatus != null)
                {
                    if (_chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == order.Id) == null)
                    {
                        ChargeAgainstOrder cao = new ChargeAgainstOrder();
                        cao.OrderId = order.Id;
                        cao.ChargeStatus = paymentstatus;
                        cao.RefundStatus = refundstatus;
                        cao.ChargeId = "Cash On Delivery";
                        cao.RefundId = "No Refund on Cash on Delivery";
                        _chargeAgainstOrderRepository.Create(cao);
                        _chargeAgainstOrderRepository.Save();
                    }
                    else
                    {
                        var pd = _chargeAgainstOrderRepository.Index().FirstOrDefault(x => x.OrderId == order.Id);
                        pd.ChargeStatus = paymentstatus;
                        pd.RefundStatus = refundstatus;
                        _chargeAgainstOrderRepository.Edit(pd);
                        _chargeAgainstOrderRepository.Save();
                    }
                }
                _orderRepository.Edit(order);
                _orderRepository.Save();
                return Ok(new { Message = "OrderUpdateSuccess" });
            }
            catch (Exception ex)
            {
                return Ok(ex.InnerException.Message);
            }

        }
        [HttpGet("GetChartData")]
        public JsonResult GetData()
        {
            var hello = _orderDetailsRepository.Index();
            foreach (var item in hello)
            {
                item.Product = _productRepository.Details(item.ProductId);
            }
            var query = hello.GroupBy(p => p.Product.Name).Select(g => new { ProductName = g.Key, OrderQuantity = g.Sum(w => w.ProductQuantity) }).ToList();
            return Json(query);
        }
    }
}
