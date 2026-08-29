using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;
using CNFL_Clientes_Prototipo.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminController : Controller
    {
        private readonly AveriaService _service = new AveriaService();

        // GET: Admin (Listado de averías para el operador)
        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _service.ListarAverias();

            ViewBag.TotalAverias = averias.Count;
            ViewBag.AveriasPendientes = averias.FindAll(a => a.Estado != "Resuelto").Count;
            ViewBag.AveriasEnRevision = averias.FindAll(a => a.Estado == "En revisión").Count;
            ViewBag.AveriasEnCamino = averias.FindAll(a => a.Estado == "En camino").Count;

            return View(averias);
        }

        // POST: Admin/CambiarEstado
        [HttpPost]
        public ActionResult CambiarEstado(int id, string estado)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            _service.CambiarEstado(id, estado);

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true, message = "✅ Estado actualizado correctamente" });
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Clientes
        public ActionResult Clientes()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var clientes = new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Juan", Apellidos = "Pérez Rodríguez", Identificacion = "1-1234-5678", Cedula = "1-1234-5678", NISE = "123456789", Telefono = "8888-1234", Correo = "juan.perez@email.com" },
                new Cliente { Id = 2, Nombre = "María", Apellidos = "Gómez Fernández", Identificacion = "1-8765-4321", Cedula = "1-8765-4321", NISE = "987654321", Telefono = "8888-5678", Correo = "maria.gomez@email.com" },
                new Cliente { Id = 3, Nombre = "Carlos", Apellidos = "Rodríguez Mora", Identificacion = "1-5555-6666", Cedula = "1-5555-6666", NISE = "456789123", Telefono = "8888-9999", Correo = "carlos.rodriguez@email.com" }
            };

            return View(clientes);
        }

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _service.ListarAverias();
            ViewBag.TotalAverias = averias.Count;
            ViewBag.AveriasPendientes = averias.FindAll(a => a.Estado != "Resuelto").Count;
            ViewBag.AveriasEnRevision = averias.FindAll(a => a.Estado == "En revisión").Count;
            ViewBag.AveriasEnCamino = averias.FindAll(a => a.Estado == "En camino").Count;
            ViewBag.AveriasResueltas = averias.FindAll(a => a.Estado == "Resuelto").Count;

            return View(averias);
        }

        // GET: Admin/Reportes
        public ActionResult Reportes()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _service.ListarAverias();

            int tasaResolucion = 0;
            if (averias.Count > 0)
            {
                int resueltas = averias.FindAll(a => a.Estado == "Resuelto").Count;
                tasaResolucion = (int)((double)resueltas / averias.Count * 100);
            }

            var averiasPorMes = new List<AveriaPorMes>();
            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var random = new Random();
            int baseCount = Math.Max(1, averias.Count / 8);

            foreach (var mes in meses)
            {
                averiasPorMes.Add(new AveriaPorMes
                {
                    Mes = mes,
                    Cantidad = random.Next(baseCount, baseCount * 3)
                });
            }

            var viewModel = new ReportesViewModel
            {
                TotalAverias = averias.Count,
                TiempoPromedioResolucion = averias.Count > 0 ? CalcularTiempoPromedio(averias) : "0 hrs",
                TasaResolucion = tasaResolucion,
                TrendAverias = averias.Count > 0 ? 12 : 0,
                TrendTiempo = averias.Count > 0 ? -8 : 0,
                TrendTasa = averias.Count > 0 ? 5 : 0,
                AveriasPorMes = averiasPorMes,
                AveriasRecientes = averias.OrderByDescending(a => a.Fecha).Take(10).Select(a => new AveriaConTiempo
                {
                    Id = a.Id,
                    Titulo = a.Titulo,
                    Direccion = a.Direccion,
                    Fecha = a.Fecha,
                    Estado = a.Estado,
                    Descripcion = a.Descripcion,
                    FotoUrl = a.FotoUrl,
                    TiempoTranscurrido = CalcularTiempoTranscurrido(a.Fecha)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/GenerarReporte
        [HttpPost]
        public JsonResult GenerarReporte(string periodo, string fechaInicio, string fechaFin)
        {
            var averias = _service.ListarAverias();

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

            var filtradas = averias.Where(a => a.Fecha >= fechaDesde).ToList();

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
        // ===== EXPORTACIÓN REAL A PDF (iTextSharp) =====
        // ==========================================================
        public ActionResult ExportarPDF()
        {
            var averias = _service.ListarAverias();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 20, 20, 20, 20);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Fuentes
                var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
                var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                var normalFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                // Título
                var title = new Paragraph("Reporte de Averías - CNFL", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                doc.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                doc.Add(new Paragraph($"Total de averías: {averias.Count}", normalFont));
                doc.Add(new Paragraph(" ", normalFont));

                // Tabla
                var table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Encabezados
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Título", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Ubicación", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                // Datos
                foreach (var a in averias)
                {
                    table.AddCell(new PdfPCell(new Phrase(a.Id.ToString(), normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Titulo ?? "N/A", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Direccion ?? "N/A", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Fecha.ToString("dd/MM/yyyy HH:mm"), normalFont)));
                    table.AddCell(new PdfPCell(new Phrase(a.Estado ?? "N/A", normalFont)));
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
        }

        // ==========================================================
        // ===== EXPORTACIÓN REAL A EXCEL (ClosedXML - GRATIS) =====
        // ==========================================================
        public ActionResult ExportarExcel()
        {
            var averias = _service.ListarAverias();

            using (var ms = new MemoryStream())
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Averías");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Título";
                    worksheet.Cell(1, 3).Value = "Ubicación";
                    worksheet.Cell(1, 4).Value = "Fecha";
                    worksheet.Cell(1, 5).Value = "Estado";
                    worksheet.Cell(1, 6).Value = "Descripción";

                    // Estilo de encabezados
                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Datos
                    int row = 2;
                    foreach (var a in averias)
                    {
                        worksheet.Cell(row, 1).Value = a.Id;
                        worksheet.Cell(row, 2).Value = a.Titulo ?? "N/A";
                        worksheet.Cell(row, 3).Value = a.Direccion ?? "N/A";
                        worksheet.Cell(row, 4).Value = a.Fecha.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(row, 5).Value = a.Estado ?? "N/A";
                        worksheet.Cell(row, 6).Value = a.Descripcion ?? "N/A";
                        row++;
                    }

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(ms);
                }

                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        // ==========================================================
        // ===== EXPORTACIÓN REAL A CSV =====
        // ==========================================================
        public ActionResult ExportarCSV()
        {
            var averias = _service.ListarAverias();
            var sb = new StringBuilder();

            // BOM para UTF-8 (compatible con Excel)
            sb.Append('\uFEFF');

            // Encabezados
            sb.AppendLine("ID,Título,Ubicación,Fecha,Estado,Descripción");

            // Datos
            foreach (var a in averias)
            {
                sb.AppendLine($"{a.Id},{EscapeCsv(a.Titulo)},{EscapeCsv(a.Direccion)},{a.Fecha:dd/MM/yyyy HH:mm},{EscapeCsv(a.Estado)},{EscapeCsv(a.Descripcion)}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Reporte_Averias_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // ===== MÉTODO AUXILIAR PARA ESCAPAR CSV =====
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        // ===== MÉTODOS AUXILIARES =====
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
            var resueltas = averias.FindAll(a => a.Estado == "Resuelto");
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
    }
}