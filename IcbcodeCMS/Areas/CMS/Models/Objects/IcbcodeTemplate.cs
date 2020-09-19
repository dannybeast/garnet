using IcbcodeCMS.Areas.CMS.Models.Repositories;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeTemplate : IcbcodeBase
    {
        public static IcbcodeTemplate Get(string template_name, long domain_id)
        {
            IcbcodeTemplate item = null;

            using (TemplateRepository template_repository = new TemplateRepository())
            {
                item = IcbcodeTemplate.Convert(template_repository.GetByName(template_name, domain_id), 1, 1);
            }

            return item;
        }

        public long ID { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }

        public static IcbcodeTemplate Convert(dynamic template, long index, long totals)
        {
            return new IcbcodeTemplate()
            {
                ID = template.template_id,
                Name = template.template_name,
                FriendlyName = template.template_friendly_name,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }

        public IcbcodeTemplate()
        {

        }
    }
}