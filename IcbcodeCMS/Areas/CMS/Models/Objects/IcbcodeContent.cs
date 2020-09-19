using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeContent : IcbcodeBase
    {
        private static string _Domain;
        private IcbcodeCollection<IcbcodeContent> _Parents;

        public IcbcodeCollection<IcbcodeContent> Parents
        {
            get
            {
                if (_Parents == null)
                {
                    _Parents = new IcbcodeCollection<IcbcodeContent>();

                    List<dynamic> contents = ModelUtility.GetNavi(ID, null, true);

                    for (int index = 0; index < contents.Count; index++)
                    {
                        _Parents.Add(IcbcodeContent.Convert(contents[index], index + 1, contents.Count));
                    }
                }

                return _Parents;
            }
        }

        public IcbcodeCollection<IcbcodeField> GetFields()
        {
            return IcbcodeField.Get(ID);
        }

        public IcbcodeContent Parent()
        {
            return Parents.Count > 1 ? Parents[Parents.Count - 2] : null;
        }

        public bool ExistsInParents(IcbcodeContent item)
        {
            return Parents.Exists(x => x.Url == item.Url);
        }

        public IcbcodeCollection<IcbcodeContent> Find(string terms, string[] template_name = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            return Find(terms, new[] { ID }, null, template_name, page, size, query, where, domain);
        }

        public static IcbcodeCollection<IcbcodeContent> Find(string terms, string[] block_name = null, string[] template_name = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            return Find(terms, null, block_name, template_name, page, size, query, where, domain);
        }

        public static IcbcodeCollection<IcbcodeContent> Find(string terms, long[] content_root = null, string[] template_name = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            return Find(terms, content_root, null, template_name, page, size, query, where, domain);
        }

        private static IcbcodeCollection<IcbcodeContent> Find(string terms, long[] content_root, string[] block_name, string[] template_name, long page, long size, string query, dynamic where, string domain)
        {
            IcbcodeCollection<IcbcodeContent> items = new IcbcodeCollection<IcbcodeContent>(); long totals = 1;

            using (ContentRepository content_repository = new ContentRepository())
            {
                List<dynamic> contents = content_repository.SearchActive(terms, content_root, block_name, template_name, page, size, out totals, query, where, domain);

                items.TotalPages = totals;
                items.CurrentPage = page;
                items.PageSize = size;

                for (int index = 0; index < contents.Count; index++)
                {
                    items.Add(IcbcodeContent.Convert(contents[index], index + 1, contents.Count));
                }
            }

            return items;
        }

        public static IcbcodeCollection<IcbcodeContent> Get(long[] content_root, string[] template_name = null, string order_fields = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            return Get(content_root, null, template_name, order_fields, page, size, query, where, domain);
        }

        public static IcbcodeCollection<IcbcodeContent> Get(string[] block_name, string[] template_name = null, string order_fields = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            return Get(null, block_name, template_name, order_fields, page, size, query, where, domain);
        }

        private static IcbcodeCollection<IcbcodeContent> Get(long[] content_root, string[] block_name, string[] template_name, string order_fields, long page, long size, string query, dynamic where, string domain)
        {
            IcbcodeCollection<IcbcodeContent> items = new IcbcodeCollection<IcbcodeContent>(); long totals;

            using (ContentRepository content_repository = new ContentRepository())
            {
                List<dynamic> contents = content_repository.GetActive(block_name, content_root, template_name, order_fields, page, size, out totals, query, where, domain);

                items.TotalPages = totals;
                items.CurrentPage = page;
                items.PageSize = size;

                for (int index = 0; index < contents.Count; index++)
                {
                    items.Add(IcbcodeContent.Convert(contents[index], index + 1, contents.Count));
                }
            }

            return items;
        }

        public IcbcodeCollection<IcbcodeContent> Childs(string[] template_name = null, string order_fields = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null, string domain = null)
        {
            IcbcodeCollection<IcbcodeContent> items = new IcbcodeCollection<IcbcodeContent>(); long totals;

            using (ContentRepository content_repository = new ContentRepository())
            {
                List<dynamic> contents = content_repository.GetActive(null, new long[] { ID }, template_name, order_fields, page, size, out totals, query, where, domain);

                items.TotalPages = totals;
                items.CurrentPage = page;
                items.PageSize = size;

                for (int index = 0; index < contents.Count; index++)
                {
                    items.Add(IcbcodeContent.Convert(contents[index], index + 1, contents.Count));
                }
            }

            return items;

        }

        public static IcbcodeContent Get(string content_url, string domain)
        {
            IcbcodeContent item = null;

            using (ContentRepository content_repository = new ContentRepository())
            {
                item = IcbcodeContent.Convert(content_repository.GetActiveByUrl(content_url.TrimEnd('/'), domain ?? _Domain), 1, 1);
            }

            return item;
        }

        public static IcbcodeContent Get(long content_id)
        {
            IcbcodeContent item = null;

            using (ContentRepository content_repository = new ContentRepository())
            {
                item = IcbcodeContent.Convert(content_repository.GetActiveByID(content_id), 1, 1);
            }

            return item;
        }

        public IcbcodeImage GetImage(string[] fields = null, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeImage> items = GetImages(fields, 1, 1, query, where);

            return items.Count == 0 ? null : items.First<IcbcodeImage>();
        }

        public IcbcodeCollection<IcbcodeImage> GetImages(string[] fields = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeImage> items = new IcbcodeCollection<IcbcodeImage>(); long totals;

            using (ImageRepository image_repository = new ImageRepository())
            {
                List<dynamic> images = image_repository.Get(ID, fields, page, size, out totals, query, where);

                items.TotalPages = totals;
                items.CurrentPage = page;
                items.PageSize = size;

                for (int index = 0; index < images.Count; index++)
                {
                    items.Add(IcbcodeImage.Convert(images[index], index + 1, images.Count));
                }
            }

            return items;
        }

        public IcbcodeFile GetFile(string[] fields = null, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeFile> items = GetFiles(fields, 1, 1, query, where);

            return items.Count == 0 ? null : items.First<IcbcodeFile>();
        }

        public IcbcodeCollection<IcbcodeFile> GetFiles(string[] fields = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeFile> items = new IcbcodeCollection<IcbcodeFile>(); long totals;

            using (FileRepository file_repository = new FileRepository())
            {
                List<dynamic> files = file_repository.Get(ID, fields, page, size, out totals, query, where);

                items.TotalPages = totals;
                items.CurrentPage = page;
                items.PageSize = size;

                for (int index = 0; index < files.Count; index++)
                {
                    items.Add(IcbcodeFile.Convert(files[index], index + 1, files.Count));
                }
            }

            return items;
        }

        public long ID { get; private set; }
        public string Url { get; private set; }
        public string Link { get; private set; }
        public string Name { get; private set; }
        public DateTime? Publish { get; private set; }
        public long? Root { get; private set; }
        public bool IsMain { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Keywords { get; private set; }
        public string H1 { get; private set; }
        public string H2 { get; private set; }
        public string Text { get; private set; }
        public string ShortText { get; private set; }
        public bool IsRedirect { get; private set; }
        public string Template { get; private set; }

        public dynamic UserDefined { get; private set; }

        public string BlockSortBy { get; private set; }

        public static IcbcodeContent Convert(dynamic content, int index, int totals)
        {
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                HttpRequest request = context.Request;

                if (request != null)
                {
                    _Domain = request.Url.Host;
                }
            }

            return content == null ? null : new IcbcodeContent()
            {
                ID = content.content_id,
                Url = content.content_url,
                Link = content.content_link,
                Name = content.content_name,
                Publish = content.content_publish,
                Root = content.content_root,
                IsMain = content.content_main,
                Title = content.content_title,
                Description = content.content_description,
                Keywords = content.content_keywords,
                H1 = content.content_h1,
                H2 = content.content_h2,
                Text = content.content_text,
                ShortText = content.content_short_text,
                IsRedirect = content.content_allow_redirect,
                Template = content.template_name,
                UserDefined = content,
                BlockSortBy = content.block_allow_sort || string.IsNullOrWhiteSpace(content.block_order_fields) ? "content_order" : content.block_order_fields,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }
    }
}