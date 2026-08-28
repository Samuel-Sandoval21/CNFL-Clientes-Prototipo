using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ReportesViewModel
    {
        // Tarjetas de resumen
        public int TotalAverias { get; set; }
        public string TiempoPromedioResolucion { get; set; } = "0 hrs";
        public int TasaResolucion { get; set; } = 0;

        // Tendencias (variación porcentual)
        public int TrendAverias { get; set; } = 0;
        public int TrendTiempo { get; set; } = 0;
        public int TrendTasa { get; set; } = 0;

        // Datos para el gráfico de barras
        public List<AveriaPorMes> AveriasPorMes { get; set; } = new List<AveriaPorMes>();

        // Averías recientes con tiempo transcurrido
        public List<AveriaConTiempo> AveriasRecientes { get; set; } = new List<AveriaConTiempo>();
    }

    public class AveriaPorMes
    {
        public string Mes { get; set; }
        public int Cantidad { get; set; }
    }

    public class AveriaConTiempo : Averia
    {
        public string TiempoTranscurrido { get; set; }
    }
}