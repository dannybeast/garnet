using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer")]
    public class ViewController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                ViewBag.items = view_repository.All(CurrentUser.user_domain);
            }

            return View();
        }

        [HttpPost()]
        public ActionResult UpdateOrder(long[] ids, long[] orders)
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    view_repository.UpdateOrder(ids[i], orders[i]);
                }
            }

            return new EmptyResult();
        }

        [HttpGet()]
        public ActionResult Remove(long view_id)
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                view_repository.Remove(view_id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? view_id)
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                if (view_id.HasValue)
                {
                    ViewBag.item = view_repository.GetByID(view_id.Value);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? view_id, string view_friendly_name, string view_name, string view_path)
        {
            using (ViewRepository view_repository = new ViewRepository())
            {
                if (view_id.HasValue)
                {
                    view_repository.Update(view_id.Value, view_friendly_name, view_name, view_path, CurrentUser.user_domain);
                }
                else
                {
                    view_id = view_repository.CreateGlobalID();

                    string new_view_name = Transliterator.Translite(view_friendly_name);

                    if (view_repository.Exists(new_view_name, CurrentUser.user_domain))
                        new_view_name = new_view_name + "-" + view_id.ToString();

                    view_repository.Create(view_id.Value, view_friendly_name, new_view_name, view_path, CurrentUser.user_domain);
                }
            }

            return RedirectToAction("Index");
        }
    }
}