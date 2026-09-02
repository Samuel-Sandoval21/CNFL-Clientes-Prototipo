using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public int UsuarioId { get; set; }
        public string MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Transaccion { get; set; }
        public string Autorizacion { get; set; }
        public string Estado { get; set; } // Completado, Pendiente, Fallido

        // Propiedades de navegación
        public virtual Factura Factura { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}