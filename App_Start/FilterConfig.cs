using System.Web;
using System.Web.Mvc;

namespace Proyecto1_Paula_Ulate
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
