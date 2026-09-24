using CNFL_Clientes_Prototipo.Data;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Models.ViewModels;
// ═══ EPPlus 4.x ═══
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingBrush = System.Drawing.SolidBrush;
using DrawingColor = System.Drawing.Color;
// ═══ System.Drawing con alias explícito ═══
using DrawingFont = System.Drawing.Font;
using DrawingFontStyle = System.Drawing.FontStyle;
using DrawingGraphics = System.Drawing.Graphics;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;
using DrawingPen = System.Drawing.Pen;
using DrawingSmoothingMode = System.Drawing.Drawing2D.SmoothingMode;
using DrawingTextRenderingHint = System.Drawing.Text.TextRenderingHint;
using PdfBaseColor = iTextSharp.text.BaseColor;
// ═══ iTextSharp (solo alias, sin importar el namespace completo para evitar conflictos) ═══
using PdfBaseFont = iTextSharp.text.pdf.BaseFont;
using PdfCell = iTextSharp.text.pdf.PdfPCell;
using PdfContentByte = iTextSharp.text.pdf.PdfContentByte;
using PdfDocument = iTextSharp.text.Document;
using PdfElement = iTextSharp.text.Element;
using PdfFont = iTextSharp.text.Font;
using PdfImage = iTextSharp.text.Image;
using PdfPageEventHelper = iTextSharp.text.pdf.PdfPageEventHelper;
using PdfPageSize = iTextSharp.text.PageSize;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfPhrase = iTextSharp.text.Phrase;
using PdfRect = iTextSharp.text.Rectangle;
using PdfTable = iTextSharp.text.pdf.PdfPTable;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using XlBorder = OfficeOpenXml.Style.ExcelBorderStyle;
using XlChart = OfficeOpenXml.Drawing.Chart;
using XlFill = OfficeOpenXml.Style.ExcelFillStyle;
using XlHAlign = OfficeOpenXml.Style.ExcelHorizontalAlignment;
using XlVAlign = OfficeOpenXml.Style.ExcelVerticalAlignment;

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
            ViewBag.MontoPendiente = facturasPendientes.Select(f => (decimal?)f.Monto).Sum() ?? 0m;

            ViewBag.IngresosMes = _db.Facturas
                .Where(f => f.Pagada && f.FechaEmision >= inicioMes)
                .Select(f => (decimal?)f.Monto).Sum() ?? 0m;

            ViewBag.AveriasAbiertas = _db.Averias.Count(a => a.Estado != "Resuelta" && a.Estado != "Cerrada");
            ViewBag.AveriasResueltas = _db.Averias.Count(a => a.Estado == "Resuelta" || a.Estado == "Cerrada");
            ViewBag.TramitesAbiertos = _db.Tramites.Count(t => t.Estado != "Completado" && t.Estado != "Cerrado");

            var averiasResueltas = _db.Averias
                .Where(a => (a.Estado == "Resuelta" || a.Estado == "Cerrada") && a.FechaActualizacion != null)
                .ToList();

            double tiempoPromedio = 0;
            if (averiasResueltas.Any())
                tiempoPromedio = averiasResueltas.Average(a => ((DateTime)a.FechaActualizacion - a.FechaReporte).TotalDays);

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
                    u.Nombre.Contains(q) || u.Apellidos.Contains(q) ||
                    u.Cedula.Contains(q) || u.Correo.Contains(q));
            }

            var lista = query.OrderBy(u => u.Nombre).Select(u => new ClienteAdminDto
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
                FacturasPendientes = _db.Facturas.Count(f => f.NISE.UsuarioId == u.UsuarioId && !f.Pagada),
                MontoPendiente = _db.Facturas.Where(f => f.NISE.UsuarioId == u.UsuarioId && !f.Pagada)
                    .Select(f => (decimal?)f.Monto).Sum() ?? 0m,
                AveriasActivas = _db.Averias.Count(a => a.UsuarioId == u.UsuarioId &&
                    a.Estado != "Resuelta" && a.Estado != "Cerrada")
            }).ToList();

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
            ViewBag.Facturas = _db.Facturas.Where(f => f.NISE.UsuarioId == id)
                .OrderByDescending(f => f.FechaEmision).Take(20).ToList();
            ViewBag.Averias = _db.Averias.Where(a => a.UsuarioId == id)
                .OrderByDescending(a => a.FechaReporte).Take(20).ToList();
            ViewBag.Tramites = _db.Tramites.Where(t => t.UsuarioId == id)
                .OrderByDescending(t => t.FechaSolicitud).Take(20).ToList();

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

            var lista = query.OrderByDescending(a => a.FechaReporte).Take(100).ToList();
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

            var query = _db.Tramites.Include("Usuario").AsQueryable();
            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(t => t.Estado == estado);

            var lista = query.OrderByDescending(t => t.FechaSolicitud).Take(100).ToList();

            ViewBag.Estado = estado;
            ViewBag.TotalSolicitados = _db.Tramites.Count(t => t.Estado == "Iniciado" || t.Estado == "Solicitado");
            ViewBag.TotalProceso = _db.Tramites.Count(t => t.Estado == "En proceso" || t.Estado == "En revisión");
            ViewBag.TotalCompletados = _db.Tramites.Count(t => t.Estado == "Completado" || t.Estado == "Cerrado" || t.Estado == "Aprobado" || t.Estado == "Resuelto");
            ViewBag.TotalCorreccion = _db.Tramites.Count(t => t.Estado == "Requiere corrección");

            return View(lista);
        }

        // ═══════════════════════════════════════════════════════════
        // DETALLE TRÁMITE
        // ═══════════════════════════════════════════════════════════
        public ActionResult DetalleTramite(int id = 0)
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var tramite = _db.Tramites.Include("Usuario").Include("TramiteDocumentos")
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

        // ═══════════════════════════════════════════════════════════════════════════
        //
        //   R E P O R T E S
        //
        //   Un solo ViewModel (ReporteClienteViewModel) alimenta la pantalla, el Excel
        //   y el PDF, así que los tres siempre muestran los mismos datos.
        //
        // ═══════════════════════════════════════════════════════════════════════════

        public ActionResult Reportes(string rango = "todo", int? clienteId = null)
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var vm = ConstruirReporteCliente(clienteId, rango);
            return View(vm);
        }

        // Exportes antiguos: ahora redirigen a los nuevos para no romper enlaces existentes
        public ActionResult ExportarExcel()
        {
            return RedirectToAction("ExportarReporteExcel");
        }

        public ActionResult ExportarPDF()
        {
            return RedirectToAction("ExportarReportePDF");
        }

        // ───────────────────────────────────────────────────────────
        // Construcción del reporte
        // ───────────────────────────────────────────────────────────
        private static readonly string[] NombresMeses = { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
        private static readonly string[] EstadosTramiteCerrado = { "Completado", "Cerrado", "Aprobado", "Resuelto" };

        private ReporteClienteViewModel ConstruirReporteCliente(int? clienteId, string rango)
        {
            var hoy = DateTime.Now;
            if (rango != "7d" && rango != "30d" && rango != "90d" && rango != "anio") rango = "todo";
            var desde = CalcularDesde(rango, hoy);

            var vm = new ReporteClienteViewModel();
            vm.Generado = hoy;
            vm.Rango = rango;
            vm.RangoTexto = TextoRango(rango, hoy);
            vm.Desde = desde;

            CargarSistema(vm, hoy, desde);
            CargarCliente(vm, clienteId, hoy, desde);

            return vm;
        }

        private static DateTime? CalcularDesde(string rango, DateTime hoy)
        {
            switch (rango)
            {
                case "7d": return hoy.Date.AddDays(-7);
                case "30d": return hoy.Date.AddDays(-30);
                case "90d": return hoy.Date.AddDays(-90);
                case "anio": return new DateTime(hoy.Year, 1, 1);
                default: return null;
            }
        }

        private static string TextoRango(string rango, DateTime hoy)
        {
            switch (rango)
            {
                case "7d": return "Últimos 7 días";
                case "30d": return "Últimos 30 días";
                case "90d": return "Últimos 90 días";
                case "anio": return "Año " + hoy.Year;
                default: return "Todo el historial";
            }
        }

        private static List<DateTime> UltimosMeses(int cantidad, DateTime hoy)
        {
            var lista = new List<DateTime>();
            var primero = new DateTime(hoy.Year, hoy.Month, 1);
            for (int i = cantidad - 1; i >= 0; i--)
                lista.Add(primero.AddMonths(-i));
            return lista;
        }

        private static string EtiquetaMes(DateTime mes)
        {
            return NombresMeses[mes.Month] + " " + mes.Year.ToString().Substring(2);
        }

        private static string EstadoFactura(bool pagada, DateTime vence, DateTime hoy)
        {
            if (pagada) return "Pagada";
            return vence.Date < hoy.Date ? "Vencida" : "Pendiente";
        }

        private static System.Data.SqlClient.SqlParameter ParamFecha(string nombre, DateTime? valor)
        {
            var p = new System.Data.SqlClient.SqlParameter(nombre, System.Data.SqlDbType.DateTime);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        // Lee una propiedad opcional por nombre (por si el modelo la tiene con otro nombre o no la tiene)
        private static string Prop(object obj, params string[] nombres)
        {
            if (obj == null) return "";
            var tipo = obj.GetType();
            foreach (var n in nombres)
            {
                var p = tipo.GetProperty(n);
                if (p == null) continue;
                var v = p.GetValue(obj, null);
                if (v != null) return Convert.ToString(v);
            }
            return "";
        }

        private static string EtiquetaMetodo(string metodo)
        {
            switch ((metodo ?? "").ToLowerInvariant())
            {
                case "factura": return "Factura";
                case "tarjeta": return "Tarjeta";
                case "sinpe": return "SINPE";
                case "iban": return "IBAN";
                case "token": return "Token";
                default: return string.IsNullOrEmpty(metodo) ? "Otro" : metodo;
            }
        }

        private static int ContarArticulos(string detalle)
        {
            if (string.IsNullOrWhiteSpace(detalle)) return 0;
            try
            {
                var arr = Newtonsoft.Json.Linq.JArray.Parse(detalle);
                int total = 0;
                foreach (var item in arr)
                    total += item["cantidad"] != null ? (int)item["cantidad"] : 1;
                return total;
            }
            catch { return 0; }
        }

        // ───────────────────────────────────────────────────────────
        // Bloque 1: resumen general del sistema
        // ───────────────────────────────────────────────────────────
        private void CargarSistema(ReporteClienteViewModel vm, DateTime hoy, DateTime? desde)
        {
            var hoyFecha = hoy.Date;

            vm.SisClientesActivos = _db.Usuarios.Count(u => u.Activo);
            vm.SisClientesTotal = _db.Usuarios.Count();
            vm.SisNISEs = _db.NISEs.Count();
            vm.SisFacturas = _db.Facturas.Count();
            vm.SisFacturado = _db.Facturas.Select(f => (decimal?)f.Monto).Sum() ?? 0m;
            vm.SisCobrado = _db.Facturas.Where(f => f.Pagada).Select(f => (decimal?)f.Monto).Sum() ?? 0m;
            vm.SisPendiente = vm.SisFacturado - vm.SisCobrado;
            vm.SisPorcentajeCobro = vm.SisFacturado > 0 ? Math.Round(vm.SisCobrado / vm.SisFacturado * 100m, 1) : 0m;
            vm.SisFacturasVencidas = _db.Facturas.Count(f => !f.Pagada && f.FechaVencimiento < hoyFecha);
            vm.SisAveriasAbiertas = _db.Averias.Count(a => a.Estado != "Resuelta" && a.Estado != "Cerrada");
            vm.SisTramitesAbiertos = _db.Tramites.Count(t => t.Estado != "Completado" && t.Estado != "Cerrado" && t.Estado != "Aprobado" && t.Estado != "Resuelto");

            var nisesConDeuda = _db.Facturas.Where(f => !f.Pagada).Select(f => f.NISE.NiseId).Distinct().ToList();
            vm.SisServiciosConDeuda = _db.NISEs.Count(n => nisesConDeuda.Contains(n.NiseId));
            vm.SisServiciosAlDia = vm.SisNISEs - vm.SisServiciosConDeuda;

            // Facturado vs cobrado, últimos 6 meses (una sola consulta)
            var meses = UltimosMeses(6, hoy);
            var desdeMeses = meses[0];
            var fs = _db.Facturas.Where(f => f.FechaEmision >= desdeMeses)
                .Select(f => new { f.FechaEmision, f.Monto, f.Pagada }).ToList();

            foreach (var m in meses)
            {
                var mm = m;
                var delMes = fs.Where(f => f.FechaEmision.Year == mm.Year && f.FechaEmision.Month == mm.Month).ToList();
                vm.SisMensual.Add(new RepPunto
                {
                    Etiqueta = EtiquetaMes(mm),
                    Valor = delMes.Sum(f => f.Monto),
                    Valor2 = delMes.Where(f => f.Pagada).Sum(f => f.Monto)
                });
            }

            // Clientes con mayor saldo pendiente
            var deuda = _db.Facturas.Where(f => !f.Pagada)
                .GroupBy(f => f.NISE.UsuarioId)
                .Select(g => new { UsuarioId = g.Key, Monto = g.Sum(x => x.Monto), Cantidad = g.Count() })
                .OrderByDescending(x => x.Monto)
                .Take(5)
                .ToList();

            var ids = deuda.Select(d => d.UsuarioId).ToList();
            var usuariosDeuda = _db.Usuarios.Where(u => ids.Contains(u.UsuarioId)).ToList();

            foreach (var d in deuda)
            {
                var dd = d;
                var u = usuariosDeuda.FirstOrDefault(x => x.UsuarioId == dd.UsuarioId);
                vm.TopDeudores.Add(new RepDeudor
                {
                    Nombre = u != null ? (u.Nombre + " " + u.Apellidos).Trim() : "Cliente " + d.UsuarioId,
                    Cedula = u != null ? u.Cedula : "",
                    Facturas = d.Cantidad,
                    Monto = d.Monto
                });
            }

            // Ventas de productos y servicios (tabla OrdenesCompra). Si falla, el reporte sigue sin esta parte.
            try
            {
                var fila = _db.Database.SqlQuery<RepVentasSistema>(
                    "SELECT ISNULL(SUM(Total), 0) AS Ingresos, COUNT(*) AS Compras, COUNT(DISTINCT UsuarioId) AS Compradores " +
                    "FROM OrdenesCompra WHERE Estado = 'Pagada' AND (@desde IS NULL OR FechaCreacion >= @desde)",
                    ParamFecha("@desde", desde)).FirstOrDefault();

                if (fila != null)
                {
                    vm.SisIngresosVentas = fila.Ingresos;
                    vm.SisCompras = fila.Compras;
                    vm.SisCompradores = fila.Compradores;
                    vm.SisTicketPromedio = fila.Compras > 0 ? Math.Round(fila.Ingresos / fila.Compras, 0) : 0m;
                }
            }
            catch { }
        }

        // ───────────────────────────────────────────────────────────
        // Bloque 2: detalle del cliente seleccionado
        // ───────────────────────────────────────────────────────────
        private void CargarCliente(ReporteClienteViewModel vm, int? clienteId, DateTime hoy, DateTime? desde)
        {
            // Lista del selector
            var listado = _db.Usuarios.OrderBy(u => u.Nombre)
                .Select(u => new { u.UsuarioId, u.Nombre, u.Apellidos, u.Cedula })
                .Take(500).ToList();

            foreach (var u in listado)
                vm.ClientesLista.Add(new KeyValuePair<int, string>(u.UsuarioId, (u.Nombre + " " + u.Apellidos).Trim() + " · " + u.Cedula));

            // Cliente elegido (si no viene ninguno, el primero de la lista, igual que el selector)
            Usuario usuario = null;
            if (clienteId.HasValue)
            {
                var idSel = clienteId.Value;
                usuario = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == idSel);
            }
            if (usuario == null)
                usuario = _db.Usuarios.OrderBy(u => u.Nombre).FirstOrDefault();
            if (usuario == null) return;

            int uid = usuario.UsuarioId;
            vm.HayCliente = true;
            vm.ClienteId = uid;

            vm.Cliente = new RepCliente
            {
                UsuarioId = uid,
                NombreCompleto = (usuario.Nombre + " " + usuario.Apellidos).Trim(),
                Cedula = Convert.ToString(usuario.Cedula),
                Correo = Convert.ToString(usuario.Correo),
                CorreoSecundario = usuario.CorreoSecundario ?? "",
                Telefono = Convert.ToString(usuario.Telefono),
                Activo = usuario.Activo,
                FacturaElectronica = usuario.FacturaElectronica,
                FechaRegistro = usuario.FechaRegistro
            };

            // ── Facturas (todas) ──
            var todas = _db.Facturas.Where(f => f.NISE.UsuarioId == uid)
                .Select(f => new { f.NumeroFactura, f.FechaEmision, f.FechaVencimiento, f.Monto, f.Pagada, Nise = f.NISE.NumeroNise })
                .ToList()
                .Select(f => new RepFactura
                {
                    Numero = Convert.ToString(f.NumeroFactura),
                    Nise = Convert.ToString(f.Nise),
                    Emision = f.FechaEmision,
                    Vencimiento = f.FechaVencimiento,
                    Monto = f.Monto,
                    Pagada = f.Pagada,
                    Estado = EstadoFactura(f.Pagada, f.FechaVencimiento, hoy),
                    DiasAtraso = (!f.Pagada && f.FechaVencimiento.Date < hoy.Date) ? (int)(hoy.Date - f.FechaVencimiento.Date).TotalDays : 0
                })
                .OrderByDescending(f => f.Emision)
                .ToList();

            var enRango = desde.HasValue ? todas.Where(f => f.Emision >= desde.Value).ToList() : todas;

            vm.Facturas = enRango.Take(200).ToList();
            vm.Facturado = enRango.Sum(f => f.Monto);
            vm.Pagado = enRango.Where(f => f.Pagada).Sum(f => f.Monto);
            vm.Pendiente = vm.Facturado - vm.Pagado;
            vm.PorcentajePagado = vm.Facturado > 0 ? Math.Round(vm.Pagado / vm.Facturado * 100m, 1) : 0m;
            vm.FacturasTotal = enRango.Count;
            vm.FacturasPagadas = enRango.Count(f => f.Pagada);
            vm.FacturasPendientes = enRango.Count(f => f.Estado == "Pendiente");
            vm.FacturasVencidas = enRango.Count(f => f.Estado == "Vencida");
            vm.SaldoPendienteTotal = todas.Where(f => !f.Pagada).Sum(f => f.Monto);
            vm.MontoVencido = todas.Where(f => f.Estado == "Vencida").Sum(f => f.Monto);

            if (todas.Any(f => f.Estado == "Vencida"))
            {
                vm.EstadoCuenta = "Con facturas vencidas";
                vm.EstadoCuentaClase = "red";
            }
            else if (todas.Any(f => !f.Pagada))
            {
                vm.EstadoCuenta = "Con facturas pendientes";
                vm.EstadoCuentaClase = "amber";
            }
            else
            {
                vm.EstadoCuenta = "Al día";
                vm.EstadoCuentaClase = "green";
            }

            foreach (var m in UltimosMeses(12, hoy))
            {
                var mm = m;
                var delMes = todas.Where(f => f.Emision.Year == mm.Year && f.Emision.Month == mm.Month).ToList();
                vm.FacturacionMensual.Add(new RepPunto
                {
                    Etiqueta = EtiquetaMes(mm),
                    Valor = delMes.Sum(f => f.Monto),
                    Valor2 = delMes.Where(f => f.Pagada).Sum(f => f.Monto)
                });
            }

            // ── Servicios (NISE) ──
            var nises = _db.NISEs.Where(n => n.UsuarioId == uid).ToList();
            foreach (var n in nises)
            {
                var numero = Convert.ToString(n.NumeroNise);
                var deEste = todas.Where(f => f.Nise == numero).ToList();
                var pend = deEste.Where(f => !f.Pagada).Sum(f => f.Monto);

                vm.Servicios.Add(new RepServicio
                {
                    Nise = numero,
                    Tipo = Convert.ToString(n.TipoServicio),
                    Provincia = Convert.ToString(n.Provincia),
                    Direccion = Prop(n, "Direccion", "DireccionServicio", "Ubicacion"),
                    Facturas = deEste.Count,
                    Pendiente = pend,
                    Estado = pend > 0 ? "Con deuda" : "Al día"
                });
            }

            // ── Averías ──
            var averiasQ = _db.Averias.Where(a => a.UsuarioId == uid);
            if (desde.HasValue)
            {
                var d = desde.Value;
                averiasQ = averiasQ.Where(a => a.FechaReporte >= d);
            }
            var averias = averiasQ.OrderByDescending(a => a.FechaReporte).Take(200).ToList();

            var sumaDias = 0.0;
            var conDias = 0;
            foreach (var a in averias)
            {
                DateTime? reportada = a.FechaReporte;
                DateTime? actualizada = a.FechaActualizacion;
                var estadoA = Convert.ToString(a.Estado);
                var cerrada = estadoA == "Resuelta" || estadoA == "Cerrada";

                double? dias = null;
                if (cerrada && actualizada.HasValue && reportada.HasValue)
                {
                    dias = (actualizada.Value - reportada.Value).TotalDays;
                    sumaDias += dias.Value;
                    conDias++;
                }

                vm.Averias.Add(new RepAveria
                {
                    Id = Prop(a, "AveriaId", "Id"),
                    Tipo = Prop(a, "Tipo", "TipoAveria", "Categoria", "Titulo"),
                    Descripcion = Prop(a, "Descripcion", "Detalle", "Comentario"),
                    Estado = estadoA,
                    Reportada = reportada,
                    Actualizada = actualizada,
                    DiasResolucion = dias
                });
            }

            vm.AveriasTotal = vm.Averias.Count;
            vm.AveriasResueltas = vm.Averias.Count(a => a.Estado == "Resuelta" || a.Estado == "Cerrada");
            vm.AveriasAbiertas = vm.AveriasTotal - vm.AveriasResueltas;
            vm.AveriasDiasPromedio = conDias > 0 ? Math.Round(sumaDias / conDias, 1) : 0.0;
            vm.AveriasPorEstado = vm.Averias.GroupBy(a => string.IsNullOrEmpty(a.Estado) ? "Sin estado" : a.Estado)
                .Select(g => new RepGrupo { Nombre = g.Key, Cantidad = g.Count() })
                .OrderByDescending(g => g.Cantidad).ToList();

            // ── Trámites ──
            var tramitesQ = _db.Tramites.Where(t => t.UsuarioId == uid);
            if (desde.HasValue)
            {
                var d = desde.Value;
                tramitesQ = tramitesQ.Where(t => t.FechaSolicitud >= d);
            }
            var tramites = tramitesQ.OrderByDescending(t => t.FechaSolicitud).Take(200).ToList();

            foreach (var t in tramites)
            {
                DateTime? solicitud = t.FechaSolicitud;
                DateTime? actualizacion = t.FechaActualizacion;
                vm.Tramites.Add(new RepTramite
                {
                    Id = Prop(t, "TramiteId", "Id"),
                    Tipo = Convert.ToString(t.Tipo),
                    Estado = Convert.ToString(t.Estado),
                    Solicitud = solicitud,
                    Actualizacion = actualizacion
                });
            }

            vm.TramitesTotal = vm.Tramites.Count;
            vm.TramitesCompletados = vm.Tramites.Count(t => Array.IndexOf(EstadosTramiteCerrado, t.Estado) >= 0);
            vm.TramitesAbiertos = vm.TramitesTotal - vm.TramitesCompletados;
            vm.TramitesPorEstado = vm.Tramites.GroupBy(t => string.IsNullOrEmpty(t.Estado) ? "Sin estado" : t.Estado)
                .Select(g => new RepGrupo { Nombre = g.Key, Cantidad = g.Count() })
                .OrderByDescending(g => g.Cantidad).ToList();

            // ── Compras (tabla OrdenesCompra). Si falla, el reporte sigue sin esta parte. ──
            try
            {
                var compras = _db.Database.SqlQuery<RepCompra>(
                    "SELECT TOP 200 OrdenId AS Id, FechaCreacion AS Fecha, Metodo, Estado, Subtotal, Impuesto, Total, Detalle " +
                    "FROM OrdenesCompra WHERE UsuarioId = @uid AND (@desde IS NULL OR FechaCreacion >= @desde) " +
                    "ORDER BY FechaCreacion DESC",
                    new System.Data.SqlClient.SqlParameter("@uid", uid),
                    ParamFecha("@desde", desde)).ToList();

                foreach (var c in compras)
                {
                    c.Articulos = ContarArticulos(c.Detalle);
                    c.Detalle = null;
                    vm.Compras.Add(c);
                }

                var pagadas = vm.Compras.Where(c => string.Equals(c.Estado, "Pagada", StringComparison.OrdinalIgnoreCase)).ToList();
                vm.ComprasPagadas = pagadas.Count;
                vm.ComprasMonto = pagadas.Sum(c => c.Total);
                vm.ComprasTicket = pagadas.Count > 0 ? Math.Round(vm.ComprasMonto / pagadas.Count, 0) : 0m;
                vm.ComprasPorMetodo = pagadas.GroupBy(c => EtiquetaMetodo(c.Metodo))
                    .Select(g => new RepGrupo { Nombre = g.Key, Cantidad = g.Count(), Monto = g.Sum(x => x.Total) })
                    .OrderByDescending(g => g.Monto).ToList();
            }
            catch { }

            // ── Actividad en la app ──
            var actQ = _db.ActividadUsuario.Where(a => a.UsuarioId == uid);
            if (desde.HasValue)
            {
                var d = desde.Value;
                actQ = actQ.Where(a => a.Fecha >= d);
            }
            var act = actQ.Select(a => new { a.Seccion, a.DuracionSegundos, a.Fecha }).ToList();

            vm.ActSesiones = act.Count;
            var totalSeg = act.Sum(a => a.DuracionSegundos ?? 0);
            vm.ActMinutos = Math.Round(totalSeg / 60m, 1);
            vm.ActPromedioMin = act.Count > 0 ? Math.Round(totalSeg / 60m / act.Count, 1) : 0m;
            vm.ActUltima = act.Count > 0 ? act.Max(a => (DateTime?)a.Fecha) : null;

            foreach (var g in act.GroupBy(a => string.IsNullOrEmpty(a.Seccion) ? "Sin sección" : a.Seccion))
            {
                var seg = g.Sum(a => a.DuracionSegundos ?? 0);
                vm.Actividad.Add(new RepActividad
                {
                    Seccion = g.Key,
                    Minutos = Math.Round(seg / 60m, 1),
                    Sesiones = g.Count(),
                    Porcentaje = totalSeg > 0 ? Math.Round(seg * 100m / totalSeg, 1) : 0m
                });
            }
            vm.Actividad = vm.Actividad.OrderByDescending(a => a.Minutos).ToList();
        }

        private static string NombreArchivo(ReporteClienteViewModel vm, string extension)
        {
            var baseNombre = vm.HayCliente ? vm.Cliente.NombreCompleto : "General";
            var limpio = new string(baseNombre.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray()).Trim().Replace(' ', '_');
            if (limpio.Length == 0) limpio = "General";
            return "CNFL_Reporte_" + limpio + "_" + vm.Generado.ToString("yyyyMMdd_HHmm") + "." + extension;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        //
        //   E X C E L   (EPPlus)
        //
        // ═══════════════════════════════════════════════════════════════════════════

        private static readonly DrawingColor XAzul = DrawingColor.FromArgb(26, 43, 107);
        private static readonly DrawingColor XMorado = DrawingColor.FromArgb(91, 63, 191);
        private static readonly DrawingColor XClaro = DrawingColor.FromArgb(244, 246, 251);
        private static readonly DrawingColor XEncab = DrawingColor.FromArgb(224, 232, 255);
        private static readonly DrawingColor XBorde = DrawingColor.FromArgb(220, 225, 238);
        private static readonly DrawingColor XGris = DrawingColor.FromArgb(110, 115, 130);

        private const string FmtMon = "\"₡\"#,##0.00";
        private const string FmtMon0 = "\"₡\"#,##0";
        private const string FmtFecha = "dd/mm/yyyy";
        private const string FmtEntero = "#,##0";
        private const string FmtDec1 = "#,##0.0";
        private const string FmtPct = "0.0\"%\"";
        private const string MimeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private class XlKpi
        {
            public string Etiqueta;
            public object Valor;
            public string Formato;
            public XlKpi(string etiqueta, object valor, string formato)
            {
                Etiqueta = etiqueta;
                Valor = valor;
                Formato = formato;
            }
        }

        private static DrawingColor XlTexto(string clase)
        {
            switch (clase)
            {
                case "green": return DrawingColor.FromArgb(4, 106, 56);
                case "red": return DrawingColor.FromArgb(183, 28, 28);
                case "amber": return DrawingColor.FromArgb(150, 100, 0);
                default: return DrawingColor.FromArgb(26, 43, 107);
            }
        }

        private static DrawingColor XlFondo(string clase)
        {
            switch (clase)
            {
                case "green": return DrawingColor.FromArgb(212, 240, 224);
                case "red": return DrawingColor.FromArgb(255, 224, 224);
                case "amber": return DrawingColor.FromArgb(255, 244, 217);
                default: return DrawingColor.FromArgb(224, 232, 255);
            }
        }

        public ActionResult ExportarReporteExcel(int? clienteId = null, string rango = "todo")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var vm = ConstruirReporteCliente(clienteId, rango);

            using (var package = new ExcelPackage())
            {
                package.Workbook.Properties.Title = "Reporte General CNFL";
                package.Workbook.Properties.Author = "CNFL - Panel Administrativo";
                package.Workbook.Properties.Company = "CNFL";

                XlHojaResumen(package, vm);

                if (vm.HayCliente)
                {
                    XlHojaCliente(package, vm);
                    XlHojaFacturas(package, vm);
                    XlHojaAverias(package, vm);
                    XlHojaTramites(package, vm);
                    XlHojaCompras(package, vm);
                    XlHojaActividad(package, vm);
                }

                return File(package.GetAsByteArray(), MimeXlsx, NombreArchivo(vm, "xlsx"));
            }
        }

        // ── Helpers de estilo ──
        private static void XlAnchos(ExcelWorksheet ws, params double[] anchos)
        {
            for (int i = 0; i < anchos.Length; i++)
                ws.Column(i + 1).Width = anchos[i];
        }

        private static void XlBordes(ExcelRange r)
        {
            r.Style.Border.Top.Style = XlBorder.Thin;
            r.Style.Border.Bottom.Style = XlBorder.Thin;
            r.Style.Border.Left.Style = XlBorder.Thin;
            r.Style.Border.Right.Style = XlBorder.Thin;
            r.Style.Border.Top.Color.SetColor(XBorde);
            r.Style.Border.Bottom.Color.SetColor(XBorde);
            r.Style.Border.Left.Color.SetColor(XBorde);
            r.Style.Border.Right.Color.SetColor(XBorde);
        }

        private static void XlImpresion(ExcelWorksheet ws)
        {
            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            ws.PrinterSettings.PaperSize = ePaperSize.A4;
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;
        }

        private static void XlBanner(ExcelWorksheet ws, string titulo, string subtitulo, int columnas)
        {
            ws.View.ShowGridLines = false;

            var r1 = ws.Cells[1, 1, 1, columnas];
            r1.Merge = true;
            ws.Cells[1, 1].Value = titulo;
            r1.Style.Fill.PatternType = XlFill.Solid;
            r1.Style.Fill.BackgroundColor.SetColor(XAzul);
            r1.Style.Font.Bold = true;
            r1.Style.Font.Size = 20;
            r1.Style.Font.Color.SetColor(DrawingColor.White);
            r1.Style.HorizontalAlignment = XlHAlign.Left;
            r1.Style.VerticalAlignment = XlVAlign.Center;
            r1.Style.Indent = 1;
            ws.Row(1).Height = 42;

            var r2 = ws.Cells[2, 1, 2, columnas];
            r2.Merge = true;
            ws.Cells[2, 1].Value = subtitulo;
            r2.Style.Fill.PatternType = XlFill.Solid;
            r2.Style.Fill.BackgroundColor.SetColor(XClaro);
            r2.Style.Font.Italic = true;
            r2.Style.Font.Size = 10;
            r2.Style.Font.Color.SetColor(XGris);
            r2.Style.HorizontalAlignment = XlHAlign.Left;
            r2.Style.VerticalAlignment = XlVAlign.Center;
            r2.Style.Indent = 1;
            ws.Row(2).Height = 22;
        }

        private static int XlSeccion(ExcelWorksheet ws, int fila, string texto, int columnas)
        {
            var r = ws.Cells[fila, 1, fila, columnas];
            r.Merge = true;
            ws.Cells[fila, 1].Value = texto;
            r.Style.Fill.PatternType = XlFill.Solid;
            r.Style.Fill.BackgroundColor.SetColor(XMorado);
            r.Style.Font.Bold = true;
            r.Style.Font.Size = 11;
            r.Style.Font.Color.SetColor(DrawingColor.White);
            r.Style.HorizontalAlignment = XlHAlign.Left;
            r.Style.VerticalAlignment = XlVAlign.Center;
            r.Style.Indent = 1;
            ws.Row(fila).Height = 24;
            return fila + 1;
        }

        // Lista vertical "indicador | valor"
        private static int XlKpis(ExcelWorksheet ws, int fila, List<XlKpi> items)
        {
            foreach (var k in items)
            {
                var a = ws.Cells[fila, 1];
                var b = ws.Cells[fila, 2];

                a.Value = k.Etiqueta;
                a.Style.Font.Bold = true;
                a.Style.Fill.PatternType = XlFill.Solid;
                a.Style.Fill.BackgroundColor.SetColor(XClaro);

                if (k.Valor != null) b.Value = k.Valor;
                if (k.Formato != null) b.Style.Numberformat.Format = k.Formato;
                b.Style.Font.Bold = true;
                b.Style.Font.Color.SetColor(XAzul);
                b.Style.HorizontalAlignment = XlHAlign.Right;

                XlBordes(ws.Cells[fila, 1, fila, 2]);
                fila++;
            }
            return fila + 1;
        }

        // Tabla con encabezado, filas alternadas, bordes, estados con color y fila de totales opcional.
        // Devuelve la siguiente fila libre (con una fila de aire).
        private static int XlTabla(ExcelWorksheet ws, int fila, string[] headers, string[] formatos,
                                   List<object[]> filas, int colEstado, object[] totales, bool autoFiltro)
        {
            int cols = headers.Length;

            for (int c = 0; c < cols; c++)
            {
                var h = ws.Cells[fila, c + 1];
                h.Value = headers[c];
                h.Style.Font.Bold = true;
                h.Style.Font.Color.SetColor(DrawingColor.White);
                h.Style.Fill.PatternType = XlFill.Solid;
                h.Style.Fill.BackgroundColor.SetColor(XAzul);
                h.Style.HorizontalAlignment = XlHAlign.Center;
                h.Style.VerticalAlignment = XlVAlign.Center;
                h.Style.WrapText = true;
            }
            ws.Row(fila).Height = 26;

            int f = fila + 1;

            if (filas.Count == 0)
            {
                var vacio = ws.Cells[f, 1, f, cols];
                vacio.Merge = true;
                ws.Cells[f, 1].Value = "Sin registros para el período seleccionado.";
                vacio.Style.Font.Italic = true;
                vacio.Style.Font.Color.SetColor(XGris);
                vacio.Style.HorizontalAlignment = XlHAlign.Center;
                XlBordes(ws.Cells[fila, 1, f, cols]);
                return f + 2;
            }

            int idx = 0;
            foreach (var fr in filas)
            {
                if (idx % 2 == 1)
                {
                    var banda = ws.Cells[f, 1, f, cols];
                    banda.Style.Fill.PatternType = XlFill.Solid;
                    banda.Style.Fill.BackgroundColor.SetColor(XClaro);
                }

                for (int c = 0; c < cols; c++)
                {
                    var cell = ws.Cells[f, c + 1];
                    var v = c < fr.Length ? fr[c] : null;
                    if (v != null) cell.Value = v;

                    if (formatos != null && c < formatos.Length && formatos[c] != null)
                        cell.Style.Numberformat.Format = formatos[c];
                    else if (v is DateTime)
                        cell.Style.Numberformat.Format = FmtFecha;

                    if (v is decimal || v is int || v is double || v is long)
                        cell.Style.HorizontalAlignment = XlHAlign.Right;
                    else if (v is DateTime)
                        cell.Style.HorizontalAlignment = XlHAlign.Center;
                    else
                        cell.Style.HorizontalAlignment = XlHAlign.Left;

                    cell.Style.VerticalAlignment = XlVAlign.Center;
                    if (v is string && ((string)v).Length > 40) cell.Style.WrapText = true;
                }

                if (colEstado >= 0 && colEstado < cols)
                {
                    var clase = RepFmt.Clase(Convert.ToString(fr[colEstado]));
                    var ce = ws.Cells[f, colEstado + 1];
                    ce.Style.Font.Bold = true;
                    ce.Style.HorizontalAlignment = XlHAlign.Center;
                    ce.Style.Font.Color.SetColor(XlTexto(clase));
                    ce.Style.Fill.PatternType = XlFill.Solid;
                    ce.Style.Fill.BackgroundColor.SetColor(XlFondo(clase));
                }

                idx++;
                f++;
            }

            int ultimaDatos = f - 1;

            if (totales != null)
            {
                for (int c = 0; c < cols; c++)
                {
                    var cell = ws.Cells[f, c + 1];
                    var v = c < totales.Length ? totales[c] : null;
                    if (v != null && !(v is string && (string)v == "")) cell.Value = v;
                    if (formatos != null && c < formatos.Length && formatos[c] != null && !(v is string))
                        cell.Style.Numberformat.Format = formatos[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = XlFill.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(XEncab);
                    cell.Style.HorizontalAlignment = (v is string) ? XlHAlign.Left : XlHAlign.Right;
                }
                f++;
            }

            XlBordes(ws.Cells[fila, 1, f - 1, cols]);

            if (autoFiltro)
                ws.Cells[fila, 1, ultimaDatos, cols].AutoFilter = true;

            return f + 1;
        }

        private static void XlGrafico(ExcelWorksheet ws, string nombre, string titulo, XlChart.eChartType tipo,
                                      int filaEncabezado, int cantidadFilas, int colCategorias,
                                      int[] colSeries, string[] nombresSeries,
                                      int filaPos, int colPos, int ancho, int alto)
        {
            var ch = ws.Drawings.AddChart(nombre, tipo);
            ch.Title.Text = titulo;
            ch.SetPosition(filaPos, 0, colPos, 0);
            ch.SetSize(ancho, alto);

            for (int i = 0; i < colSeries.Length; i++)
            {
                var serie = ch.Series.Add(
                    ws.Cells[filaEncabezado + 1, colSeries[i], filaEncabezado + cantidadFilas, colSeries[i]],
                    ws.Cells[filaEncabezado + 1, colCategorias, filaEncabezado + cantidadFilas, colCategorias]);
                serie.Header = nombresSeries[i];
            }

            if (colSeries.Length > 1)
                ch.Legend.Position = XlChart.eLegendPosition.Bottom;
            else
                ch.Legend.Remove();
        }

        // ── Hoja 1: Resumen ──
        private void XlHojaResumen(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Resumen");
            ws.TabColor = XAzul;
            XlAnchos(ws, 38, 22, 22, 18, 3);

            var sub = "Generado el " + vm.Generado.ToString("dd/MM/yyyy HH:mm") + "   ·   Período: " + vm.RangoTexto;
            if (vm.HayCliente) sub += "   ·   Cliente: " + vm.Cliente.NombreCompleto;
            XlBanner(ws, "CNFL · Reporte general", sub, 4);

            int fila = 4;

            // Sistema
            fila = XlSeccion(ws, fila, "Resumen general del sistema", 4);
            fila = XlKpis(ws, fila, new List<XlKpi>
            {
                new XlKpi("Clientes activos", vm.SisClientesActivos, FmtEntero),
                new XlKpi("Clientes registrados", vm.SisClientesTotal, FmtEntero),
                new XlKpi("NISEs (servicios)", vm.SisNISEs, FmtEntero),
                new XlKpi("Servicios al día", vm.SisServiciosAlDia, FmtEntero),
                new XlKpi("Servicios con deuda", vm.SisServiciosConDeuda, FmtEntero),
                new XlKpi("Facturas emitidas", vm.SisFacturas, FmtEntero),
                new XlKpi("Facturas vencidas", vm.SisFacturasVencidas, FmtEntero),
                new XlKpi("Total facturado", vm.SisFacturado, FmtMon0),
                new XlKpi("Total cobrado", vm.SisCobrado, FmtMon0),
                new XlKpi("Saldo pendiente", vm.SisPendiente, FmtMon0),
                new XlKpi("Porcentaje cobrado", vm.SisPorcentajeCobro, FmtPct),
                new XlKpi("Averías abiertas", vm.SisAveriasAbiertas, FmtEntero),
                new XlKpi("Trámites abiertos", vm.SisTramitesAbiertos, FmtEntero),
                new XlKpi("Ingresos por ventas (período)", vm.SisIngresosVentas, FmtMon0),
                new XlKpi("Compras pagadas (período)", vm.SisCompras, FmtEntero),
                new XlKpi("Ticket promedio", vm.SisTicketPromedio, FmtMon0)
            });

            fila = XlSeccion(ws, fila, "Facturación de los últimos 6 meses", 4);
            var mensual = new List<object[]>();
            foreach (var m in vm.SisMensual) mensual.Add(new object[] { m.Etiqueta, m.Valor, m.Valor2 });
            int encMensual = fila;
            fila = XlTabla(ws, fila, new[] { "Mes", "Facturado", "Cobrado" }, new string[] { null, FmtMon0, FmtMon0 }, mensual, -1, null, false);
            if (mensual.Count > 0)
                XlGrafico(ws, "chSistema", "Facturado vs cobrado", XlChart.eChartType.ColumnClustered,
                          encMensual, mensual.Count, 1, new[] { 2, 3 }, new[] { "Facturado", "Cobrado" }, 3, 5, 560, 300);

            fila = XlSeccion(ws, fila, "Clientes con mayor saldo pendiente", 4);
            var deudores = new List<object[]>();
            foreach (var d in vm.TopDeudores) deudores.Add(new object[] { d.Nombre, d.Cedula, d.Facturas, d.Monto });
            fila = XlTabla(ws, fila, new[] { "Cliente", "Cédula", "Facturas", "Saldo pendiente" },
                           new string[] { null, null, FmtEntero, FmtMon0 }, deudores, -1, null, false);

            if (!vm.HayCliente) { XlImpresion(ws); return; }

            // Cliente
            int inicioCliente = fila;
            fila = XlSeccion(ws, fila, "Cliente: " + vm.Cliente.NombreCompleto, 4);
            fila = XlKpis(ws, fila, new List<XlKpi>
            {
                new XlKpi("Estado de cuenta", vm.EstadoCuenta, null),
                new XlKpi("Facturado (período)", vm.Facturado, FmtMon0),
                new XlKpi("Pagado (período)", vm.Pagado, FmtMon0),
                new XlKpi("Pendiente (período)", vm.Pendiente, FmtMon0),
                new XlKpi("Porcentaje pagado", vm.PorcentajePagado, FmtPct),
                new XlKpi("Saldo pendiente total", vm.SaldoPendienteTotal, FmtMon0),
                new XlKpi("Monto vencido total", vm.MontoVencido, FmtMon0),
                new XlKpi("Facturas del período", vm.FacturasTotal, FmtEntero),
                new XlKpi("Averías (total)", vm.AveriasTotal, FmtEntero),
                new XlKpi("Averías abiertas", vm.AveriasAbiertas, FmtEntero),
                new XlKpi("Días promedio de resolución", vm.AveriasDiasPromedio, FmtDec1),
                new XlKpi("Trámites (total)", vm.TramitesTotal, FmtEntero),
                new XlKpi("Trámites abiertos", vm.TramitesAbiertos, FmtEntero),
                new XlKpi("Compras pagadas", vm.ComprasPagadas, FmtEntero),
                new XlKpi("Monto en compras", vm.ComprasMonto, FmtMon0),
                new XlKpi("Sesiones en la app", vm.ActSesiones, FmtEntero),
                new XlKpi("Minutos en la app", vm.ActMinutos, FmtDec1)
            });

            fila = XlSeccion(ws, fila, "Facturación del cliente · últimos 12 meses", 4);
            var mensualCli = new List<object[]>();
            foreach (var m in vm.FacturacionMensual) mensualCli.Add(new object[] { m.Etiqueta, m.Valor, m.Valor2 });
            int encCli = fila;
            fila = XlTabla(ws, fila, new[] { "Mes", "Facturado", "Pagado" }, new string[] { null, FmtMon0, FmtMon0 }, mensualCli, -1, null, false);
            if (mensualCli.Count > 0)
                XlGrafico(ws, "chCliente", "Facturado vs pagado del cliente", XlChart.eChartType.ColumnClustered,
                          encCli, mensualCli.Count, 1, new[] { 2, 3 }, new[] { "Facturado", "Pagado" }, inicioCliente, 5, 560, 300);

            XlImpresion(ws);
        }

        // ── Hoja 2: Cliente ──
        private void XlHojaCliente(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Cliente");
            ws.TabColor = XMorado;
            XlAnchos(ws, 28, 40, 16, 30, 12, 18, 14);
            XlBanner(ws, "Ficha del cliente", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 7);

            var c = vm.Cliente;
            int fila = 4;
            fila = XlSeccion(ws, fila, "Datos personales", 7);
            fila = XlKpis(ws, fila, new List<XlKpi>
            {
                new XlKpi("Nombre completo", c.NombreCompleto, null),
                new XlKpi("Cédula", c.Cedula, null),
                new XlKpi("Correo principal", c.Correo, null),
                new XlKpi("Correo secundario", string.IsNullOrEmpty(c.CorreoSecundario) ? "—" : c.CorreoSecundario, null),
                new XlKpi("Teléfono", string.IsNullOrEmpty(c.Telefono) ? "—" : c.Telefono, null),
                new XlKpi("Fecha de registro", RepFmt.Fecha(c.FechaRegistro), null),
                new XlKpi("Estado del cliente", c.Activo ? "Activo" : "Inactivo", null),
                new XlKpi("Factura electrónica", c.FacturaElectronica ? "Sí" : "No", null),
                new XlKpi("Estado de cuenta", vm.EstadoCuenta, null)
            });

            fila = XlSeccion(ws, fila, "Servicios (NISE)", 7);
            var filas = new List<object[]>();
            foreach (var s in vm.Servicios)
                filas.Add(new object[] { s.Nise, s.Tipo, s.Provincia, s.Direccion, s.Facturas, s.Pendiente, s.Estado });

            XlTabla(ws, fila, new[] { "NISE", "Tipo de servicio", "Provincia", "Dirección", "Facturas", "Saldo pendiente", "Estado" },
                    new string[] { null, null, null, null, FmtEntero, FmtMon, null }, filas, 6, null, false);
            XlImpresion(ws);
        }

        // ── Hoja 3: Facturas ──
        private void XlHojaFacturas(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Facturas");
            ws.TabColor = XAzul;
            XlAnchos(ws, 22, 16, 14, 14, 18, 14, 14);
            XlBanner(ws, "Facturas del cliente", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 7);

            var filas = new List<object[]>();
            foreach (var f in vm.Facturas)
                filas.Add(new object[] { f.Numero, f.Nise, f.Emision, f.Vencimiento, f.Monto, f.Estado, f.DiasAtraso });

            object[] totales = filas.Count > 0
                ? new object[] { "TOTAL", "", "", "", vm.Facturas.Sum(f => f.Monto), "", "" }
                : null;

            XlTabla(ws, 4, new[] { "Número", "NISE", "Emisión", "Vencimiento", "Monto", "Estado", "Días de atraso" },
                    new string[] { null, null, FmtFecha, FmtFecha, FmtMon, null, FmtEntero }, filas, 5, totales, true);

            ws.View.FreezePanes(5, 1);
            XlImpresion(ws);
        }

        // ── Hoja 4: Averías ──
        private void XlHojaAverias(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Averías");
            ws.TabColor = XMorado;
            XlAnchos(ws, 10, 20, 44, 16, 16, 16, 16);
            XlBanner(ws, "Averías reportadas", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 7);

            var filas = new List<object[]>();
            foreach (var a in vm.Averias)
                filas.Add(new object[] { a.Id, a.Tipo, a.Descripcion, a.Estado, a.Reportada, a.Actualizada, a.DiasResolucion });

            XlTabla(ws, 4, new[] { "ID", "Tipo", "Descripción", "Estado", "Reportada", "Actualizada", "Días de resolución" },
                    new string[] { null, null, null, null, FmtFecha, FmtFecha, FmtDec1 }, filas, 3, null, true);

            ws.View.FreezePanes(5, 1);
            XlImpresion(ws);
        }

        // ── Hoja 5: Trámites ──
        private void XlHojaTramites(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Trámites");
            ws.TabColor = XAzul;
            XlAnchos(ws, 10, 30, 20, 16, 20);
            XlBanner(ws, "Trámites del cliente", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 5);

            var filas = new List<object[]>();
            foreach (var t in vm.Tramites)
                filas.Add(new object[] { t.Id, t.Tipo, t.Estado, t.Solicitud, t.Actualizacion });

            XlTabla(ws, 4, new[] { "ID", "Tipo de trámite", "Estado", "Solicitado", "Última actualización" },
                    new string[] { null, null, null, FmtFecha, FmtFecha }, filas, 2, null, true);

            ws.View.FreezePanes(5, 1);
            XlImpresion(ws);
        }

        // ── Hoja 6: Compras ──
        private void XlHojaCompras(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Compras");
            ws.TabColor = XMorado;
            XlAnchos(ws, 12, 14, 16, 14, 12, 16, 14, 16);
            XlBanner(ws, "Compras de productos y servicios", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 8);

            var filas = new List<object[]>();
            foreach (var c in vm.Compras)
                filas.Add(new object[] { c.Id, c.Fecha, EtiquetaMetodo(c.Metodo), c.Estado, c.Articulos, c.Subtotal, c.Impuesto, c.Total });

            object[] totales = filas.Count > 0
                ? new object[] { "TOTAL", "", "", "", vm.Compras.Sum(c => c.Articulos), vm.Compras.Sum(c => c.Subtotal), vm.Compras.Sum(c => c.Impuesto), vm.Compras.Sum(c => c.Total) }
                : null;

            XlTabla(ws, 4, new[] { "N.º orden", "Fecha", "Método de pago", "Estado", "Artículos", "Subtotal", "Impuesto", "Total" },
                    new string[] { null, FmtFecha, null, null, FmtEntero, FmtMon, FmtMon, FmtMon }, filas, 3, totales, true);

            ws.View.FreezePanes(5, 1);
            XlImpresion(ws);
        }

        // ── Hoja 7: Actividad ──
        private void XlHojaActividad(ExcelPackage p, ReporteClienteViewModel vm)
        {
            var ws = p.Workbook.Worksheets.Add("Actividad");
            ws.TabColor = XAzul;
            XlAnchos(ws, 34, 16, 14, 16, 3);
            XlBanner(ws, "Actividad del cliente en la app", vm.Cliente.NombreCompleto + "   ·   " + vm.RangoTexto, 4);

            int fila = 4;
            fila = XlSeccion(ws, fila, "Resumen de uso", 4);
            fila = XlKpis(ws, fila, new List<XlKpi>
            {
                new XlKpi("Sesiones registradas", vm.ActSesiones, FmtEntero),
                new XlKpi("Minutos totales", vm.ActMinutos, FmtDec1),
                new XlKpi("Minutos promedio por sesión", vm.ActPromedioMin, FmtDec1),
                new XlKpi("Última actividad", RepFmt.Fecha(vm.ActUltima), null)
            });

            fila = XlSeccion(ws, fila, "Uso por sección", 4);
            var filas = new List<object[]>();
            foreach (var a in vm.Actividad)
                filas.Add(new object[] { a.Seccion, a.Minutos, a.Sesiones, a.Porcentaje });

            int enc = fila;
            XlTabla(ws, fila, new[] { "Sección", "Minutos", "Sesiones", "% del tiempo" },
                    new string[] { null, FmtDec1, FmtEntero, FmtPct }, filas, -1, null, false);

            if (filas.Count > 0)
                XlGrafico(ws, "chActividad", "Minutos por sección", XlChart.eChartType.ColumnClustered,
                          enc, filas.Count, 1, new[] { 2 }, new[] { "Minutos" }, 3, 5, 520, 300);

            XlImpresion(ws);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        //
        //   P D F   (iTextSharp)
        //
        // ═══════════════════════════════════════════════════════════════════════════

        private static readonly PdfBaseColor PAzul = new PdfBaseColor(26, 43, 107);
        private static readonly PdfBaseColor PMorado = new PdfBaseColor(91, 63, 191);
        private static readonly PdfBaseColor PGris = new PdfBaseColor(110, 115, 130);
        private static readonly PdfBaseColor PClaro = new PdfBaseColor(244, 246, 251);
        private static readonly PdfBaseColor PBorde = new PdfBaseColor(224, 228, 238);
        private static readonly PdfBaseColor PBlanco = new PdfBaseColor(255, 255, 255);
        private static readonly PdfBaseColor PNegro = new PdfBaseColor(30, 35, 55);
        private static readonly PdfBaseColor PVerde = new PdfBaseColor(4, 106, 56);
        private static readonly PdfBaseColor PRojo = new PdfBaseColor(183, 28, 28);
        private static readonly PdfBaseColor PAmbar = new PdfBaseColor(160, 105, 0);

        // Fuentes y símbolo de moneda del PDF
        private class PdfCtx
        {
            public PdfBaseFont Normal;
            public PdfBaseFont Negrita;
            public string Simbolo;

            public PdfFont F(float tamano, bool negrita, PdfBaseColor color)
            {
                return new PdfFont(negrita ? Negrita : Normal, tamano, PdfFont.NORMAL, color);
            }

            public string Mon(decimal v) { return Simbolo + RepFmt.Numero(v, 0); }
            public string Mon2(decimal v) { return Simbolo + RepFmt.Numero(v, 2); }
        }

        // Pie de página con número de página
        private class PdfPie : PdfPageEventHelper
        {
            public PdfBaseFont Fuente;
            public string Texto;

            public override void OnEndPage(PdfWriter writer, PdfDocument document)
            {
                var cb = writer.DirectContent;
                float izq = document.LeftMargin;
                float der = document.PageSize.Width - document.RightMargin;

                cb.SetColorStroke(new PdfBaseColor(224, 228, 238));
                cb.SetLineWidth(0.6f);
                cb.MoveTo(izq, 34);
                cb.LineTo(der, 34);
                cb.Stroke();

                cb.BeginText();
                cb.SetFontAndSize(Fuente, 7.5f);
                cb.SetColorFill(new PdfBaseColor(110, 115, 130));
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Texto, izq, 22, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Página " + writer.PageNumber, der, 22, 0);
                cb.EndText();
            }
        }

        // Busca una fuente de Windows que tenga el símbolo ₡; si no hay, usa "CRC " como prefijo.
        // (Helvetica estándar de PDF no incluye ₡ y saldría en blanco.)
        private PdfCtx CrearContextoPdf()
        {
            var carpeta = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var candidatos = new[]
            {
                new[] { "segoeui.ttf", "segoeuib.ttf" },
                new[] { "arial.ttf", "arialbd.ttf" },
                new[] { "calibri.ttf", "calibrib.ttf" },
                new[] { "tahoma.ttf", "tahomabd.ttf" },
                new[] { "verdana.ttf", "verdanab.ttf" }
            };

            PdfCtx respaldo = null;
            foreach (var par in candidatos)
            {
                var rutaN = Path.Combine(carpeta, par[0]);
                var rutaB = Path.Combine(carpeta, par[1]);
                if (!System.IO.File.Exists(rutaN) || !System.IO.File.Exists(rutaB)) continue;

                try
                {
                    var bn = PdfBaseFont.CreateFont(rutaN, PdfBaseFont.IDENTITY_H, PdfBaseFont.EMBEDDED);
                    var bb = PdfBaseFont.CreateFont(rutaB, PdfBaseFont.IDENTITY_H, PdfBaseFont.EMBEDDED);
                    bool tieneColon = bn.CharExists('\u20A1');
                    var ctx = new PdfCtx { Normal = bn, Negrita = bb, Simbolo = tieneColon ? "₡" : "CRC " };
                    if (tieneColon) return ctx;
                    if (respaldo == null) respaldo = ctx;
                }
                catch { }
            }

            if (respaldo != null) return respaldo;

            return new PdfCtx
            {
                Normal = PdfBaseFont.CreateFont(PdfBaseFont.HELVETICA, PdfBaseFont.CP1252, false),
                Negrita = PdfBaseFont.CreateFont(PdfBaseFont.HELVETICA_BOLD, PdfBaseFont.CP1252, false),
                Simbolo = "CRC "
            };
        }

        public ActionResult ExportarReportePDF(int? clienteId = null, string rango = "todo")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var vm = ConstruirReporteCliente(clienteId, rango);
            var c = CrearContextoPdf();

            using (var ms = new MemoryStream())
            {
                var doc = new PdfDocument(PdfPageSize.A4, 36, 36, 36, 50);
                var writer = PdfWriter.GetInstance(doc, ms);
                writer.PageEvent = new PdfPie
                {
                    Fuente = c.Normal,
                    Texto = "CNFL · Reporte general · Generado el " + vm.Generado.ToString("dd/MM/yyyy HH:mm")
                };

                doc.AddTitle("Reporte General CNFL");
                doc.AddAuthor("CNFL - Panel Administrativo");
                doc.AddCreator("CNFL Clientes");
                doc.Open();

                PdfBanner(doc, c, vm);

                // ── Página 1: sistema ──
                PdfSeccion(doc, c, "Resumen general del sistema");
                doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                {
                    KV("Clientes activos", RepFmt.Numero(vm.SisClientesActivos, 0)),
                    KV("NISEs (servicios)", RepFmt.Numero(vm.SisNISEs, 0)),
                    KV("Facturas emitidas", RepFmt.Numero(vm.SisFacturas, 0)),
                    KV("Servicios con deuda", RepFmt.Numero(vm.SisServiciosConDeuda, 0)),
                    KV("Total facturado", c.Mon(vm.SisFacturado)),
                    KV("Total cobrado", c.Mon(vm.SisCobrado)),
                    KV("Saldo pendiente", c.Mon(vm.SisPendiente)),
                    KV("Porcentaje cobrado", RepFmt.Pct(vm.SisPorcentajeCobro)),
                    KV("Facturas vencidas", RepFmt.Numero(vm.SisFacturasVencidas, 0)),
                    KV("Averías abiertas", RepFmt.Numero(vm.SisAveriasAbiertas, 0)),
                    KV("Trámites abiertos", RepFmt.Numero(vm.SisTramitesAbiertos, 0)),
                    KV("Ingresos por ventas", c.Mon(vm.SisIngresosVentas))
                }, 4));

                PdfSeccion(doc, c, "Facturación de los últimos 6 meses");
                try
                {
                    var bytes = GenerarGraficoBarras(
                        vm.SisMensual.Select(x => x.Etiqueta).ToArray(),
                        vm.SisMensual.Select(x => x.Valor).ToArray(),
                        vm.SisMensual.Select(x => x.Valor2).ToArray(),
                        "Facturado vs cobrado (colones)", "Facturado", "Cobrado",
                        DrawingColor.FromArgb(26, 43, 107), DrawingColor.FromArgb(4, 150, 90));
                    PdfGrafico(doc, bytes);
                }
                catch { }

                PdfSeccion(doc, c, "Clientes con mayor saldo pendiente");
                var filasDeudores = new List<string[]>();
                foreach (var d in vm.TopDeudores)
                    filasDeudores.Add(new[] { d.Nombre, d.Cedula, d.Facturas.ToString(), c.Mon(d.Monto) });
                doc.Add(PdfTablaDatos(c, new[] { "Cliente", "Cédula", "Facturas", "Saldo pendiente" },
                    new float[] { 40, 22, 14, 24 }, new[] { 0, 1, 1, 2 }, filasDeudores, -1));

                if (vm.HayCliente)
                {
                    var cli = vm.Cliente;

                    // ── Página 2: cliente + facturación ──
                    doc.NewPage();
                    PdfSeccion(doc, c, "Detalle del cliente");
                    doc.Add(PdfInfo(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Nombre", cli.NombreCompleto),
                        KV("Cédula", cli.Cedula),
                        KV("Correo", cli.Correo),
                        KV("Correo secundario", Vacio(cli.CorreoSecundario)),
                        KV("Teléfono", Vacio(cli.Telefono)),
                        KV("Fecha de registro", RepFmt.Fecha(cli.FechaRegistro)),
                        KV("Estado del cliente", cli.Activo ? "Activo" : "Inactivo"),
                        KV("Factura electrónica", cli.FacturaElectronica ? "Sí" : "No"),
                        KV("Estado de cuenta", vm.EstadoCuenta),
                        KV("Servicios (NISE)", vm.Servicios.Count.ToString())
                    }));

                    PdfSeccion(doc, c, "Servicios (NISE)");
                    var filasServ = new List<string[]>();
                    foreach (var s in vm.Servicios)
                        filasServ.Add(new[] { s.Nise, Vacio(s.Tipo), Vacio(s.Provincia), s.Facturas.ToString(), c.Mon(s.Pendiente), s.Estado });
                    doc.Add(PdfTablaDatos(c, new[] { "NISE", "Tipo", "Provincia", "Facturas", "Saldo pendiente", "Estado" },
                        new float[] { 20, 20, 18, 10, 18, 14 }, new[] { 0, 0, 0, 1, 2, 1 }, filasServ, 5));

                    PdfSeccion(doc, c, "Facturación · " + vm.RangoTexto);
                    doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Facturado", c.Mon(vm.Facturado)),
                        KV("Pagado", c.Mon(vm.Pagado)),
                        KV("Pendiente", c.Mon(vm.Pendiente)),
                        KV("Porcentaje pagado", RepFmt.Pct(vm.PorcentajePagado)),
                        KV("Saldo pendiente total", c.Mon(vm.SaldoPendienteTotal)),
                        KV("Monto vencido total", c.Mon(vm.MontoVencido)),
                        KV("Facturas del período", RepFmt.Numero(vm.FacturasTotal, 0)),
                        KV("Facturas vencidas", RepFmt.Numero(vm.FacturasVencidas, 0))
                    }, 4));

                    try
                    {
                        var bytes = GenerarGraficoBarras(
                            vm.FacturacionMensual.Select(x => x.Etiqueta).ToArray(),
                            vm.FacturacionMensual.Select(x => x.Valor).ToArray(),
                            vm.FacturacionMensual.Select(x => x.Valor2).ToArray(),
                            "Facturación del cliente, últimos 12 meses (colones)", "Facturado", "Pagado",
                            DrawingColor.FromArgb(91, 63, 191), DrawingColor.FromArgb(4, 150, 90));
                        PdfGrafico(doc, bytes);
                    }
                    catch { }

                    PdfSeccion(doc, c, "Facturas");
                    const int maxFacturas = 40;
                    var filasFact = new List<string[]>();
                    foreach (var f in vm.Facturas.Take(maxFacturas))
                        filasFact.Add(new[] { f.Numero, f.Nise, RepFmt.Fecha(f.Emision), RepFmt.Fecha(f.Vencimiento), c.Mon2(f.Monto), f.Estado });
                    doc.Add(PdfTablaDatos(c, new[] { "Número", "NISE", "Emisión", "Vencimiento", "Monto", "Estado" },
                        new float[] { 22, 16, 15, 15, 18, 14 }, new[] { 0, 1, 1, 1, 2, 1 }, filasFact, 5));
                    if (vm.Facturas.Count > maxFacturas)
                        doc.Add(PdfNota(c, "Se muestran las " + maxFacturas + " facturas más recientes de " + vm.Facturas.Count + ". El Excel incluye el detalle completo."));

                    // ── Página siguiente: averías y trámites ──
                    doc.NewPage();
                    PdfSeccion(doc, c, "Averías · " + vm.RangoTexto);
                    doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Total", RepFmt.Numero(vm.AveriasTotal, 0)),
                        KV("Abiertas", RepFmt.Numero(vm.AveriasAbiertas, 0)),
                        KV("Resueltas", RepFmt.Numero(vm.AveriasResueltas, 0)),
                        KV("Promedio de resolución", RepFmt.Numero((decimal)vm.AveriasDiasPromedio, 1) + " días")
                    }, 4));
                    if (vm.AveriasPorEstado.Count > 0)
                        doc.Add(PdfNota(c, "Por estado: " + ResumenGrupos(vm.AveriasPorEstado)));

                    const int maxAverias = 25;
                    var filasAv = new List<string[]>();
                    foreach (var a in vm.Averias.Take(maxAverias))
                        filasAv.Add(new[]
                        {
                            Vacio(a.Id), Vacio(a.Tipo), Recortar(a.Descripcion, 70), a.Estado,
                            RepFmt.Fecha(a.Reportada),
                            a.DiasResolucion.HasValue ? RepFmt.Numero((decimal)a.DiasResolucion.Value, 1) + " d" : "—"
                        });
                    doc.Add(PdfTablaDatos(c, new[] { "ID", "Tipo", "Descripción", "Estado", "Reportada", "Resolución" },
                        new float[] { 8, 17, 33, 14, 14, 14 }, new[] { 1, 0, 0, 1, 1, 1 }, filasAv, 3));
                    if (vm.Averias.Count > maxAverias)
                        doc.Add(PdfNota(c, "Se muestran las " + maxAverias + " averías más recientes de " + vm.Averias.Count + ". El Excel incluye el detalle completo."));

                    PdfSeccion(doc, c, "Trámites · " + vm.RangoTexto);
                    doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Total", RepFmt.Numero(vm.TramitesTotal, 0)),
                        KV("Abiertos", RepFmt.Numero(vm.TramitesAbiertos, 0)),
                        KV("Completados", RepFmt.Numero(vm.TramitesCompletados, 0))
                    }, 4));
                    if (vm.TramitesPorEstado.Count > 0)
                        doc.Add(PdfNota(c, "Por estado: " + ResumenGrupos(vm.TramitesPorEstado)));

                    const int maxTramites = 25;
                    var filasTr = new List<string[]>();
                    foreach (var t in vm.Tramites.Take(maxTramites))
                        filasTr.Add(new[] { Vacio(t.Id), Vacio(t.Tipo), t.Estado, RepFmt.Fecha(t.Solicitud), RepFmt.Fecha(t.Actualizacion) });
                    doc.Add(PdfTablaDatos(c, new[] { "ID", "Tipo de trámite", "Estado", "Solicitado", "Actualizado" },
                        new float[] { 10, 32, 20, 19, 19 }, new[] { 1, 0, 1, 1, 1 }, filasTr, 2));
                    if (vm.Tramites.Count > maxTramites)
                        doc.Add(PdfNota(c, "Se muestran los " + maxTramites + " trámites más recientes de " + vm.Tramites.Count + ". El Excel incluye el detalle completo."));

                    // ── Página siguiente: compras y actividad ──
                    doc.NewPage();
                    PdfSeccion(doc, c, "Compras · " + vm.RangoTexto);
                    doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Compras pagadas", RepFmt.Numero(vm.ComprasPagadas, 0)),
                        KV("Monto total", c.Mon(vm.ComprasMonto)),
                        KV("Ticket promedio", c.Mon(vm.ComprasTicket))
                    }, 4));

                    if (vm.ComprasPorMetodo.Count > 0)
                    {
                        var filasMet = new List<string[]>();
                        foreach (var m in vm.ComprasPorMetodo)
                            filasMet.Add(new[] { m.Nombre, m.Cantidad.ToString(), c.Mon(m.Monto) });
                        doc.Add(PdfTablaDatos(c, new[] { "Método de pago", "Compras", "Monto" },
                            new float[] { 50, 20, 30 }, new[] { 0, 1, 2 }, filasMet, -1));
                    }

                    const int maxCompras = 25;
                    var filasCo = new List<string[]>();
                    foreach (var o in vm.Compras.Take(maxCompras))
                        filasCo.Add(new[] { o.Id.ToString(), RepFmt.Fecha(o.Fecha), EtiquetaMetodo(o.Metodo), o.Estado, o.Articulos.ToString(), c.Mon2(o.Total) });
                    doc.Add(PdfTablaDatos(c, new[] { "Orden", "Fecha", "Método", "Estado", "Artículos", "Total" },
                        new float[] { 10, 16, 18, 16, 14, 26 }, new[] { 1, 1, 0, 1, 1, 2 }, filasCo, 3));
                    if (vm.Compras.Count > maxCompras)
                        doc.Add(PdfNota(c, "Se muestran las " + maxCompras + " compras más recientes de " + vm.Compras.Count + ". El Excel incluye el detalle completo."));

                    PdfSeccion(doc, c, "Actividad en la app · " + vm.RangoTexto);
                    doc.Add(PdfKpiGrid(c, new List<KeyValuePair<string, string>>
                    {
                        KV("Sesiones", RepFmt.Numero(vm.ActSesiones, 0)),
                        KV("Minutos totales", RepFmt.Numero(vm.ActMinutos, 1)),
                        KV("Promedio por sesión", RepFmt.Numero(vm.ActPromedioMin, 1) + " min"),
                        KV("Última actividad", RepFmt.Fecha(vm.ActUltima))
                    }, 4));

                    if (vm.Actividad.Count > 0)
                    {
                        try
                        {
                            var top = vm.Actividad.Take(8).ToList();
                            var bytes = GenerarGraficoBarras(
                                top.Select(x => Recortar(x.Seccion, 14)).ToArray(),
                                top.Select(x => x.Minutos).ToArray(),
                                null,
                                "Minutos por sección", "Minutos", null,
                                DrawingColor.FromArgb(91, 63, 191), DrawingColor.FromArgb(91, 63, 191));
                            PdfGrafico(doc, bytes);
                        }
                        catch { }
                    }

                    var filasAct = new List<string[]>();
                    foreach (var a in vm.Actividad)
                        filasAct.Add(new[] { a.Seccion, RepFmt.Numero(a.Minutos, 1), a.Sesiones.ToString(), RepFmt.Pct(a.Porcentaje) });
                    doc.Add(PdfTablaDatos(c, new[] { "Sección", "Minutos", "Sesiones", "% del tiempo" },
                        new float[] { 40, 20, 20, 20 }, new[] { 0, 2, 1, 2 }, filasAct, -1));
                }

                doc.Close();
                return File(ms.ToArray(), "application/pdf", NombreArchivo(vm, "pdf"));
            }
        }

        // ── Helpers de PDF ──
        private static KeyValuePair<string, string> KV(string clave, string valor)
        {
            return new KeyValuePair<string, string>(clave, valor);
        }

        private static string Vacio(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "—" : s;
        }

        private static string Recortar(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "—";
            return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
        }

        private static string ResumenGrupos(List<RepGrupo> grupos)
        {
            return string.Join("   ·   ", grupos.Select(g => g.Nombre + ": " + g.Cantidad).ToArray());
        }

        private static PdfBaseColor PColor(string clase)
        {
            switch (clase)
            {
                case "green": return PVerde;
                case "red": return PRojo;
                case "amber": return PAmbar;
                default: return PAzul;
            }
        }

        private static int Alin(int a)
        {
            if (a == 0) return PdfElement.ALIGN_LEFT;
            if (a == 2) return PdfElement.ALIGN_RIGHT;
            return PdfElement.ALIGN_CENTER;
        }

        private void PdfBanner(PdfDocument doc, PdfCtx c, ReporteClienteViewModel vm)
        {
            var t = new PdfTable(2) { WidthPercentage = 100, SpacingAfter = 6 };
            t.SetWidths(new float[] { 66, 34 });

            var izq = new PdfCell();
            izq.BackgroundColor = PAzul;
            izq.Border = 0;
            izq.Padding = 16;
            izq.AddElement(new PdfParagraph("CNFL · Reporte general", c.F(20, true, PBlanco)));
            izq.AddElement(new PdfParagraph("Resumen del sistema y detalle por cliente", c.F(9, false, new PdfBaseColor(200, 210, 245))) { SpacingBefore = 3 });
            t.AddCell(izq);

            var der = new PdfCell();
            der.BackgroundColor = PAzul;
            der.Border = 0;
            der.Padding = 16;
            der.HorizontalAlignment = PdfElement.ALIGN_RIGHT;
            der.AddElement(new PdfParagraph("Período", c.F(7.5f, false, new PdfBaseColor(200, 210, 245))) { Alignment = PdfElement.ALIGN_RIGHT });
            der.AddElement(new PdfParagraph(vm.RangoTexto, c.F(10, true, PBlanco)) { Alignment = PdfElement.ALIGN_RIGHT, SpacingAfter = 4 });
            der.AddElement(new PdfParagraph("Generado", c.F(7.5f, false, new PdfBaseColor(200, 210, 245))) { Alignment = PdfElement.ALIGN_RIGHT });
            der.AddElement(new PdfParagraph(vm.Generado.ToString("dd/MM/yyyy HH:mm"), c.F(10, true, PBlanco)) { Alignment = PdfElement.ALIGN_RIGHT });
            t.AddCell(der);

            doc.Add(t);
        }

        private void PdfSeccion(PdfDocument doc, PdfCtx c, string titulo)
        {
            var t = new PdfTable(1) { WidthPercentage = 100, SpacingBefore = 10, SpacingAfter = 6 };
            var cell = new PdfCell(new PdfPhrase(titulo, c.F(12, true, PMorado)));
            cell.Border = PdfRect.BOTTOM_BORDER;
            cell.BorderColor = PMorado;
            cell.BorderWidth = 1.2f;
            cell.PaddingLeft = 0;
            cell.PaddingBottom = 4;
            t.AddCell(cell);
            doc.Add(t);
        }

        private PdfParagraph PdfNota(PdfCtx c, string texto)
        {
            var p = new PdfParagraph(texto, c.F(8, false, PGris));
            p.SpacingAfter = 6;
            return p;
        }

        private PdfTable PdfKpiGrid(PdfCtx c, List<KeyValuePair<string, string>> items, int columnas)
        {
            var t = new PdfTable(columnas) { WidthPercentage = 100, SpacingAfter = 4 };
            int n = 0;

            foreach (var it in items)
            {
                var cell = new PdfCell();
                cell.BackgroundColor = PClaro;
                cell.BorderColor = PBlanco;
                cell.BorderWidth = 3f;
                cell.Padding = 7;
                cell.PaddingLeft = 9;
                cell.AddElement(new PdfParagraph(it.Key, c.F(7.5f, false, PGris)) { SpacingAfter = 2 });
                cell.AddElement(new PdfParagraph(string.IsNullOrEmpty(it.Value) ? "—" : it.Value, c.F(12, true, PAzul)));
                t.AddCell(cell);
                n++;
            }

            while (n % columnas != 0)
            {
                var vacio = new PdfCell(new PdfPhrase(" "));
                vacio.Border = 0;
                t.AddCell(vacio);
                n++;
            }
            return t;
        }

        private PdfTable PdfInfo(PdfCtx c, List<KeyValuePair<string, string>> items)
        {
            var t = new PdfTable(4) { WidthPercentage = 100, SpacingAfter = 4 };
            t.SetWidths(new float[] { 17, 33, 17, 33 });
            int n = 0;

            foreach (var it in items)
            {
                var etiqueta = new PdfCell(new PdfPhrase(it.Key, c.F(8, true, PGris)));
                etiqueta.BackgroundColor = PClaro;
                etiqueta.BorderColor = PBorde;
                etiqueta.BorderWidth = 0.5f;
                etiqueta.Padding = 6;
                t.AddCell(etiqueta);

                var valor = new PdfCell(new PdfPhrase(string.IsNullOrEmpty(it.Value) ? "—" : it.Value, c.F(9, false, PNegro)));
                valor.BorderColor = PBorde;
                valor.BorderWidth = 0.5f;
                valor.Padding = 6;
                t.AddCell(valor);
                n++;
            }

            if (n % 2 == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    var vacio = new PdfCell(new PdfPhrase(" "));
                    vacio.BorderColor = PBorde;
                    vacio.BorderWidth = 0.5f;
                    t.AddCell(vacio);
                }
            }
            return t;
        }

        // alin: 0 = izquierda, 1 = centro, 2 = derecha
        private PdfTable PdfTablaDatos(PdfCtx c, string[] headers, float[] anchos, int[] alin, List<string[]> filas, int colEstado)
        {
            var t = new PdfTable(headers.Length) { WidthPercentage = 100, HeaderRows = 1, SpacingAfter = 6 };
            t.SetWidths(anchos);

            var fH = c.F(8, true, PBlanco);
            for (int i = 0; i < headers.Length; i++)
            {
                var h = new PdfCell(new PdfPhrase(headers[i], fH));
                h.BackgroundColor = PAzul;
                h.BorderColor = PAzul;
                h.Padding = 5;
                h.PaddingTop = 6;
                h.PaddingBottom = 6;
                h.HorizontalAlignment = Alin(alin[i]);
                h.VerticalAlignment = PdfElement.ALIGN_MIDDLE;
                t.AddCell(h);
            }

            if (filas.Count == 0)
            {
                var vacio = new PdfCell(new PdfPhrase("Sin registros para el período seleccionado.", c.F(8.5f, false, PGris)));
                vacio.Colspan = headers.Length;
                vacio.Padding = 10;
                vacio.HorizontalAlignment = PdfElement.ALIGN_CENTER;
                vacio.BackgroundColor = PClaro;
                vacio.BorderColor = PBorde;
                vacio.BorderWidth = 0.5f;
                t.AddCell(vacio);
                return t;
            }

            var fN = c.F(8.5f, false, PNegro);
            int idx = 0;
            foreach (var fila in filas)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    var txt = i < fila.Length ? (fila[i] ?? "") : "";
                    var fuente = fN;
                    if (i == colEstado) fuente = c.F(8.5f, true, PColor(RepFmt.Clase(txt)));

                    var cell = new PdfCell(new PdfPhrase(txt, fuente));
                    cell.Padding = 5;
                    cell.BorderColor = PBorde;
                    cell.BorderWidth = 0.5f;
                    cell.HorizontalAlignment = Alin(alin[i]);
                    cell.VerticalAlignment = PdfElement.ALIGN_MIDDLE;
                    if (idx % 2 == 1) cell.BackgroundColor = PClaro;
                    t.AddCell(cell);
                }
                idx++;
            }
            return t;
        }

        private void PdfGrafico(PdfDocument doc, byte[] png)
        {
            var img = PdfImage.GetInstance(png);
            img.ScaleToFit(520f, 200f);
            img.Alignment = PdfElement.ALIGN_CENTER;
            img.SpacingAfter = 6f;
            doc.Add(img);
        }

        // ── Gráfico de barras (1 o 2 series) como PNG ──
        private static string Compacto(decimal v)
        {
            var a = Math.Abs(v);
            var inv = CultureInfo.InvariantCulture;
            if (a >= 1000000m) return (v / 1000000m).ToString("0.#", inv) + "M";
            if (a >= 1000m) return (v / 1000m).ToString("0.#", inv) + "K";
            return v.ToString("0.#", inv);
        }

        private byte[] GenerarGraficoBarras(string[] etiquetas, decimal[] serie1, decimal[] serie2, string titulo,
                                            string leyenda1, string leyenda2, DrawingColor color1, DrawingColor color2)
        {
            int ancho = 900, alto = 340;

            using (var bmp = new DrawingBitmap(ancho, alto))
            using (var g = DrawingGraphics.FromImage(bmp))
            {
                g.SmoothingMode = DrawingSmoothingMode.AntiAlias;
                g.TextRenderingHint = DrawingTextRenderingHint.AntiAliasGridFit;
                g.Clear(DrawingColor.White);

                int mIzq = 78, mDer = 24, mSup = 66, mInf = 46;
                int aG = ancho - mIzq - mDer;
                int hG = alto - mSup - mInf;

                int n = etiquetas.Length;
                int k = serie2 == null ? 1 : 2;

                decimal max = serie1.Length > 0 ? serie1.Max() : 0m;
                if (serie2 != null && serie2.Length > 0) max = Math.Max(max, serie2.Max());
                if (max <= 0) max = 1;

                using (var fTit = new DrawingFont("Arial", 15, DrawingFontStyle.Bold))
                    g.DrawString(titulo, fTit, System.Drawing.Brushes.Black, mIzq, 12);

                // Leyenda
                using (var fLeg = new DrawingFont("Arial", 10, DrawingFontStyle.Regular))
                {
                    using (var b1 = new DrawingBrush(color1))
                        g.FillRectangle(b1, mIzq, 42, 12, 12);
                    g.DrawString(leyenda1, fLeg, System.Drawing.Brushes.DimGray, mIzq + 17, 39);

                    if (serie2 != null)
                    {
                        using (var b2 = new DrawingBrush(color2))
                            g.FillRectangle(b2, mIzq + 150, 42, 12, 12);
                        g.DrawString(leyenda2, fLeg, System.Drawing.Brushes.DimGray, mIzq + 167, 39);
                    }
                }

                // Líneas guía y eje Y
                using (var pen = new DrawingPen(DrawingColor.FromArgb(225, 229, 240), 1))
                using (var fEje = new DrawingFont("Arial", 9, DrawingFontStyle.Regular))
                using (var sf = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Far })
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        int y = mSup + hG * i / 4;
                        g.DrawLine(pen, mIzq, y, ancho - mDer, y);
                        var val = max * (4 - i) / 4m;
                        g.DrawString(Compacto(val), fEje, System.Drawing.Brushes.DimGray,
                            new System.Drawing.RectangleF(0, y - 8, mIzq - 8, 16), sf);
                    }
                }

                // Barras
                double slot = aG / (double)Math.Max(n, 1);
                double barW = slot * 0.72 / k;
                double gap = slot * 0.28;
                bool valoresVisibles = n * k <= 16;

                using (var br1 = new DrawingBrush(color1))
                using (var br2 = new DrawingBrush(color2))
                using (var fEt = new DrawingFont("Arial", 9, DrawingFontStyle.Regular))
                using (var fVal = new DrawingFont("Arial", 8, DrawingFontStyle.Bold))
                {
                    for (int i = 0; i < n; i++)
                    {
                        double x0 = mIzq + i * slot + gap / 2;

                        for (int j = 0; j < k; j++)
                        {
                            var serie = j == 0 ? serie1 : serie2;
                            var valor = i < serie.Length ? serie[i] : 0m;
                            float h = (float)((double)valor / (double)max * hG);
                            float x = (float)(x0 + j * barW);
                            float y = mSup + hG - h;

                            g.FillRectangle(j == 0 ? br1 : br2, x, y, (float)barW - 1f, h);

                            if (valoresVisibles && valor > 0)
                            {
                                var txt = Compacto(valor);
                                var sz = g.MeasureString(txt, fVal);
                                g.DrawString(txt, fVal, System.Drawing.Brushes.Black,
                                    x + ((float)barW - sz.Width) / 2f, y - sz.Height);
                            }
                        }

                        var et = etiquetas[i];
                        var se = g.MeasureString(et, fEt);
                        g.DrawString(et, fEt, System.Drawing.Brushes.DimGray,
                            (float)(x0 + (barW * k - se.Width) / 2), mSup + hG + 8);
                    }
                }

                using (var penBase = new DrawingPen(DrawingColor.FromArgb(26, 43, 107), 2))
                    g.DrawLine(penBase, mIzq, mSup + hG, ancho - mDer, mSup + hG);

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, DrawingImageFormat.Png);
                    return ms.ToArray();
                }
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
            ViewBag.FotoAdmin = Session["AdminFoto"] as string;

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // SUBIR FOTO DE PERFIL DEL ADMIN
        // ═══════════════════════════════════════════════════════════
        [HttpPost]
        public JsonResult SubirFotoAdmin(System.Web.HttpPostedFileBase fotoAdmin)
        {
            if (!EsAdmin())
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            if (fotoAdmin == null || fotoAdmin.ContentLength == 0)
                return Json(new { ok = false, mensaje = "No se recibió ninguna imagen." });

            if (fotoAdmin.ContentLength > 4 * 1024 * 1024)
                return Json(new { ok = false, mensaje = "La imagen no puede superar 4 MB." });

            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(fotoAdmin.FileName).ToLowerInvariant();

            if (Array.IndexOf(extensionesValidas, extension) < 0)
                return Json(new { ok = false, mensaje = "Formato no válido. Usá JPG, PNG, GIF o WEBP." });

            try
            {
                var carpetaDestino = Server.MapPath("~/Content/img/admin");
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                var nombreArchivo = "admin_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;
                var rutaFisica = Path.Combine(carpetaDestino, nombreArchivo);

                fotoAdmin.SaveAs(rutaFisica);

                var rutaVirtual = "~/Content/img/admin/" + nombreArchivo;
                Session["AdminFoto"] = rutaVirtual;

                return Json(new
                {
                    ok = true,
                    mensaje = "Foto actualizada correctamente.",
                    ruta = Url.Content(rutaVirtual)
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al guardar la imagen: " + ex.Message });
            }
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