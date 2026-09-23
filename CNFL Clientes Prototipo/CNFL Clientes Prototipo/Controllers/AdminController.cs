using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;
using ClosedXML.Excel;

// iTextSharp
using iTextSharp.text;
using iTextSharp.text.pdf;

// System.Drawing para gráfico
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ═══════════════════════════════════════════════════════════
        // VALIDACIÓN DE SESIÓN ADMIN
        // ═══════════════════════════════════════════════════════════
        private bool EsAdmin()
        {
            return Session["AdminLogueado"] != null && (bool)Session["AdminLogueado"];
        }

        private ActionResult RedirigirSiNoEsAdmin()
        {
            if (!EsAdmin())
                return RedirectToAction("Login", "Cuenta");
            return null;
        }

        // ═══════════════════════════════════════════════════════════
        // DASHBOARD
        // ═══════════════════════════════════════════════════════════
        public ActionResult Dashboard()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            ViewBag.TotalClientes = _db.Usuarios.Count();
            ViewBag.ClientesActivos = _db.Usuarios.Count(u => u.Activo);
            ViewBag.TotalNISEs = _db.NISEs.Count();
            ViewBag.NISEsActivos = _db.NISEs.Count();

            var facturasPendientes = _db.Facturas.Where(f => !f.Pagada);
            ViewBag.FacturasPendientes = facturasPendientes.Count();
            ViewBag.MontoPendiente = facturasPendientes
                .Select(f => (decimal?)f.Monto)
                .Sum() ?? 0m;

            ViewBag.IngresosMes = _db.Facturas
                .Where(f => f.Pagada && f.FechaEmision >= inicioMes)
                .Select(f => (decimal?)f.Monto)
                .Sum() ?? 0m;

            ViewBag.AveriasAbiertas = _db.Averias.Count(a =>
                a.Estado != "Resuelta" && a.Estado != "Cerrada");
            ViewBag.AveriasResueltas = _db.Averias.Count(a =>
                a.Estado == "Resuelta" || a.Estado == "Cerrada");

            ViewBag.TramitesAbiertos = _db.Tramites.Count(t =>
                t.Estado != "Completado" && t.Estado != "Cerrado");

            var averiasResueltas = _db.Averias
                .Where(a => (a.Estado == "Resuelta" || a.Estado == "Cerrada")
                            && a.FechaActualizacion != null)
                .ToList();

            double tiempoPromedio = 0;
            if (averiasResueltas.Any())
            {
                tiempoPromedio = averiasResueltas
                    .Average(a => ((DateTime)a.FechaActualizacion - a.FechaReporte).TotalDays);
            }
            ViewBag.TiempoPromedio = Math.Round(tiempoPromedio, 1);

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // ALERTAS
        // ═══════════════════════════════════════════════════════════
        public ActionResult Alertas()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // CLIENTES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Clientes(string q = "")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(u =>
                    u.Nombre.Contains(q) ||
                    u.Apellidos.Contains(q) ||
                    u.Cedula.Contains(q) ||
                    u.Correo.Contains(q));
            }

            var lista = query
                .OrderBy(u => u.Nombre)
                .Select(u => new ClienteAdminDto
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Cedula = u.Cedula,
                    Correo = u.Correo,
                    Telefono = u.Telefono,
                    Activo = u.Activo,
                    FechaRegistro = u.FechaRegistro,
                    TotalNISEs = _db.NISEs.Count(n => n.UsuarioId == u.UsuarioId),
                    FacturasPendientes = _db.Facturas.Count(f =>
                        f.NISE.UsuarioId == u.UsuarioId && !f.Pagada),
                    MontoPendiente = _db.Facturas
                        .Where(f => f.NISE.UsuarioId == u.UsuarioId && !f.Pagada)
                        .Select(f => (decimal?)f.Monto)
                        .Sum() ?? 0m,
                    AveriasActivas = _db.Averias.Count(a =>
                        a.UsuarioId == u.UsuarioId &&
                        a.Estado != "Resuelta" &&
                        a.Estado != "Cerrada")
                })
                .ToList();

            ViewBag.Busqueda = q;
            return View(lista);
        }

        // ═══════════════════════════════════════════════════════════
        // DETALLE CLIENTE
        // ═══════════════════════════════════════════════════════════
        public ActionResult DetalleCliente(int id)
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var usuario = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == id);
            if (usuario == null) return HttpNotFound();

            ViewBag.NISEs = _db.NISEs.Where(n => n.UsuarioId == id).ToList();

            ViewBag.Facturas = _db.Facturas
                .Where(f => f.NISE.UsuarioId == id)
                .OrderByDescending(f => f.FechaEmision)
                .Take(20)
                .ToList();

            ViewBag.Averias = _db.Averias
                .Where(a => a.UsuarioId == id)
                .OrderByDescending(a => a.FechaReporte)
                .Take(20)
                .ToList();

            ViewBag.Tramites = _db.Tramites
                .Where(t => t.UsuarioId == id)
                .OrderByDescending(t => t.FechaSolicitud)
                .Take(20)
                .ToList();

            return View(usuario);
        }

        // ═══════════════════════════════════════════════════════════
        // AVERÍAS
        // ═══════════════════════════════════════════════════════════
        public ActionResult Averias(string estado = "")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var query = _db.Averias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(a => a.Estado == estado);

            var lista = query
                .OrderByDescending(a => a.FechaReporte)
                .Take(100)
                .ToList();

            ViewBag.Estado = estado;
            return View(lista);
        }

        // ═══════════════════════════════════════════════════════════
        // TRÁMITES (LISTA)
        // ═══════════════════════════════════════════════════════════
        public ActionResult Tramites(string estado = "")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var query = _db.Tramites
                .Include("Usuario")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(t => t.Estado == estado);

            var lista = query
                .OrderByDescending(t => t.FechaSolicitud)
                .Take(100)
                .ToList();

            ViewBag.Estado = estado;
            ViewBag.TotalSolicitados = _db.Tramites.Count(t => t.Estado == "Iniciado" || t.Estado == "Solicitado");
            ViewBag.TotalProceso = _db.Tramites.Count(t => t.Estado == "En proceso" || t.Estado == "En revisión");
            ViewBag.TotalCompletados = _db.Tramites.Count(t => t.Estado == "Completado" || t.Estado == "Cerrado" || t.Estado == "Aprobado" || t.Estado == "Resuelto");
            ViewBag.TotalCorreccion = _db.Tramites.Count(t => t.Estado == "Requiere corrección");

            return View(lista);
        }

        // ═══════════════════════════════════════════════════════════
        // DETALLE TRÁMITE (ADMIN)
        // ═══════════════════════════════════════════════════════════
        public ActionResult DetalleTramite(int id = 0)
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var tramite = _db.Tramites
                .Include("Usuario")
                .Include("TramiteDocumentos")
                .FirstOrDefault(t => t.TramiteId == id);

            if (tramite == null) return HttpNotFound();

            return View(tramite);
        }

        // ═══════════════════════════════════════════════════════════
        // CAMBIAR ESTADO DEL TRÁMITE (AJAX)
        // ═══════════════════════════════════════════════════════════
        [HttpPost]
        public JsonResult CambiarEstadoTramite(int tramiteId, string nuevoEstado, string comentario = "")
        {
            if (!EsAdmin())
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            var estadosValidos = new[] { "Iniciado", "En revisión", "En proceso", "Aprobado", "Requiere corrección", "Resuelto", "Completado", "Cerrado" };
            if (Array.IndexOf(estadosValidos, nuevoEstado) < 0)
                return Json(new { ok = false, mensaje = "Estado no válido." });

            var tramite = _db.Tramites.Find(tramiteId);
            if (tramite == null)
                return Json(new { ok = false, mensaje = "Trámite no encontrado." });

            tramite.Estado = nuevoEstado;
            tramite.FechaActualizacion = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(comentario))
            {
                var actual = tramite.DatosFormulario ?? "{}";
                if (actual.EndsWith("}"))
                    actual = actual.Substring(0, actual.Length - 1);

                var comentarioEscapado = comentario.Replace("\\", "\\\\").Replace("\"", "\\\"");
                tramite.DatosFormulario = actual + ",\"comentarioAdmin\":\"" + comentarioEscapado + "\"}";
            }

            _db.SaveChanges();

            var notif = new Notificacion
            {
                UsuarioId = tramite.UsuarioId,
                Titulo = "Trámite actualizado · " + nuevoEstado,
                Mensaje = "Tu trámite <b>" + tramite.Tipo + "</b> cambió a estado <b>" + nuevoEstado + "</b>." +
                          (string.IsNullOrWhiteSpace(comentario) ? "" : " Comentario: " + comentario),
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Tramite",
                Estado = nuevoEstado
            };
            _db.Notificaciones.Add(notif);
            _db.SaveChanges();

            return Json(new { ok = true, mensaje = "Estado actualizado.", estado = nuevoEstado });
        }

        // ═══════════════════════════════════════════════════════════
        // REPORTES (Facturación + Ventas + Actividad de uso)
        // ═══════════════════════════════════════════════════════════
        public ActionResult Reportes(string rango = "30d")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;

            // ═══════════ SECCIÓN 1: FACTURACIÓN ═══════════
            var consumos = new List<ConsumoMensualDto>();
            for (int i = 5; i >= 0; i--)
            {
                var fecha = hoy.AddMonths(-i);
                var inicioMesFact = new DateTime(fecha.Year, fecha.Month, 1);
                var finMesFact = inicioMesFact.AddMonths(1);

                var inicioLocal = inicioMesFact;
                var finLocal = finMesFact;

                var facturasMes = _db.Facturas
                    .Where(f => f.FechaEmision >= inicioLocal && f.FechaEmision < finLocal)
                    .ToList();

                consumos.Add(new ConsumoMensualDto
                {
                    mes = inicioMesFact.ToString("MMM"),
                    monto = facturasMes.Sum(f => f.Monto),
                    kwh = 0
                });
            }

            ViewBag.Consumos = consumos;

            ViewBag.TotalFacturado = _db.Facturas
                .Select(f => (decimal?)f.Monto)
                .Sum() ?? 0m;

            ViewBag.TotalCobrado = _db.Facturas
                .Where(f => f.Pagada)
                .Select(f => (decimal?)f.Monto)
                .Sum() ?? 0m;

            ViewBag.TotalPendiente = (decimal)ViewBag.TotalFacturado - (decimal)ViewBag.TotalCobrado;

            // ═══════════ SECCIÓN 2: VENTAS Y COMPRAS ═══════════
            var vmVentas = new CNFL_Clientes_Prototipo.Models.ViewModels.ReporteVentasViewModel();
            vmVentas.FiltroRapido = rango;

            DateTime fechaDesde = hoy.AddDays(-30);
            if (rango == "7d") fechaDesde = hoy.AddDays(-7);
            else if (rango == "90d") fechaDesde = hoy.AddDays(-90);
            else if (rango == "anio") fechaDesde = new DateTime(hoy.Year, 1, 1);

            var connStr = ConfigurationManager.ConnectionStrings["CNFLDbContext"].ConnectionString;

            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();

                // ═══ KPIs ═══
                string sqlKPI = @"
                    SELECT 
                        ISNULL(SUM(Total), 0) AS Ingresos,
                        COUNT(*) AS TotalCompras,
                        COUNT(DISTINCT UsuarioId) AS ClientesUnicos,
                        ISNULL(AVG(Total), 0) AS TicketPromedio
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada' AND FechaCreacion >= @desde";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlKPI, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            vmVentas.IngresosTotales = Convert.ToDecimal(r["Ingresos"]);
                            vmVentas.TotalCompras = Convert.ToInt32(r["TotalCompras"]);
                            vmVentas.ClientesUnicos = Convert.ToInt32(r["ClientesUnicos"]);
                            vmVentas.TicketPromedio = Convert.ToDecimal(r["TicketPromedio"]);
                        }
                    }
                }

                // ═══ Top clientes por gasto ═══
                string sqlTopCli = @"
                    SELECT TOP 10 
                        u.UsuarioId, u.Nombre + ' ' + u.Apellidos AS Nombre, u.Correo,
                        COUNT(o.OrdenId) AS TotalCompras, SUM(o.Total) AS TotalGastado
                    FROM OrdenesCompra o
                    INNER JOIN Usuarios u ON o.UsuarioId = u.UsuarioId
                    WHERE o.Estado = 'Pagada' AND o.FechaCreacion >= @desde
                    GROUP BY u.UsuarioId, u.Nombre, u.Apellidos, u.Correo
                    ORDER BY TotalGastado DESC";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlTopCli, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            vmVentas.TopClientes.Add(new CNFL_Clientes_Prototipo.Models.ViewModels.TopCliente
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

                // ═══ Métodos de pago ═══
                string sqlMetodos = @"
                    SELECT Metodo, COUNT(*) AS TotalCompras, SUM(Total) AS TotalIngresos
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada' AND FechaCreacion >= @desde
                    GROUP BY Metodo ORDER BY TotalIngresos DESC";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlMetodos, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var m = r["Metodo"].ToString();
                            var etiquetas = new System.Collections.Generic.Dictionary<string, string>
                            {
                                {"factura", "💡 Factura"},
                                {"tarjeta", "💳 Tarjeta"},
                                {"sinpe", "📱 SINPE"},
                                {"iban", "🏦 IBAN"},
                                {"token", "🔐 Token"}
                            };
                            vmVentas.VentasPorMetodo.Add(new CNFL_Clientes_Prototipo.Models.ViewModels.VentaPorMetodo
                            {
                                Metodo = m,
                                Etiqueta = etiquetas.ContainsKey(m) ? etiquetas[m] : m,
                                TotalCompras = Convert.ToInt32(r["TotalCompras"]),
                                TotalIngresos = Convert.ToDecimal(r["TotalIngresos"]),
                                Porcentaje = vmVentas.IngresosTotales > 0
                                    ? (Convert.ToDecimal(r["TotalIngresos"]) / vmVentas.IngresosTotales) * 100
                                    : 0
                            });
                        }
                    }
                }

                // ═══ Ventas por mes ═══
                string sqlMes = @"
                    SELECT YEAR(FechaCreacion) AS Anio, MONTH(FechaCreacion) AS Mes,
                           COUNT(*) AS TotalCompras, SUM(Total) AS TotalIngresos
                    FROM OrdenesCompra
                    WHERE Estado = 'Pagada' AND FechaCreacion >= DATEADD(month, -6, GETDATE())
                    GROUP BY YEAR(FechaCreacion), MONTH(FechaCreacion)
                    ORDER BY Anio, Mes";

                var meses = new[] { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlMes, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var mes = Convert.ToInt32(r["Mes"]);
                        vmVentas.VentasPorMes.Add(new CNFL_Clientes_Prototipo.Models.ViewModels.VentaPorMes
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
                    SELECT TOP 50
                        o.OrdenId, o.FechaCreacion, o.Subtotal, o.Impuesto, o.Total, o.Metodo, o.Estado, o.Detalle,
                        u.Nombre + ' ' + u.Apellidos AS ClienteNombre, u.Correo AS ClienteCorreo
                    FROM OrdenesCompra o
                    INNER JOIN Usuarios u ON o.UsuarioId = u.UsuarioId
                    WHERE o.FechaCreacion >= @desde
                    ORDER BY o.FechaCreacion DESC";

                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlCompras, conn))
                {
                    cmd.Parameters.AddWithValue("@desde", fechaDesde);
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

                            vmVentas.Compras.Add(new CNFL_Clientes_Prototipo.Models.ViewModels.CompraDetalle
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

            ViewBag.VentasViewModel = vmVentas;

            // ═══════════════════════════════════════════════════════════
            // SECCIÓN 3: ACTIVIDAD DE USO DE LA APP
            // ═══════════════════════════════════════════════════════════
            var actividadQuery = _db.ActividadUsuario.AsQueryable();

            // ✅ Calcular fechas fuera del Where (EF no traduce AddDays)
            DateTime? actividadDesde = null;
            if (rango == "7d") actividadDesde = hoy.AddDays(-7);
            else if (rango == "30d") actividadDesde = hoy.AddDays(-30);
            else if (rango == "90d") actividadDesde = hoy.AddDays(-90);
            else if (rango == "anio") actividadDesde = new DateTime(hoy.Year, 1, 1);

            if (actividadDesde.HasValue)
            {
                var desde = actividadDesde.Value;
                actividadQuery = actividadQuery.Where(a => a.Fecha >= desde);
            }

            // ═══ Secciones más usadas ═══
            var actividadSecciones = actividadQuery
                .GroupBy(a => a.Seccion)
                .Select(g => new ActividadSeccionDto
                {
                    Seccion = g.Key,
                    MinutosTotales = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                    Sesiones = g.Count()
                })
                .OrderByDescending(a => a.MinutosTotales)
                .ToList();

            ViewBag.ActividadSecciones = actividadSecciones;
            ViewBag.TotalSesiones = actividadQuery.Count();
            ViewBag.TotalMinutosApp = actividadQuery.Any()
                ? (actividadQuery.Sum(a => a.DuracionSegundos) ?? 0) / 60
                : 0;
            ViewBag.PromedioMinutosPorSesion = actividadQuery.Any()
                ? (double)(actividadQuery.Average(a => a.DuracionSegundos ?? 0)) / 60.0
                : 0.0;

            // ═══ Actividad por mes (últimos 6 meses) ═══
            var actividadMeses = new List<ActividadMesDto>();
            var nombresMeses = new[] { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

            for (int i = 5; i >= 0; i--)
            {
                var fecha = hoy.AddMonths(-i);
                var inicioMes = new DateTime(fecha.Year, fecha.Month, 1);
                var finMes = inicioMes.AddMonths(1);

                // ✅ Variables locales para que EF las traduzca
                var inicioLocal = inicioMes;
                var finLocal = finMes;

                var datosMes = actividadQuery
                    .Where(a => a.Fecha >= inicioLocal && a.Fecha < finLocal)
                    .ToList();

                actividadMeses.Add(new ActividadMesDto
                {
                    Etiqueta = nombresMeses[inicioMes.Month],
                    Anio = inicioMes.Year,
                    Mes = inicioMes.Month,
                    MinutosTotales = (datosMes.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                    Sesiones = datosMes.Count
                });
            }

            ViewBag.ActividadMeses = actividadMeses;

            // ═══ Top 10 usuarios por uso de la app ═══
            var topUsuarios = actividadQuery
                .GroupBy(a => a.UsuarioId)
                .Select(g => new TopUsuarioUsoDto
                {
                    UsuarioId = g.Key,
                    Nombre = _db.Usuarios
                        .Where(u => u.UsuarioId == g.Key)
                        .Select(u => u.Nombre + " " + u.Apellidos)
                        .FirstOrDefault(),
                    Correo = _db.Usuarios
                        .Where(u => u.UsuarioId == g.Key)
                        .Select(u => u.Correo)
                        .FirstOrDefault(),
                    MinutosTotales = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                    Secciones = g.Select(x => x.Seccion).Distinct().Count(),
                    Visitas = g.Count()
                })
                .OrderByDescending(u => u.MinutosTotales)
                .Take(10)
                .ToList();

            ViewBag.TopUsuarios = topUsuarios;

            // ═══ Actividad semanal (últimos 7 días: facturas + averías + trámites) ═══
            var actividadSemanal = new List<ActividadSemanalDto>();
            for (int i = 6; i >= 0; i--)
            {
                var dia = hoy.AddDays(-i);
                var inicioDia = dia.Date;
                var finDia = inicioDia.AddDays(1);

                // ✅ Variables locales
                var inicioLocal = inicioDia;
                var finLocal = finDia;

                actividadSemanal.Add(new ActividadSemanalDto
                {
                    dia = dia.ToString("ddd"),
                    facturas = _db.Facturas.Count(f => f.FechaEmision >= inicioLocal && f.FechaEmision < finLocal),
                    reportes = _db.Averias.Count(a => a.FechaReporte >= inicioLocal && a.FechaReporte < finLocal),
                    tramites = _db.Tramites.Count(t => t.FechaSolicitud >= inicioLocal && t.FechaSolicitud < finLocal),
                    perfil = 0
                });
            }

            ViewBag.ActividadSemanal = actividadSemanal;

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // EXPORTAR A EXCEL (reporte general)
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarExcel()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Resumen General");
                ws.Cell("A1").Value = "CNFL · Panel Administrativo";
                ws.Range("A1:B1").Merge().Style.Font.SetBold().Font.SetFontSize(16).Font.FontColor = XLColor.White;
                ws.Range("A1:B1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                ws.Range("A1:B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Height = 30;

                ws.Cell("A2").Value = "Reporte generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                ws.Range("A2:B2").Merge().Style.Font.SetItalic().Font.FontColor = XLColor.Gray;

                ws.Cell("A4").Value = "MÉTRICA";
                ws.Cell("B4").Value = "VALOR";
                ws.Range("A4:B4").Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");

                var filas = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Total clientes", _db.Usuarios.Count().ToString()),
                    new KeyValuePair<string, string>("Clientes activos", _db.Usuarios.Count(u => u.Activo).ToString()),
                    new KeyValuePair<string, string>("Total NISEs", _db.NISEs.Count().ToString()),
                    new KeyValuePair<string, string>("Total facturas", _db.Facturas.Count().ToString()),
                    new KeyValuePair<string, string>("Facturas pendientes", _db.Facturas.Count(f => !f.Pagada).ToString()),
                    new KeyValuePair<string, string>("Monto pendiente", "₡" + (_db.Facturas.Where(f => !f.Pagada).Select(f => (decimal?)f.Monto).Sum() ?? 0m).ToString("N0")),
                    new KeyValuePair<string, string>("Ingresos del mes", "₡" + (_db.Facturas.Where(f => f.Pagada && f.FechaEmision >= inicioMes).Select(f => (decimal?)f.Monto).Sum() ?? 0m).ToString("N0")),
                    new KeyValuePair<string, string>("Averías abiertas", _db.Averias.Count(a => a.Estado != "Resuelta" && a.Estado != "Cerrada").ToString()),
                    new KeyValuePair<string, string>("Averías resueltas", _db.Averias.Count(a => a.Estado == "Resuelta" || a.Estado == "Cerrada").ToString()),
                    new KeyValuePair<string, string>("Trámites abiertos", _db.Tramites.Count(t => t.Estado != "Completado" && t.Estado != "Cerrado").ToString()),
                    new KeyValuePair<string, string>("Trámites totales", _db.Tramites.Count().ToString())
                };

                int row = 5;
                foreach (var f in filas)
                {
                    ws.Cell(row, 1).Value = f.Key;
                    ws.Cell(row, 2).Value = f.Value;
                    row++;
                }
                ws.Columns().AdjustToContents();

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    var bytes = ms.ToArray();
                    return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "CNFL_Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // EXPORTAR A PDF (reporte general)
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarPDF()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 54, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(26, 43, 107));
                var texto = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                doc.Add(new Paragraph("CNFL · Reporte Administrativo", titulo));
                doc.Add(new Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), texto));

                doc.Close();
                return File(ms.ToArray(), "application/pdf",
                    "CNFL_Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
            }
        }

        // ═══════════════════════════════════════════════════════════
        // CUENTA
        // ═══════════════════════════════════════════════════════════
        public ActionResult Cuenta()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            ViewBag.NombreAdmin = Session["AdminNombre"] as string ?? "Administrador";
            ViewBag.CorreoAdmin = Session["AdminCorreo"] as string ?? "admin@cnfl.go.cr";

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // CONFIGURACIÓN
        // ═══════════════════════════════════════════════════════════
        public ActionResult Configuracion()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            ViewBag.TotalActividades = _db.ActividadesEconomicas.Count();
            ViewBag.TotalSuscripciones = _db.Suscripciones.Count();

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // LOGOUT
        // ═══════════════════════════════════════════════════════════
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Cuenta");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}