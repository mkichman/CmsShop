using System.Web.Mvc;

namespace CmsShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        // GET: /account/login
        public ActionResult Login()
        {
            // sprawdzenie czy użytkownik nie jest już zalogowany
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            // zwrócenie widoku logowania
            return View();
        }


        // GET: /account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }


        
    }
}