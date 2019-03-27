using CmsShop.Models.ViewModels.Cart;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CmsShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CartPartial()
        {
            // inicjalizacja CartVM
            CartVM model = new CartVM();

            // inicjalizacja ilosc i cena
            int qty = 0;
            decimal price = 0;

            // sprawdzenie czy dane koszyka są zapisane w sesji
            if (Session["cart"] != null)
            {
                // pobieranie wartości z sesji
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.ProductId;
                }
            }
            else
            {
                // ustawienie ilość i cena na 0
                qty = 0;
                price = 0m;
            }

            return PartialView(model);
        }

    }
}