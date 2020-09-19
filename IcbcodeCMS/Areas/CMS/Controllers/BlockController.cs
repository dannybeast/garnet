using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer")]
    public class BlockController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (BlockRepository block_repository = new BlockRepository())
            {
                ViewBag.items = block_repository.All(CurrentUser.user_domain);
            }

            return View();
        }

        public ActionResult AddSeparate()
        {
            string block_friendly_name = "Разделитель";

            using (BlockRepository block_repository = new BlockRepository())
            {
                long block_id = block_repository.CreateGlobalID();

                string new_block_name = Transliterator.Translite(block_friendly_name);

                if (block_repository.Exists(new_block_name, CurrentUser.user_domain))
                    new_block_name = new_block_name + "-" + block_id.ToString();

                block_repository.Create(block_id, new_block_name, block_friendly_name, string.Empty, null, null, null, null, false, string.Empty, true, CurrentUser.user_domain);
            }

            return RedirectToAction("Index");
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (BlockRepository block_repository = new BlockRepository())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    block_repository.UpdateOrder(ids[i], orders[i]);
                }
            }

            return new EmptyResult();
        }

        [HttpGet()]
        public ActionResult Remove(long block_id)
        {
            using (BlockRepository block_repository = new BlockRepository())
            {
                block_repository.Remove(block_id);

                using (ContentRepository content_repository = new ContentRepository())
                {
                    foreach (var item in content_repository.GetByBlock(block_id))
                    {
                        ModelUtility.RemoveContent(item.content_id);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? block_id)
        {
            using (TemplateRepository template_repository = new TemplateRepository())
            {
                ViewBag.templates = template_repository.All(CurrentUser.user_domain);

                if (block_id.HasValue)
                {
                    using (BlockRepository block_repository = new BlockRepository())
                    {
                        ViewBag.item = block_repository.GetByID(block_id.Value);
                    }
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? block_id, string block_name, string block_friendly_name, long?[] block_templates, long? block_content_root, string block_area, string block_controller, string block_action, bool block_allow_sort, string block_order_fields)
        {
            using (BlockRepository block_repository = new BlockRepository())
            {
                if (block_id.HasValue)
                {
                    block_repository.Update(block_id.Value, block_name, block_friendly_name, (block_templates == null ? string.Empty : string.Join(",", block_templates)), block_content_root, block_area, block_controller, block_action, block_allow_sort, block_order_fields, CurrentUser.user_domain);
                }
                else
                {
                    block_id = block_repository.CreateGlobalID();

                    string new_block_name = Transliterator.Translite(block_friendly_name);

                    if (block_repository.Exists(new_block_name, CurrentUser.user_domain))
                        new_block_name = new_block_name + "-" + block_id.ToString();

                    block_repository.Create(block_id.Value, new_block_name, block_friendly_name, (block_templates == null ? string.Empty : string.Join(",", block_templates)), block_content_root, block_area, block_controller, block_action, block_allow_sort, block_order_fields, false, CurrentUser.user_domain);
                }
            }

            return RedirectToAction("Index");
        }
    }
}