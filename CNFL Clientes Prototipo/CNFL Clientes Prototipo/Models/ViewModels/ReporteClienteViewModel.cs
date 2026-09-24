using System;
using System.Collections.Generic;
using System.Globalization;

namespace CNFL_Clientes_Prototipo.Models.ViewModels
{
    // ═══════════════════════════════════════════════════════════
    // VIEWMODEL PRINCIPAL DEL REPORTE
    // ═══════════════════════════════════════════════════════════
    public class ReporteClienteViewModel
    {
        public DateTime Generado { get; set; }
        public string Rango { get; set; }
        public string RangoTexto { get; set; }
        public DateTime? Desde { get; set; }

        public int? ClienteId { get; set; }
        public bool HayCliente { get; set; }
        public List<KeyValuePair<int, string>> ClientesLista { get; set; }

        // Sistema
        public int SisClientesActivos { get; set; }
        public int SisClientesTotal { get; set; }
        public int SisNISEs { get; set; }
        public int SisFacturas { get; set; }
        public decimal SisFacturado { get; set; }
        public decimal SisCobrado { get; set; }
        public decimal SisPendiente { get; set; }
        public decimal SisPorcentajeCobro { get; set; }
        public int SisFacturasVencidas { get; set; }
        public int SisAveriasAbiertas { get; set; }
        public int SisTramitesAbiertos { get; set; }
        public int SisServiciosConDeuda { get; set; }
        public int SisServiciosAlDia { get; set; }
        public decimal SisIngresosVentas { get; set; }
        public int SisCompras { get; set; }
        public int SisCompradores { get; set; }
        public decimal SisTicketPromedio { get; set; }
        public List<RepPunto> SisMensual { get; set; }
        public List<RepDeudor> TopDeudores { get; set; }

        // Cliente
        public RepCliente Cliente { get; set; }
        public string EstadoCuenta { get; set; }
        public string EstadoCuentaClase { get; set; }

        public decimal Facturado { get; set; }
        public decimal Pagado { get; set; }
        public decimal Pendiente { get; set; }
        public decimal PorcentajePagado { get; set; }
        public int FacturasTotal { get; set; }
        public int FacturasPagadas { get; set; }
        public int FacturasPendientes { get; set; }
        public int FacturasVencidas { get; set; }
        public decimal SaldoPendienteTotal { get; set; }
        public decimal MontoVencido { get; set; }

        public List<RepFactura> Facturas { get; set; }
        public List<RepPunto> FacturacionMensual { get; set; }
        public List<RepServicio> Servicios { get; set; }

        // Averías
        public List<RepAveria> Averias { get; set; }
        public int AveriasTotal { get; set; }
        public int AveriasAbiertas { get; set; }
        public int AveriasResueltas { get; set; }
        public double AveriasDiasPromedio { get; set; }
        public List<RepGrupo> AveriasPorEstado { get; set; }

        // Trámites
        public List<RepTramite> Tramites { get; set; }
        public int TramitesTotal { get; set; }
        public int TramitesAbiertos { get; set; }
        public int TramitesCompletados { get; set; }
        public List<RepGrupo> TramitesPorEstado { get; set; }

        // Compras
        public List<RepCompra> Compras { get; set; }
        public int ComprasPagadas { get; set; }
        public decimal ComprasMonto { get; set; }
        public decimal ComprasTicket { get; set; }
        public List<RepGrupo> ComprasPorMetodo { get; set; }

        // Actividad
        public List<RepActividad> Actividad { get; set; }
        public int ActSesiones { get; set; }
        public decimal ActMinutos { get; set; }
        public decimal ActPromedioMin { get; set; }
        public DateTime? ActUltima { get; set; }

        public ReporteClienteViewModel()
        {
            ClientesLista = new List<KeyValuePair<int, string>>();
            SisMensual = new List<RepPunto>();
            TopDeudores = new List<RepDeudor>();
            Cliente = new RepCliente();
            Facturas = new List<RepFactura>();
            FacturacionMensual = new List<RepPunto>();
            Servicios = new List<RepServicio>();
            Averias = new List<RepAveria>();
            AveriasPorEstado = new List<RepGrupo>();
            Tramites = new List<RepTramite>();
            TramitesPorEstado = new List<RepGrupo>();
            Compras = new List<RepCompra>();
            ComprasPorMetodo = new List<RepGrupo>();
            Actividad = new List<RepActividad>();
        }
    }

