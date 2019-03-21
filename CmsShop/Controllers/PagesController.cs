using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // ustawienie adresu strony
            if (page == "")
                page = "home";

            // deklaracja pageVM i pageDTO
            PageVM model;
            PageDTO dto;

            // sprawdzenie czy strona istnieje 
            using (Db db = new Db())
            {
                if(! db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });

            }

            // pobranie pageDTO
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // ustawienie tytułu strony
            ViewBag.PageTitle = dto.Title;

            // sprawdzenie czy strona ma pasek boczny
            if (dto.HasSidebar == true)
                ViewBag.SideBar = "Tak";
            else
                ViewBag.SideBar = "Nie";

            // inicjalizacja pageVM
            model = new PageVM(dto);

            // zwrócenie widoku z modelem
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // deklaracja PageVM
            List<PageVM> pageVMList;

            // pobranie stron 
            using(Db db = new Db())
            {
                pageVMList = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // deklaracja modelu
            SidebarVM model;

            // inicjaliacja modelu
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                model = new SidebarVM(dto);
            }

                return PartialView(model);
        }


    }
}