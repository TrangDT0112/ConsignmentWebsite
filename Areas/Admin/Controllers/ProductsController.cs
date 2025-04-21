using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsignmentWebsite.Models;
using ConsignmentWebsite.Models.EF;

namespace ConsignmentWebsite.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string searchText, int? page)
        {
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.ConsignmentOrder = new SelectList(db.ProductCategories.ToList(), "Id", "ConsignmentCode");
            var brandList = db.Brands
                              .Select(b => new SelectListItem
                              {
                                  Text = b.BrandName,
                                  Value = b.Id.ToString()
                              }).ToList();
            brandList.Add(new SelectListItem
            {
                Text = "Other Brand",
                Value = "0"
            });
            ViewBag.BrandList = brandList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault, string SelectedBrandId)
        {
            if (ModelState.IsValid)
            {
                int brandId = 0;
                if (!string.IsNullOrEmpty(SelectedBrandId) && int.TryParse(SelectedBrandId, out brandId))
                {
                    if (brandId == 0)
                    {
                        var newBrand = new Brand {
                            BrandName = "Other Brand",
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };
                        db.Brands.Add(newBrand);
                        db.SaveChanges();
                        model.BrandId = newBrand.Id;
                    }
                    else
                    {
                        model.BrandId = brandId;
                    }
                }
                else
                {
                    ModelState.AddModelError("SelectedBrandId", "Please choose brand");
                    goto ReloadDropdown;
                }

                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        bool isDefault = (i + 1 == rDefault[0]);
                        model.ProductImage.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            Image = Images[i],
                            IsDefault = isDefault
                        });

                        if (isDefault)
                        {
                            model.Image = Images[i];
                        }
                    }
                }

                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }

                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        ReloadDropdown:
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title", model.ProductCategoryId);
            ViewBag.ConsignmentOrder = new SelectList(db.ConsignmentOrders.ToList(), "Id", "ConsignmentCode", model.ConsignmentOrderId);
            var brandList = db.Brands
                              .Select(b => new SelectListItem
                              {
                                  Text = b.BrandName,
                                  Value = b.Id.ToString()
                              }).ToList();
            
            ViewBag.BrandList = brandList;

            return View(model);
        }

        // Edit method without duplication
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Load Product Category dropdown
            ViewBag.ProductCategory = db.ProductCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title,
                    Selected = (c.Id == product.ProductCategoryId)
                }).ToList();

            ViewBag.ConsignmentOrder = db.ConsignmentOrders
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ConsignmentCode,
                    Selected = (p.Id == product.ConsignmentOrderId)
                }).ToList();
            // Load Brand dropdown + "Other Brand"
            var brandList = db.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.BrandName,
                    Selected = (b.Id == product.BrandId)
                }).ToList();

            brandList.Add(new SelectListItem
            {
                Text = "Other Brand",
                Value = "0",
                Selected = (product.BrandId == null) // If "Other Brand", selected
            });

            ViewBag.BrandList = brandList;

            return View(product);
        }

        // Edit POST method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, string SelectedBrandId)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.Find(model.Id);
                if (existingProduct != null)
                {
                    existingProduct.Title = model.Title;
                    existingProduct.Description = model.Description;
                    existingProduct.Detail = model.Detail;
                    existingProduct.Price = model.Price;
                    existingProduct.Quantity = model.Quantity;
                    existingProduct.SeoTitle = string.IsNullOrEmpty(model.SeoTitle) ? model.Title : model.SeoTitle;
                    existingProduct.ModifiedDate = DateTime.Now;
                    existingProduct.ProductCategoryId = model.ProductCategoryId;
                    existingProduct.ConsignmentOrderId = model.ConsignmentOrderId;

                    int? brandId = null;
                    if (!string.IsNullOrEmpty(SelectedBrandId) && int.TryParse(SelectedBrandId, out int tempBrandId))
                    {
                        brandId = tempBrandId;
                    }

                    if (brandId == 0)
                    {
                        // If "Other Brand", set to null
                        existingProduct.BrandId = null;
                    }
                    else
                    {
                        existingProduct.BrandId = brandId;
                    }

                    db.Entry(existingProduct).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // Reload dropdown if model has errors
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title", model.ProductCategoryId);

            ViewBag.ConsignmentOrder = db.ConsignmentOrders
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ConsignmentCode,
                    Selected = (p.Id == model.ConsignmentOrderId)
                }).ToList();

            var brandList = db.Brands
                .Select(b => new SelectListItem
                {
                    Text = b.BrandName,
                    Value = b.Id.ToString()
                }).ToList();

            ViewBag.BrandList = brandList;


            return View(model);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHome = item.IsHome });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsSale(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsSale = item.IsSale });
            }
            return Json(new { success = false });
        }
    }
}
