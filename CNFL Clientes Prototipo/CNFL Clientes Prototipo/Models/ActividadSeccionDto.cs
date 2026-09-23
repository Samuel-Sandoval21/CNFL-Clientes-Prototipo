using System;

namespace CNFL_Clientes_Prototipo.Models
{
    // ═══ Para la sección "Secciones más usadas" en Reportes ═══
    public class ActividadSeccionDto
    {
        public string Seccion { get; set; }
        public int MinutosTotales { get; set; }
        public int Sesiones { get; set; }
    }

    // ═══ Para el gráfico "Actividad últimos 6 meses" ═══
    public class ActividadMesDto
    {
        public string Etiqueta { get; set; }   // "Ene", "Feb", ...
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int MinutosTotales { get; set; }
        public int Sesiones { get; set; }
    }

    // ═══ NUEVO: para "Top 10 uso de la app" dentro de Reportes ═══
    public class TopUsuarioUsoDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int MinutosTotales { get; set; }
        public int Secciones { get; set; }
        public int Visitas { get; set; }
    }
}