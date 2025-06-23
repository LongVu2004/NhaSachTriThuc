using Microsoft.AspNetCore.Mvc;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Services;

namespace NhaSachTriThuc.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            Console.WriteLine("CreatePaymentUrlVnpay called with model: " + ViewBag.Total);
            Console.WriteLine($"Amount = {model.Amount}, OrderDescription = {model.OrderDescription}, Name = {model.Name}");
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        
    }
}
