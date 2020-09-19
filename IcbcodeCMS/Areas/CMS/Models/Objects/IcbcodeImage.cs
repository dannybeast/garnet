using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Collections.Generic;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeImage : IcbcodeBase
    {
        public long ID { get; private set; }
        public string Url { get; private set; }
        public string Description { get; private set; }

        public static IcbcodeImage Convert(dynamic image, int index, int totals)
        {
            return new IcbcodeImage()
            {
                ID = image.image_id,
                Url = string.Format("/content/cms/files/{0}{1}", image.image_id, image.image_extension),
                Description = image.image_desc,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }

        public IcbcodeImage()
        {

        }

        public static IcbcodeCollection<IcbcodeImage> Find(string terms, string field = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            return Find(terms, null, field, page, size, query, where);
        }

        public static IcbcodeCollection<IcbcodeImage> Find(string terms, string[] block_name, string field = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeImage> items = new IcbcodeCollection<IcbcodeImage>(); long totals;

            using (ImageRepository image_repository = new ImageRepository())
            {
                List<dynamic> images = image_repository.Search(terms, block_name, string.IsNullOrWhiteSpace(field) ? string.Empty : field, -1, page, size, out totals, query, where);

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
    }
}