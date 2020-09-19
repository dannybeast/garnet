using System.Collections.Generic;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public class IcbcodeCollection<T> : List<T> where T : IcbcodeBase
    {
        public long CurrentPage { get; set; }
        public long PageSize { get; set; }
        public long TotalPages { get; set; }
    }
}