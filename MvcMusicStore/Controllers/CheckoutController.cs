using MvcMusicStore.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        const string PromoCode = "FREE";

        //
        // GET: /Checkout/AddressAndPayment

        public ActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        public ActionResult AddressAndPayment([FromForm] Order order, [FromForm] string PromoCode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.Equals(PromoCode, PromoCode,
                        StringComparison.OrdinalIgnoreCase) == false)
                    {
                        return View(order);
                    }
                    else
                    {
                        order.Username = User.Identity.Name;
                        order.OrderDate = DateTime.Now;

                        //Save Order
                        storeDB.Orders.Add(order);
                        storeDB.SaveChanges();

                        //Process the order
                        var cart = ShoppingCart.GetCart(this.HttpContext);
                        cart.CreateOrder(order);

                        return RedirectToAction("Complete",
                            new { id = order.OrderId });
                    }
                }
                catch
                {
                    //Invalid - redisplay with errors
                    return View(order);
                }
            }

            //If we got this far, something failed, redisplay form
            return View(order);
        }

        //
        // GET: /Checkout/Complete

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Orders.Any(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}