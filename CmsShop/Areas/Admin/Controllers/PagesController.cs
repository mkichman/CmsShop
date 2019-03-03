using System.Web.Mvc;
using System.Collections.Generic;
using CmsShop.Models.ViewModels.Pages;
using CmsShop.Models.Data;
using System.Linq;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // deklaracja listy PageVM
            List<PageVM> pagesList;

            
            using (Db db = new Db())
            {
                // inicjalizaja listy
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList(); 
            }

            
            // zwracanie stron do widoku
            return View(pagesList);
        }
        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Sprawdzenie model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                string slug;

                //inicjalizacja PageDTO
                PageDTO dto = new PageDTO();

                //jezeli nie ma adresu strony to przypisany jest jej tytuł

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //zapobiegnięcie dodaniu takiej samej nazwy strony
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł lub adres strony już istnieje");
                    return View(model);
                }
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;

                //zapis dto 
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Dodałeś nową stronę";

                return RedirectToAction("AddPage");
        }

        //GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // deklaracja Page View Model
            PageVM model;

            using (Db db = new Db())
            {
                //pobranie strony o przekazanym id 
                PageDTO dto = db.Pages.Find(id);

                //sprawdzenie czy strona istnieje 
                if (dto == null)
                {
                    return Content("Strona nie istnieje");
                }

                model = new PageVM(dto);
            }

                return View(model);
        }

        //POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                // pobranie id strony doo edycji
                int id = model.Id;

                // inicjalizacja slug
                string slug = "home";

                // pobranie strony do edycji 
                PageDTO dto = db.Pages.Find(id);


                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    } else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // sprawdzenie unikalnosc strony, adresu 
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || 
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona lub tytuł już istnije");
                }

                // modyfikacja DTO 

                dto.Title = model.Title;
                dto.Slug = slug;
                dto.HasSidebar = model.HasSidebar;
                dto.Body = model.Body;

                // zapisanie do bazy

                db.SaveChanges();
            }

            // ustawienie komunikatu
            TempData["SM"] = "Wyedytowałeś stronę";

            //przekierowanie do strony editpage 
            return RedirectToAction("EditPage");
        }

        //GET: Admin/Pages/Details
        public ActionResult Details(int id)
        {
            // deklaracja Page VM
            PageVM model;

            using (Db db = new Db())
            {
                // pobranie strony o id
                PageDTO dto = db.Pages.Find(id);

                // sprawdzenie czy strona istnieje
                if (dto == null)
                {
                    return Content("Strona o podanym id nie istnieje");
                }

                // inicjalizacja PageVM
                model = new PageVM(dto);
            }
                return View(model); 
        }

        // GET: Admin/Pages/Delete/id
        public ActionResult Delete(int id)
        {
            using (Db db = new Db())
            {
                // pobranie strony do usuniecia
                PageDTO dto = db.Pages.Find(id);

                // usuwanie strony z bazy
                db.Pages.Remove(dto);

                //zapis zmian
                db.SaveChanges();
            }
            // przekierowanie
            return RedirectToAction("Index");
        }
    }
}