using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
