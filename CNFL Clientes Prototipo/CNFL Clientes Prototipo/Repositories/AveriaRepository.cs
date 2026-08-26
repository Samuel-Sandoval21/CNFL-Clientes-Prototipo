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
            new Averia { Id = 1, Titulo = "Transformador dañado", Descripcion = "Se escuchan explosiones y no hay luz", Direccion = "Barrio Los Ángeles, Cartago", Estado = "En revisión", Fecha = System.DateTime.Now.AddHours(-2), Tipo = "Eléctrica" }
        };

        public List<Averia> ObtenerTodas()
        {
            return _averias;
        }

        public void Agregar(Averia averia)
        {
            averia.Id = _averias.Max(a => a.Id) + 1;
            averia.Fecha = System.DateTime.Now;
            averia.Estado = "Reportado";
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
    }
}