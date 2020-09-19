using IcbcodeCMS.Areas.CMS.Models.Repositories;
using System.Collections.Specialized;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    public static class IcbcodeVariable
    {
        public static NameValueCollection All(string domain)
        {
            NameValueCollection variables = new NameValueCollection();

            using (VariableRepository variable_repository = new VariableRepository())
            {
                foreach (var item in variable_repository.All(domain))
                {
                    variables.Add(item.var_name, item.var_value);
                }
            }

            return variables;
        }
    }
}