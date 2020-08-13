using System.Web.Mvc;

namespace MyFirstShopInASP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
    }
}