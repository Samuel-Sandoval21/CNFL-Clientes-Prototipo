using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class NISE
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Numero { get; set; }
        public string Direccion { get; set; }
        public bool Activo { get; set; }

        // Propiedades de navegación
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<Factura> Facturas { get; set; }
        public virtual ICollection<Averia> Averias { get; set; }
        public virtual ICollection<Tramite> Tramites { get; set; }
    }
}