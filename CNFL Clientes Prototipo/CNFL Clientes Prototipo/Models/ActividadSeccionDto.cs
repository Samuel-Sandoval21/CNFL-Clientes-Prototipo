using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ActividadSeccionDto
    {
        public string Seccion { get; set; }
        public int MinutosTotales { get; set; }
        public int Sesiones { get; set; }
    }

    public class ActividadMesDto
    {
        public string Etiqueta { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int MinutosTotales { get; set; }
        public int Sesiones { get; set; }
    }

    public class TopUsuarioUsoDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int MinutosTotales { get; set; }
        public int Secciones { get; set; }
        public int Visitas { get; set; }
    }

    public class ActividadSemanalDto
    {
        public string dia { get; set; }
        public int facturas { get; set; }
        public int reportes { get; set; }
        public int tramites { get; set; }
        public int perfil { get; set; }
    }
}