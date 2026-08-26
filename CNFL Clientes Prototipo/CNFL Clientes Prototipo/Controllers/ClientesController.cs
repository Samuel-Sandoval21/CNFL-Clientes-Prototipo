using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClienteService _service = new ClienteService();

        // GET: Clientes
        public ActionResult Index()
        {
            var clientes = _service.ListarClientes();
            return View(clientes);
        }
    }
}