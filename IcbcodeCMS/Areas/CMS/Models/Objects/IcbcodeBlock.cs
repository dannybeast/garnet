using IcbcodeCMS.Areas.CMS.Models.Repositories;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeBlock : IcbcodeBase
    {
        public static IcbcodeBlock Get(string block_name, string domain_name)
        {
            IcbcodeBlock item = null;

            using (BlockRepository block_repository = new BlockRepository())
            {
                item = IcbcodeBlock.Convert(block_repository.GetByName(block_name, domain_name), 1, 1);
            }

            return item;
        }

        public long ID { get; private set; }
        public long? ContentRoot { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }

        public static IcbcodeBlock Convert(dynamic block, long index, long totals)
        {
            return new IcbcodeBlock()
            {
                ID = block.block_id,
                ContentRoot = block.block_content_root,
                Name = block.block_name,
                FriendlyName = block.block_friendly_name,
                Index = index,
                IsEven = index % 2 == 0,
                IsFirst = index == 1,
                IsLast = index == totals
            };
        }

        public IcbcodeBlock()
        {

        }
    }
}