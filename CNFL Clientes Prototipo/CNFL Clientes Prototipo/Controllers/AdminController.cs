using System;
using System.Collections.Generic;
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
        // REPORTES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Reportes()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;

            var consumos = new List<ConsumoMensualDto>();
            for (int i = 5; i >= 0; i--)
            {
                var fecha = hoy.AddMonths(-i);
                var inicio = new DateTime(fecha.Year, fecha.Month, 1);
                var fin = inicio.AddMonths(1);

                var facturasMes = _db.Facturas
                    .Where(f => f.FechaEmision >= inicio && f.FechaEmision < fin)
                    .ToList();

                consumos.Add(new ConsumoMensualDto
                {
                    mes = inicio.ToString("MMM"),
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

                var wsC = wb.Worksheets.Add("Clientes");
                wsC.Cell("A1").Value = "LISTADO DE CLIENTES";
                wsC.Range("A1:G1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsC.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsC.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hC = { "ID", "Cédula", "Nombre", "Apellidos", "Correo", "Teléfono", "Estado" };
                for (int i = 0; i < hC.Length; i++)
                {
                    wsC.Cell(3, i + 1).Value = hC[i];
                    wsC.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var clientes = _db.Usuarios.OrderBy(u => u.Nombre).ToList();
                int rc = 4;
                foreach (var c in clientes)
                {
                    wsC.Cell(rc, 1).Value = c.UsuarioId;
                    wsC.Cell(rc, 2).Value = c.Cedula;
                    wsC.Cell(rc, 3).Value = c.Nombre;
                    wsC.Cell(rc, 4).Value = c.Apellidos;
                    wsC.Cell(rc, 5).Value = c.Correo;
                    wsC.Cell(rc, 6).Value = c.Telefono;
                    wsC.Cell(rc, 7).Value = c.Activo ? "Activo" : "Inactivo";
                    rc++;
                }
                wsC.Columns().AdjustToContents();

                var wsN = wb.Worksheets.Add("NISEs");
                wsN.Cell("A1").Value = "LISTADO DE NISEs";
                wsN.Range("A1:H1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsN.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsN.Range("A1:H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hN = { "ID", "Número", "Cliente", "Dirección", "Provincia", "Cantón", "Distrito", "Tipo Servicio" };
                for (int i = 0; i < hN.Length; i++)
                {
                    wsN.Cell(3, i + 1).Value = hN[i];
                    wsN.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var nises = _db.NISEs.ToList();
                int rn = 4;
                foreach (var n in nises)
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == n.UsuarioId);
                    wsN.Cell(rn, 1).Value = n.NiseId;
                    wsN.Cell(rn, 2).Value = n.NumeroNise;
                    wsN.Cell(rn, 3).Value = cli != null ? (cli.Nombre + " " + cli.Apellidos) : "";
                    wsN.Cell(rn, 4).Value = n.Direccion;
                    wsN.Cell(rn, 5).Value = n.Provincia;
                    wsN.Cell(rn, 6).Value = n.Canton;
                    wsN.Cell(rn, 7).Value = n.Distrito;
                    wsN.Cell(rn, 8).Value = n.TipoServicio;
                    rn++;
                }
                wsN.Columns().AdjustToContents();

                var wsF = wb.Worksheets.Add("Facturas");
                wsF.Cell("A1").Value = "LISTADO DE FACTURAS";
                wsF.Range("A1:F1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsF.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsF.Range("A1:F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hF = { "Nº Factura", "NISE", "Monto", "Emisión", "Vencimiento", "Estado" };
                for (int i = 0; i < hF.Length; i++)
                {
                    wsF.Cell(3, i + 1).Value = hF[i];
                    wsF.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var facturas = _db.Facturas.OrderByDescending(f => f.FechaEmision).ToList();
                int rf = 4;
                foreach (var f in facturas)
                {
                    var nis = _db.NISEs.FirstOrDefault(n => n.NiseId == f.NiseId);
                    wsF.Cell(rf, 1).Value = f.NumeroFactura;
                    wsF.Cell(rf, 2).Value = nis != null ? nis.NumeroNise : "";
                    wsF.Cell(rf, 3).Value = f.Monto;
                    wsF.Cell(rf, 3).Style.NumberFormat.Format = "₡#,##0.00";
                    wsF.Cell(rf, 4).Value = f.FechaEmision;
                    wsF.Cell(rf, 4).Style.DateFormat.Format = "dd/MM/yyyy";
                    wsF.Cell(rf, 5).Value = f.FechaVencimiento;
                    wsF.Cell(rf, 5).Style.DateFormat.Format = "dd/MM/yyyy";
                    wsF.Cell(rf, 6).Value = f.Pagada ? "Pagada" : "Pendiente";
                    rf++;
                }
                wsF.Columns().AdjustToContents();

                var wsA = wb.Worksheets.Add("Averías");
                wsA.Cell("A1").Value = "LISTADO DE AVERÍAS";
                wsA.Range("A1:G1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsA.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsA.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hA = { "ID", "Tipo", "Cliente", "NISE", "Descripción", "Estado", "Fecha reporte" };
                for (int i = 0; i < hA.Length; i++)
                {
                    wsA.Cell(3, i + 1).Value = hA[i];
                    wsA.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();
                int ra = 4;
                foreach (var a in averias)
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == a.UsuarioId);
                    var nis = _db.NISEs.FirstOrDefault(n => n.NiseId == a.NiseId);
                    wsA.Cell(ra, 1).Value = a.AveriaId;
                    wsA.Cell(ra, 2).Value = a.Tipo;
                    wsA.Cell(ra, 3).Value = cli != null ? (cli.Nombre + " " + cli.Apellidos) : "";
                    wsA.Cell(ra, 4).Value = nis != null ? nis.NumeroNise : "";
                    wsA.Cell(ra, 5).Value = a.Descripcion;
                    wsA.Cell(ra, 6).Value = a.Estado;
                    wsA.Cell(ra, 7).Value = a.FechaReporte;
                    wsA.Cell(ra, 7).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                    ra++;
                }
                wsA.Columns().AdjustToContents();

                var wsT = wb.Worksheets.Add("Trámites");
                wsT.Cell("A1").Value = "LISTADO DE TRÁMITES";
                wsT.Range("A1:F1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsT.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsT.Range("A1:F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hT = { "ID", "Tipo", "Cliente", "Estado", "Referencia", "Fecha" };
                for (int i = 0; i < hT.Length; i++)
                {
                    wsT.Cell(3, i + 1).Value = hT[i];
                    wsT.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var tramites = _db.Tramites.OrderByDescending(t => t.FechaSolicitud).ToList();
                int rt = 4;
                foreach (var t in tramites)
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == t.UsuarioId);
                    wsT.Cell(rt, 1).Value = t.TramiteId;
                    wsT.Cell(rt, 2).Value = t.Tipo;
                    wsT.Cell(rt, 3).Value = cli != null ? (cli.Nombre + " " + cli.Apellidos) : "";
                    wsT.Cell(rt, 4).Value = t.Estado;
                    wsT.Cell(rt, 5).Value = t.NumeroReferencia;
                    wsT.Cell(rt, 6).Value = t.FechaSolicitud;
                    wsT.Cell(rt, 6).Style.DateFormat.Format = "dd/MM/yyyy";
                    rt++;
                }
                wsT.Columns().AdjustToContents();

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
                var subtitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(91, 63, 191));
                var texto = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var textoBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var textoBlanco = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);

                var headerTable = new PdfPTable(1) { WidthPercentage = 100 };
                var headerCell = new PdfPCell(new Phrase("CNFL · Reporte Administrativo", titulo))
                {
                    BackgroundColor = new BaseColor(238, 240, 255),
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    Padding = 14,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                headerTable.AddCell(headerCell);
                doc.Add(headerTable);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), texto));
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph("1. Resumen General", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaResumen = new PdfPTable(2) { WidthPercentage = 100 };
                tablaResumen.SetWidths(new float[] { 60, 40 });

                tablaResumen.AddCell(new PdfPCell(new Phrase("Métrica", textoBlanco))
                { BackgroundColor = new BaseColor(26, 43, 107), Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });
                tablaResumen.AddCell(new PdfPCell(new Phrase("Valor", textoBlanco))
                { BackgroundColor = new BaseColor(26, 43, 107), Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });

                var filasResumen = new List<KeyValuePair<string, string>>
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
                    new KeyValuePair<string, string>("Trámites abiertos", _db.Tramites.Count(t => t.Estado != "Completado" && t.Estado != "Cerrado").ToString())
                };

                foreach (var f in filasResumen)
                {
                    tablaResumen.AddCell(new PdfPCell(new Phrase(f.Key, texto)) { Padding = 6, BackgroundColor = new BaseColor(249, 250, 252) });
                    tablaResumen.AddCell(new PdfPCell(new Phrase(f.Value, textoBold)) { Padding = 6, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(tablaResumen);
                doc.NewPage();

                doc.Add(new Paragraph("2. Listado de Clientes", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaC = new PdfPTable(5) { WidthPercentage = 100 };
                tablaC.SetWidths(new float[] { 15, 25, 30, 15, 15 });
                string[] hC = { "Cédula", "Nombre", "Correo", "Teléfono", "Estado" };
                foreach (var h in hC)
                {
                    tablaC.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    { BackgroundColor = new BaseColor(26, 43, 107), Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                foreach (var c in _db.Usuarios.OrderBy(u => u.Nombre).ToList())
                {
                    tablaC.AddCell(new PdfPCell(new Phrase(c.Cedula, texto)) { Padding = 5 });
                    tablaC.AddCell(new PdfPCell(new Phrase(c.Nombre + " " + c.Apellidos, texto)) { Padding = 5 });
                    tablaC.AddCell(new PdfPCell(new Phrase(c.Correo, texto)) { Padding = 5 });
                    tablaC.AddCell(new PdfPCell(new Phrase(c.Telefono ?? "", texto)) { Padding = 5 });
                    tablaC.AddCell(new PdfPCell(new Phrase(c.Activo ? "Activo" : "Inactivo", texto)) { Padding = 5 });
                }
                doc.Add(tablaC);
                doc.NewPage();

                doc.Add(new Paragraph("3. Listado de NISEs", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaN = new PdfPTable(4) { WidthPercentage = 100 };
                tablaN.SetWidths(new float[] { 20, 25, 30, 25 });
                string[] hN = { "Número", "Cliente", "Dirección", "Tipo Servicio" };
                foreach (var h in hN)
                {
                    tablaN.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    { BackgroundColor = new BaseColor(26, 43, 107), Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                foreach (var n in _db.NISEs.ToList())
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == n.UsuarioId);
                    tablaN.AddCell(new PdfPCell(new Phrase(n.NumeroNise, textoBold)) { Padding = 5 });
                    tablaN.AddCell(new PdfPCell(new Phrase(cli != null ? cli.Nombre + " " + cli.Apellidos : "", texto)) { Padding = 5 });
                    tablaN.AddCell(new PdfPCell(new Phrase(n.Direccion ?? "", texto)) { Padding = 5 });
                    tablaN.AddCell(new PdfPCell(new Phrase(n.TipoServicio ?? "", texto)) { Padding = 5 });
                }
                doc.Add(tablaN);
                doc.NewPage();

                doc.Add(new Paragraph("4. Listado de Facturas", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaF = new PdfPTable(5) { WidthPercentage = 100 };
                tablaF.SetWidths(new float[] { 22, 20, 18, 20, 20 });
                string[] hF = { "Nº Factura", "NISE", "Monto", "Emisión", "Estado" };
                foreach (var h in hF)
                {
                    tablaF.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    { BackgroundColor = new BaseColor(26, 43, 107), Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                foreach (var f in _db.Facturas.OrderByDescending(x => x.FechaEmision).ToList())
                {
                    var nis = _db.NISEs.FirstOrDefault(n => n.NiseId == f.NiseId);
                    tablaF.AddCell(new PdfPCell(new Phrase(f.NumeroFactura ?? "", texto)) { Padding = 5 });
                    tablaF.AddCell(new PdfPCell(new Phrase(nis != null ? nis.NumeroNise : "", texto)) { Padding = 5 });
                    tablaF.AddCell(new PdfPCell(new Phrase("₡" + f.Monto.ToString("N0"), textoBold)) { Padding = 5 });
                    tablaF.AddCell(new PdfPCell(new Phrase(f.FechaEmision.ToString("dd/MM/yyyy"), texto)) { Padding = 5 });
                    tablaF.AddCell(new PdfPCell(new Phrase(f.Pagada ? "Pagada" : "Pendiente", texto)) { Padding = 5 });
                }
                doc.Add(tablaF);
                doc.NewPage();

                doc.Add(new Paragraph("5. Listado de Averías", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaA = new PdfPTable(4) { WidthPercentage = 100 };
                tablaA.SetWidths(new float[] { 25, 25, 25, 25 });
                string[] hA = { "Tipo", "Cliente", "Estado", "Fecha" };
                foreach (var h in hA)
                {
                    tablaA.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    { BackgroundColor = new BaseColor(26, 43, 107), Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                foreach (var a in _db.Averias.OrderByDescending(x => x.FechaReporte).ToList())
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == a.UsuarioId);
                    tablaA.AddCell(new PdfPCell(new Phrase(a.Tipo ?? "", texto)) { Padding = 5 });
                    tablaA.AddCell(new PdfPCell(new Phrase(cli != null ? cli.Nombre + " " + cli.Apellidos : "", texto)) { Padding = 5 });
                    tablaA.AddCell(new PdfPCell(new Phrase(a.Estado ?? "", texto)) { Padding = 5 });
                    tablaA.AddCell(new PdfPCell(new Phrase(a.FechaReporte.ToString("dd/MM/yyyy"), texto)) { Padding = 5 });
                }
                doc.Add(tablaA);
                doc.NewPage();

                doc.Add(new Paragraph("6. Listado de Trámites", subtitulo));
                doc.Add(new Paragraph(" "));

                var tablaT = new PdfPTable(4) { WidthPercentage = 100 };
                tablaT.SetWidths(new float[] { 30, 25, 20, 25 });
                string[] hT = { "Tipo", "Cliente", "Estado", "Fecha" };
                foreach (var h in hT)
                {
                    tablaT.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    { BackgroundColor = new BaseColor(26, 43, 107), Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                foreach (var t in _db.Tramites.OrderByDescending(x => x.FechaSolicitud).ToList())
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == t.UsuarioId);
                    tablaT.AddCell(new PdfPCell(new Phrase(t.Tipo ?? "", texto)) { Padding = 5 });
                    tablaT.AddCell(new PdfPCell(new Phrase(cli != null ? cli.Nombre + " " + cli.Apellidos : "", texto)) { Padding = 5 });
                    tablaT.AddCell(new PdfPCell(new Phrase(t.Estado ?? "", texto)) { Padding = 5 });
                    tablaT.AddCell(new PdfPCell(new Phrase(t.FechaSolicitud.ToString("dd/MM/yyyy"), texto)) { Padding = 5 });
                }
                doc.Add(tablaT);

                doc.Close();

                var bytes = ms.ToArray();
                return File(bytes, "application/pdf",
                    "CNFL_Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
            }
        }

        // ═══════════════════════════════════════════════════════════
        // ACTIVIDAD (solo clientes, no admin)
        // ═══════════════════════════════════════════════════════════
        public ActionResult Actividad()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;
            var lista = new List<ActividadSemanalDto>();

            for (int i = 6; i >= 0; i--)
            {
                var dia = hoy.AddDays(-i);
                var inicio = dia.Date;
                var fin = inicio.AddDays(1);

                lista.Add(new ActividadSemanalDto
                {
                    dia = dia.ToString("ddd"),
                    facturas = _db.Facturas.Count(f => f.FechaEmision >= inicio && f.FechaEmision < fin),
                    reportes = _db.Averias.Count(a => a.FechaReporte >= inicio && a.FechaReporte < fin),
                    tramites = _db.Tramites.Count(t => t.FechaSolicitud >= inicio && t.FechaSolicitud < fin),
                    perfil = 0
                });
            }

            ViewBag.Actividad = lista;

            var clientesIds = _db.UsuarioRoles
                .Where(ur => ur.Rol.NombreRol == "Cliente")
                .Select(ur => ur.UsuarioId)
                .Distinct()
                .ToList();

            ViewBag.TiempoUso = _db.ActividadUsuario
                .Where(a => clientesIds.Contains(a.UsuarioId))
                .GroupBy(a => a.UsuarioId)
                .Select(g => new TiempoUsoClienteDto
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
                    MinutosTotales = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60
                })
                .OrderByDescending(t => t.MinutosTotales)
                .Take(10)
                .ToList();

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // EXPORTAR ACTIVIDAD A EXCEL (con gráfico generado como imagen)
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarActividadExcel()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var clientesIds = _db.UsuarioRoles
                .Where(ur => ur.Rol.NombreRol == "Cliente")
                .Select(ur => ur.UsuarioId)
                .Distinct()
                .ToList();

            var datos = _db.ActividadUsuario
                .Where(a => clientesIds.Contains(a.UsuarioId))
                .GroupBy(a => a.UsuarioId)
                .Select(g => new
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
                    Minutos = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                    Secciones = g.Select(x => x.Seccion).Distinct().Count(),
                    Visitas = g.Count()
                })
                .OrderByDescending(x => x.Minutos)
                .Take(20)
                .ToList();

            // ═══ GENERAR IMAGEN DEL GRÁFICO CON SYSTEM.DRAWING ═══
            byte[] chartImageBytes = null;

            if (datos.Any())
            {
                int ancho = 600;
                int alto = 400;
                int margenIzq = 60;
                int margenDer = 20;
                int margenSup = 40;
                int margenInf = 80;

                using (var bmp = new System.Drawing.Bitmap(ancho, alto))
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    var tituloFuente = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
                    var ejeFuente = new System.Drawing.Font("Arial", 8);
                    var pincelTitulo = System.Drawing.Brushes.DarkSlateBlue;
                    var pincelTexto = System.Drawing.Brushes.Black;
                    var pincelEje = System.Drawing.Brushes.Gray;
                    var pincelBarra = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(91, 63, 191));

                    g.DrawString("Top clientes por minutos de uso", tituloFuente, pincelTitulo, new System.Drawing.PointF(margenIzq, 10));

                    int maxMinutos = datos.Max(d => d.Minutos);
                    if (maxMinutos <= 0) maxMinutos = 1;

                    int anchoDisponible = ancho - margenIzq - margenDer;
                    int altoDisponible = alto - margenSup - margenInf;
                    int cantidadBarras = datos.Count;
                    int espacioEntreBarras = 8;
                    int anchoBarra = (anchoDisponible - (espacioEntreBarras * (cantidadBarras + 1))) / cantidadBarras;
                    if (anchoBarra < 5) anchoBarra = 5;

                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.LightGray), margenIzq, margenSup, margenIzq, alto - margenInf);

                    for (int i = 0; i < cantidadBarras; i++)
                    {
                        var d = datos[i];
                        double porcentaje = (double)d.Minutos / maxMinutos;
                        int alturaBarra = (int)(porcentaje * altoDisponible);
                        if (alturaBarra < 4) alturaBarra = 4;

                        int x = margenIzq + espacioEntreBarras + (i * (anchoBarra + espacioEntreBarras));
                        int y = alto - margenInf - alturaBarra;

                        g.FillRectangle(pincelBarra, x, y, anchoBarra, alturaBarra);

                        string valor = d.Minutos.ToString();
                        var tamañoValor = g.MeasureString(valor, ejeFuente);
                        g.DrawString(valor, ejeFuente, pincelTexto, x + (anchoBarra - tamañoValor.Width) / 2, y - 16);

                        string nombreCorto = d.Nombre;
                        if (nombreCorto.Length > 10) nombreCorto = nombreCorto.Substring(0, 9) + "…";
                        var tamañoNombre = g.MeasureString(nombreCorto, ejeFuente);
                        g.DrawString(nombreCorto, ejeFuente, pincelEje, x + (anchoBarra - tamañoNombre.Width) / 2, alto - margenInf + 5);
                    }

                    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.LightGray, 1), margenIzq, margenSup, anchoDisponible, altoDisponible);

                    using (var msImg = new MemoryStream())
                    {
                        bmp.Save(msImg, ImageFormat.Png);
                        chartImageBytes = msImg.ToArray();
                    }
                }
            }

            // ═══ CREAR EXCEL ═══
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Actividad");

                ws.Cell("A1").Value = "CNFL · Actividad de uso de la app";
                ws.Range("A1:E1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                ws.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                ws.Range("A1:E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("A2").Value = "Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                ws.Range("A2:E2").Merge().Style.Font.SetItalic().Font.FontColor = XLColor.Gray;

                string[] headers = { "Cliente", "Correo", "Minutos", "Secciones", "Visitas" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(4, i + 1).Value = headers[i];
                    ws.Cell(4, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }

                int row = 5;
                foreach (var d in datos)
                {
                    ws.Cell(row, 1).Value = d.Nombre ?? "—";
                    ws.Cell(row, 2).Value = d.Correo ?? "—";
                    ws.Cell(row, 3).Value = d.Minutos;
                    ws.Cell(row, 4).Value = d.Secciones;
                    ws.Cell(row, 5).Value = d.Visitas;
                    row++;
                }

                ws.Columns().AdjustToContents();

                if (chartImageBytes != null)
                {
                    using (var msImg = new MemoryStream(chartImageBytes))
                    {
                        var imagen = ws.AddPicture(msImg, "GraficoActividad")
                            .MoveTo(ws.Cell(5, 7));

                        imagen.Width = 600;
                        imagen.Height = 400;
                    }
                }

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Actividad_Uso_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // EXPORTAR ACTIVIDAD A PDF (con tabla)
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarActividadPDF()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var clientesIds = _db.UsuarioRoles
                .Where(ur => ur.Rol.NombreRol == "Cliente")
                .Select(ur => ur.UsuarioId)
                .Distinct()
                .ToList();

            var datos = _db.ActividadUsuario
                .Where(a => clientesIds.Contains(a.UsuarioId))
                .GroupBy(a => a.UsuarioId)
                .Select(g => new
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
                    Minutos = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                    Secciones = g.Select(x => x.Seccion).Distinct().Count(),
                    Visitas = g.Count()
                })
                .OrderByDescending(x => x.Minutos)
                .Take(20)
                .ToList();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4.Rotate(), 36, 36, 54, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(26, 43, 107));
                var texto = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var textoBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var textoBlanco = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);

                doc.Add(new Paragraph("CNFL · Actividad de uso de la app", titulo));
                doc.Add(new Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), texto));
                doc.Add(new Paragraph(" "));

                var tabla = new PdfPTable(5) { WidthPercentage = 100 };
                tabla.SetWidths(new float[] { 30, 30, 12, 14, 14 });

                string[] headers = { "Cliente", "Correo", "Minutos", "Secciones", "Visitas" };
                foreach (var h in headers)
                {
                    tabla.AddCell(new PdfPCell(new Phrase(h, textoBlanco))
                    {
                        BackgroundColor = new BaseColor(26, 43, 107),
                        Padding = 8,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                }

                foreach (var d in datos)
                {
                    tabla.AddCell(new PdfPCell(new Phrase(d.Nombre ?? "—", texto)) { Padding = 6 });
                    tabla.AddCell(new PdfPCell(new Phrase(d.Correo ?? "—", texto)) { Padding = 6 });
                    tabla.AddCell(new PdfPCell(new Phrase(d.Minutos.ToString(), textoBold)) { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                    tabla.AddCell(new PdfPCell(new Phrase(d.Secciones.ToString(), texto)) { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                    tabla.AddCell(new PdfPCell(new Phrase(d.Visitas.ToString(), texto)) { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf",
                    "Actividad_Uso_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
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