using IcbcodeCMS.Areas.CMS.Models.Objects;
using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    public class CartController : Controller
    {
        [HttpPost()]
        public ActionResult Doooo()
        {
            

            return new EmptyResult();
        }

        public decimal GetDiscountInRub(string code)
        {
            var promocode = IcbcodeContent.Get(new string[] { "promokodi" }, new string[] { "promokod" }, query: $"content_name = '{code}'").FirstOrDefault();

            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            decimal result = 0;

            if (promocode != null && shopping_cart.GetTotal() >= Convert.ToDecimal(promocode.UserDefined.minimaljnaya_summa_zakaza_rub) && shopping_cart.GetCount() >= Convert.ToInt32(promocode.UserDefined.minimaljnoe_ko_vo_tovarov_v_zakaze_sht) && (promocode.UserDefined.dejstvitelen_do == null || (promocode.UserDefined.dejstvitelen_do != null && Convert.ToDateTime(promocode.UserDefined.dejstvitelen_do) >= DateTime.Now)))
            {
                foreach (var item in shopping_cart.Items)
                {
                    var product = IcbcodeContent.Get(item.item_id);

                    if (product.Url.Contains((string)promocode.UserDefined.url_razdela))
                    {
                        int discount = 0; var promo_discount = Convert.ToInt32(product.UserDefined.skidka);

                        if (product.UserDefined.staraya_cena_rub != 0 && product.UserDefined.staraya_cena_rub != product.UserDefined.cena_rub)
                        {
                            decimal z = (decimal)product.UserDefined.staraya_cena_rub - (decimal)product.UserDefined.cena_rub;

                            decimal x = (decimal)product.UserDefined.staraya_cena_rub / (decimal)100;

                            discount = Convert.ToInt32((Math.Floor(z / x)));
                        }

                        if (discount < promo_discount)
                        {
                            result += Math.Floor((item.item_price / 100 * promo_discount)) * item.item_count;
                        }
                    }
                }
            }

            return result;
        }

        private decimal GetDeliveryPrice(string deliveryMethod)
        {
            switch (deliveryMethod)
            {
                case "IML":
                    return 270;

                case "Почта России":
                    return 340;

                default:
                    return 270;
            }
        }

        [HttpPost()]
        public ActionResult Checkout(string name, string phone, string email, string address, string delivery_type, string region, string pd, string payment_type)
        {
            if (delivery_type == "Почта России")
            {
                region = string.Empty; pd = string.Empty;
            }

            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            var edinici_izmereniya = IcbcodeContent.Get(new long[] { 33455 });

            var proizvoditeli = IcbcodeContent.Get(new long[] { 33454 });

            Dictionary<string, string> data = null;

            if (shopping_cart.Items.Count > 0)
            {
                StringBuilder body = new StringBuilder();

                body.AppendFormat("Имя: {0}", name);
                body.AppendLine("<br/>");
                body.AppendFormat("Телефон: {0}", phone);
                body.AppendLine("<br/>");
                body.AppendFormat("Email: {0}", email);
                body.AppendLine("<br/>");
                body.AppendFormat("Способ доставки: {0}", delivery_type);
                body.AppendLine("<br/>");
                body.AppendFormat("Город / населенный пункт (IML): {0}", region);
                body.AppendLine("<br/>");
                body.AppendFormat("Код ПД (IML): {0}", pd);
                body.AppendLine("<br/>");
                body.AppendFormat("Адрес доставки: {0}", address);
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");
                body.AppendLine("<table style=\"border: 1px solid black;\" cellpadding=\"5\" cellspacing=\"5\">");
                body.AppendLine("<tr><td>Наименование</td><td>Производитель</td><td>Арт.</td><td>Цена</td><td>Ко-во</td><td>Итого</td></tr>");

                decimal price = 0;

                foreach (var item in shopping_cart.Items)
                {
                    price += item.item_price * item.item_count;

                    var product = IcbcodeContent.Get(item.item_id);

                    var unit = edinici_izmereniya.Find(x => x.ID == product.UserDefined.edinica_izmereniya).Name;

                    var manufacturer = proizvoditeli.Find(x => x.ID == product.UserDefined.proizvoditelj).Name;

                    body.AppendLine(string.Format("<tr><td><a href=\"{0}\">{1}</a><td>{6}</td><td>{5}</td></td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", string.Format("{0}/{1}", Request.Url.Host, item.item_url), item.item_name, item.item_price, item.item_count, item.item_price * item.item_count, product.UserDefined.artikul, manufacturer));
                }

                decimal delivery_price = GetDeliveryPrice(delivery_type); price += delivery_price;

                body.AppendLine($"<tr><td>Доставка: {delivery_type}<td></td><td></td></td><td>{delivery_price}</td><td>1</td><td>{delivery_price}</td></tr>");


                body.AppendLine(string.Format("<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>{0}</td></tr>", price));
                body.AppendLine("</table>");

                long order_id;

                using (ContentRepository content = new ContentRepository())
                {
                    order_id = content.CreateGlobalID(); string order_url = "/" + order_id.ToString();

                    content.Create(order_id, null, string.Format("Заказ #{0}", order_id), 33542, order_url, 33543, null, string.Format("fio = '{0}', adres_dostavki = '{1}', telefon = '{2}', email = '{3}', sposob_oplati = '{4}', oplachen = false, summa_rub = {5}, content_publish = now(), sposob_dostavki = '{6}', gorod_naselennij_punkt_iml = '{7}', kod_pd_iml = '{8}'", name, address, phone, email, payment_type, price.ToString("0.00", CultureInfo.InvariantCulture), delivery_type, region, pd), 15984);

                    foreach (var item in shopping_cart.Items)
                    {
                        var product = IcbcodeContent.Get(item.item_id);

                        var position_id = content.CreateGlobalID();

                        content.Create(position_id, null, item.item_name, 33535, order_url + "/" + position_id.ToString(), 33543, order_id, string.Format("cena_rub = {0}, kolichestvo = {1}, artikul = '{2}', edinica_izmereniya = {3}, proizvoditelj = {4}", item.item_price.ToString("0.00", CultureInfo.InvariantCulture), item.item_count, product.UserDefined.artikul, product.UserDefined.edinica_izmereniya, product.UserDefined.proizvoditelj), 15984);
                    }
                }

                data = new Dictionary<string, string>()
                {
                    {"amount", price.ToString("0.00", CultureInfo.InvariantCulture)},
                    {"description", $"Заказ №{order_id}"},
                    {"merchant", "cfde6441-4fb3-46f5-b085-48beacb1beaf"},
                    {"order_id", order_id.ToString()},
                    {"testing", "0"},
                    {"unix_timestamp", ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString()},
                    { "callback_url", "https://lemil.ru/utility/check-payment" },
                    { "cancel_url", "https://lemil.ru/k-sozhaleniyu-vi-otmenili-oplatu"},
                    { "success_url", $"https://lemil.ru/spasibo-vash-zakaz-oplachen"},
                    { "fail_url", "https://lemil.ru/platezh-zavershilsya-s-oshibkoj" }
                };

                data.Add("signature", IcbcodeCMS.Areas.CMS.Utilities.SignatureHelper.GetSignature("CCF34CBC5A8A479A1DF6FF86AC6B2C43", data));

                IcbcodeUtility.SendEmail(null, null, string.Format($"Заказ #{order_id} с сайта lemil.ru"), body.ToString());

                shopping_cart.EmptyCart();
            }

            return View("Pay", data);
        }



        [HttpPost()]
        public ActionResult CheckPayment()
        {
            var data = IcbcodeUtility.ToDictionary(Request.Form);

            var signature = IcbcodeCMS.Areas.CMS.Utilities.SignatureHelper.GetSignature("CCF34CBC5A8A479A1DF6FF86AC6B2C43", data);

            if (signature == data["signature"])
            {
                long orderId = Int64.Parse(data["order_id"]);

                using (ContentRepository content = new ContentRepository())
                {
                    content.Update("oplachen = true", $"content_id = {orderId}");
                }

                var order = IcbcodeContent.Get(orderId);

                var productItems = new List<ProductItem>();

                foreach (var item in order.Childs())
                {
                    productItems.Add(new ProductItem() { name = item.Name, price = (decimal)item.UserDefined.cena_rub, quantity = (int)item.UserDefined.kolichestvo, VATrate = 6 });
                }

                EOFD eofd = new EOFD();

                eofd.SendCheck(order.UserDefined.email, productItems);

                IcbcodeUtility.SendEmail(null, null, $"Поступил платеж по заказу №{orderId}", "");
            }

            return Content("");
        }

        public ActionResult SetPromocode(string promocode)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            shopping_cart.SetPromocode(promocode);

            return new EmptyResult();
        }

        public ActionResult SetDeliveryMethod(string deliveryMethod)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            shopping_cart.SetDeliveryMethod(deliveryMethod);

            return new EmptyResult();
        }

        public ActionResult SetItemInCart(long product_id, int product_count)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            var item = shopping_cart.Items.Find(x => x.item_id == product_id);

            item.item_count = product_count;

            return new EmptyResult();
        }

        public ActionResult MinusFromCart(long product_id)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            shopping_cart.RemoveFromCart(product_id, 1);

            return new EmptyResult();
        }

        public ActionResult RemoveFromCart(long product_id)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            var item = shopping_cart.Items.Find(x => x.item_id == product_id);

            shopping_cart.RemoveFromCart(product_id, item.item_count);

            return new EmptyResult();
        }

        public ActionResult AddToCart(long product_id, int? product_count)
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            var product = IcbcodeContent.Get(product_id);

            shopping_cart.AddToCart(product_id, product.Link, null, product.Url, product.UserDefined.cena_rub, null, 0);

            var image = product.GetImage();

            var item = new { product_index = shopping_cart.Items.Count, product_id = product_id, product_name = product.Link, product_url = product.Url, product_count = product_count ?? 1, product_price = ((decimal)product.UserDefined.cena_rub).ToString("N0"), product_image = (image == null ? null : image.Url) };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCartResult()
        {
            IcbcodeCart shopping_cart = new IcbcodeCart(HttpContext);

            decimal discount = GetDiscountInRub(shopping_cart.Promocode);

            var item = new { delivery_price = GetDeliveryPrice(shopping_cart.DeliveryMethod), product_count = shopping_cart.GetCount(), product_amount = (shopping_cart.GetTotal() - discount).ToString("N0"), discount = discount };

            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}