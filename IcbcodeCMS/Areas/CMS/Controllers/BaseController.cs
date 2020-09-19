using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System.Web.Mvc;

namespace IcbcodeCMS.Areas.CMS.Controllers
{
    public class BaseController : Controller
    {
        private dynamic _current_user = null;

        public dynamic CurrentUser
        {
            get
            {
                if (_current_user == null)
                {
                    using (UserRepository database = new UserRepository())
                    {
                        _current_user = database.GetByLogin(User.Identity.Name);
                    }
                }

                return _current_user;
            }
        }
    }
}