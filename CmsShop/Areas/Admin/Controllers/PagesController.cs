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
    }
}