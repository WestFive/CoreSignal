using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace CoreSignal.Controllers
{
    public class HomeController : Controller
    {
        [EnableCors("CorsSample")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            //ViewData["Message"] = "Your application description page.";

            return View();
        }


        [EnableCors("CorsSample")]
        public IActionResult Contact()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        [EnableCors("CorsSample")]
        public IActionResult Chat()
        {
           
            return View();
        }

        [EnableCors("CorsSample")]
        public IActionResult Error()
        {
            return View();
        }

    }
}
