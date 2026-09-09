using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Suspension
    {
        public int SuspensionId { get; set; }
        public int NiseId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }

        public virtual NISE NISE { get; set; }
    }
}