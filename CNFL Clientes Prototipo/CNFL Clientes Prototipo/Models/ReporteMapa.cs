using System;

namespace CNFL_Clientes_Prototipo.Models
{
    /// <summary>
    /// DTO ligero para representar una avería en el mapa.
    /// No expone toda la entidad Averia por seguridad y rendimiento.
    /// </summary>
    public class ReporteMapa
    {
        public int AveriaId { get; set; }
        public string Titulo { get; set; }        // "Avería #10234"
        public string Tipo { get; set; }          // "Poste caído"
        public string Estado { get; set; }        // "En revisión", "Resuelto", etc.
        public string Direccion { get; set; }     // Dirección textual
        public string NiseNumero { get; set; }    // Número del NISE asociado
        public double Latitud { get; set; }       // Coordenada
        public double Longitud { get; set; }      // Coordenada
        public DateTime FechaReporte { get; set; }
    }
}