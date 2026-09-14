using System;

namespace CNFL_Clientes_Prototipo.Models
{
    /// <summary>
    /// Registro de actividad del usuario dentro de la app.
    /// Tabla: ActividadUsuario
    /// </summary>
    public class ActividadUsuario
    {
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public string Seccion { get; set; }
        public string Accion { get; set; }
        public string Detalle { get; set; }
        public int? DuracionSegundos { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}