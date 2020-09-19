using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin, icbcode_cms_content_manager")]
    public class SnapshotController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}