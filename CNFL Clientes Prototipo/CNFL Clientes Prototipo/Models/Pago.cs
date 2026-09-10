using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int FacturaId { get; set; }
        public int UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }              // "Tarjeta", "SINPE Móvil", "IBAN", "Tokens"
        public string ReferenciaExterna { get; set; }    // ID que devuelve la pasarela
        public string Estado { get; set; }              // "Pendiente", "Confirmado", "Rechazado"
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public string RawResponse { get; set; }         // JSON crudo de la pasarela (debug)

        // Navegación
        public virtual Factura Factura { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}