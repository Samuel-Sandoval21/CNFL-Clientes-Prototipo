using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Averia
    {
        public int AveriaId { get; set; }
        public int UsuarioId { get; set; }
        public int NiseId { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; } // ✅ Ya coincide
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string FotoUrl { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual NISE NISE { get; set; }
    }
}