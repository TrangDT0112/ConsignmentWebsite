using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using ConsignmentWebsite.Models.Payments;

namespace ConsignmentWebsite.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: ShoppingCart
        [AllowAnonymous]
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;

            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Status = 2;//đã thanh toán
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Transaction was successful. Thank you for using our service.";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "An error occurred during processing. Error code: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.SuccessfulPayment = "Payment amount (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }
        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;

            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult Partial_CheckOut()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }
            return PartialView();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel order)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {
                    Order or = new Order();
                    or.CustomerName = order.CustomerName;
                    or.Phone = order.Phone;
                    or.Address = order.Address;
                    or.Email = order.Email;
                    or.Status = 1;
                    cart.Items.ForEach(x => or.Order_Details.Add(new Order_Details
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    or.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                    or.TypePayment = order.TypePayment;
                    or.ShippingStatus = order.ShippingStatus;
                    or.CreatedDate = DateTime.Now;
                    or.ModifiedDate = DateTime.Now;
                    or.CreatedBy = order.Phone;
                    if (User.Identity.IsAuthenticated)
                        or.CustomerId = User.Identity.GetUserId();
                    Random rd = new Random();
                    or.Code = "CT" + rd.Next(10000, 99999).ToString();
                    db.Orders.Add(or);
                    db.SaveChanges();


                    //send mail to customer
                    var strProduct = "";
                    var totalAmount = decimal.Zero;
                    var Total = decimal.Zero;
                    foreach (var pr in cart.Items)
                    {
                        strProduct += "<tr>";
                        strProduct += "<td>" + pr.ProductName + "</td>";
                        strProduct += "<td>" + pr.Quantity + "</td>";
                        strProduct += "<td>" + ConsignmentWebsite.Common.Common.FormatNumber(pr.TotalPrice, 0) + "</td>";
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
                    contentCustomer = contentCustomer.Replace("{{TotalAmount}}", ConsignmentWebsite.Common.Common.FormatNumber(totalAmount, 0));
                    contentCustomer = contentCustomer.Replace("{{Total}}", ConsignmentWebsite.Common.Common.FormatNumber(Total, 0));
                    ConsignmentWebsite.Common.Common.SendMail(ConfigurationManager.AppSettings["Email"], order.Email, "Order#" + or.Code, contentCustomer);
                    


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
                    ConsignmentWebsite.Common.Common.SendMail(ConfigurationManager.AppSettings["Email"], ConfigurationManager.AppSettings["EmailAdmin"], "New Order#" + or.Code, contentAdmin);
                    cart.ClearCart();
                    string url = "";
                    if (order.TypePayment == 1) // COD
                    {
                        foreach (var item in cart.Items)
                        {
                            var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                product.Quantity -= item.Quantity;
                                if (product.Quantity <= 0)
                                {
                                    var defaultImage = product.ProductImage.FirstOrDefault(i => i.IsDefault);
                                    if (defaultImage != null)
                                    {
                                        defaultImage.IsSold = true;
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        url = Url.Action("CheckOutSuccess", "ShoppingCart", new { code = or.Code });
                        cart.ClearCart();
                        code = new { Success = true, Code = 1, Url = url };
                    }
                    else // VNPay
                    {
                        // Cập nhật SOLD như COD
                        foreach (var item in cart.Items)
                        {
                            var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                product.Quantity -= item.Quantity;
                                if (product.Quantity <= 0)
                                {
                                    var defaultImage = product.ProductImage.FirstOrDefault(i => i.IsDefault);
                                    if (defaultImage != null)
                                    {
                                        defaultImage.IsSold = true;
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        url = UrlPayment(order.TypePaymentVN, or.Code);
                        code = new { Success = true, Code = 2, Url = url };
                    }
                }
            }
            return Json(code);
        }

        [AllowAnonymous]
        public ActionResult CheckOutSuccess(string orderCode)
        {
            var order = db.Orders.Include("Order_Details").FirstOrDefault(o => o.Code == orderCode);
            if (order != null && order.Status == 1)
            {
                foreach (var item in order.Order_Details)
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        if (product.Quantity <= 0)
                        {
                            var defaultImage = product.ProductImage.FirstOrDefault(i => i.IsDefault);
                            if (defaultImage != null)
                            {
                                defaultImage.IsSold = true;
                            }
                        }
                    }
                }
                order.Status = 2; // đã xử lý
                db.SaveChanges();
            }

            return View(order);
        }

        [AllowAnonymous]
        public ActionResult Partial_ItemCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult Partial_Item_Payment()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        [AllowAnonymous]
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
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                //if (checkProduct == null)
                //{
                //    return Json(code);
                //}

                if (checkProduct.Quantity == 0)
                {
                    code = new { success = false, msg = "This product is out of stock, you cannot add to cart", code = 0, Count = 0 };
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
                code = new { success = true, msg = "Product added to cart successfully!", code = 1, Count = cart.Items.Count() };
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
                if (checkProduct != null)
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
       
        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Pay for order:" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}
