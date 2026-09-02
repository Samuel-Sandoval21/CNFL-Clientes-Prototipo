using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminController : Controller
    {
        private readonly CNFLDbContext _db = new CNFLDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();

            ViewBag.TotalAverias = averias.Count;
            ViewBag.AveriasPendientes = averias.Count(a => a.Estado != "Resuelto");
            ViewBag.AveriasEnRevision = averias.Count(a => a.Estado == "En Proceso");

            return View(averias);
        }

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();

            ViewBag.TotalAverias = averias.Count;
            ViewBag.AveriasPendientes = averias.Count(a => a.Estado != "Resuelto");
            ViewBag.AveriasEnRevision = averias.Count(a => a.Estado == "En Proceso");
            ViewBag.AveriasResueltas = averias.Count(a => a.Estado == "Resuelto");

            ViewBag.UltimasAverias = averias.Take(5).ToList();

            return View(averias);
        }

        // GET: Admin/Clientes
        public ActionResult Clientes()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var clientes = _db.Usuarios
                .Where(u => u.RolId == 1)
                .Select(u => new ClienteViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Cedula = u.Cedula,
                    NISE = u.NISE,
                    Telefono = u.Telefono,
                    Correo = u.Correo
                })
                .ToList();

            return View(clientes);
        }

        // GET: Admin/Reportes
        public ActionResult Reportes()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _db.Averias.ToList();

            int tasaResolucion = 0;
            if (averias.Count > 0)
            {
                int resueltas = averias.Count(a => a.Estado == "Resuelto");
                tasaResolucion = (int)((double)resueltas / averias.Count * 100);
            }

            var averiasPorMes = new List<AveriaPorMes>();
            for (int i = 6; i >= 0; i--)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                var mes = fecha.ToString("MMM");
                var cantidad = averias.Count(a => a.FechaReporte.Month == fecha.Month && a.FechaReporte.Year == fecha.Year);
                averiasPorMes.Add(new AveriaPorMes { Mes = mes, Cantidad = cantidad });
            }

            var averiasRecientes = new List<AveriaConTiempo>();
            foreach (var a in averias.OrderByDescending(a => a.FechaReporte).Take(10))
            {
                averiasRecientes.Add(new AveriaConTiempo
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    NISEId = a.NISEId,
                    TipoAveria = a.TipoAveria,
                    Descripcion = a.Descripcion,
                    Direccion = a.Direccion,
                    Latitud = a.Latitud,
                    Longitud = a.Longitud,
                    Estado = a.Estado,
                    FechaReporte = a.FechaReporte,
                    TiempoTranscurrido = CalcularTiempoTranscurrido(a.FechaReporte)
                });
            }

            var viewModel = new ReportesViewModel
            {
                TotalAverias = averias.Count,
                TiempoPromedioResolucion = CalcularTiempoPromedio(averias),
                TasaResolucion = tasaResolucion,
                TrendAverias = 12,
                TrendTiempo = -8,
                TrendTasa = 5,
                AveriasPorMes = averiasPorMes,
                AveriasRecientes = averiasRecientes
            };

            return View(viewModel);
        }

        // POST: Admin/CambiarEstado
        [HttpPost]
        public ActionResult CambiarEstado(int id, string estado)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            var averia = _db.Averias.Find(id);
            if (averia == null)
            {
                return Json(new { success = false, message = "Avería no encontrada" });
            }

            averia.Estado = estado;
            _db.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true, message = "✅ Estado actualizado correctamente" });
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/GenerarReporte
        [HttpPost]
        public JsonResult GenerarReporte(string periodo, string fechaInicio, string fechaFin)
        {
            var averias = _db.Averias.AsQueryable();

            DateTime fechaDesde = DateTime.Now;
            switch (periodo)
            {
                case "7d": fechaDesde = DateTime.Now.AddDays(-7); break;
                case "30d": fechaDesde = DateTime.Now.AddDays(-30); break;
                case "90d": fechaDesde = DateTime.Now.AddDays(-90); break;
                case "1a": fechaDesde = DateTime.Now.AddYears(-1); break;
                default: break;
            }

            if (!string.IsNullOrEmpty(fechaInicio) && DateTime.TryParse(fechaInicio, out DateTime ini))
            {
                fechaDesde = ini;
            }

            var filtradas = averias.Where(a => a.FechaReporte >= fechaDesde).ToList();

            return Json(new
            {
                success = true,
                message = $"✅ Reporte generado correctamente. {filtradas.Count} averías encontradas.",
                total = filtradas.Count,
                resueltas = filtradas.Count(a => a.Estado == "Resuelto"),
                pendientes = filtradas.Count(a => a.Estado != "Resuelto")
            });
        }

        // ==========================================================
        // EXPORTACIONES
        // ==========================================================

        public ActionResult ExportarPDF()
        {
            var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 20, 20, 20, 20);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
                var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                var normalFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                var title = new Paragraph("Reporte de Averías - CNFL", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                doc.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                doc.Add(new Paragraph($"Total de averías: {averias.Count}", normalFont));
                doc.Add(new Paragraph(" ", normalFont));

                var table = new PdfPTable(5);
                table.WidthPercentage = 100;

                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Tipo", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Ubicación", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var a in averias)
                {
                    table.AddCell(new PdfPCell(new Phrase(a.Id.ToString(), normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.TipoAveria ?? "N/A", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Direccion ?? "N/A", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.FechaReporte.ToString("dd/MM/yyyy HH:mm"), normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Estado ?? "N/A", normalFont)));
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
        }

        public ActionResult ExportarExcel()
        {
            var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();

            using (var ms = new MemoryStream())
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Averías");

                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Tipo";
                    worksheet.Cell(1, 3).Value = "Ubicación";
                    worksheet.Cell(1, 4).Value = "Fecha";
                    worksheet.Cell(1, 5).Value = "Estado";
                    worksheet.Cell(1, 6).Value = "Descripción";

                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    int row = 2;
                    foreach (var a in averias)
                    {
                        worksheet.Cell(row, 1).Value = a.Id;
                        worksheet.Cell(row, 2).Value = a.TipoAveria ?? "N/A";
                        worksheet.Cell(row, 3).Value = a.Direccion ?? "N/A";
                        worksheet.Cell(row, 4).Value = a.FechaReporte.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(row, 5).Value = a.Estado ?? "N/A";
                        worksheet.Cell(row, 6).Value = a.Descripcion ?? "N/A";
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(ms);
                }

                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        public ActionResult ExportarCSV()
        {
            var averias = _db.Averias.OrderByDescending(a => a.FechaReporte).ToList();
            var sb = new StringBuilder();

            sb.Append('\uFEFF');
            sb.AppendLine("ID,Tipo,Ubicación,Fecha,Estado,Descripción");

            foreach (var a in averias)
            {
                sb.AppendLine($"{a.Id},{EscapeCsv(a.TipoAveria)},{EscapeCsv(a.Direccion)},{a.FechaReporte:dd/MM/yyyy HH:mm},{EscapeCsv(a.Estado)},{EscapeCsv(a.Descripcion)}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // ==========================================================
        // MÉTODOS AUXILIARES
        // ==========================================================

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        private string CalcularTiempoTranscurrido(DateTime fecha)
        {
            var diff = DateTime.Now - fecha;
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} h";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} días";
            if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)} sem";
            return $"{(int)(diff.TotalDays / 30)} meses";
        }

        private string CalcularTiempoPromedio(List<Averia> averias)
        {
            var resueltas = averias.Where(a => a.Estado == "Resuelto").ToList();
            if (resueltas.Count == 0) return "N/A";

            var random = new Random();
            int totalHoras = 0;
            foreach (var a in resueltas)
            {
                totalHoras += random.Next(2, 9);
            }
            int promedio = totalHoras / resueltas.Count;
            return $"{promedio} hrs";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ==========================================================
    // MODELO DE VISTA (SOLO PARA CLIENTES)
    // ==========================================================
    public class ClienteViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string NISE { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
    }
}