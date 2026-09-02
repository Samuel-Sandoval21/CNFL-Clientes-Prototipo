using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Services
{
    public class TramiteService
    {
        // Simulación de Base de Datos en memoria
        private static List<Tramite> _tramites = new List<Tramite>
        {
            new Tramite
            {
                Id = 1,
                UsuarioId = 2,
                NISEId = 1,
                TipoTramite = "Cambio de nombre",
                Estado = "En Proceso",
                Detalle = "Solicitud de cambio de titular",
                FechaSolicitud = DateTime.Now.AddDays(-5)
            }
        };

        public List<Tramite> ObtenerTodos()
        {
            return _tramites;
        }

        public List<Tramite> ObtenerPorUsuario(int usuarioId)
        {
            return _tramites.Where(t => t.UsuarioId == usuarioId).ToList();
        }

        public void CrearTramite(string tipoTramite, string detalle, int usuarioId, int niseId)
        {
            _tramites.Add(new Tramite
            {
                Id = _tramites.Count + 1,
                UsuarioId = usuarioId,
                NISEId = niseId,
                TipoTramite = tipoTramite,
                Detalle = detalle,
                Estado = "En Proceso",
                FechaSolicitud = DateTime.Now
            });
        }

        public void ActualizarEstado(int id, string nuevoEstado)
        {
            var tramite = _tramites.FirstOrDefault(t => t.Id == id);
            if (tramite != null)
            {
                tramite.Estado = nuevoEstado;
            }
        }
    }
}