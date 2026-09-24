using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ClienteAdminDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int TotalNISEs { get; set; }
        public int FacturasPendientes { get; set; }
        public decimal MontoPendiente { get; set; }
        public int AveriasActivas { get; set; }
    }
}