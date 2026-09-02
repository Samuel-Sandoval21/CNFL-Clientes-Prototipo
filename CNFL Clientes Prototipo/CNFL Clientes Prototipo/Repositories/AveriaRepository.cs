using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class AveriaRepository
    {
        private static List<Averia> _averias = new List<Averia>()
        {
            new Averia {
                Id = 1,
                UsuarioId = 2,
                NISEId = 1,
                TipoAveria = "Eléctrica",
                Descripcion = "Se escuchan explosiones y no hay luz",
                Direccion = "Barrio Los Ángeles, Cartago",
                Estado = "En Proceso",
                FechaReporte = System.DateTime.Now.AddHours(-2),
                Latitud = null,
                Longitud = null
            },
            new Averia {
                Id = 2,
                UsuarioId = 2,
                NISEId = 1,
                TipoAveria = "Alumbrado Público",
                Descripcion = "Poste de luz sin funcionar en toda la calle",
                Direccion = "San José, La Uruca",
                Estado = "Pendiente",
                FechaReporte = System.DateTime.Now.AddHours(-5),
                Latitud = null,
                Longitud = null
            },
            new Averia {
                Id = 3,
                UsuarioId = 3,
                NISEId = 2,
                TipoAveria = "Ajena",
                Descripcion = "Árbol caído sobre cables eléctricos",
                Direccion = "Heredia, San Pablo",
                Estado = "Resuelto",
                FechaReporte = System.DateTime.Now.AddDays(-1),
                Latitud = null,
                Longitud = null
            }
        };

        public List<Averia> ObtenerTodas()
        {
            return _averias.OrderByDescending(a => a.FechaReporte).ToList();
        }

        public void Agregar(Averia averia)
        {
            averia.Id = _averias.Count > 0 ? _averias.Max(a => a.Id) + 1 : 1;
            averia.FechaReporte = System.DateTime.Now;
            averia.Estado = "Pendiente";
            _averias.Add(averia);
        }

        public void ActualizarEstado(int id, string nuevoEstado)
        {
            var averia = _averias.FirstOrDefault(a => a.Id == id);
            if (averia != null)
            {
                averia.Estado = nuevoEstado;
            }
        }

        public Averia ObtenerPorId(int id)
        {
            return _averias.FirstOrDefault(a => a.Id == id);
        }

        public void Eliminar(int id)
        {
            var averia = _averias.FirstOrDefault(a => a.Id == id);
            if (averia != null)
            {
                _averias.Remove(averia);
            }
        }

        public List<Averia> ObtenerPorEstado(string estado)
        {
            return _averias.Where(a => a.Estado == estado).ToList();
        }
    }
}