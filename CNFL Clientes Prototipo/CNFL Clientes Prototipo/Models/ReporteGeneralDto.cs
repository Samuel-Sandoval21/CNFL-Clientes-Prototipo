using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ReporteGeneralDto
    {
        public ClienteReporteDto Cliente { get; set; }
        public DatosComercialesDto Comerciales { get; set; }
        public DatosAMIDto AMI { get; set; }
        public List<ConsumoPeriodoDto> Historial { get; set; }
        public List<FacturaDto> FacturasPendientes { get; set; }
        public List<SuspensionDto> Suspensiones { get; set; }
        public ResumenReporteDto Resumen { get; set; }

        public ReporteGeneralDto()
        {
            Cliente = new ClienteReporteDto();
            Comerciales = new DatosComercialesDto();
            AMI = new DatosAMIDto();
            Historial = new List<ConsumoPeriodoDto>();
            FacturasPendientes = new List<FacturaDto>();
            Suspensiones = new List<SuspensionDto>();
            Resumen = new ResumenReporteDto();
        }
    }

    public class ClienteReporteDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string CorreoSecundario { get; set; }
        public string Telefono { get; set; }
        public bool CopiaCorreo { get; set; } = true;
        public string Consentimiento { get; set; } = "NF";
    }

    public class DatosComercialesDto
    {
        public string Servicio { get; set; } = "Activo";
        public string Contrato { get; set; } = "Activo";
        public string Negocio { get; set; } = "Activo";
        public string Plan { get; set; } = "RESIDENCIAL TR";
        public string Sucursal { get; set; } = "SUCURSAL HEREDIA";
        public DateTime FechaIngreso { get; set; }
        public decimal Deposito { get; set; }
        public string IdPersona { get; set; }
    }

    public class DatosAMIDto
    {
        public DateTime? UltimaFechaLectura { get; set; }
        public decimal? UltimaLectura { get; set; }
        public decimal? UltimaLecturaFacturada { get; set; }
        public decimal? ConsumoAcumulado { get; set; }
        public decimal? CostoEstimado { get; set; }
        public decimal? ConsumoPromedioKWh { get; set; }
        public bool TieneMedidorAMI { get; set; }
    }

    public class ConsumoPeriodoDto
    {
        public string Periodo { get; set; }
        public DateTime Vence { get; set; }
        public decimal ConsumoKWh { get; set; }
        public int VariacionPorcentaje { get; set; }
        public decimal Monto { get; set; }
        public string NumeroFactura { get; set; }
    }

    public class FacturaDto
    {
        public int FacturaId { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public bool Pagada { get; set; }
        public string Nise { get; set; }
    }

    public class SuspensionDto
    {
        public string Tipo { get; set; } = "Sin suspensiones";
        public DateTime? FechaProgramada { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; } = "Servicio Activo";
    }

    public class ResumenReporteDto
    {
        public int TotalFacturas { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal TotalPendiente { get; set; }
        public decimal PromedioMensualKWh { get; set; }
        public decimal PromedioMensualMonto { get; set; }
        public int MesesConConsumo { get; set; }
    }
}