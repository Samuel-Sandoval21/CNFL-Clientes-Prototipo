using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Repositories;

namespace CNFL_Clientes_Prototipo.Services
{
    public class NotificacionService
    {
        private readonly NotificacionRepository _repo = new NotificacionRepository();
        public List<Notificacion> Listar() => _repo.ObtenerTodas();
        public void LeerTodas() => _repo.MarcarComoLeidas();
    }
}