using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer")]
    public class FieldController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (FieldRepository field_repository = new FieldRepository())
            {
                ViewBag.items = field_repository.All();
            }

            return View();
        }

        [HttpGet()]
        public ActionResult Remove(long field_id)
        {
            dynamic field;

            using (FieldRepository field_repository = new FieldRepository())
            {
                field = field_repository.GetByID(field_id);

                try
                {
                    using (ContentRepository content_repository = new ContentRepository())
                    {
                        content_repository.RemoveColumn(field.field_name);

                        field_repository.Remove(field_id);
                    }
                }
                catch { }
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? field_id)
        {
            if (field_id.HasValue)
            {
                using (FieldRepository field_repository = new FieldRepository())
                {
                    ViewBag.item = field_repository.GetByID(field_id.Value);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? field_id, string field_name, string field_friendly_name, string field_type, string field_description, long? filed_content_root, string filed_template_name, bool? field_is_filter, bool? field_inside_domain, long? filed_tags_limit, string field_rel_field)
        {
            field_is_filter = field_is_filter ?? false; field_inside_domain = field_inside_domain ?? true;

            string column_data_type;

            switch (field_type)
            {
                case "string":
                case "html":
                case "multiplelist":
                case "tags":
                    column_data_type = "TEXT";
                    break;
                case "decimal":
                    column_data_type = "DECIMAL(10,2)";
                    break;
                case "integer":
                    column_data_type = "INT";
                    break;
                case "boolean":
                    column_data_type = "BOOL";
                    break;
                case "date":
                    column_data_type = "DATE";
                    break;
                case "datetime":
                    column_data_type = "DATETIME";
                    break;
                case "list":
                    column_data_type = "BIGINT";
                    break;
                default:
                    column_data_type = "TEXT";
                    break;
            }

            field_name = Transliterator.Translite(string.IsNullOrWhiteSpace(field_name) ? field_friendly_name : field_name, '_');

            using (FieldRepository field_repository = new FieldRepository())
            {
                using (ContentRepository content_repository = new ContentRepository())
                {
                    if (field_id.HasValue)
                    {
                        var old_field = field_repository.GetByID(field_id.Value);

                        if (!old_field.field_system)
                        {
                            if (field_name != old_field.field_name && field_repository.Exists(field_name))
                                field_name = field_name + "_" + field_id.Value.ToString();

                            content_repository.EditColumn(old_field.field_name, field_name, column_data_type);
                        }

                        field_repository.Update(field_id.Value, field_name, field_friendly_name, field_type, field_description, filed_content_root, string.IsNullOrWhiteSpace(filed_template_name) ? null : filed_template_name, field_is_filter.Value, field_inside_domain.Value, filed_tags_limit, field_rel_field);
                    }
                    else
                    {
                        field_id = field_repository.CreateGlobalID();

                        if (field_repository.Exists(field_name))
                            field_name = field_name + "_" + field_id.ToString();

                        content_repository.AddColumn(field_name, column_data_type);

                        field_repository.Create(field_name, field_friendly_name, field_type, field_description, "Пользовательские", filed_content_root, string.IsNullOrWhiteSpace(filed_template_name) ? null : filed_template_name, field_is_filter.Value, field_inside_domain.Value, filed_tags_limit, field_rel_field);
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}