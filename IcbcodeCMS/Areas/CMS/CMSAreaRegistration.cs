using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS
{
    public class CMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.IgnoreRoute("content/{*queryvalues}");

            // file result

            context.MapRoute(
                "",
                "utility/send-email",
                new { controller = "Cart", action = "SendEmail" }
            );

            // cart

            context.MapRoute(
                "",
                "utility/check-payment",
                new { controller = "Cart", action = "CheckPayment" }
            );

            context.MapRoute(
                "",
                "utility/checkout",
                new { controller = "Cart", action = "Checkout" }
            );

            context.MapRoute(
                "",
                "utility/set-item-in-cart",
                new { controller = "Cart", action = "SetItemInCart" }
            );

            context.MapRoute(
                "",
                "utility/minus-from-cart",
                new { controller = "Cart", action = "MinusFromCart" }
            );

            context.MapRoute(
                "",
                "utility/get-cart-result",
                new { controller = "Cart", action = "GetCartResult" }
            );

            context.MapRoute(
                "",
                "utility/add-to-cart",
                new { controller = "Cart", action = "AddToCart" }
            );

            context.MapRoute(
                "",
                "utility/remove-from-cart",
                new { controller = "Cart", action = "RemoveFromCart" }
            );

            context.MapRoute(
                "",
                "utility/set-promocode",
                new { controller = "Cart", action = "SetPromocode" }
            );

            context.MapRoute(
                "",
                "utility/set-delivery-method",
                new { controller = "Cart", action = "SetDeliveryMethod" }
            );

            // snapshot

            context.MapRoute(
                "",
                "cms",
                new { controller = "Snapshot", action = "Index" }
            );

            // authorize

            context.MapRoute(
                "",
                "cms/account/sign-in",
                new { controller = "Account", action = "SignIn" }
            );

            context.MapRoute(
                "",
                "cms/account/sign-out",
                new { controller = "Account", action = "SignOut" }
            );

            // users

            context.MapRoute(
                "",
                "cms/users",
                new { controller = "User", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/users/edit",
                new { controller = "User", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/users/remove",
                new { controller = "User", action = "Remove" }
            );

            // views

            context.MapRoute(
                "",
                "cms/views",
                new { controller = "View", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/views/edit",
                new { controller = "View", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/views/remove",
                new { controller = "View", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/views/update-order",
                new { controller = "View", action = "UpdateOrder" }
            );

            // templates

            context.MapRoute(
                "",
                "cms/templates",
                new { controller = "Template", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/templates/edit",
                new { controller = "Template", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/templates/remove",
                new { controller = "Template", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/templates/update-order",
                new { controller = "Template", action = "UpdateOrder" }
            );

            // variables

            context.MapRoute(
                "",
                "cms/vars",
                new { controller = "Variable", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/vars/edit",
                new { controller = "Variable", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/vars/remove",
                new { controller = "Variable", action = "Remove" }
            );

            // tags

            context.MapRoute(
                "",
                "cms/tags",
                new { controller = "Tag", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/tags/edit",
                new { controller = "Tag", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/tags/remove",
                new { controller = "Tag", action = "Remove" }
            );

            // blocks

            context.MapRoute(
                "",
                "cms/blocks",
                new { controller = "Block", action = "Index" }
            );

            context.MapRoute(
               "",
               "cms/blocks/update-order",
               new { controller = "Block", action = "UpdateOrder" }
           );

            context.MapRoute(
                "",
                "cms/blocks/edit",
                new { controller = "Block", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/blocks/remove",
                new { controller = "Block", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/blocks/add-separate",
                new { controller = "Block", action = "AddSeparate" }
            );

            // domains

            context.MapRoute(
                "",
                "cms/domains",
                new { controller = "Domain", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/domains/edit",
                new { controller = "Domain", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/domains/remove",
                new { controller = "Domain", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/domains/clone",
                new { controller = "Domain", action = "Clone" }
            );

            // fields

            context.MapRoute(
                "",
                "cms/fields",
                new { controller = "Field", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/fields/edit",
                new { controller = "Field", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/fields/remove",
                new { controller = "Field", action = "Remove" }
            );

            // contents

            context.MapRoute(
                "",
                "cms/contents",
                new { controller = "Content", action = "Index" }
            );

            context.MapRoute(
                "",
                "cms/contents/import",
                new { controller = "Content", action = "Import" }
            );

            context.MapRoute(
                "",
                "cms/contents/edit",
                new { controller = "Content", action = "Edit" }
            );

            context.MapRoute(
                "",
                "cms/contents/remove",
                new { controller = "Content", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/contents/update-order",
                new { controller = "Content", action = "UpdateOrder" }
            );

            context.MapRoute(
                "",
                "cms/contents/paste",
                new { controller = "Content", action = "Paste" }
            );

            context.MapRoute(
                "",
                "cms/contents/copy",
                new { controller = "Content", action = "Copy" }
            );

            context.MapRoute(
                "",
                "cms/contents/get-rel-items",
                new { controller = "Content", action = "GetRelItems" }
            );

            // images

            context.MapRoute(
                "",
                "cms/images/upload",
                new { controller = "Image", action = "Upload" }
            );

            context.MapRoute(
                "",
                "cms/images/load",
                new { controller = "Image", action = "Load" }
            );

            context.MapRoute(
                "",
                "cms/images/remove",
                new { controller = "Image", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/images/update-order",
                new { controller = "Image", action = "UpdateOrder" }
            );

            context.MapRoute(
                "",
                "cms/images/update-mode",
                new { controller = "Image", action = "UpdateMode" }
            );

            // files

            context.MapRoute(
                "",
                "cms/files/upload",
                new { controller = "File", action = "Upload" }
            );

            context.MapRoute(
                "",
                "cms/files/load",
                new { controller = "File", action = "Load" }
            );

            context.MapRoute(
                "",
                "cms/files/remove",
                new { controller = "File", action = "Remove" }
            );

            context.MapRoute(
                "",
                "cms/files/update-order",
                new { controller = "File", action = "UpdateOrder" }
            );

            // sitemap

            context.MapRoute(
                "",
                "sitemap.xml",
                new { controller = "Generator", action = "SiteMapXml" }
            );

            // general

            context.MapRoute(
                "",
                "{*queryvalues}",
                new { controller = "Main", action = "Index" }
            );
        }
    }
}