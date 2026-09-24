using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models.ViewModels
{
    public class ReporteVentasViewModel
    {
        public string FiltroRapido { get; set; } = "30d";
        public string MetodoPago { get; set; } = "todos";
        public string Tipo { get; set; } = "todos";
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public decimal IngresosTotales { get; set; }
        public int TotalCompras { get; set; }
        public int ClientesUnicos { get; set; }
        public decimal TicketPromedio { get; set; }

        public List<TopCliente> TopClientes { get; set; }
        public List<VentaPorMetodo> VentasPorMetodo { get; set; }
        public List<VentaPorMes> VentasPorMes { get; set; }
        public List<CompraDetalle> Compras { get; set; }

        public ReporteVentasViewModel()
        {
            TopClientes = new List<TopCliente>();
            VentasPorMetodo = new List<VentaPorMetodo>();
            VentasPorMes = new List<VentaPorMes>();
            Compras = new List<CompraDetalle>();
            FechaDesde = DateTime.Now.AddDays(-30);
            FechaHasta = DateTime.Now;
        }
    }

    public class TopCliente
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalGastado { get; set; }
    }

    public class VentaPorMetodo
    {
        public string Metodo { get; set; }
        public string Etiqueta { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class VentaPorMes
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string Etiqueta { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class CompraDetalle
    {
        public int CompraId { get; set; }
        public DateTime Fecha { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteCorreo { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public int TotalItems { get; set; }
        public string ItemsResumen { get; set; }
    }
}