using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //deklaracja listy kategorii do wyświetlenia
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

                return View(categoryVMList);
        }
        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory( string catName)
        {
            //Deklaracja id
            string id;

            using (Db db = new Db())
            {
                // sprawdzenie czy kategoria jest unikalna
                if(db.Categories.Any(x => x.Name == catName))
                    return "tytulzajety";

                // inicjalizacja dto
                CategoryDTO dto = new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 1000;

                // zapis do bazy
                db.Categories.Add(dto);
                db.SaveChanges();

                // pobieranie id
                id = dto.Id.ToString();
            }

            return id;
        }
        //POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // inicjalizacja licznika
                int count = 1;

                // deklaraja DTO 
                CategoryDTO dto;

                // sortowanie kategorii
                foreach(var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
                return View();
            }

        // GET: Admin/Shop/DeleteCategory
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // pobranie kategorii do usunięcia o podanym id 
                CategoryDTO dto = db.Categories.Find(id);

                //usunięcie kategorii
                db.Categories.Remove(dto);

                //zapis bazy
                db.SaveChanges();
            }

                return RedirectToAction("Categories");
        }
    }
}