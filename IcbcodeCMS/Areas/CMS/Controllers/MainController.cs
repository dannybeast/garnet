using IcbcodeCMS.Areas.CMS.Models.Objects;
using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    public class MainController : BaseController
    {
        [HttpGet()]
        public ActionResult GetFileStream()
        {
            FileStream fs = new FileStream(HostingEnvironment.MapPath("~/content/cms/files/33533.pdf"), FileMode.Open, FileAccess.Read);

            return File(fs, "application/pdf");
        }

        [HandleError(ExceptionType = typeof(HttpException))]
        public ActionResult Index()
        {
            string domain = Request.Url.Host;

            string url = Request.Url.LocalPath;

            dynamic content = null;

            using (ContentRepository content_repository = new ContentRepository())
            {
                content = url == "/" ? content_repository.GetActiveMain(domain) : content_repository.GetActiveByUrl(url.TrimEnd('/'), domain);
            }

            if (content == null)
            {
                throw new HttpException(404, "Not found");
            }
            else
            {
                string redirect_url = string.IsNullOrWhiteSpace(content.content_redirect_url) ? Url.Content("~/") : content.content_redirect_url;

                string view_path = content.view_path;

                if (content.content_allow_redirect)
                {
                    return content.content_redirect_permanent ? RedirectPermanent(redirect_url) : Redirect(redirect_url);
                }
                else if (content.content_export_rss)
                {
                    var feed = new SyndicationFeed
                    {
                        Title = new TextSyndicationContent(content.content_export_rss_title),
                        Description = new TextSyndicationContent(content.content_export_rss_title),
                        BaseUri = new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, content.content_url)),
                        Id = Transliterator.Translite(content.content_export_rss_title, '_'),
                        LastUpdatedTime = DateTime.Now,
                        Items = new List<SyndicationItem>()
                    };

                    List<SyndicationItem> items = new List<SyndicationItem>();

                    using (ContentRepository content_repository = new ContentRepository())
                    {
                        long totals;

                        foreach (var item in content_repository.GetActive(null, ModelUtility.GetLongArray(content.content_export_rss_ids), null, "content_last_update desc", 1, 30, out totals, null, null, domain))
                        {
                            var syndItem = new SyndicationItem
                            {
                                Title = new TextSyndicationContent(item.content_link ?? item.content_h1),
                                Content = new TextSyndicationContent(IcbcodeUtility.GetPlainText(item.content_short_text)),
                                Id = item.content_id.ToString(),
                                LastUpdatedTime = item.content_publish ?? item.content_last_update
                            };

                            syndItem.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, item.content_url))));

                            items.Add(syndItem);
                        }
                    }

                    feed.Items = items;

                    return new RssActionResult() { Feed = feed };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(view_path))
                    {
                        throw new HttpException(404, "Not found");
                    }

                    return View(view_path, IcbcodeContent.Convert(content, 1, 1));
                }
            }
        }
    }

    public class RssActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            var rssFormatter = new Rss20FeedFormatter(Feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }
}