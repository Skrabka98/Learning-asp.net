using MyFirstShopInASP.Models.Data;
using MyFirstShopInASP.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MyFirstShopInASP.Areas.Admin.Controllers
{
	public class ShopController : Controller
	{
		// GET: Admin/Shop/Categories
		public ActionResult Categories()
		{
			List<CategoryVM> categoryVMList;
			using (Db db = new Db())
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

		public ActionResult DeleteCategory(int id)
		{
			using (Db db = new Db())
			{
				CategoryDTO dto = db.Categories.Find(id);
				db.Categories.Remove(dto);
				db.SaveChanges();
			}




			return RedirectToAction("Categories");
		}
		[HttpPost]
		public string RenameCategory(string newCategoryName, int id)
		{
			using (Db db = new Db())
			{
				if (db.Categories.Any(x => x.Name == newCategoryName))
				{
					return "bussyTitle";
				}
				CategoryDTO dto = db.Categories.Find(id);
				dto.Name = newCategoryName;
				dto.Slug = newCategoryName.Trim().Replace(" ", "-").ToLower();
				db.SaveChanges();
			}

			return "good job";
		}

		public ActionResult AddProduct()
		{
			ProductVM productVM = new ProductVM();
			using (Db db = new Db())
			{
				productVM.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
			}


			return View(productVM);
		}
		[HttpPost]
		public ActionResult AddProduct(ProductVM productVM, HttpPostedFileBase file)
		{

			if (!ModelState.IsValid)
			{
				using (Db db = new Db())
				{
					productVM.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
					return View(productVM);
				}
			}
			using (Db db = new Db())
			{
				if (db.Products.Any(x => x.Name == productVM.Name))
				{
					ModelState.AddModelError("", "Podana nazwa jest zajęta");
					productVM.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
					return View(productVM);
				}
			}
			int id;
			using (Db db = new Db())
			{
				ProductDTO dto = new ProductDTO();
				dto.Name = productVM.Name;
				dto.Slug = productVM.Name.Replace(" ", "-").ToLower();
				dto.Description = productVM.Description;
				dto.Price = productVM.Price;
				dto.CategoryId = productVM.CategoryId;
				CategoryDTO categoryDTO = db.Categories.FirstOrDefault(x => x.Id == productVM.CategoryId);

				dto.CategoryName = categoryDTO.Name;

				db.Products.Add(dto);
				db.SaveChanges();

				id = dto.Id;
			}
			TempData["TK"] = "Pomyślnie dodano produkt";
			#region Upload
			var orginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
			var pathStr1 = Path.Combine(orginalDirectory.ToString(), "Products");
			var pathStr2 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString());
			var pathStr3 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
			var pathStr4 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
			var pathStr5 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

			if (!Directory.Exists(pathStr1))
			{
				Directory.CreateDirectory(pathStr1);
			}
			if (!Directory.Exists(pathStr2))
			{
				Directory.CreateDirectory(pathStr2);
			}
			if (!Directory.Exists(pathStr3))
			{
				Directory.CreateDirectory(pathStr3);
			}
			if (!Directory.Exists(pathStr4))
			{
				Directory.CreateDirectory(pathStr4);
			}
			if (!Directory.Exists(pathStr4))
			{
				Directory.CreateDirectory(pathStr5);
			}

			if ((file != null) && (file.ContentLength > 0))
			{
				string type = file.ContentType.ToLower();
				if (type != "image/jpg" &&
					type != "image/jpeg" &&
					type != "image/png" &&
					type != "image/gif")
				{

					using (Db db = new Db())
					{
						ModelState.AddModelError("", "Błędny format obrazu");
						productVM.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
						return View(productVM);
					}
				}
			}

			string imageName = file.FileName;
			using (Db db = new Db())
			{
				ProductDTO dto = db.Products.Find(id);
				dto.Image = imageName;
				db.SaveChanges();
			}

			var path = string.Format("{0}\\{1}", pathStr2, imageName);
			var pathThumb = string.Format("{0}\\{1}", pathStr3, imageName);
			file.SaveAs(path);
			WebImage img = new WebImage(file.InputStream);
			img.Resize(200, 200);
			img.Save(pathThumb);




			#endregion

			return RedirectToAction("AddProduct");
		}

		public ActionResult Product(int? page, int? catId)
        {
			List<ProductVM> listOfProducts;
			var pageNumber = page ?? 1;
			using (Db db = new Db())
            {
				listOfProducts = db.Products.ToArray().Where(x => catId == null || catId == 0 || x.CategoryId == catId)
					.Select(x => new ProductVM(x)).ToList();

				ViewBag.Categories = new SelectList (db.Categories.ToList(), "Id","Name");
				ViewBag.SelectedCat = catId.ToString();
            }

				return View();
        }
	}
}