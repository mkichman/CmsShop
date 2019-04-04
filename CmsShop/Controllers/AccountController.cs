using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Account;
using CmsShop.Views.Account;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

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

        // POST: /account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // sprawdzenie czy formularze są dobrze wypełnione
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            // sprawdzenie czy użytkownik jest prawidłowy
            bool isValid = false;
            using(Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło");
                return View(model);
            } else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
        }

        // GET: /account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // sprawdzenie model state
            if(!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // sprawdzenie hasła
            if(!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Podane hasła się różnią");
                return View("CreateAccount", model);
            }

            // sprawdzenie unikalności nazwy użytkownika
            using (Db db = new Db())
            {
                if(db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Ta nazwa jest już zajęta");
                    model.Username = "";
                    return View("CreateAccount", model);
                }
                // utworzenie użytkownika
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password,
                };

                // dodanie użytkownika i zapis na bazie
                db.Users.Add(userDTO);
                db.SaveChanges();

                // dodanie roli dla użytkownika
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = userDTO.Id,
                    RoleId = 2
                };

                // dodanie roli
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // komunikat 
            TempData["SM"] = "Jesteś teraz zarejestrowany i możesz się zalogować";

            return Redirect("~/account/login");
        }

        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();


            return Redirect("~/account/login");
        }

        public ActionResult UserNavPartial()
        {
            // pobranie username
            string username = User.Identity.Name;

            // deklaracja modelu
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // pobranie użytkownika
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            return PartialView(model);
        }
    }
}