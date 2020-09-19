using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin, icbcode_cms_content_manager")]
    public class TagController : BaseController
    {
        [HttpGet()]
        public ActionResult Index(string field_name)
        {
            using (UserRepository database = new UserRepository())
            {
                ViewBag.user = database.GetByLogin(User.Identity.Name);
            }

            using (TagRepository tag_repository = new TagRepository())
            {
                ViewBag.items = tag_repository.Get(field_name, CurrentUser.user_domain);
            }

            ViewBag.field_name = field_name;

            return View();
        }

        [HttpGet()]
        public ActionResult Remove(long tag_id, string field_name)
        {
            using (TagRepository tag_repository = new TagRepository())
            {
                tag_repository.Remove(tag_id);
            }

            return RedirectToAction("Index", new { field_name = field_name });
        }

        [HttpGet()]
        public ActionResult Edit(long? tag_id, long field_id)
        {
            using (TagRepository tag_repository = new TagRepository())
            {
                if (tag_id.HasValue)
                {
                    ViewBag.item = tag_repository.GetByID(tag_id.Value);
                }
            }

            using (FieldRepository field_repository = new FieldRepository())
            {
                ViewBag.field = field_repository.GetByID(field_id);
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? tag_id, string tag_name, string tag_field_name)
        {
            using (TagRepository tag_repository = new TagRepository())
            {
                if (tag_id.HasValue)
                {
                    tag_repository.Update(tag_id.Value, tag_name, tag_field_name, CurrentUser.user_domain);
                }
                else
                {
                    tag_id = tag_repository.CreateGlobalID();

                    tag_repository.Create(tag_id.Value, tag_name, tag_field_name, CurrentUser.user_domain);
                }
            }

            return RedirectToAction("Index", new { field_name = tag_field_name });
        }
    }
}