    // ═══════════════════════════════════════════════════════════
    // SUB-DTOs
    // ═══════════════════════════════════════════════════════════
    public class RepCliente
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string CorreoSecundario { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public bool FacturaElectronica { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }

    public class RepFactura
    {
        public string Numero { get; set; }
        public string Nise { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Vencimiento { get; set; }
        public decimal Monto { get; set; }
        public bool Pagada { get; set; }
        public string Estado { get; set; }
        public int DiasAtraso { get; set; }
    }

    public class RepServicio
    {
        public string Nise { get; set; }
        public string Tipo { get; set; }
        public string Provincia { get; set; }
        public string Direccion { get; set; }
        public int Facturas { get; set; }
        public decimal Pendiente { get; set; }
        public string Estado { get; set; }
    }

    public class RepAveria
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime? Reportada { get; set; }
        public DateTime? Actualizada { get; set; }
        public double? DiasResolucion { get; set; }
    }

    public class RepTramite
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime? Solicitud { get; set; }
        public DateTime? Actualizacion { get; set; }
    }

    public class RepCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; }
        public string Estado { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string Detalle { get; set; }
        public int Articulos { get; set; }
    }

    public class RepActividad
    {
        public string Seccion { get; set; }
        public decimal Minutos { get; set; }
        public int Sesiones { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class RepPunto
    {
        public string Etiqueta { get; set; }
        public decimal Valor { get; set; }
        public decimal Valor2 { get; set; }
    }

    public class RepGrupo
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
    }

    public class RepDeudor
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public int Facturas { get; set; }
        public decimal Monto { get; set; }
    }

    public class RepVentasSistema
    {
        public decimal Ingresos { get; set; }
        public int Compras { get; set; }
        public int Compradores { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // HELPERS DE FORMATO COMPARTIDOS
    // ═══════════════════════════════════════════════════════════
    public static class RepFmt
    {
        public static readonly CultureInfo Cult = new CultureInfo("es-CR");

        public static string Numero(decimal valor, int decimales = 0)
        {
            return valor.ToString(decimales > 0 ? "N" + decimales : "N0", Cult);
        }

        public static string Pct(decimal valor)
        {
            return valor.ToString("0.0", Cult) + "%";
        }

        public static string Fecha(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy", Cult) : "—";
        }

        public static string FechaHora(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy HH:mm", Cult) : "—";
        }

        // ═══ Nombres largos ═══
        public static string Moneda(decimal valor)
        {
            return "₡" + valor.ToString("N0", Cult);
        }

        public static string Moneda2(decimal valor)
        {
            return "₡" + valor.ToString("N2", Cult);
        }

        // ═══ Alias cortos ═══
        public static string Mon(decimal valor)
        {
            return Moneda(valor);
        }

        public static string Mon2(decimal valor)
        {
            return Moneda2(valor);
        }

        // ═══ NUEVO: Para usar en CSS (width:X%) siempre con punto decimal y sin símbolo % ═══
        public static string Css(decimal valor)
        {
            // En CSS siempre se usa punto como separador decimal, no coma.
            // Usamos InvariantCulture y limitamos a 2 decimales.
            return valor.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public static string Clase(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "gray";
            var e = estado.Trim().ToLowerInvariant();

            if (e.Contains("pagada") || e.Contains("resuelta") || e.Contains("cerrada")
                || e.Contains("completado") || e.Contains("aprobado") || e.Contains("activo")
                || e.Contains("al día") || e.Contains("al dia") || e.Contains("finalizada"))
                return "green";

            if (e.Contains("vencida") || e.Contains("suspendido") || e.Contains("rechazado")
                || e.Contains("inactivo") || e.Contains("error") || e.Contains("cancelada"))
                return "red";

            if (e.Contains("pendiente") || e.Contains("proceso") || e.Contains("revisión")
                || e.Contains("revision") || e.Contains("iniciado") || e.Contains("solicitado")
                || e.Contains("requiere"))
                return "amber";

            return "blue";
        }
    }
}