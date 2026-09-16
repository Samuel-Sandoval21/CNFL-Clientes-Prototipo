using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

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
        // EXPORTAR REPORTES
        // ═══════════════════════════════════════════════════════════
        public ActionResult ExportarExcel()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            TempData["Mensaje"] = "Export a Excel próximamente.";
            return RedirectToAction("Reportes");
        }

        public ActionResult ExportarPDF()
        {
            var redir = RedirigirSiNoEsAdmin();
            if (redir != null) return redir;

            TempData["Mensaje"] = "Export a PDF próximamente.";
            return RedirectToAction("Reportes");
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