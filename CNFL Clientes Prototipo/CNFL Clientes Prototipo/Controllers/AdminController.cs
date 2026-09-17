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
        // TRÁMITES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Tramites(string estado = "")
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var query = _db.Tramites.AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(t => t.Estado == estado);

            var lista = query
                .OrderByDescending(t => t.FechaSolicitud)
                .Take(100)
                .ToList();

            ViewBag.Estado = estado;
            return View(lista);
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
        // EXPORTAR A EXCEL
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarExcel()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            var hoy = DateTime.Now;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            using (var wb = new XLWorkbook())
            {
                // ═══ HOJA 1: RESUMEN GENERAL ═══
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

                // ═══ HOJA 2: CLIENTES ═══
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

                // ═══ HOJA 3: NISEs ═══
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

                // ═══ HOJA 4: FACTURAS ═══
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

                // ═══ HOJA 5: AVERÍAS ═══
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

                // ═══ HOJA 6: TRÁMITES ═══
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

                // ═══ HOJA 7: ACTIVIDAD DE USO ═══
                var wsAc = wb.Worksheets.Add("Actividad de uso");
                wsAc.Cell("A1").Value = "ACTIVIDAD DE USO DE LA APP";
                wsAc.Range("A1:D1").Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.FontColor = XLColor.White;
                wsAc.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1a2b6b");
                wsAc.Range("A1:D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] hAc = { "Usuario", "Correo", "Minutos totales", "Secciones usadas" };
                for (int i = 0; i < hAc.Length; i++)
                {
                    wsAc.Cell(3, i + 1).Value = hAc[i];
                    wsAc.Cell(3, i + 1).Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                }
                var actividad = _db.ActividadUsuario
                    .GroupBy(a => a.UsuarioId)
                    .Select(g => new
                    {
                        UsuarioId = g.Key,
                        Minutos = (g.Sum(a => a.DuracionSegundos) ?? 0) / 60,
                        Secciones = g.Select(x => x.Seccion).Distinct().Count()
                    })
                    .OrderByDescending(x => x.Minutos)
                    .ToList();
                int rac = 4;
                foreach (var a in actividad)
                {
                    var cli = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == a.UsuarioId);
                    wsAc.Cell(rac, 1).Value = cli != null ? (cli.Nombre + " " + cli.Apellidos) : "";
                    wsAc.Cell(rac, 2).Value = cli != null ? cli.Correo : "";
                    wsAc.Cell(rac, 3).Value = a.Minutos;
                    wsAc.Cell(rac, 4).Value = a.Secciones;
                    rac++;
                }
                wsAc.Columns().AdjustToContents();

                // ═══ HOJA 8: DATOS FACTURACIÓN ═══
                var wsGF = wb.Worksheets.Add("Datos Facturación");
                wsGF.Cell("A1").Value = "Mes";
                wsGF.Cell("B1").Value = "Facturado";
                wsGF.Cell("C1").Value = "Cobrado";
                wsGF.Range("A1:C1").Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");

                for (int i = 5; i >= 0; i--)
                {
                    var fecha = hoy.AddMonths(-i);
                    var inicio = new DateTime(fecha.Year, fecha.Month, 1);
                    var fin = inicio.AddMonths(1);

                    var facMes = _db.Facturas.Where(f => f.FechaEmision >= inicio && f.FechaEmision < fin).ToList();
                    var facMesCob = facMes.Where(f => f.Pagada).ToList();

                    int rw = 7 - i;
                    wsGF.Cell(rw, 1).Value = inicio.ToString("MMM yyyy");
                    wsGF.Cell(rw, 2).Value = facMes.Sum(f => f.Monto);
                    wsGF.Cell(rw, 3).Value = facMesCob.Sum(f => f.Monto);
                }
                wsGF.Columns().AdjustToContents();

                // ═══ HOJA 9: DATOS CLIENTES ═══
                var wsGC = wb.Worksheets.Add("Datos Clientes");
                wsGC.Cell("A1").Value = "Estado";
                wsGC.Cell("B1").Value = "Cantidad";
                wsGC.Range("A1:B1").Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                wsGC.Cell("A2").Value = "Activos";
                wsGC.Cell("B2").Value = _db.Usuarios.Count(u => u.Activo);
                wsGC.Cell("A3").Value = "Inactivos";
                wsGC.Cell("B3").Value = _db.Usuarios.Count(u => !u.Activo);
                wsGC.Columns().AdjustToContents();

                // ═══ HOJA 10: DATOS AVERÍAS ═══
                var wsGA = wb.Worksheets.Add("Datos Averías");
                wsGA.Cell("A1").Value = "Estado";
                wsGA.Cell("B1").Value = "Cantidad";
                wsGA.Range("A1:B1").Style.Font.SetBold().Fill.BackgroundColor = XLColor.FromHtml("#eef0ff");
                wsGA.Cell("A2").Value = "Abiertas";
                wsGA.Cell("B2").Value = _db.Averias.Count(a => a.Estado != "Resuelta" && a.Estado != "Cerrada");
                wsGA.Cell("A3").Value = "Resueltas";
                wsGA.Cell("B3").Value = _db.Averias.Count(a => a.Estado == "Resuelta" || a.Estado == "Cerrada");
                wsGA.Columns().AdjustToContents();

                // ═══ GUARDAR Y DEVOLVER ═══
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
        // EXPORTAR A PDF
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

                // Fuentes — CORREGIDO: BaseColor.BLACK y BaseColor.WHITE en MAYÚSCULAS
                var titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(26, 43, 107));
                var subtitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(91, 63, 191));
                var texto = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var textoBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var textoBlanco = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);

                // HEADER
                var headerTable = new PdfPTable(1) { WidthPercentage = 100 };
                var headerCell = new PdfPCell(new Phrase("CNFL · Reporte Administrativo", titulo))
                {
                    BackgroundColor = new BaseColor(238, 240, 255),
                    Border = Rectangle.NO_BORDER,
                    Padding = 14,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                headerTable.AddCell(headerCell);
                doc.Add(headerTable);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), texto));
                doc.Add(new Paragraph(" "));

                // 1. RESUMEN
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

                // 2. CLIENTES
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

                // 3. NISEs
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

                // 4. FACTURAS
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

                // 5. AVERÍAS
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

                // 6. TRÁMITES
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
        // ACTIVIDAD
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

            ViewBag.TiempoUso = _db.ActividadUsuario
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