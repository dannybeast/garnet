using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Collections.Generic;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeFile : IcbcodeBase
    {
        public long ID { get; private set; }
        public string Url { get; private set; }
        public string Extension { get; private set; }
        public string Description { get; private set; }
        public long Size { get; private set; }
        public DateTime Publish { get; private set; }

        public static IcbcodeFile Convert(dynamic file, int index, int totals)
        {
            return new IcbcodeFile()
            {
                ID = file.file_id,
                Url = string.Format("/content/cms/files/{0}{1}", file.file_id, file.file_extension),
                Extension = file.file_extension,
                Description = file.file_desc,
                Size = file.file_size,
                Publish = file.file_publish,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }

        public IcbcodeFile()
        {

        }

        public static IcbcodeCollection<IcbcodeFile> All(string file_group)
        {
            IcbcodeCollection<IcbcodeFile> items = new IcbcodeCollection<IcbcodeFile>();

            using (FileRepository file_repository = new FileRepository())
            {
                List<dynamic> files = file_repository.Get(file_group);

                items.TotalPages = 1;
                items.CurrentPage = 1;
                items.PageSize = files.Count;

                for (int index = 0; index < files.Count; index++)
                {
                    items.Add(IcbcodeFile.Convert(files[index], index + 1, files.Count));
                }
            }

            return items;
        }

        public static IcbcodeCollection<IcbcodeFile> Find(string terms, string field = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            return Find(terms, null, field, page, size, query, where);
        }

        public static IcbcodeCollection<IcbcodeFile> Find(string terms, string[] block_name, string field = null, long page = 1, long size = Int64.MaxValue, string query = null, dynamic where = null)
        {
            IcbcodeCollection<IcbcodeFile> items = new IcbcodeCollection<IcbcodeFile>(); long totals;

            using (FileRepository file_repository = new FileRepository())
            {
                List<dynamic> files = file_repository.Search(terms, block_name, string.IsNullOrWhiteSpace(field) ? string.Empty : field, -1, page, size, out totals, query, where);

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
    }
}