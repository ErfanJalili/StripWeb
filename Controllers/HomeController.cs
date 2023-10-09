using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripWeb.Models;
using System.Diagnostics;

namespace StripWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly StripSettings _stripSettings;

        public string SessionId { get; set; }
        public HomeController(ILogger<HomeController> logger ,IOptions<StripSettings> stripSettings)
        {
            _logger = logger;
            _stripSettings =stripSettings.Value;
        }

        public IActionResult Index()
        {
            ViewBag.Message="Hello";
            return View();
        }
        public IActionResult Succsses()
        {
            ViewBag.Message="Success";

            return View("Index");
        }
        public IActionResult Cancel()
        {
            ViewBag.Message="Cancel";
            return View("Index");
        }
        public IActionResult CreateCheckoutSession(string amount)
        {
            var currency = "usd";
            var succssesUrl = "https://localhost:44397/Home/Succsses";
            var cancelUrl = "https://localhost:44397/Home/Cancel";

            StripeConfiguration.ApiKey=_stripSettings.SecretKey;

            var opt = new SessionCreateOptions
            {

                PaymentMethodTypes = new List<string>
                {
                       "pm_card_visa"
                },
                LineItems= new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = Convert.ToInt32(amount) * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name="My Product",
                                Description = "Prodcut Description",
                                
                            }
                            
                        },
                        Quantity =1,

                    }
                },
                Mode ="payment",
                SuccessUrl=succssesUrl,
                CancelUrl=cancelUrl,
            };

            var service = new SessionService();
            var session = service.Create(opt,null); 
            SessionId = session.Id;

            return Redirect(session.Url);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}