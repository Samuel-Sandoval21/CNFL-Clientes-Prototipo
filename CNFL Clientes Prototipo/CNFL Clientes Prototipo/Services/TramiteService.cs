using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Services
{
    public class TramiteService
    {
        // Simulación de Base de Datos en memoria
        private static List<Tramite> _tramites = new List<Tramite>()
        {
            new Tramite { Id = 1, Nombre = "Solicitud servicio nuevo monofásico", Descripcion = "#TR-20482 · NISE 4021", Estado = "En proceso", Fecha = System.DateTime.Now.AddDays(-2) },
        };

        public List<Tramite> ObtenerTodos()
        {
            return _tramites;
        }

        public void CrearTramite(string nombreTramite, FormularioTramite datos)
        {
            _tramites.Add(new Tramite
            {
                Id = _tramites.Count + 1,
                Nombre = nombreTramite,
                Descripcion = datos.NISE,
                Estado = "Iniciado", // Estado inicial según diagrama
                Fecha = System.DateTime.Now
            });
        }
    }
}