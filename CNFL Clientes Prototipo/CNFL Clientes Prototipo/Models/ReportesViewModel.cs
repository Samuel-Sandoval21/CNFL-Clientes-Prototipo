using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ReportesViewModel
    {
        public int TotalAverias { get; set; }
        public string TiempoPromedioResolucion { get; set; } = "0 hrs";
        public int TasaResolucion { get; set; } = 0;
        public int TrendAverias { get; set; } = 0;
        public int TrendTiempo { get; set; } = 0;
        public int TrendTasa { get; set; } = 0;
        public List<AveriaPorMes> AveriasPorMes { get; set; } = new List<AveriaPorMes>();
        public List<AveriaConTiempo> AveriasRecientes { get; set; } = new List<AveriaConTiempo>();
    }

    public class AveriaPorMes
    {
        public string Mes { get; set; }
        public int Cantidad { get; set; }
    }

    // NO HEREDA DE Averia - ES UN MODELO DE VISTA INDEPENDIENTE
    public class AveriaConTiempo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int NISEId { get; set; }
        public string TipoAveria { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }
        public string TiempoTranscurrido { get; set; }
    }
}