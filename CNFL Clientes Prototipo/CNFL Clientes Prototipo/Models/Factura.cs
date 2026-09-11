using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Factura
    {
        public int FacturaId { get; set; }
        public int NiseId { get; set; }
        public string NumeroFactura { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Pagada { get; set; }
        public string Descripcion { get; set; }

        // Propiedades de navegación
        public virtual NISE NISE { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}