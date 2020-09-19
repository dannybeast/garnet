using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeYml
    {
        public static Yml_catalog GetCatalog(string url)
        {
            Yml_catalog result;

            XmlSerializer ser = new XmlSerializer(typeof(Yml_catalog));

            using (WebClient client = new WebClient())
            {
                Stream stream = new MemoryStream(client.DownloadData(url));

                result = (Yml_catalog)ser.Deserialize(stream);
            }

            return result;
        }

        public static string Do()
        {
            var catalog = GetCatalog("http://mikkimama.ru/marketplace/44666.xml");

            var partitions = IcbcodeContent.Get(new string[] { "katalog" }, new string[] { "razdel-kataloga" });

            var products = IcbcodeContent.Get(new string[] { "katalog" }, new string[] { "poziciya" });

            var oldProducts = IcbcodeContent.Get(new string[] { "katalog" }, new string[] { "poziciya" });

            List<string> newPartitions = new List<string>(); List<string> newProducts = new List<string>(); List<string> noNames = new List<string>();

            using (WebClient web = new WebClient())
            {
                using (ContentRepository content = new ContentRepository())
                using (ImageRepository image = new ImageRepository())
                {
                    foreach (var offer in catalog.Shop.Offers.Offer)
                    {
                        var partition = partitions.Find(x => x.UserDefined.vneshnij_id != null && x.UserDefined.vneshnij_id.ToString() == offer.CategoryId);

                        if (partition == null)
                        {
                            newPartitions.Add(offer.CategoryId);
                        }
                        else
                        {
                            oldProducts.RemoveAll(x => (long)x.UserDefined.vneshnij_id == Convert.ToInt64(offer.Id));

                            var product = products.Find(x => x.UserDefined.vneshnij_id != null && (long)x.UserDefined.vneshnij_id == Convert.ToInt64(offer.Id));

                            if (product == null)
                            {
                                var product_id = content.CreateGlobalID();

                                if (string.IsNullOrWhiteSpace(offer.Name))
                                {
                                    offer.Name = offer.Url;

                                    noNames.Add($"{partition.Url}/{product_id}");
                                }

                                content.Create(product_id, 16512, offer.Name, 16515, $"{partition.Url}/{product_id}", 33436, partition.ID, $"proizvoditelj = 33457, edinica_izmereniya = 33456, cena_rub = {offer.Price}, artikul = '{offer.VendorCode}', vneshnij_id = {offer.Id}", 15984);

                                newProducts.Add($"{partition.Url}/{product_id}");

                                foreach (var pic in offer.Picture)
                                {
                                    var image_id = image.CreateGlobalID();

                                    string extension = Path.GetExtension(pic);

                                    string savePath = Path.Combine(HostingEnvironment.MapPath("~/content/cms/files"), image_id.ToString() + extension);

                                    web.DownloadFile(pic, savePath);

                                    image.Save(image_id, extension, product_id.ToString(), null, "", 100000, product_id);
                                }
                            }
                            else
                            {
                                // обновляем только цену, удаляем и загружаем заново картинки
                                content.Update($"cena_rub = {offer.Price}, content_active = true", $"content_id = {product.ID}");

                                image.RemoveByRef(product.ID);

                                foreach (var pic in offer.Picture)
                                {
                                    var image_id = image.CreateGlobalID();

                                    string extension = Path.GetExtension(pic);

                                    string savePath = Path.Combine(HostingEnvironment.MapPath("~/content/cms/files"), image_id.ToString() + extension);

                                    web.DownloadFile(pic, savePath);

                                    image.Save(image_id, extension, product.ID.ToString(), null, "", 100000, product.ID);
                                }
                            }
                        }
                    }

                    foreach (var oldProduct in oldProducts)
                    {
                        content.Update($"artikul = '', content_active = false", $"vneshnij_id = '{oldProduct.UserDefined.vneshnij_id}' and content_template = 16515");
                    }
                }
            }

            StringBuilder result = new StringBuilder();

            if (newPartitions.Count > 0)
            {
                result.Append("Новые разделы: <br/>");
                foreach (string item in newPartitions)
                {
                    result.Append($"{item}<br/>");
                }
                result.Append("<br/><br/><br/>");
            }

            if (newProducts.Count > 0)
            {
                result.Append("Новые товары: <br/>");
                foreach (string item in newProducts)
                {
                    result.Append($"{item}<br/>");
                }
                result.Append("<br/><br/><br/>");
            }

            if (noNames.Count > 0)
            {
                result.Append("Товары без имени: <br/>");
                foreach (string item in noNames)
                {
                    result.Append($"{item}<br/>");
                }
                result.Append("<br/><br/><br/>");
            }

            if (oldProducts.Count > 0)
            {
                result.Append("Удаленные товары: <br/>");
                foreach (var item in oldProducts)
                {
                    result.Append($"{item.Url}<br/>");
                }
                result.Append("<br/><br/><br/>");
            }

            return result.ToString();
        }
    }

    [XmlRoot(ElementName = "currency")]
    public class Currency
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "rate")]
        public string Rate { get; set; }
    }

    [XmlRoot(ElementName = "currencies")]
    public class Currencies
    {
        [XmlElement(ElementName = "currency")]
        public Currency Currency { get; set; }
    }

    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "parentId")]
        public string ParentId { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "categories")]
    public class Categories
    {
        [XmlElement(ElementName = "category")]
        public List<Category> Category { get; set; }
    }

    [XmlRoot(ElementName = "option")]
    public class Option
    {
        [XmlAttribute(AttributeName = "cost")]
        public string Cost { get; set; }
        [XmlAttribute(AttributeName = "days")]
        public string Days { get; set; }
        [XmlAttribute(AttributeName = "order-before")]
        public string Orderbefore { get; set; }
    }

    [XmlRoot(ElementName = "delivery-options")]
    public class Deliveryoptions
    {
        [XmlElement(ElementName = "option")]
        public Option Option { get; set; }
    }

    [XmlRoot(ElementName = "offer")]
    public class Offer
    {
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "price")]
        public string Price { get; set; }
        [XmlElement(ElementName = "currencyId")]
        public string CurrencyId { get; set; }
        [XmlElement(ElementName = "categoryId")]
        public string CategoryId { get; set; }
        [XmlElement(ElementName = "picture")]
        public List<string> Picture { get; set; }
        [XmlElement(ElementName = "model")]
        public string Name { get; set; }
        [XmlElement(ElementName = "vendor")]
        public string Vendor { get; set; }
        [XmlElement(ElementName = "vendorCode")]
        public string VendorCode { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "available")]
        public string Available { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "offers")]
    public class Offers
    {
        [XmlElement(ElementName = "offer")]
        public List<Offer> Offer { get; set; }
    }

    [XmlRoot(ElementName = "shop")]
    public class Shop
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "company")]
        public string Company { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "platform")]
        public string Platform { get; set; }
        [XmlElement(ElementName = "currencies")]
        public Currencies Currencies { get; set; }
        [XmlElement(ElementName = "categories")]
        public Categories Categories { get; set; }
        [XmlElement(ElementName = "delivery-options")]
        public Deliveryoptions Deliveryoptions { get; set; }
        [XmlElement(ElementName = "offers")]
        public Offers Offers { get; set; }
    }

    [XmlRoot(ElementName = "yml_catalog")]
    public class Yml_catalog
    {
        [XmlElement(ElementName = "shop")]
        public Shop Shop { get; set; }
        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }
    }
}