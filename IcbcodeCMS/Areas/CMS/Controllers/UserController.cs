using IcbcodeCMS.Areas.CMS.Models.Repositories;
using IcbcodeCMS.Areas.CMS.Utilities.Attributes;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    [CMSAuthorize(Roles = "icbcode_cms_developer, icbcode_cms_admin")]
    public class UserController : BaseController
    {
        [HttpGet()]
        public ActionResult Index()
        {
            using (UserRepository user_repository = new UserRepository())
            {
                ViewBag.items = user_repository.All();
            }

            return View();
        }

        [HttpGet()]
        public ActionResult Remove(long user_id)
        {
            using (UserRepository user_repository = new UserRepository())
            {
                user_repository.Remove(user_id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public ActionResult Edit(long? user_id, long? user_domain)
        {
            using (UserRepository user_repository = new UserRepository())
            {
                ViewBag.user_roles = user_repository.AllUserRoles();

                if (user_id.HasValue)
                {
                    ViewBag.item = user_repository.GetByID(user_id.Value);
                }
            }

            using (DomainRepository domain_repository = new DomainRepository())
            {
                ViewBag.domains = domain_repository.All();
            }

            if (user_id.HasValue && !user_domain.HasValue)
            {
                ViewBag.current_domain_id = ViewBag.item.user_domain;
            }
            else if (user_domain.HasValue)
            {
                ViewBag.current_domain_id = user_domain;
            }
            else
            {
                ViewBag.current_domain_id = ViewBag.domains[0].domain_id;
            }

            using (BlockRepository block_repository = new BlockRepository())
            {
                ViewBag.blocks = block_repository.All(user_domain ?? ViewBag.current_domain_id);
            }

            using (VariableRepository variable_repository = new VariableRepository())
            {
                ViewBag.var_groups = variable_repository.AllGroups(user_domain ?? ViewBag.current_domain_id);
            }



            return View();
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Edit(long? user_id, long user_role, string user_name, string user_login, string user_password, long[] user_blocks, string[] user_var_groups, long user_domain)
        {
            using (UserRepository user_repository = new UserRepository())
            {
                if (user_id.HasValue)
                {
                    user_repository.Update(user_id.Value, user_role, user_name, user_login, user_password, (user_blocks == null ? string.Empty : string.Join(",", user_blocks)), (user_var_groups == null ? string.Empty : string.Join(",", user_var_groups)), user_domain);
                }
                else
                {
                    user_id = user_repository.CreateGlobalID();

                    string new_user_login = user_login;

                    if (user_repository.Exists(user_login))
                        new_user_login = user_login + "-" + user_id.ToString();

                    user_repository.Create(user_id.Value, user_role, user_name, new_user_login, user_password, (user_blocks == null ? string.Empty : string.Join(",", user_blocks)), (user_var_groups == null ? string.Empty : string.Join(",", user_var_groups)), user_domain);
                }
            }

            return RedirectToAction("Index");
        }
    }
}