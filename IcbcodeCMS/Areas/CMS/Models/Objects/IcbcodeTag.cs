using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System.Collections.Generic;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeTag : IcbcodeBase
    {
        public static IcbcodeCollection<IcbcodeTag> Get(string field_name, long? domain = null)
        {
            IcbcodeCollection<IcbcodeTag> items = new IcbcodeCollection<IcbcodeTag>();

            using (TagRepository tag_repository = new TagRepository())
            {
                List<dynamic> tags = tag_repository.Get(field_name, domain);

                items.TotalPages = items.Count;
                items.CurrentPage = 1;
                items.PageSize = 1;

                for (int index = 0; index < tags.Count; index++)
                {
                    items.Add(IcbcodeTag.Convert(tags[index], index + 1, tags.Count));
                }
            }

            return items;
        }

        public long ID { get; private set; }
        public string Name { get; private set; }

        public static IcbcodeTag Convert(dynamic tag, long index, long totals)
        {
            return new IcbcodeTag()
            {
                ID = tag.tag_id,
                Name = tag.tag_name,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }

        public IcbcodeTag()
        {

        }
    }
}