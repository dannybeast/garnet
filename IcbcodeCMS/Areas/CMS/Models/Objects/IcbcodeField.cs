using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System.Collections.Generic;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeField : IcbcodeBase
    {
        public static IcbcodeCollection<IcbcodeField> All()
        {
            IcbcodeCollection<IcbcodeField> items = new IcbcodeCollection<IcbcodeField>();

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.All();

                items.TotalPages = items.Count;
                items.CurrentPage = 1;
                items.PageSize = 1;

                for (int index = 0; index < fields.Count; index++)
                {
                    items.Add(IcbcodeField.Convert(fields[index], index + 1, fields.Count));
                }
            }

            return items;
        }

        public static IcbcodeCollection<IcbcodeField> All(long template_id)
        {
            IcbcodeCollection<IcbcodeField> items = new IcbcodeCollection<IcbcodeField>();

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.All(template_id);

                items.TotalPages = items.Count;
                items.CurrentPage = 1;
                items.PageSize = 1;

                for (int index = 0; index < fields.Count; index++)
                {
                    items.Add(IcbcodeField.Convert(fields[index], index + 1, fields.Count));
                }
            }

            return items;
        }

        public static IcbcodeCollection<IcbcodeField> AllAll()
        {
            IcbcodeCollection<IcbcodeField> items = new IcbcodeCollection<IcbcodeField>();

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.AllAll();

                items.TotalPages = items.Count;
                items.CurrentPage = 1;
                items.PageSize = 1;

                for (int index = 0; index < fields.Count; index++)
                {
                    items.Add(IcbcodeField.Convert(fields[index], index + 1, fields.Count));
                }
            }

            return items;
        }

        public static IcbcodeCollection<IcbcodeField> Get(long content_id)
        {
            IcbcodeCollection<IcbcodeField> items = new IcbcodeCollection<IcbcodeField>();

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.GetFilterFields(content_id);

                items.TotalPages = items.Count;
                items.CurrentPage = 1;
                items.PageSize = 1;

                for (int index = 0; index < fields.Count; index++)
                {
                    items.Add(IcbcodeField.Convert(fields[index], index + 1, fields.Count));
                }
            }

            return items;
        }

        public long ID { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }
        public string Type { get; private set; }
        public string Group { get; private set; }
        public long? ContentRoot { get; private set; }
        public string TemplateName { get; private set; }

        public bool IsFilter { get; private set; }

        public static IcbcodeField Convert(dynamic field, long index, long totals)
        {
            return new IcbcodeField()
            {
                ID = field.field_id,
                Name = field.field_name,
                FriendlyName = field.field_friendly_name,
                Description = field.field_description,
                Type = field.field_type,
                Group = field.field_group,
                ContentRoot = field.filed_content_root,
                TemplateName = field.filed_template_name,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals,
                IsFilter = field.field_is_filter
            };
        }

        public IcbcodeField()
        {

        }
    }
}