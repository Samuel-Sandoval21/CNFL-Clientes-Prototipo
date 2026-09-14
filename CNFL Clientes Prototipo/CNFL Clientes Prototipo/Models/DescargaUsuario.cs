using System;

namespace CNFL_Clientes_Prototipo.Models
{
    /// <summary>
    /// Registro de archivos descargados por el usuario (PDF, Excel, CSV).
    /// Tabla: DescargasUsuario
    /// </summary>
    public class DescargaUsuario
    {
        public int DescargaId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }      // PDF, Excel, CSV
        public string Seccion { get; set; }
        public int? TamanoKB { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}