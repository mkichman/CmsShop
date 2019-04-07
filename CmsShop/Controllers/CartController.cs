﻿using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // inicjalizacja koszyka
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // sprawdzenie czy koszyk jest pusty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Twój koszyk jest pusty";
                return View();
            }

            // obliczenie podsumowania koszyka i przekazanie do ViewBag
            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(cart);
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
                    price += item.Quantity * item.Price;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // ustawienie ilość i cena na 0
                qty = 0;
                price = 0m;
            }

            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // inicjalizacja CartVM List
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // inicjalizacja CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // pobranie produktu
                ProductDTO product = db.Products.Find(id);

                // sprawdzenie czy produkt nie znajduje się w koszyku
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // dodanie produktu do koszyka
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
            }
            // pobranie wartosci ilosci i ceny i dodanie do modelu
            int qty = 0;
            decimal price = 0;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }
            model.Quantity = qty;
            model.Price = price;

            // zapis w sesji
            Session["cart"] = cart;

            return PartialView(model);
        }

        public JsonResult IncrementProduct(int productId)
        {
            // inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobranie CartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // zwiekszenie ilosci produktu
            model.Quantity++;

            // przygotowanie danych do jsona 
            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult DecrementProduct(int productId)
        {
            // inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobranie CartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // zmniejszenie ilosci produktu
            if (model.Quantity > 1)
            {
                model.Quantity--;
            } else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }           

            // przygotowanie danych do jsona 
            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void RemoveProduct(int productId)
        {
            // inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobranie CartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            //usuwanie produktu
            cart.Remove(model);
        }

        public ActionResult PaypalPartial()
        {
            // inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }

        [HttpPost]
        public void PlaceOrder()
        {
            // pobranie zawartości koszyka z sesji
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobranie nazwy użytkownika
            string username = User.Identity.Name;

            // deklaracja numer zamówienia
            int orderId = 0;

            using (Db db = new Db())
            {
                // inicjalizacja OrderDTO
                OrderDTO orderDTO = new OrderDTO();

                // pobranie user id
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                int userId = user.Id;

                // ustawienie DTO i zapis 
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO);
                db.SaveChanges();

                // pobranie id zapisanego zamówienia
                orderId = orderDTO.OrderId;

                // inicjalizacja orderDetailsDTO
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

                foreach(var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }
            }
                       
            // zresetowanie sesji
            Session["cart"] = null;
        }


    }
}