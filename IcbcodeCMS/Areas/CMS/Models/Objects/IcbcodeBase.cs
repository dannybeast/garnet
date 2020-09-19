namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public abstract class IcbcodeBase
    {
        public bool IsEven { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public long Index { get; set; }
    }
}