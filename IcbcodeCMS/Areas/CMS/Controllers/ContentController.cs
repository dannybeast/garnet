using IcbcodeCMS.Areas.CMS.Models.Objects;
using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin, icbcode_cms_content_manager")]
    public class ContentController : BaseController
    {
        [HttpGet()]
        public ActionResult Import(long block_id, long? content_root, long? content_template)
        {
            ViewBag.domain_id = CurrentUser.user_domain;
            ViewBag.block_id = block_id;
            ViewBag.content_root = content_root;

            return View();
        }

        private void _import(string path, long root)
        {
        }

        [HttpPost()]
        public ActionResult Import(long block_id, long? content_root, long domain_id)
        {
            using (ContentRepository content_repository = new ContentRepository())
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];

                    if (file != null)
                    {
                        var file_id = content_repository.CreateGlobalID();

                        var file_extension = Path.GetExtension(file.FileName);

                        string path = Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", file_id, file_extension));

                        file.SaveAs(path);

                        if (content_root.HasValue)
                        {
                            _import(path, content_root.Value);
                        }
                    }
                }
            }

            return RedirectToAction("Index", new { block_id = block_id, content_root = content_root });
        }

        [HttpGet()]
        public ActionResult GetRelItems(long content_id, string template_name)
        {
            var items = new List<dynamic>();

            using (ContentRepository content_repository = new ContentRepository())
            {
                foreach (var item in content_repository.GetByParent(content_id, ModelUtility.GetStringArray(template_name)))
                {
                    items.Add(new { content_id = item.content_id, content_name = item.content_name });
                }
            }


            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet()]
        public ActionResult Copy(string content_ids, string retUrl)
        {
            List<long> ids = null;

            foreach (string id in (content_ids ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (ids == null)
                    ids = new List<long>();

                ids.Add(Int64.Parse(id));
            }

            Session.Remove("copy_ids");

            Session.Add("copy_ids", ids == null ? null : ids.ToArray());

            return Redirect(retUrl);
        }

        [HttpGet()]
        public ActionResult Paste(long? content_id, string retUrl, long block_id)
        {
            long[] content_ids = (long[])Session["copy_ids"];

            if (content_ids != null)
            {
                using (ContentRepository content_repository = new ContentRepository())
                {
                    foreach (var id in content_ids)
                    {
                        _Paste(content_id, id, block_id);
                    }
                }

                Session.Remove("copy_ids");
            }

            return Redirect(retUrl);
        }

        private void _Paste(long? content_id, long id, long block_id)
        {
            using (ContentRepository content_repository = new ContentRepository())
            {
                var content = content_repository.GetByID(id);
                // build user data

                string user_data = string.Empty;

                using (FieldRepository field_repository = new FieldRepository())
                {
                    List<dynamic> fields = field_repository.GetByIDs(content.template_fields);

                    foreach (var item in fields)
                    {
                        if (item.field_group != "Пользовательские")
                            continue;

                        var value = ((IDictionary<string, object>)content)[item.field_name];

                        if (value != null)
                        {
                            switch ((string)item.field_type)
                            {
                                case "string":
                                case "html":
                                    value = string.Format("'{0}'", MySql.Data.MySqlClient.MySqlHelper.EscapeString(value));
                                    break;

                                case "boolean":

                                    break;

                                case "list":
                                    break;

                                case "date":
                                    value = string.Format("'{0}'", value.ToString("yyyy-MM-dd"));
                                    break;

                                case "decimal":
                                    value = value.ToString().Replace(",", ".");
                                    break;

                                default:
                                    value = value == null ? null : string.Format("'{0}'", value);
                                    break;
                            }
                        }

                        user_data += string.Format(",{0} = {1}", item.field_name, (value == null ? "NULL" : value));
                    }
                }

                // end build user data

                string parent_url = string.Empty;

                if (content_id.HasValue)
                {
                    var r_content = content_repository.GetByID(content_id.Value);

                    parent_url += r_content.content_url;
                }

                string new_url = parent_url + "/" + Transliterator.Translite(content.content_name.Substring(0, (content.content_name.Length > 100 ? 100 : content.content_name.Length)));

                long new_content_id = content_repository.CreateGlobalID();

                if (content_repository.Exists(new_url, CurrentUser.user_domain))
                    new_url = new_url + "-" + new_content_id.ToString();

                content_repository.Create(new_content_id, content.content_in_sitemap, content.content_export_rss, content.content_active, content.content_main, content.content_allow_deleted, content.content_name,
                    content.content_view,
                    new_url, block_id, content.content_title, content.content_description, content.content_keywords, content.content_h1, content.content_h2, content.content_short_text, content.content_text,
                    content.content_publish, content_id, content.content_template, content.content_allow_redirect, content.content_redirect_permanent, content.content_redirect_url, content.content_link,
                    content.content_export_rss_ids, content.content_export_rss_title, CurrentUser.user_domain, user_data, content.content_order);


                // create files

                using (FileRepository file_repository = new FileRepository())
                {
                    foreach (var file in file_repository.Get(content.content_id.ToString()))
                    {
                        if (file.file_group == null) { continue; }

                        var new_file_id = file_repository.CreateGlobalID();

                        file_repository.Save(new_file_id, file.file_extension, new_content_id.ToString(), file.file_desc, (int)file.file_size, file.file_field, file.file_publish, file.file_order);

                        try
                        {
                            System.IO.File.Copy(
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", file.file_id, file.file_extension)),
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", new_file_id, file.file_extension))
                                );
                        }
                        catch
                        {
                            using (FileRepository database = new FileRepository())
                            {
                                database.Remove(new_file_id);
                            }
                        }
                    }
                }

                // end create files

                // create images

                using (ImageRepository image_repository = new ImageRepository())
                {
                    foreach (var image in image_repository.Get(content.content_id.ToString()))
                    {
                        if (image.image_group == null) { continue; }

                        var new_image_id = image_repository.CreateGlobalID();

                        image_repository.Save(new_image_id, image.image_extension, new_content_id.ToString(), image.image_field, image.image_desc, image.image_order);

                        try
                        {
                            System.IO.File.Copy(
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", image.image_id, image.image_extension)),
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", new_image_id, image.image_extension))
                                );
                        }
                        catch
                        {
                            using (FileRepository database = new FileRepository())
                            {
                                database.Remove(new_image_id);
                            }
                        }
                    }
                }

                using (ImageRepository image_repository = new ImageRepository())
                {
                    foreach (var image in image_repository.Get(content.content_id.ToString(), "kartinka"))
                    {
                        if (image.image_group == null) { continue; }

                        var new_image_id = image_repository.CreateGlobalID();

                        image_repository.Save(new_image_id, image.image_extension, new_content_id.ToString(), image.image_field, image.image_desc, image.image_order);

                        try
                        {
                            System.IO.File.Copy(
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", image.image_id, image.image_extension)),
                                Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", new_image_id, image.image_extension))
                                );
                        }
                        catch
                        {
                            using (FileRepository database = new FileRepository())
                            {
                                database.Remove(new_image_id);
                            }
                        }
                    }
                }


                // end create images

                foreach (var c in content_repository.GetsByParent(id))
                {
                    _Paste(new_content_id, c.content_id, block_id);
                }
            }
        }

        [HttpGet()]
        public ActionResult Index(long block_id, long? page, long? content_root)
        {
            ViewBag.block_id = block_id;

            dynamic block = null;

            using (BlockRepository block_repository = new BlockRepository())
            {
                block = block_repository.GetByID(block_id);

                ViewBag.blcoks = block_repository.All(CurrentUser.user_domain);
            }

            content_root = content_root ?? block.block_content_root;

            ViewBag.content_root = content_root;

            page = page ?? 1;

            long totals;

            using (ContentRepository content_repository = new ContentRepository())
            {
                string order_by = (string.IsNullOrWhiteSpace(block.block_order_fields) || block.block_allow_sort ? "content_order, content_publish desc" : block.block_order_fields);

                if (content_root.HasValue)
                {
                    using (TemplateRepository template_repository = new TemplateRepository())
                    {
                        dynamic root_item = content_repository.GetByID(content_root.Value);

                        dynamic root_template = template_repository.GetByID(root_item.content_template);

                        if (root_template.template_allow_subpartitions)
                        {
                            ViewBag.template_allow_sort = root_template.template_allow_sort;

                            order_by = (string.IsNullOrWhiteSpace(root_template.template_order_fields) || root_template.template_allow_sort ? "content_order, content_publish desc" : root_template.template_order_fields);
                        }
                    }
                }

                ViewBag.items = content_repository.Get(CurrentUser.user_domain, block_id, content_root, order_by, page ?? 1, block.block_allow_sort ? Int64.MaxValue : 50, out totals);
            }

            ViewBag.page = page;

            ViewBag.totals = totals;

            List<dynamic> nav = content_root != null ? ModelUtility.GetNavi(content_root, block.block_content_root, false) : new List<dynamic>();

            if (nav.Count > 0 && block.block_content_root == null)
            {
                nav.Insert(0, null);
            }

            ViewBag.nav = nav;

            ViewBag.block = block;


            return View();
        }

        [HttpGet()]
        public ActionResult Edit(long? content_id, long block_id, long? content_root, long? content_template)
        {
            ViewBag.domain_id = CurrentUser.user_domain;

            ViewBag.content_id = content_id;

            ViewBag.block_id = block_id;

            ViewBag.content_root = content_root;

            using (BlockRepository block_repository = new BlockRepository())
            {
                using (ContentRepository content_repository = new ContentRepository())
                {
                    using (TemplateRepository template_repository = new TemplateRepository())
                    {
                        ViewBag.block = block_repository.GetByID(block_id);

                        string allow_templates = string.Empty;

                        if (content_root.HasValue)
                        {
                            dynamic root_item = content_repository.GetByID(content_root.Value);

                            dynamic root_template = template_repository.GetByID(root_item.content_template);

                            allow_templates = root_template.template_allow_subpartitions && root_item.content_block == block_id ? root_template.template_templates : string.Empty;
                        }

                        if (content_id.HasValue)
                        {
                            ViewBag.item = content_repository.GetByID(content_id.Value);
                        }

                        ViewBag.templates = template_repository.GetByIDs(string.IsNullOrWhiteSpace(allow_templates) ? ViewBag.block.block_templates : allow_templates);
                    }
                }
            }

            if (ViewBag.templates.Count > 0)
            {
                ViewBag.current_template = ViewBag.templates[0];

                foreach (var template in ViewBag.templates)
                {
                    if (content_template.HasValue)
                    {
                        if (content_template.Value == template.template_id)
                        {
                            ViewBag.current_template = template;
                            break;
                        }
                    }
                    else if (ViewBag.item != null && template.template_id == ViewBag.item.content_template)
                    {
                        ViewBag.current_template = template;
                        break;
                    }
                }

                using (ViewRepository view_repository = new ViewRepository())
                {
                    ViewBag.content_views = view_repository.GetByIDs(ViewBag.current_template.template_views);
                }

                using (FieldRepository field_repository = new FieldRepository())
                {
                    ViewBag.fields = field_repository.GetByIDs(ViewBag.current_template.template_fields);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(
            // base
            string guid, long? content_id, long content_block, long? content_root, string template_fields,
            // main
            string content_name, string content_url, long content_template, long? content_view, string content_link,
            // settings
            bool? content_in_sitemap, bool? content_active, bool? content_main, bool? content_allow_deleted, bool? content_allow_redirect, bool? content_redirect_permanent, string content_redirect_url,
            // seo
            string content_title, string content_description, string content_keywords,
            // rss
            bool? content_export_rss, string content_export_rss_ids, string content_export_rss_title,
            // content
            string content_h1, string content_h2, string content_short_text, string content_text, DateTime? content_publish,
            // images
            string[] image_desc, long[] image_ids_for_desc,
            // files
            string[] file_desc, long[] file_ids_for_desc,
            // user fields
            FormCollection user_fileds)
        {
            content_in_sitemap = content_in_sitemap ?? false; content_export_rss = content_export_rss ?? false; content_active = content_active ?? true; content_main = content_main ?? false; content_allow_deleted = content_allow_deleted ?? true; content_allow_redirect = content_allow_redirect ?? false; content_redirect_permanent = content_redirect_permanent ?? false;

            string user_data = string.Empty; int itemCount = 1;

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.GetByIDs(template_fields);

                foreach (var item in fields)
                {
                    if (item.field_group != "Пользовательские")
                        continue;

                    string value = user_fileds[item.field_name];

                    if (value != null)
                    {
                        switch ((string)item.field_type)
                        {
                            case "string":
                            case "html":
                            case "tags":
                                value = string.Format("'{0}'", MySql.Data.MySqlClient.MySqlHelper.EscapeString(value));
                                break;

                            case "date":
                            case "datetime":
                                value = string.IsNullOrWhiteSpace(value) ? null : string.Format("'{0}'", value);
                                break;

                            case "boolean":
                                value = value == "true,false" ? "1" : "0";
                                break;

                            case "decimal":
                                decimal decimal_val;

                                value = value.Replace(",", ".");

                                if (!Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal_val))
                                    value = "0";
                                break;

                            case "integer":
                                int int_val;
                                if (!Int32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int_val))
                                    value = "0";
                                break;

                            case "list":
                                long list_val;
                                if (!Int64.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out list_val))
                                    value = null;
                                break;

                            default:
                                value = value == null ? null : string.Format("'{0}'", value);
                                break;
                        }

                        if (item.field_name == "tirazh_sht")
                        {
                            itemCount = Convert.ToInt32(value);
                        }
                    }

                    user_data += string.Format(",{0} = {1}", item.field_name, value ?? "NULL");
                }
            }

            List<long> other_ids = null; List<string> promocodes = new List<string>();

            using (ContentRepository content_repository = new ContentRepository())
            {
                using (ImageRepository image_repository = new ImageRepository())
                {
                    using (FileRepository file_repository = new FileRepository())
                    {
                        if (content_id.HasValue)
                        {
                            var _old = content_repository.GetByID(content_id.Value);

                            content_repository.Update(content_id.Value, content_in_sitemap.Value, content_export_rss.Value, content_active.Value, content_main.Value, content_allow_deleted.Value, content_name, content_view, content_block, content_title, content_description, content_keywords, content_h1, content_h2, content_short_text, content_text, content_publish, content_root, content_template, content_allow_redirect, content_redirect_permanent, content_redirect_url, content_link, content_export_rss_ids, content_export_rss_title, CurrentUser.domain_id, user_data);
                        }
                        else
                        {
                            for (int z = 0; z < itemCount; z++)
                            {
                                if (content_template == 42528)
                                {

                                    do
                                    {
                                        content_name = IcbcodeUtility.GetPromocode().ToLowerInvariant();
                                    }
                                    while (IcbcodeContent.Get(new string[] { "promokodi" }, new string[] { "promokod" }, query: $"content_name = '{content_name}'").Count != 0);
                                }

                                content_id = content_repository.CreateGlobalID();

                                string parent_url = string.Empty;

                                if (content_root.HasValue)
                                {
                                    var content = content_repository.GetByID(content_root.Value);

                                    parent_url += content.content_url;
                                }

                                string new_url = parent_url + "/" + Transliterator.Translite(content_url.Substring(0, (content_url.Length > 100 ? 100 : content_url.Length)));

                                if (content_repository.Exists(new_url, CurrentUser.domain_id))
                                {
                                    if (content_template == 42528)
                                    {
                                        new_url = parent_url + "/" + content_id.ToString();
                                    }
                                    else
                                    {
                                        new_url = new_url + "-" + content_id.ToString();
                                    }
                                }

                                promocodes.Add(content_name);

                                content_repository.Create(content_id.Value, content_in_sitemap.Value, content_export_rss.Value, content_active.Value, content_main.Value, content_allow_deleted.Value, content_name, content_view, new_url, content_block, content_title, content_description, content_keywords, content_h1, content_h2, content_short_text, content_text, content_publish, content_root, content_template, content_allow_redirect, content_redirect_permanent, content_redirect_url, content_link, content_export_rss_ids, content_export_rss_title, CurrentUser.domain_id, user_data);

                                using (TemplateRepository template_repository = new TemplateRepository())
                                {
                                    var current_template = template_repository.GetByID(content_template);

                                    if (current_template.template_create_child)
                                    {
                                        CreateChildsDefaultContent(content_id.Value);
                                    }
                                }
                            }

                            if (content_template == 42528)
                            {
                                IcbcodeUtility.SendEmail(null, null, $"Промокоды сгенерированы от {DateTime.Now}", string.Join("<br/>", promocodes));
                            }
                        }

                        image_repository.Update(content_id.Value, guid);

                        if (image_desc != null && image_ids_for_desc != null && image_desc.Length == image_ids_for_desc.Length)
                        {
                            for (int i = 0; i <= image_ids_for_desc.Length - 1; i++)
                            {
                                image_repository.UpdateDescr(image_ids_for_desc[i], image_desc[i]);
                            }
                        }

                        file_repository.Update(content_id.Value, guid);

                        if (file_desc != null && file_ids_for_desc != null && file_desc.Length == file_ids_for_desc.Length)
                        {
                            for (int i = 0; i <= file_ids_for_desc.Length - 1; i++)
                            {
                                file_repository.UpdateDescr(file_ids_for_desc[i], file_desc[i]);
                            }
                        }

                        if (other_ids != null)
                        {
                            foreach (var other_id in other_ids)
                            {
                                file_repository.RemoveByRef(other_id);

                                if (file_ids_for_desc != null && file_ids_for_desc.Length > 0)
                                {
                                    CopyFiles(content_id.Value, other_id);
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", new { block_id = content_block, content_root = content_root });
        }

        public void CopyFiles(long from_content_id, long to_content_id)
        {
            // create files

            using (FileRepository file_repository = new FileRepository())
            {
                foreach (var file in file_repository.Get(from_content_id.ToString()))
                {
                    if (file.file_group == null) { continue; }

                    var new_file_id = file_repository.CreateGlobalID();

                    file_repository.Save(new_file_id, file.file_extension, to_content_id.ToString(), file.file_desc, (int)file.file_size, file.file_field, file.file_publish, file.file_order, to_content_id);

                    try
                    {
                        System.IO.File.Copy(
                            Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", file.file_id, file.file_extension)),
                            Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", new_file_id, file.file_extension))
                            );
                    }
                    catch
                    {
                        using (FileRepository database = new FileRepository())
                        {
                            database.Remove(new_file_id);
                        }
                    }
                }
            }
        }

        private string GetUserData(string template_fields, out bool conent_publish)
        {
            string user_data = string.Empty; conent_publish = false;

            using (FieldRepository field_repository = new FieldRepository())
            {
                List<dynamic> fields = field_repository.GetByIDs(template_fields);

                foreach (var item in fields)
                {
                    if (item.field_name == "content_publish")
                        conent_publish = true;

                    if (item.field_group != "Пользовательские")
                        continue;

                    string value = null;

                    switch ((string)item.field_type)
                    {
                        case "boolean":
                            value = "0";
                            break;

                        case "decimal":
                            decimal decimal_val;

                            value = value.Replace(",", ".");

                            if (!Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal_val))
                                value = "0";
                            break;

                        case "integer":
                            int int_val;
                            if (!Int32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int_val))
                                value = "0";
                            break;

                            // !!! ЕСЛИ ЛИСТ - ТО ВЫБРАТЬ ПЕРВОЕ ЗНАЧЕНИЕ
                    }


                    user_data += string.Format(",{0} = {1}", item.field_name, value ?? "NULL");
                }
            }

            return user_data;
        }

        private void CreateChildsDefaultContent(long content_root)
        {
            bool content_in_sitemap = false; bool content_export_rss = false; bool content_active = true; bool content_main = false; bool content_allow_deleted = true; bool content_allow_redirect = false; bool content_redirect_permanent = false;

            using (ContentRepository content_repository = new ContentRepository())
            {
                using (TemplateRepository template_repository = new TemplateRepository())
                {
                    var current_content = content_repository.GetByID(content_root);

                    var current_template = template_repository.GetByID(current_content.content_template);

                    var child_templates = template_repository.GetByIDs(current_template.template_templates);

                    foreach (var child_template in child_templates)
                    {
                        string content_url = child_template.template_friendly_name;

                        long content_id = content_repository.CreateGlobalID();

                        string parent_url = current_content.content_url;

                        string new_url = parent_url + "/" + Transliterator.Translite(content_url.Substring(0, (content_url.Length > 100 ? 100 : content_url.Length)));

                        if (content_repository.Exists(new_url, CurrentUser.domain_id))
                            new_url = new_url + "-" + content_id.ToString();

                        bool content_publish;

                        string user_data = GetUserData(child_template.template_fields, out content_publish);

                        long? template_view = null;

                        if (child_template.template_views != null)
                        {
                            string[] view_ids = child_template.template_views.Split(new char[] { ',' });

                            if (view_ids.Length > 0)
                            {
                                template_view = Int64.Parse(view_ids[0]);
                            }
                        }

                        content_repository.Create(content_id, content_in_sitemap, content_export_rss, content_active, content_main, content_allow_deleted, child_template.template_friendly_name, template_view, new_url, current_content.content_block, child_template.template_friendly_name, null, null, child_template.template_friendly_name, child_template.template_friendly_name, null, null, content_publish ? (DateTime?)DateTime.Now.Date : (DateTime?)null, content_root, child_template.template_id, content_allow_redirect, content_redirect_permanent, null, child_template.template_friendly_name, null, null, CurrentUser.domain_id, user_data);
                    }
                }
            }
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (ContentRepository content_repository = new ContentRepository())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    content_repository.UpdateOrder(ids[i], orders[i]);
                }
            }

            return new EmptyResult();
        }

        [HttpGet()]
        public ActionResult Remove(long content_id, long? content_root, long block_id)
        {
            ModelUtility.RemoveContent(content_id);

            return RedirectToAction("Index", new { block_id = block_id, content_root = content_root });
        }
    }
}