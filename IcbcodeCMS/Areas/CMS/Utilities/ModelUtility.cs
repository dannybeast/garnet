using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace IcbcodeCMS.Areas.CMS.Utilities
{
    public class ModelUtility
    {
        public static long[] GetLongArray(string source)
        {
            List<long> items = new List<long>();

            StringBuilder new_source = new StringBuilder();

            foreach (char chr in source)
            {
                new_source.Append(char.IsDigit(chr) ? chr : ',');
            }

            foreach (var item in new_source.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                items.Add(Int64.Parse(item));
            }

            return items.ToArray();
        }

        public static string[] GetStringArray(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            List<string> items = new List<string>();

            foreach (string line in source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                items.Add(line.Trim());
            }

            return items.ToArray();
        }

        public static void RemoveImage(long image_id)
        {
            using (ImageRepository image_repository = new ImageRepository())
            {
                var image = image_repository.Get(image_id);

                if (image != null)
                {
                    image_repository.Remove(image_id);

                    try
                    {
                        string deletedFileName = Path.Combine(HttpContext.Current.Server.MapPath("~/content/cms/files"), Path.Combine(string.Format("{0}{1}", image.image_id, image.image_extension)));

                        File.Delete(deletedFileName);
                    }
                    catch { }
                }
            }
        }

        public static void RemoveFile(long file_id)
        {
            using (FileRepository file_repository = new FileRepository())
            {
                var file = file_repository.Get(file_id);

                if (file != null)
                {
                    file_repository.Remove(file_id);

                    try
                    {
                        string deletedFileName = Path.Combine(HttpContext.Current.Server.MapPath("~/content/cms/files"), Path.Combine(string.Format("{0}{1}", file.file_id, file.file_extension)));

                        File.Delete(deletedFileName);
                    }
                    catch { }
                }
            }
        }

        public static void RemoveContent(long content_id)
        {
            using (ContentRepository content_repository = new ContentRepository())
            {
                using (ImageRepository image_repository = new ImageRepository())
                {
                    using (FileRepository file_repository = new FileRepository())
                    {
                        var contents = content_repository.GetByParent(content_id);

                        foreach (var item in contents)
                        {
                            RemoveContent(item.content_id);
                        }

                        foreach (var image in image_repository.Get(content_id.ToString()))
                        {
                            ModelUtility.RemoveImage(image.image_id);
                        }

                        foreach (var file in file_repository.Get(content_id.ToString()))
                        {
                            ModelUtility.RemoveFile(file.file_id);
                        }

                        content_repository.Remove(content_id);

                        image_repository.RemoveByRef(content_id);

                        file_repository.RemoveByRef(content_id);
                    }
                }
            }
        }

        public static List<dynamic> GetNavi(long? current_content_id, long? block_content_root, bool only_content_active)
        {
            List<dynamic> nav = new List<dynamic>();

            using (ContentRepository content_repository = new ContentRepository())
            {
                dynamic content = null;

                if (current_content_id.HasValue)
                {
                    if (only_content_active)
                        content = content_repository.GetActiveByID(current_content_id.Value);
                    else
                        content = content_repository.GetByID(current_content_id.Value);
                }

                if (content != null)
                {
                    if (block_content_root != content.content_id) // if (block_content_root != content.content_id)
                    {
                        nav.AddRange(GetNavi(content.content_root, block_content_root, only_content_active));
                    }

                    nav.Add(content);
                }
            }

            return nav;
        }
    }
}