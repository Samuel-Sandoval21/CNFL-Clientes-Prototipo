using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string CorreoSecundario { get; set; }
        public string Telefono { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Sexo { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public int? ActividadEconomicaId { get; set; }
        public virtual ActividadEconomica ActividadEconomica { get; set; }

        // Propiedades de navegación
        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; }
        public virtual ICollection<NISE> NISEs { get; set; }
        public virtual ICollection<Averia> Averias { get; set; }
        public virtual ICollection<Notificacion> Notificaciones { get; set; }
        public virtual ICollection<Tramite> Tramites { get; set; }
        public virtual ICollection<Suscripcion> Suscripciones { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}