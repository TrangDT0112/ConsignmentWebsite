using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Controllers
{
    [Authorize(Roles ="Customer")]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;

            }
            return View();
        }
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;

            }
            return View();
        }
        public ActionResult CheckOutSuccess()
        {
            
            return View();
        }
        public ActionResult Partial_CheckOut()
        {
            
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel order)
        {
            var code = new { success = false, Code = -1 };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {
                    Order or = new Order();
                    or.CustomerName = order.CustomerName;
                    or.Phone = order.Phone;
                    or.Address = order.Address;
                    cart.Items.ForEach(x => or.Order_Details.Add(new Order_Details
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    or.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                    or.TypePayment = order.TypePayment;
                    or.CreatedDate = DateTime.Now;
                    or.ModifiedDate = DateTime.Now;
                    or.CreatedBy = order.Phone;
                    Random rd = new Random();
                    or.Code = "CT" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    db.Orders.Add(or);
                    db.SaveChanges();

                    foreach (var item in cart.Items)
                    {
                        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Quantity -= item.Quantity;

                            if (product.Quantity <= 0)
                            {
                                // Cập nhật trạng thái SOLD cho ảnh mặc định
                                var defaultImage = product.ProductImage.FirstOrDefault(i => i.IsDefault);
                                if (defaultImage != null)
                                {
                                    defaultImage.IsSold = true; // -> Bạn cần thêm cột IsSold vào bảng ProductImage
                                }
                            }
                        }
                    }
                    db.SaveChanges();

                    //send mail to customer
                    var strProduct = "";
                    var totalAmount = decimal.Zero;
                    var Total = decimal.Zero;
                    foreach(var pr in cart.Items)
                    {
                        strProduct += "<tr>";
                        strProduct += "<td>"+pr.ProductName+"</td>";
                        strProduct += "<td>"+pr.Quantity+"</td>";
                        strProduct += "<td>"+ConsignmentWebsite.Common.Common.FormatNumber(pr.TotalPrice,0)+ "</td>";
                        strProduct += "</tr>";
                        totalAmount += pr.Price * pr.Quantity;
                    }
                    Total = totalAmount;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{OrderCode}}", or.Code);
                    contentCustomer = contentCustomer.Replace("{{Product}}", strProduct);
                    contentCustomer = contentCustomer.Replace("{{CustomerName}}", or.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", or.Phone);
                    contentCustomer = contentCustomer.Replace("{{OrderDate}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
                    contentCustomer = contentCustomer.Replace("{{DeliveryAddress}}", or.Address);
                    contentCustomer = contentCustomer.Replace("{{TotalAmount}}", ConsignmentWebsite.Common.Common.FormatNumber(totalAmount,0));
                    contentCustomer = contentCustomer.Replace("{{Total}}", ConsignmentWebsite.Common.Common.FormatNumber(Total, 0));
                    ConsignmentWebsite.Common.Common.SendMail("CiaraCycleFashion", "Order#" + or.Code, contentCustomer, order.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{OrderCode}}", or.Code);
                    contentAdmin = contentAdmin.Replace("{{Product}}", strProduct);
                    contentAdmin = contentAdmin.Replace("{{CustomerName}}", or.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", or.Phone);
                    contentAdmin = contentAdmin.Replace("{{OrderDate}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentAdmin = contentAdmin.Replace("{{Email}}", order.Email);
                    contentAdmin = contentAdmin.Replace("{{DeliveryAddress}}", or.Address);
                    contentAdmin = contentAdmin.Replace("{{TotalAmount}}", ConsignmentWebsite.Common.Common.FormatNumber(totalAmount, 0));
                    contentAdmin = contentAdmin.Replace("{{Total}}", ConsignmentWebsite.Common.Common.FormatNumber(Total, 0));
                    ConsignmentWebsite.Common.Common.SendMail("CiaraCycleFashion", "New Order#" + or.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
        }
       
        public ActionResult Partial_ItemCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        public ActionResult Partial_Item_Payment()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        //public ActionResult Index()
        //{
        //    ShoppingCart cart = (ShoppingCart)Session["Cart"];

        //    if (cart == null)
        //    {
        //        cart = new ShoppingCart(); // Khởi tạo giỏ hàng nếu chưa có
        //        Session["Cart"] = cart;
        //    }

        //    ViewBag.CheckCart = cart;
        //    return View();
        //}

        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart == null)
            {
                cart = new ShoppingCart();
                Session["Cart"] = cart;
            }

            int totalQuantity = cart.Items.Sum(x => x.Quantity);
            return Json(new { success = true, Count = totalQuantity }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    isRedirect = true,
                    redirectUrl = Url.Action("Login", "Account") 
                });
            }
            var code = new { success = false, msg = "", code = -1 , Count = 0};
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                if (checkProduct == null)
                {
                    return Json(code); 
                }

                if (checkProduct.Quantity == 0)
                {
                    code = new{ success = false, msg = "This product is out of stock, you cannot add to cart", code = 0, Count = 0 };
                    return Json(code);
                }
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == id);
                int currentCartQuantity = existingItem != null ? existingItem.Quantity : 0;

                if (currentCartQuantity + quantity > checkProduct.Quantity)
                { 
                    code = new { success = false, msg = "Insufficient quantity, please try other models!", code = 0, Count = cart.Items.Count };
                    return Json(code);
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Quantity = quantity
                };
                if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                }
                item.Price = checkProduct.Price;
                if (checkProduct.PriceSale > 0)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { success = true, msg = "Product added to cart successfully!", code = 1, Count = cart.Items.Count()};
            }
        
            return Json(code);
        }
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
       
        public ActionResult Delete(int id)
        {
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if(checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}