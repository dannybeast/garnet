using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    public class GeneratorController : BaseController
    {
        [HttpGet()]
        public ActionResult SiteMapXml()
        {
            string domain = Request.Url.Host;

            Sitemap sitemap = new Sitemap();

            using (ContentRepository content_repository = new ContentRepository())
            {
                long totals;

                foreach (var item in content_repository.GetActive((string[])null, null, null, null, 1, Int64.MaxValue, out totals, "content_in_sitemap = true", null, domain))
                {
                    sitemap.Add(new Location() { Url = string.Format("http://{0}{1}", Request.Url.Host, item.content_url) });
                }
            }

            XmlSerializer xs = new XmlSerializer(typeof(Sitemap));

            MemoryStream ms = new MemoryStream();

            XmlTextWriter xw = new XmlTextWriter(ms, Encoding.UTF8);

            xs.Serialize(xw, sitemap);

            ms.Position = 0;

            return File(ms, "text/xml");
        }

        #region sitemap definition

        [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
        public class Sitemap
        {
            private ArrayList map;

            public Sitemap()
            {
                map = new ArrayList();
            }

            [XmlElement("url")]
            public Location[] Locations
            {
                get
                {
                    Location[] items = new Location[map.Count];
                    map.CopyTo(items);
                    return items;
                }
                set
                {
                    if (value == null)
                        return;
                    Location[] items = (Location[])value;
                    map.Clear();
                    foreach (Location item in items)
                        map.Add(item);
                }
            }

            public int Add(Location item)
            {
                return map.Add(item);
            }
        }

        // Items in the shopping list
        public class Location
        {
            public enum eChangeFrequency
            {
                always,
                hourly,
                daily,
                weekly,
                monthly,
                yearly,
                never
            }

            [XmlElement("loc")]
            public string Url { get; set; }

            [XmlElement("changefreq")]
            public eChangeFrequency? ChangeFrequency { get; set; }
            public bool ShouldSerializeChangeFrequency() { return ChangeFrequency.HasValue; }

            [XmlElement("lastmod")]
            public DateTime? LastModified { get; set; }
            public bool ShouldSerializeLastModified() { return LastModified.HasValue; }

            [XmlElement("priority")]
            public double? Priority { get; set; }
            public bool ShouldSerializePriority() { return Priority.HasValue; }
        }

        #endregion

    }
}