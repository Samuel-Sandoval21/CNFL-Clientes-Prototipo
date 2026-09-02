using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public int NISEId { get; set; }
        public string NumeroFactura { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; } // Pendiente, Pagada, Vencida

        // Propiedades de navegación
        public virtual NISE NISE { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}