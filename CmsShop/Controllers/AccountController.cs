using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Account;
using CmsShop.Models.ViewModels.Shop;
using CmsShop.Views.Account;
using System.Collections.Generic;
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
                    Address = model.Address,
                    Post = model.Post,
                    PostOffice = model.PostOffice
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
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();


            return Redirect("~/account/login");
        }

        [Authorize]
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

        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            // pobranie nazwy użytkownika 
            string username = User.Identity.Name;

            // deklaracja modelu
            UserProfileVM model;

            using(Db db = new Db())
            {
                // pobranie użytkownika 
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                model = new UserProfileVM(dto);
            }
            
            return View("UserProfile",model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            // sprawdzenie model state
            if(!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // sprawdzenie czy hasła się zgadzają 
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Hasła nie pasują do siebie");
                    return View("UserProfile", model);
                }
            }

            using(Db db = new Db())
            {
                // pobranie nazwy użytkownika 
                string username = User.Identity.Name;
                
                // sprawdzenie unikalności nazwy użytkownika
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "Ta nazwa użytkownika jest już zajęta");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                // edycja DTO
                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // zapis 
                db.SaveChanges();
            }

            // ustawienie komunikatu 
            TempData["SM"] = "Profil został zaktualizowany";

            return Redirect("~/account/user-profile");
        }

        // GET: /account/orders
        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            // inicjalizacja listy zamówień dla użytkownika
            List<OrderUserVM> ordersUser = new List<OrderUserVM>();

            using (Db db = new Db())
            {
                // pobranie id użytkownika
                UserDTO user = db.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;

                // pobranie zamówienia dla użytkownika
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();

                foreach (var order in orders)
                {
                    // inicjalizacja słownika produktów 
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    decimal total = 0m;

                    // pobranie szczegółów zamówienia
                    List<OrderDetailsDTO> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    foreach(var orderDetails in orderDetailsDTO)
                    {
                        // pobranie produktu
                        ProductDTO product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();

                        // pobranie ceny
                        decimal price = product.Price;

                        // pobranie nazwy
                        string productName = product.Name;

                        // dodanie produktu do słownika 
                        productsAndQty.Add(productName, orderDetails.Quantity);

                        total += orderDetails.Quantity * price;
                    }

                    ordersUser.Add(new OrderUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
            }
            return View(ordersUser);
        }




    }
}