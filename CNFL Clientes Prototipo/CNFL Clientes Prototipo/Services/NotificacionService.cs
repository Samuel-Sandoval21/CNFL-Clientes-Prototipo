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

        public List<Notificacion> Listar()
        {
            return _repo.ObtenerTodas();
        }

        public List<Notificacion> ListarPorUsuario(int usuarioId)
        {
            return _repo.ObtenerPorUsuario(usuarioId);
        }

        public Notificacion ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public void Agregar(Notificacion notificacion)
        {
            _repo.Agregar(notificacion);
        }

        public void MarcarComoLeida(int id)
        {
            _repo.MarcarComoLeida(id);
        }

        public void MarcarTodasComoLeidas(int usuarioId)
        {
            _repo.MarcarTodasComoLeidas(usuarioId);
        }
    }
}