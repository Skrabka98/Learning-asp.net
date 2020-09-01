using MyFirstShopInASP.Models.Data;
using MyFirstShopInASP.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFirstShopInASP.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVMList;
            using(Db db = new Db())
            {
                categoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }


            return View(categoryVMList);
        }
        [HttpPost]
        public string AddCategory(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "bussyTitle";
                }
                CategoryDTO dto = new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 1000;
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
            }

                return id;
        }

        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }

            }



                return View();
        }



    }
}