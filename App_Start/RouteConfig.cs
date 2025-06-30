using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ConsignmentWebsite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Detail",
                url: "detail/{title}/{id}",
                defaults: new { controller = "Products", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "CheckOut",
                url: "payment",
                defaults: new { controller = "ShoppingCart", action = "CheckOut", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "Contact",
                url: "contact",
                defaults: new { controller = "Contact", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "Homepage",
                url: "homepage",
                defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "ShoppingCart",
                url: "shoppingcart",
                defaults: new { controller = "ShoppingCart", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "ProductCategory",
                url: "product-category/{title}/{id}",
                defaults: new { controller = "Products", action = "ProductCategory", id = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "vnpay_return",
                url: "vnpay_return",
                defaults: new { controller = "ShoppingCart", action = "VnpayReturn", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "NewsList",
                url: "news",
                defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "DetailNew",
                url: "news/{title}/{id}",
                defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "Promotion",
                url: "post/{title}",
                defaults: new { controller = "Article", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );
            routes.MapRoute(
                name: "DetailPost",
                url: "post/{title}/{id}",
                defaults: new { controller = "Posts", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "PostList",
                url: "post",
                defaults: new { controller = "Posts", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            // NÊN đặt Collection gần cuối vì url {collection} rất rộng dễ bị bắt nhầm
            routes.MapRoute(
                name: "Collection",
                url: "{collection}",
                defaults: new { controller = "Products", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ConsignmentWebsite.Controllers" }
            );
        }
    }
}
