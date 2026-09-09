using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class NISE
    {
        public int NiseId { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroNise { get; set; }
        public string Direccion { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string TipoServicio { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Factura> Facturas { get; set; }
        public virtual ICollection<Averia> Averias { get; set; }
        public virtual ICollection<Suspension> Suspensiones { get; set; }
        public virtual ICollection<Tramite> Tramites { get; set; }
    }
}