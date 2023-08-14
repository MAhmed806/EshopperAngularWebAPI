using EShopperAngular.DTOs;
using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EShopperAngular.Controllers.Customer
{
    [Route("/api/Home")]
    [ApiController]
    public class HomeController : Controller
    {
        private IGenericRepository<Products> _productsRepository;
        private IGenericRepository<ProductTypes> _productTypesRepository;
        private IGenericRepository<Order> _orderRepository;
        private IGenericRepository<OrderDetails> _orderDetailsRepository;
        private IGenericRepository<ChargeAgainstOrder> _paymentdetails;
        private IGenericRepository<ChargeAgainstOrder> _chargeao;
        public HomeController(IGenericRepository<Products> prepo, IGenericRepository<ProductTypes> ptrepo, IGenericRepository<Order> orepo, IGenericRepository<OrderDetails> odrepo, IGenericRepository<ChargeAgainstOrder> paymnetdetails, IGenericRepository<ChargeAgainstOrder> charge)
        {
            _productsRepository = prepo;
            _productTypesRepository = ptrepo;
            _orderRepository = orepo;
            _orderDetailsRepository = odrepo;
            _paymentdetails = paymnetdetails;
            _chargeao = charge;
        }

        [HttpGet("Products")]
        // GET: HomeController
        public IEnumerable<Products> Get()
        {
            var products = _productsRepository.Index();
            foreach (var item in products)
            {
                item.ProductTypes = _productTypesRepository.Details(item.ProductTypeID);
            }
            return products;
        }
        [HttpGet("lowtohigh")]
        // GET: HomeController
        public IEnumerable<Products> Filterproductslowtohigh()
        {
            var products = _productsRepository.Index();
            var orderedproducts = from product in products orderby product.Price ascending select product;
            foreach (var item in orderedproducts)
            {
                item.ProductTypes = _productTypesRepository.Details(item.ProductTypeID);
            }
            return orderedproducts;
        }
        [HttpGet("hightolow")]
        // GET: HomeController
        public IEnumerable<Products> Filterproductshightolow()
        {
            var products = _productsRepository.Index();
            var orderedproducts = from product in products orderby product.Price descending select product;
            foreach (var item in orderedproducts)
            {
                item.ProductTypes = _productTypesRepository.Details(item.ProductTypeID);
            }
            return orderedproducts;
        }

        [HttpGet("ProductTypes")]
        public IEnumerable<ProductTypes> GetPT()
        {
            var producttypes = _productTypesRepository.Index();
            return producttypes;
        }
        [HttpGet("Shop")]
        public ProductAndProductTypesDTO Shop()
        {
            var model = new ProductAndProductTypesDTO
            {
                products = _productsRepository.Index(),
                ptypes = _productTypesRepository.Index(),
            };
            return model;
        }
        [HttpGet("Search/{searchstring}")]
        public IEnumerable<Products> SearchProd(string searchstring)
        {
            if (!string.IsNullOrEmpty(searchstring))
            {
                string loweredstring = searchstring.ToLower();
                var products = _productsRepository.Index().Where(s => s.Name.ToLower().Contains(loweredstring));
                return products;
            }
            return null;
        }
        [HttpGet("ProductDetails/{id}")]
        public ActionResult<Products> ProductDetails(int id)
        {
            if (id != null)
            {
                var prod = _productsRepository.Details(id);
                if (prod != null)
                {
                    prod.ProductTypes = _productTypesRepository.Details(prod.ProductTypeID);
                    return prod;
                }
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("ProductTypeDetails/{id}")]
        public ProductAndProductTypesDTO CategoryDetails(int id)
        {
            if (id != 0)
            {
                var producttype = _productTypesRepository.Index().Where(x => x.Id == id);
                var prod = _productsRepository.Index().Where(x => x.ProductTypeID == id);
                if (prod != null)
                {
                    foreach (var item in prod)
                    {
                        item.ProductTypes = _productTypesRepository.Details(item.ProductTypeID);
                    }
                    var final = new ProductAndProductTypesDTO
                    {
                        products = prod,
                        ptypes = producttype
                    };
                    return final;
                }
                return null;
            }
            return null;
        }



        [HttpPost("Checkout")]
        public IActionResult Checkout(JsonValue Order)
        {
            if (Order != null)
            {
                var order = JsonSerializer.Deserialize<Order>(Order);
                foreach (var item in order.OrderDetails)
                {
                    item.Product = _productsRepository.Details(item.ProductId);

                }
                string GetOrderNo()
                {
                    int rowCount = _orderRepository.Index().Count() + 1;
                    return rowCount.ToString("000");
                }
                order.OrderNum = GetOrderNo();
                order.OrderDate = DateTime.Now;
                order.DeliveryDate = order.OrderDate.AddDays(5);
                order.OrderStatus = "Order Placed";
                _orderRepository.Create(order);
                _orderRepository.Save();
                if (order.PaymentMethod == "Cash On Delivery")
                {
                    ChargeAgainstOrder charge = new ChargeAgainstOrder();
                    charge.OrderId = order.Id;
                    charge.ChargeStatus = "pending";
                    charge.ChargeId = "";
                    _chargeao.Create(charge);
                    _chargeao.Save();

                }
                return Ok(order);
            }
            return Ok("Order Place Unsuccessfull");
        }
        [HttpGet("Filter/{min}/{max}")]
        public IEnumerable<Products> Filteration(int min, int max)
        {

            var products = _productsRepository.Index().Where(x => x.Price >= min && x.Price <= max);
            foreach (var prod in products)
            {
                prod.ProductTypes = _productTypesRepository.Details(prod.ProductTypeID);
            }
            return products;
        }
        [HttpGet("OrdersList")]
        public IEnumerable<Order> Orderslist()
        {
            return _orderRepository.Index();
        }
        [HttpGet("OrderDetails/{id}")]
        public ActionResult<Order> OrderDetails(int id)
        {
            Order order = _orderRepository.Details(id);
            if (order != null)
            {
                List<OrderDetails> details = _orderDetailsRepository.Index().Where(x => x.OrderId == id).ToList();
                foreach (var item in details)
                {
                    item.Product = _productsRepository.Details(item.ProductId);
                    item.Product.ProductTypes = _productTypesRepository.Details(item.Product.ProductTypeID);
                }
                order.OrderDetails = details;
                order.PaymentDetails = _paymentdetails.Index().FirstOrDefault(x => x.OrderId == id);
                return order;

            }
            return NotFound();

        }
        [HttpDelete("CancelOrder/{id}")]
        public ActionResult<Order> CancelOrder(int id)
        {
            if (id != null)
            {
                Order order = _orderRepository.Details(id);
                if (order != null)
                {
                    order.OrderStatus = "Order Cancelled";
                    _orderRepository.Edit(order);
                    _orderRepository.Save();
                    return order;
                }
                return NotFound();
            }
            return NotFound();

        }
        [HttpGet("MyOrderList/{username}")]
        public IEnumerable<Order> GetOrders(string username)
        {
            if (username != null)
            {
                IEnumerable<Order> Order = _orderRepository.Index().Where(x => x.UserId == username);
                return Order;
            }
            return Enumerable.Empty<Order>();
        }

        //// GET: HomeController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: HomeController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: HomeController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: HomeController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: HomeController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: HomeController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
