using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;
using System.Collections.Generic;
using CNFL_Clientes_Prototipo.Models;
using System.Linq;
using System;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminController : Controller
    {
        private readonly AveriaService _service = new AveriaService();

        // GET: Admin (Listado de averías para el operador)
        public ActionResult Index()
        {
            // Validación de rol Admin
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _service.ListarAverias();

            // Agregamos estadísticas al ViewBag para el dashboard
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
                return Json(new { success = true, message = "Estado actualizado correctamente" });
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

            // Datos de ejemplo
            var clientes = new List<Cliente>
            {
                new Cliente {
                    Id = 1,
                    Nombre = "Juan",
                    Apellidos = "Pérez Rodríguez",
                    Identificacion = "1-1234-5678",
                    Cedula = "1-1234-5678",
                    NISE = "123456789",
                    Telefono = "8888-1234",
                    Correo = "juan.perez@email.com"
                },
                new Cliente {
                    Id = 2,
                    Nombre = "María",
                    Apellidos = "Gómez Fernández",
                    Identificacion = "1-8765-4321",
                    Cedula = "1-8765-4321",
                    NISE = "987654321",
                    Telefono = "8888-5678",
                    Correo = "maria.gomez@email.com"
                },
                new Cliente {
                    Id = 3,
                    Nombre = "Carlos",
                    Apellidos = "Rodríguez Mora",
                    Identificacion = "1-5555-6666",
                    Cedula = "1-5555-6666",
                    NISE = "456789123",
                    Telefono = "8888-9999",
                    Correo = "carlos.rodriguez@email.com"
                }
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

            // Calcular tasa de resolución
            int tasaResolucion = 0;
            if (averias.Count > 0)
            {
                int resueltas = averias.FindAll(a => a.Estado == "Resuelto").Count;
                tasaResolucion = (int)((double)resueltas / averias.Count * 100);
            }

            // Datos para el gráfico (simulados con datos reales de averías)
            var averiasPorMes = new List<AveriaPorMes>();
            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var random = new Random();

            // Generar datos realistas basados en el total de averías
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

        // Método auxiliar para calcular tiempo transcurrido
        private string CalcularTiempoTranscurrido(DateTime fecha)
        {
            var diff = DateTime.Now - fecha;
            if (diff.TotalHours < 1)
                return $"{(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours} h";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} días";
            if (diff.TotalDays < 30)
                return $"{(int)(diff.TotalDays / 7)} sem";
            return $"{(int)(diff.TotalDays / 30)} meses";
        }

        // Método auxiliar para calcular tiempo promedio de resolución
        private string CalcularTiempoPromedio(List<Averia> averias)
        {
            var resueltas = averias.FindAll(a => a.Estado == "Resuelto");
            if (resueltas.Count == 0) return "N/A";

            // Simulación: asumimos que cada avería tomó entre 2 y 8 horas
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