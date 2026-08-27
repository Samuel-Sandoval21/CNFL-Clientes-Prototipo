namespace CNFL_Clientes_Prototipo.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public string Periodo { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } // "Pendiente" o "Pagada"
    }
}