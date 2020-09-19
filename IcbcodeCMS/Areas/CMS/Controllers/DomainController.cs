using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer")]
    public class DomainController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (DomainRepository domain_repository = new DomainRepository())
            {
                ViewBag.items = domain_repository.All();
            }

            return View();
        }

        [HttpGet()]
        public ActionResult Remove(long domain_id)
        {
            using (DomainRepository domain_repository = new DomainRepository())
            {
                domain_repository.Remove(domain_id);

                using (ContentRepository content_repository = new ContentRepository())
                {
                    foreach (var item in content_repository.GetByDomain(domain_id))
                    {
                        ModelUtility.RemoveContent(item.content_id);
                    }
                }

                using (TemplateRepository template_repository = new TemplateRepository())
                {
                    template_repository.RemoveByDomain(domain_id);
                }

                using (ViewRepository view_repository = new ViewRepository())
                {
                    view_repository.RemoveByDomain(domain_id);
                }

                using (BlockRepository block_repository = new BlockRepository())
                {
                    block_repository.RemoveByDomain(domain_id);
                }

                using (VariableRepository variable_repository = new VariableRepository())
                {
                    variable_repository.RemoveByDomain(domain_id);
                }

                using (TagRepository tag_repository = new TagRepository())
                {
                    tag_repository.RemoveByDomain(domain_id);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? domain_id)
        {
            if (domain_id.HasValue)
            {
                using (DomainRepository domain_repository = new DomainRepository())
                {
                    ViewBag.item = domain_repository.GetByID(domain_id.Value);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? domain_id, string domain_name, string domain_comment)
        {
            using (DomainRepository domain_repository = new DomainRepository())
            {
                if (domain_id.HasValue)
                {
                    domain_repository.Update(domain_id.Value, domain_name, domain_comment);
                }
                else
                {
                    domain_id = domain_repository.CreateGlobalID();

                    domain_repository.Create(domain_id.Value, domain_name, domain_comment);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Clone(long? domain_id)
        {
            Dictionary<long, long> views = new Dictionary<long, long>();

            Dictionary<long, long> templates = new Dictionary<long, long>();

            Dictionary<long, long> blocks = new Dictionary<long, long>();

            Dictionary<long, long> contents = new Dictionary<long, long>();

            if (domain_id.HasValue)
            {
                using (DomainRepository domain_repository = new DomainRepository())
                {
                    var source_domain = domain_repository.GetByID(domain_id.Value);

                    // create domain

                    long new_domain_id = domain_repository.CreateGlobalID();

                    domain_repository.Create(new_domain_id, string.Format("new-{0}", new_domain_id), string.Empty);

                    // end create domain

                    // create views

                    using (ViewRepository view_repository = new ViewRepository())
                    {
                        foreach (var view in view_repository.All(source_domain.domain_id))
                        {
                            long new_view_id = view_repository.CreateGlobalID();

                            view_repository.Create(new_view_id, view.view_friendly_name, view.view_name, view.view_path, new_domain_id, view.view_order);

                            views.Add(view.view_id, new_view_id);
                        }
                    }

                    // end create views

                    // create templates

                    using (TemplateRepository template_repository = new TemplateRepository())
                    {
                        foreach (var template in template_repository.All(source_domain.domain_id))
                        {
                            long new_template_id = template_repository.CreateGlobalID();

                            string template_views = string.Empty;

                            foreach (var template_view_id in ((string)template.template_views ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                template_views += string.Format("{0},", views[Int64.Parse(template_view_id)]);
                            }

                            template_repository.Create(new_template_id, template.template_friendly_name, template.template_name, template.template_allow_subpartitions, template.template_fields, template_views.Trim(','), template.template_templates, template.template_allow_sort, template.template_order_fields, template.template_create_child, new_domain_id, template.template_order);

                            templates.Add(template.template_id, new_template_id);
                        }

                        foreach (var template in template_repository.All(new_domain_id))
                        {
                            string template_templates = string.Empty;

                            foreach (var template_template_id in ((string)template.template_templates ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                template_templates += string.Format("{0},", templates[Int64.Parse(template_template_id)]);
                            }

                            template_repository.UpdateRefsTemplates(template.template_id, template_templates.Trim(','));
                        }
                    }

                    // end create templates

                    // create variables

                    using (VariableRepository variable_repository = new VariableRepository())
                    {
                        foreach (var variable in variable_repository.All(source_domain.domain_id))
                        {
                            long new_variable_id = variable_repository.CreateGlobalID();

                            variable_repository.Create(new_variable_id, variable.var_name, variable.var_friendly_name, variable.var_value, variable.var_type, variable.var_comment, variable.var_group, new_domain_id);
                        }
                    }

                    // end create variables

                    // create blocks

                    using (BlockRepository block_repository = new BlockRepository())
                    {
                        foreach (var block in block_repository.All(source_domain.domain_id))
                        {
                            long new_block_id = block_repository.CreateGlobalID();

                            string block_templates = string.Empty;

                            foreach (var block_template_id in ((string)block.block_templates ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                block_templates += string.Format("{0},", templates[Int64.Parse(block_template_id)]);
                            }

                            block_repository.Create(new_block_id, block.block_name, block.block_friendly_name, block_templates.Trim(','), block.block_content_root, block.block_area, block.block_controller, block.block_action, block.block_allow_sort, block.block_order_fields, block.block_separated, new_domain_id, block.block_order);

                            blocks.Add(block.block_id, new_block_id);
                        }
                    }

                    // end create blocks

                    // create contents

                    using (ContentRepository content_repository = new ContentRepository())
                    {
                        foreach (var content in content_repository.GetByDomain(source_domain.domain_id))
                        {
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


                            long new_content_id = content_repository.CreateGlobalID();

                            content_repository.Create(new_content_id, content.content_in_sitemap, content.content_export_rss, content.content_active, content.content_main, content.content_allow_deleted, content.content_name,
                                (content.content_view == null ? null : views[content.content_view]),
                                content.content_url, blocks[content.content_block], content.content_title, content.content_description, content.content_keywords, content.content_h1, content.content_h2, content.content_short_text, content.content_text,
                                content.content_publish, content.content_root, templates[content.content_template], content.content_allow_redirect, content.content_redirect_permanent, content.content_redirect_url, content.content_link, content.content_export_rss_ids, content.content_export_rss_title,
                                new_domain_id, user_data, content.content_order);

                            contents.Add(content.content_id, new_content_id);
                        }

                        foreach (var content in content_repository.GetByDomain(new_domain_id))
                        {
                            string user_data = string.Empty;

                            using (FieldRepository field_repository = new FieldRepository())
                            {
                                List<dynamic> fields = field_repository.GetByIDs(content.template_fields);

                                foreach (var item in fields)
                                {
                                    if (item.field_group == "Пользовательские")
                                    {

                                        var value = ((IDictionary<string, object>)content)[item.field_name];

                                        if (value != null)
                                        {
                                            if (item.field_type == "list")
                                            {
                                                value = contents.ContainsKey(value) ? contents[value] : value;
                                            }
                                            else
                                                if (item.field_type == "multiplelist")
                                            {
                                                string values = string.Empty;

                                                foreach (var id in (value ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                                {
                                                    values += string.Format("{0},", contents.ContainsKey(Int64.Parse(id)) ? contents[Int64.Parse(id)] : Int64.Parse(id));
                                                }

                                                value = string.Format("'{0}'", values).Trim(',');
                                            }
                                            else { continue; }
                                        }

                                        user_data += string.Format(",{0} = {1}", item.field_name, value ?? "NULL");
                                    }
                                }
                            }

                            content_repository.UpdateRefsRoot(content.content_id, content.content_root == null ? null : contents.ContainsKey(content.content_root) ? contents[content.content_root] : content.content_root, user_data);
                        }

                    }

                    // end create contents

                    // start blocks refs content_root update

                    using (BlockRepository block_repository = new BlockRepository())
                    {
                        foreach (var block in block_repository.All(new_domain_id))
                        {
                            if (block.block_content_root == null) { continue; }

                            block_repository.UpdateRefsRoot(block.block_id, contents[block.block_content_root]);
                        }
                    }

                    // end blocks refs content_root update

                    // create files

                    using (FileRepository file_repository = new FileRepository())
                    {
                        foreach (var file in file_repository.All(source_domain.domain_id))
                        {
                            if (file.file_ref == null) { continue; }

                            var new_file_id = file_repository.CreateGlobalID();

                            file_repository.Save(new_file_id, file.file_extension, contents[file.file_ref].ToString(), file.file_desc, (int)file.file_size, file.file_field, (DateTime)file.file_publish, (int)file.file_order, contents[file.file_ref]);

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
                        foreach (var image in image_repository.All(source_domain.domain_id))
                        {
                            if (image.image_ref == null) { continue; }

                            var new_image_id = image_repository.CreateGlobalID();

                            image_repository.Save(new_image_id, image.image_extension, contents[image.image_ref].ToString(), image.image_field, image.image_desc, (int)image.image_order, contents[image.image_ref]);

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
                }


            }

            return RedirectToAction("Index");
        }
    }
}