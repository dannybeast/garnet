using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System;
using System.Web;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class CMSAuthorizeAttribute : AuthorizeAttribute
    {
        public new string Roles { set; get; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                dynamic user;

                using (UserRepository user_repository = new UserRepository())
                {
                    user = user_repository.GetByLogin(httpContext.User.Identity.Name);
                }

                httpContext.Session["CURRENT_USER"] = user;

                return user == null ? false : Roles.Contains(user.user_role_name);
            }

            return false;
        }
    }
}