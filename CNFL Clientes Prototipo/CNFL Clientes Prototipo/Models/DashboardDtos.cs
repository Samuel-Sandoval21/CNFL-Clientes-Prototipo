using System;

namespace CNFL_Clientes_Prototipo.Models
{
    // ═══════════════════════════════════════════════════════════
    // DTOs del DASHBOARD (Inicio)
    // ═══════════════════════════════════════════════════════════

    public class AveriaResumenDto
    {
        public int AveriaId { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }
    }

    public class TramiteResumenDto
    {
        public int TramiteId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string NumeroReferencia { get; set; }
        public string Descripcion { get; set; }
    }

    public class NotificacionResumenDto
    {
        public int NotificacionId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // DTOs para los GRÁFICOS del Dashboard
    // ═══════════════════════════════════════════════════════════

    public class DistribucionNiseDto
    {
        public string label { get; set; }
        public decimal value { get; set; }
        public string color { get; set; }
    }

    public class ConsumoMensualDto
    {
        public string mes { get; set; }
        public decimal monto { get; set; }
        public double kwh { get; set; }
    }

    public class ActividadSemanalDto
    {
        public string dia { get; set; }
        public int facturas { get; set; }
        public int reportes { get; set; }
        public int tramites { get; set; }
        public int perfil { get; set; }
    }

    public class SeccionTopDto
    {
        public string nombre { get; set; }
        public int visitas { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // DTOs de la TIENDA
    // ═══════════════════════════════════════════════════════════

    public class BannerDto
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Icono { get; set; }
        public string CategoriaId { get; set; }
        public string ColorInicio { get; set; }
        public string ColorFin { get; set; }
    }
}