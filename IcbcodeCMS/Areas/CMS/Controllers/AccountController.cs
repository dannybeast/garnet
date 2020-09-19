using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System.Web.Mvc;
using System.Web.Security;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet()]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost()]
        public ActionResult SignIn(string login, string password, string returnUrl)
        {
            dynamic user;

            using (UserRepository user_repository = new UserRepository())
            {
                user = user_repository.GetByLogin(login);
            }

            if (user == null || user.user_password != password)
            {
                return View();
            }
            else
            {
                FormsAuthentication.SetAuthCookie(user.user_login, false);

                return Redirect(string.IsNullOrEmpty(returnUrl) ? "~/cms" : returnUrl);
            }
        }

        [HttpGet()]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}