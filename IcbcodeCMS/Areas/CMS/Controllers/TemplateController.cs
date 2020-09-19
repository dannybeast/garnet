using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer")]
    public class TemplateController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (TemplateRepository template_repository = new TemplateRepository())
            {
                ViewBag.items = template_repository.All(CurrentUser.user_domain);
            }

            return View();
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (TemplateRepository template_repository = new TemplateRepository())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    template_repository.UpdateOrder(ids[i], orders[i]);
                }
            }

            return new EmptyResult();
        }

        [HttpGet()]
        public ActionResult Remove(long template_id)
        {
            using (TemplateRepository template_repository = new TemplateRepository())
            {
                template_repository.Remove(template_id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? template_id)
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                ViewBag.views = view_repository.All(CurrentUser.user_domain);
            }

            using (FieldRepository field_repository = new FieldRepository())
            {
                ViewBag.fields = field_repository.All();
            }

            using (TemplateRepository template_repository = new TemplateRepository())
            {
                ViewBag.templates = template_repository.All(CurrentUser.user_domain);

                if (template_id.HasValue)
                {

                    ViewBag.item = template_repository.GetByID(template_id.Value);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? template_id, string template_friendly_name, string template_name, bool template_allow_subpartitions, string[] template_fields, string[] template_views, long?[] template_templates, bool template_allow_sort, string template_order_fields, bool template_create_child)
        {
            using (TemplateRepository template_repository = new TemplateRepository())
            {
                if (template_id.HasValue)
                {
                    template_repository.Update(template_id.Value, template_friendly_name, template_name, template_allow_subpartitions, (template_fields == null ? string.Empty : string.Join(",", template_fields)), (template_views == null ? string.Empty : string.Join(",", template_views)), (template_templates == null ? string.Empty : string.Join(",", template_templates)), template_allow_sort, template_order_fields, template_create_child, CurrentUser.user_domain);
                }
                else
                {
                    template_id = template_repository.CreateGlobalID();

                    string new_template_name = Transliterator.Translite(template_friendly_name);

                    if (template_repository.Exists(new_template_name, CurrentUser.user_domain))
                        new_template_name = new_template_name + "-" + template_id.ToString();

                    template_repository.Create(template_id.Value, template_friendly_name, new_template_name, template_allow_subpartitions, (template_fields == null ? string.Empty : string.Join(",", template_fields)), (template_views == null ? string.Empty : string.Join(",", template_views)), (template_templates == null ? string.Empty : string.Join(",", template_templates)), template_allow_sort, template_order_fields, template_create_child, CurrentUser.user_domain);
                }
            }

            return RedirectToAction("Index");
        }
    }
}