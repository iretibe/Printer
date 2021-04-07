using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neodynamic.SDK.Web;
using Printer.Models;
using System.Diagnostics;

namespace Printer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["WCPPDetectionScript"] = WebClientPrint.CreateWcppDetectionScript(
                Url.Action("ProcessRequest", "WebClientPrintAPI", null, 
                Url.ActionContext.HttpContext.Request.Scheme), Url.ActionContext.HttpContext.Session.Id);

            return View();
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
