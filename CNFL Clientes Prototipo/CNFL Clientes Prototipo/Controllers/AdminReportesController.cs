using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models.ViewModels;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminReportesController : Controller
    {
        // ⬇️ USA EL NOMBRE REAL DE TU WEB.CONFIG: CNFLDbContext
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["CNFLDbContext"].ConnectionString;

        // ═══════════════════════════════════════════
        // GET: AdminReportes/ReportesVentas
        // ═══════════════════════════════════════════
        public ActionResult ReportesVentas(string rango = "30d", string metodo = "todos", string tipo = "todos")
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Cuenta");

            DateTime fechaDesde, fechaHasta = DateTime.Now;
            switch (rango)
            {
                case "7d": fechaDesde = DateTime.Now.AddDays(-7); break;
                case "30d": fechaDesde = DateTime.Now.AddDays(-30); break;
                case "90d": fechaDesde = DateTime.Now.AddDays(-90); break;
                case "anio": fechaDesde = new DateTime(DateTime.Now.Year, 1, 1); break;
                default: fechaDesde = DateTime.Now.AddDays(-30); break;
            }

            var vm = new ReporteVentasViewModel
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                FiltroRapido = rango,
                MetodoPago = metodo,
                Tipo = tipo
            };

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // ═══ KPIs ═══
                string filtroMetodo = metodo == "todos" ? "" : " AND Metodo = @metodo ";
                string sqlKPI = @"
                    SELECT 
                        ISNULL(SUM(Total), 0) AS Ingresos,
                        COUNT(*) AS TotalCompras,
                        COUNT(DISTINCT UsuarioId) AS ClientesUnicos,
                        ISNULL(AVG(Total), 0) AS TicketPromedio
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada'
                      AND FechaCreacion >= @desde AND FechaCreacion <= @hasta
                      " + filtroMetodo;

                using (var cmd = new SqlCommand(sqlKPI, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                    if (metodo != "todos") cmd.Parameters.AddWithValue("@metodo", metodo);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            vm.IngresosTotales = Convert.ToDecimal(r["Ingresos"]);
                            vm.TotalCompras = Convert.ToInt32(r["TotalCompras"]);
                            vm.ClientesUnicos = Convert.ToInt32(r["ClientesUnicos"]);
                            vm.TicketPromedio = Convert.ToDecimal(r["TicketPromedio"]);
                        }
                    }
                }

                // ═══ Top Clientes ═══
                string sqlTopCli = @"
                    SELECT TOP 10 
                        u.UsuarioId, u.Nombre + ' ' + u.Apellidos AS Nombre, u.Correo,
                        COUNT(o.OrdenId) AS TotalCompras,
                        SUM(o.Total) AS TotalGastado
                    FROM OrdenesCompra o
                    INNER JOIN Usuarios u ON o.UsuarioId = u.UsuarioId
                    WHERE o.Estado = 'Pagada' AND o.FechaCreacion >= @desde AND o.FechaCreacion <= @hasta
                    GROUP BY u.UsuarioId, u.Nombre, u.Apellidos, u.Correo
                    ORDER BY TotalGastado DESC";

                using (var cmd = new SqlCommand(sqlTopCli, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            vm.TopClientes.Add(new TopCliente
                            {
                                UsuarioId = Convert.ToInt32(r["UsuarioId"]),
                                Nombre = r["Nombre"].ToString(),
                                Correo = r["Correo"].ToString(),
                                TotalCompras = Convert.ToInt32(r["TotalCompras"]),
                                TotalGastado = Convert.ToDecimal(r["TotalGastado"])
                            });
                        }
                    }
                }

                // ═══ Ventas por método ═══
                string sqlMetodos = @"
                    SELECT Metodo, COUNT(*) AS TotalCompras, SUM(Total) AS TotalIngresos
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada' AND FechaCreacion >= @desde AND FechaCreacion <= @hasta
                    GROUP BY Metodo
                    ORDER BY TotalIngresos DESC";

                using (var cmd = new SqlCommand(sqlMetodos, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var m = r["Metodo"].ToString();
                            var etiquetas = new Dictionary<string, string>
                            {
                                {"factura", "💡 Factura"},
                                {"tarjeta", "💳 Tarjeta"},
                                {"sinpe", "📱 SINPE"},
                                {"iban", "🏦 IBAN"},
                                {"token", "🔐 Token"}
                            };

                            vm.VentasPorMetodo.Add(new VentaPorMetodo
                            {
                                Metodo = m,
                                Etiqueta = etiquetas.ContainsKey(m) ? etiquetas[m] : m,
                                TotalCompras = Convert.ToInt32(r["TotalCompras"]),
                                TotalIngresos = Convert.ToDecimal(r["TotalIngresos"]),
                                Porcentaje = vm.IngresosTotales > 0
                                    ? (Convert.ToDecimal(r["TotalIngresos"]) / vm.IngresosTotales) * 100
                                    : 0
                            });
                        }
                    }
                }

                // ═══ Ventas por mes ═══
                string sqlMes = @"
                    SELECT 
                        YEAR(FechaCreacion) AS Anio,
                        MONTH(FechaCreacion) AS Mes,
                        COUNT(*) AS TotalCompras,
                        SUM(Total) AS TotalIngresos
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada'
                      AND FechaCreacion >= DATEADD(month, -6, GETDATE())
                    GROUP BY YEAR(FechaCreacion), MONTH(FechaCreacion)
                    ORDER BY Anio, Mes";

                var meses = new[] { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
                using (var cmd = new SqlCommand(sqlMes, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var mes = Convert.ToInt32(r["Mes"]);
                        vm.VentasPorMes.Add(new VentaPorMes
                        {
                            Anio = Convert.ToInt32(r["Anio"]),
                            Mes = mes,
                            Etiqueta = meses[mes],
                            TotalCompras = Convert.ToInt32(r["TotalCompras"]),
                            TotalIngresos = Convert.ToDecimal(r["TotalIngresos"])
                        });
                    }
                }

                // ═══ Lista de compras ═══
                string sqlCompras = @"
                    SELECT TOP 100
                        o.OrdenId, o.NumeroOrden, o.FechaCreacion, o.Subtotal, o.Impuesto, o.Total,
                        o.Metodo, o.Estado, o.Detalle,
                        u.Nombre + ' ' + u.Apellidos AS ClienteNombre, u.Correo AS ClienteCorreo
                    FROM OrdenesCompra o
                    INNER JOIN Usuarios u ON o.UsuarioId = u.UsuarioId
                    WHERE o.FechaCreacion >= @desde AND o.FechaCreacion <= @hasta
                      " + (metodo == "todos" ? "" : " AND o.Metodo = @metodo ") + @"
                    ORDER BY o.FechaCreacion DESC";

                using (var cmd = new SqlCommand(sqlCompras, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                    if (metodo != "todos") cmd.Parameters.AddWithValue("@metodo", metodo);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var detalle = r["Detalle"] != DBNull.Value ? r["Detalle"].ToString() : "";
                            var numItems = 0;
                            if (!string.IsNullOrEmpty(detalle))
                            {
                                try
                                {
                                    var doc = Newtonsoft.Json.Linq.JArray.Parse(detalle);
                                    foreach (var item in doc)
                                    {
                                        numItems += item["cantidad"] != null ? (int)item["cantidad"] : 1;
                                    }
                                }
                                catch { }
                            }

                            vm.Compras.Add(new CompraDetalle
                            {
                                CompraId = Convert.ToInt32(r["OrdenId"]),
                                Fecha = Convert.ToDateTime(r["FechaCreacion"]),
                                ClienteNombre = r["ClienteNombre"].ToString(),
                                ClienteCorreo = r["ClienteCorreo"].ToString(),
                                Subtotal = Convert.ToDecimal(r["Subtotal"]),
                                Impuesto = Convert.ToDecimal(r["Impuesto"]),
                                Total = Convert.ToDecimal(r["Total"]),
                                MetodoPago = r["Metodo"].ToString(),
                                Estado = r["Estado"].ToString(),
                                TotalItems = numItems,
                                ItemsResumen = detalle
                            });
                        }
                    }
                }
            }

            return View(vm);
        }

        // ═══════════════════════════════════════════
        // POST: AdminReportes/GuardarOrden
        // ═══════════════════════════════════════════
        [HttpPost]
        public JsonResult GuardarOrden(string detalleJson, decimal subtotal, decimal impuesto, decimal total, string metodo)
        {
            if (Session["UsuarioId"] == null)
                return Json(new { success = false, message = "No autenticado" });

            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

                using (var conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    string numeroOrden = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + usuarioId;

                    string sql = @"
                        INSERT INTO OrdenesCompra
                            (NumeroOrden, UsuarioId, Subtotal, Impuesto, Total, Metodo, Estado, FechaCreacion, Detalle)
                        OUTPUT INSERTED.OrdenId
                        VALUES (@num, @usr, @sub, @imp, @tot, @met, 'Pagada', GETDATE(), @det)";

                    int ordenId = 0;
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@num", numeroOrden);
                        cmd.Parameters.AddWithValue("@usr", usuarioId);
                        cmd.Parameters.AddWithValue("@sub", subtotal);
                        cmd.Parameters.AddWithValue("@imp", impuesto);
                        cmd.Parameters.AddWithValue("@tot", total);
                        cmd.Parameters.AddWithValue("@met", metodo);
                        cmd.Parameters.AddWithValue("@det", (object)detalleJson ?? DBNull.Value);

                        ordenId = (int)cmd.ExecuteScalar();
                    }

                    return Json(new { success = true, ordenId = ordenId, numeroOrden = numeroOrden });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}