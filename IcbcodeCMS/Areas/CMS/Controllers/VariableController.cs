using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin, icbcode_cms_content_manager")]
    public class VariableController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (UserRepository database = new UserRepository())
            {
                ViewBag.user = database.GetByLogin(User.Identity.Name);
            }

            using (VariableRepository variable_repository = new VariableRepository())
            {
                ViewBag.items = variable_repository.All(CurrentUser.user_domain);
            }

            return View();
        }

        [HttpGet()]
        public ActionResult Remove(long var_id)
        {
            using (VariableRepository variable_repository = new VariableRepository())
            {
                variable_repository.Remove(var_id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? var_id)
        {
            using (VariableRepository variable_repository = new VariableRepository())
            {
                ViewBag.groups = variable_repository.AllGroups(CurrentUser.user_domain);

                if (var_id.HasValue)
                {
                    ViewBag.item = variable_repository.GetByID(var_id.Value);
                }
            }

            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? var_id, string var_name, string var_friendly_name, string var_value, string var_type, string var_comment, string var_group, FormCollection form)
        {
            //
            bool file_is_change = false;

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName] as HttpPostedFileBase;

                if (file.ContentLength == 0 || file.FileName == "blob" || file.FileName == "var_value")
                {
                    continue;
                }

                file_is_change = true;

                // save in db

                long file_id; string file_desc = file.FileName; int file_size = file.ContentLength; string file_extension = Path.GetExtension(file.FileName);

                using (FileRepository database = new FileRepository())
                {
                    file_id = database.CreateGlobalID();

                    database.Save(file_id, file_extension, null, file.FileName, file.ContentLength, string.Empty);
                }

                // save in file system

                try
                {
                    file.SaveAs(Path.Combine(Server.MapPath("~/content/cms/files"), string.Format("{0}{1}", file_id, file_extension)));

                    var_value = string.Format("/content/cms/files/{0}{1}", file_id, file_extension);
                }
                catch
                {
                    using (FileRepository database = new FileRepository())
                    {
                        database.Remove(file_id);
                    }
                }
            }

            //

            var_group = var_group.Replace(",", string.Empty);

            using (VariableRepository variable_repository = new VariableRepository())
            {
                if (var_id.HasValue)
                {
                    var v = variable_repository.GetByID(var_id.Value);

                    variable_repository.Update(var_id.Value, var_name, var_friendly_name, (file_is_change && v.var_type == "Файл" ? var_value : (v.var_type == "Файл" ? v.var_value : var_value)), var_comment, var_group, CurrentUser.user_domain);
                }
                else
                {
                    var_id = variable_repository.CreateGlobalID();

                    string new_var_name = Transliterator.Translite(var_friendly_name);

                    if (variable_repository.Exists(new_var_name, CurrentUser.user_domain))
                        new_var_name = new_var_name + "-" + var_id.ToString();

                    variable_repository.Create(var_id.Value, new_var_name, var_friendly_name, var_value, var_type, var_comment, var_group, CurrentUser.user_domain);
                }
            }

            return RedirectToAction("Index");
        }
    }
}