using System;
using System.Collections.Generic;


namespace CNFL_Clientes_Prototipo.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Correo { get; set; }
        public string CorreoSecundario { get; set; }
        public string Sexo { get; set; }
        public string SexoPersonalizado { get; set; }
        public string Direccion { get; set; }
        public string NISE { get; set; }
        public string UserName { get; set; }
        public string Contraseña { get; set; }
        public int RolId { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public bool AceptaPolitica { get; set; }
        public bool AceptaConsentimiento { get; set; }

        // ==========================================================
        // NUEVAS PROPIEDADES
        // ==========================================================
        public bool? FacturaElectronica { get; set; }
        public string ActividadEconomicaCodigo { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }

        // Propiedades de navegación
        public virtual Rol RolNavigation { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<Notificacion> Notificaciones { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
        public virtual ICollection<Averia> Averias { get; set; }
        public virtual ICollection<Tramite> Tramites { get; set; }
        public virtual ICollection<Suscripcion> Suscripciones { get; set; }
    }
}