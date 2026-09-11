using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Averia
    {
        public int AveriaId { get; set; }
        public int UsuarioId { get; set; }
        public int NiseId { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string FotoUrl { get; set; }
        public string Direccion { get; set; }

        // 👇 NUEVAS: coordenadas para el mapa real
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual NISE NISE { get; set; }
    }
}