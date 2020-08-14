using MyFirstShopInASP.Models.Data;
using MyFirstShopInASP.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyFirstShopInASP.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        public ActionResult Index()
        {
            //Lista Stron
            List<PageVM> pagesList;
            using (Db db = new Db())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pagesList);
        }
        //GET
        [HttpGet]
        public ActionResult AddPage()
        {
           return View();
        }
        //POST
        [HttpPost]
        public ActionResult AddPage(PageVM pageVM)
        {

            if (!ModelState.IsValid)
            {
                return View(pageVM);
            }

            using (Db db = new Db())
            {
                string slug;
                PageDTO dto = new PageDTO();

                //usupełnienie sluga
                if (string.IsNullOrWhiteSpace(pageVM.Slug))
                {
                    slug = pageVM.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = pageVM.Slug.Replace(" ", "-").ToLower();
                }
                //uniemożliwienie dodawania 2x tą samą strone
                if (db.Pages.Any(x => x.Title == pageVM.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Taki tytuł lub adres strony jest zajęty.");
                    return View(pageVM);

                }
                dto.Title = pageVM.Title;
                dto.Slug = slug;
                dto.Body = pageVM.Body;
                dto.HasSidebar = pageVM.HasSidebar;
                dto.Sorting = 10000;
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["TK"] = "Strona została utworzona!";

                return Redirect("AddPage");
        }
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM pageVM;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if(db == null)
                {
                    return Content("Taka strona nie istnieje");
                }

                pageVM = new PageVM(dto);
            }

                return View(pageVM);
        }
        [HttpPost]
        public ActionResult EditPage(PageVM pageVM)
        {
            if (!ModelState.IsValid)
            {
                return View(pageVM);
            }

            using (Db db = new Db())
            {
                int id = pageVM.Id;
            }

                return View();
        }

    }
}