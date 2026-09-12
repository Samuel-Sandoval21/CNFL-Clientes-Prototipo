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

    /// <summary>
    /// Gráfico 1: Dona — Distribución de consumo por NISE
    /// </summary>
    public class DistribucionNiseDto
    {
        public string label { get; set; }
        public decimal value { get; set; }
        public string color { get; set; }
    }

    /// <summary>
    /// Gráfico 2: Línea — Evolución mensual del consumo
    /// </summary>
    public class ConsumoMensualDto
    {
        public string mes { get; set; }
        public decimal monto { get; set; }
        public double kwh { get; set; }
    }

    /// <summary>
    /// Gráfico 3: Barras apiladas — Actividad semanal en la app
    /// </summary>
    public class ActividadSemanalDto
    {
        public string dia { get; set; }
        public int facturas { get; set; }
        public int reportes { get; set; }
        public int tramites { get; set; }
        public int perfil { get; set; }
    }

    /// <summary>
    /// Gráfico 4: Barras horizontales — Secciones más visitadas
    /// </summary>
    public class SeccionTopDto
    {
        public string nombre { get; set; }
        public int visitas { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // DTOs de la TIENDA (Productos y Servicios)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Banners del carrusel en Productos y Servicios
    /// </summary>
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