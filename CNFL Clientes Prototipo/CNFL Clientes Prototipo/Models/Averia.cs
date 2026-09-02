using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNFL_Clientes_Prototipo.Models
{
    public class Averia
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int NISEId { get; set; }
        public string TipoAveria { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual NISE NISE { get; set; }
    }
}