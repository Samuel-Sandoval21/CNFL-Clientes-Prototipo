using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    [CNFL_Clientes_Prototipo.Filters.SessionAuthorize(RequiredRole = "Admin")]
    public class AdminController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ═══════════════════════════════════════════════════════════
        // DASHBOARD ADMIN
        // ═══════════════════════════════════════════════════════════
        public ActionResult Dashboard()
        {
            var hoy = DateTime.Now.Date;
            var hace7 = hoy.AddDays(-7);
            var hace30 = hoy.AddDays(-30);

            var totalUsuarios = _db.Usuarios.Count();
            var totalClientes = _db.Usuarios.Count(u =>
                _db.UsuarioRoles.Any(ur => ur.UsuarioId == u.UsuarioId && ur.Rol.NombreRol == "Cliente"));
            var totalNISEs = _db.NISEs.Count();
            var facturasPendientes = _db.Facturas.Count(f => !f.Pagada);
            var montoPendiente = _db.Facturas.Where(f => !f.Pagada).Sum(f => (decimal?)f.Monto) ?? 0m;

            var averiasAbiertas = _db.Averias.Count(a => a.Estado != "Resuelto" && a.Estado != "Problema resuelto");
            var averiasResueltas = _db.Averias.Count(a => a.Estado == "Resuelto" || a.Estado == "Problema resuelto");
            var tasaResolucion = (averiasAbiertas + averiasResueltas) > 0
                ? Math.Round((double)averiasResueltas / (averiasAbiertas + averiasResueltas) * 100, 0)
                : 0;

            var tramitesAbiertos = _db.Tramites.Count(t => t.Estado != "Resuelto");

            var averiasResueltasList = _db.Averias
                .Where(a => (a.Estado == "Resuelto" || a.Estado == "Problema resuelto")
                         && a.FechaActualizacion != null)
                .ToList();
            var tiempoPromedio = averiasResueltasList.Any()
                ? Math.Round(averiasResueltasList.Average(a =>
                    (a.FechaActualizacion.Value - a.FechaReporte).TotalDays), 0)
                : 0;

            var actividad = _db.ActividadUsuario
                .Where(a => a.Fecha >= hace7)
                .ToList();

            var ingresosMes = _db.Pagos
                .Where(p => p.FechaCreacion >= hace30 && p.Estado == "Completado")
                .Sum(p => (decimal?)p.Monto) ?? 0m;

            var clientesRecientes = _db.Usuarios
                .Where(u => _db.UsuarioRoles.Any(ur => ur.UsuarioId == u.UsuarioId && ur.Rol.NombreRol == "Cliente"))
                .OrderByDescending(u => u.FechaRegistro)
                .Take(5)
                .ToList();

            var averiasPorEstado = _db.Averias
                .GroupBy(a => a.Estado)
                .Select(g => new { Estado = g.Key, Count = g.Count() })
                .ToList();

            var averiasPorDia = new List<int>();
            var diasLabels = new List<string>();
            for (int i = 6; i >= 0; i--)
            {
                var dia = hoy.AddDays(-i);
                averiasPorDia.Add(_db.Averias.Count(a => DbFunctions.TruncateTime(a.FechaReporte) == dia));
                diasLabels.Add(dia.ToString("ddd", new System.Globalization.CultureInfo("es-CR")));
            }

            ViewBag.NombreAdmin = Session["Nombre"] as string ?? "Admin";
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.TotalClientes = totalClientes;
            ViewBag.TotalNISEs = totalNISEs;
            ViewBag.FacturasPendientes = facturasPendientes;
            ViewBag.MontoPendiente = montoPendiente;
            ViewBag.AveriasAbiertas = averiasAbiertas;
            ViewBag.AveriasResueltas = averiasResueltas;
            ViewBag.TasaResolucion = tasaResolucion;
            ViewBag.TramitesAbiertos = tramitesAbiertos;
            ViewBag.TiempoPromedio = tiempoPromedio;
            ViewBag.IngresosMes = ingresosMes;
            ViewBag.ActividadSemana = actividad.Count;

            // ❌ SIN AVERÍAS RECIENTES (a pedido del bloc de notas)

            ViewBag.ClientesRecientes = clientesRecientes;

            ViewBag.AveriasEstadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(averiasPorEstado);
            ViewBag.AveriasDiaJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { labels = diasLabels, data = averiasPorDia });

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // GESTIÓN DE CLIENTES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Clientes(string q, string rol = "Cliente")
        {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(q) ||
                    u.Apellidos.ToLower().Contains(q) ||
                    u.Cedula.Contains(q) ||
                    u.Correo.ToLower().Contains(q));
            }

            if (rol == "Cliente")
            {
                query = query.Where(u => _db.UsuarioRoles.Any(ur => ur.UsuarioId == u.UsuarioId && ur.Rol.NombreRol == "Cliente"));
            }
            else if (rol == "Admin")
            {
                query = query.Where(u => _db.UsuarioRoles.Any(ur => ur.UsuarioId == u.UsuarioId && ur.Rol.NombreRol == "Admin"));
            }

            var clientes = query.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombre).ToList();

            var viewModel = new List<ClienteAdminDto>();
            foreach (var u in clientes)
            {
                var nises = _db.NISEs.Where(n => n.UsuarioId == u.UsuarioId).ToList();
                var niseIds = nises.Select(n => n.NiseId).ToList();

                viewModel.Add(new ClienteAdminDto
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Cedula = u.Cedula,
                    Correo = u.Correo,
                    Telefono = u.Telefono,
                    Activo = u.Activo,
                    FechaRegistro = u.FechaRegistro,
                    TotalNISEs = nises.Count,
                    FacturasPendientes = _db.Facturas.Count(f => niseIds.Contains(f.NiseId) && !f.Pagada),
                    MontoPendiente = _db.Facturas.Where(f => niseIds.Contains(f.NiseId) && !f.Pagada).Sum(f => (decimal?)f.Monto) ?? 0m,
                    AveriasActivas = _db.Averias.Count(a => a.UsuarioId == u.UsuarioId && a.Estado != "Resuelto" && a.Estado != "Problema resuelto")
                });
            }

            ViewBag.Busqueda = q;
            ViewBag.RolFiltro = rol;
            ViewBag.TotalClientes = viewModel.Count;
            return View(viewModel);
        }

        // GET: Admin/DetalleCliente/5
        public ActionResult DetalleCliente(int id)
        {
            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .FirstOrDefault(u => u.UsuarioId == id);

            if (usuario == null) return HttpNotFound();

            var nises = _db.NISEs.Where(n => n.UsuarioId == id).ToList();
            var niseIds = nises.Select(n => n.NiseId).ToList();

            var facturas = _db.Facturas
                .Where(f => niseIds.Contains(f.NiseId))
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            var averias = _db.Averias
                .Where(a => a.UsuarioId == id)
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            var tramites = _db.Tramites
                .Where(t => t.UsuarioId == id)
                .OrderByDescending(t => t.FechaSolicitud)
                .ToList();

            var pagos = _db.Pagos
                .Where(p => p.UsuarioId == id)
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();

            ViewBag.NISEs = nises;
            ViewBag.Facturas = facturas;
            ViewBag.Averias = averias;
            ViewBag.Tramites = tramites;
            ViewBag.Pagos = pagos;
            ViewBag.TotalFacturado = facturas.Sum(f => (decimal?)f.Monto) ?? 0m;
            ViewBag.TotalPagado = pagos.Where(p => p.Estado == "Completado").Sum(p => (decimal?)p.Monto) ?? 0m;

            return View(usuario);
        }

        // POST: Admin/ToggleActivo/5
        [HttpPost]
        public JsonResult ToggleActivo(int id)
        {
            var usuario = _db.Usuarios.Find(id);
            if (usuario == null) return Json(new { success = false, message = "Usuario no encontrado." });

            usuario.Activo = !usuario.Activo;
            _db.SaveChanges();

            return Json(new { success = true, activo = usuario.Activo });
        }

        // ═══════════════════════════════════════════════════════════
        // GESTIÓN DE AVERÍAS
        // ═══════════════════════════════════════════════════════════
        public ActionResult Averias(string estado, string q, string tipo)
        {
            var query = _db.Averias.Include("NISE").AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                query = query.Where(a => a.Estado == estado);

            if (!string.IsNullOrWhiteSpace(tipo) && tipo != "Todos")
                query = query.Where(a => a.Tipo == tipo);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(a =>
                    a.AveriaId.ToString().Contains(q) ||
                    (a.NISE != null && a.NISE.NumeroNise.ToLower().Contains(q)) ||
                    a.Descripcion.ToLower().Contains(q));
            }

            var averias = query.OrderByDescending(a => a.FechaReporte).ToList();

            ViewBag.EstadoFiltro = estado;
            ViewBag.TipoFiltro = tipo;
            ViewBag.Busqueda = q;
            ViewBag.Total = averias.Count;
            ViewBag.Estados = new[] { "Todos", "Ingresado", "En revisión", "Operador en camino", "Resuelto", "Problema resuelto" };
            ViewBag.Tipos = _db.Averias.Select(a => a.Tipo).Distinct().ToList();

            return View(averias);
        }

        // GET: Admin/DetalleAveria/5
        public ActionResult DetalleAveria(int id)
        {
            var averia = _db.Averias
                .Include("NISE")
                .FirstOrDefault(a => a.AveriaId == id);

            if (averia == null) return HttpNotFound();

            var usuario = _db.Usuarios.Find(averia.UsuarioId);
            ViewBag.Usuario = usuario;

            var historial = _db.Notificaciones
                .Where(n => n.UsuarioId == averia.UsuarioId && n.Tipo == "Averia")
                .OrderByDescending(n => n.Fecha)
                .Take(10)
                .ToList();
            ViewBag.Historial = historial;

            return View(averia);
        }

        // POST: Admin/CambiarEstadoAveria
        [HttpPost]
        public JsonResult CambiarEstadoAveria(int averiaId, string nuevoEstado, string comentario)
        {
            var averia = _db.Averias.Find(averiaId);
            if (averia == null) return Json(new { success = false, message = "Avería no encontrada." });

            averia.Estado = nuevoEstado;
            averia.FechaActualizacion = DateTime.Now;

            var notif = new Notificacion
            {
                UsuarioId = averia.UsuarioId,
                Titulo = "Actualización de avería #" + averiaId,
                Mensaje = "Su avería cambió a estado: " + nuevoEstado + (string.IsNullOrWhiteSpace(comentario) ? "" : ". " + comentario),
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Averia",
                Estado = nuevoEstado
            };
            _db.Notificaciones.Add(notif);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // Alias para compatibilidad con vistas antiguas
        [HttpPost]
        public JsonResult ActualizarEstadoAveria(int averiaId, string nuevoEstado)
        {
            return CambiarEstadoAveria(averiaId, nuevoEstado, null);
        }

        // ═══════════════════════════════════════════════════════════
        // GESTIÓN DE TRÁMITES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Tramites(string estado, string q)
        {
            var query = _db.Tramites.Include("Usuario").AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                query = query.Where(t => t.Estado == estado);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(t =>
                    t.NumeroReferencia.ToLower().Contains(q) ||
                    t.Tipo.ToLower().Contains(q) ||
                    t.Usuario.Nombre.ToLower().Contains(q) ||
                    t.Usuario.Apellidos.ToLower().Contains(q));
            }

            var tramites = query.OrderByDescending(t => t.FechaSolicitud).ToList();

            ViewBag.EstadoFiltro = estado;
            ViewBag.Busqueda = q;
            ViewBag.Total = tramites.Count;
            ViewBag.Estados = new[] { "Todos", "Iniciado", "En Proceso", "Resuelto" };

            return View(tramites);
        }

        // POST: Admin/CambiarEstadoTramite
        [HttpPost]
        public JsonResult CambiarEstadoTramite(int tramiteId, string nuevoEstado, string comentario)
        {
            var tramite = _db.Tramites.Find(tramiteId);
            if (tramite == null) return Json(new { success = false, message = "Trámite no encontrado." });

            tramite.Estado = nuevoEstado;
            tramite.FechaActualizacion = DateTime.Now;

            var notif = new Notificacion
            {
                UsuarioId = tramite.UsuarioId,
                Titulo = "Actualización de trámite " + tramite.NumeroReferencia,
                Mensaje = "Su trámite cambió a estado: " + nuevoEstado + (string.IsNullOrWhiteSpace(comentario) ? "" : ". " + comentario),
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Tramite",
                Estado = nuevoEstado
            };
            _db.Notificaciones.Add(notif);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // ═══════════════════════════════════════════════════════════
        // REPORTES
        // ═══════════════════════════════════════════════════════════
        public ActionResult Reportes(DateTime? desde, DateTime? hasta)
        {
            var d = desde ?? DateTime.Now.AddDays(-30);
            var h = hasta ?? DateTime.Now;

            var facturas = _db.Facturas.Where(f => f.FechaEmision >= d && f.FechaEmision <= h).ToList();
            var averias = _db.Averias.Where(a => a.FechaReporte >= d && a.FechaReporte <= h).ToList();
            var pagos = _db.Pagos.Where(p => p.FechaCreacion >= d && p.FechaCreacion <= h).ToList();
            var tramites = _db.Tramites.Where(t => t.FechaSolicitud >= d && t.FechaSolicitud <= h).ToList();

            ViewBag.Desde = d.ToString("yyyy-MM-dd");
            ViewBag.Hasta = h.ToString("yyyy-MM-dd");
            ViewBag.TotalFacturado = facturas.Sum(f => (decimal?)f.Monto) ?? 0m;
            ViewBag.TotalCobrado = pagos.Where(p => p.Estado == "Completado").Sum(p => (decimal?)p.Monto) ?? 0m;
            ViewBag.TotalAverias = averias.Count;
            ViewBag.TotalTramites = tramites.Count;

            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var facturasPorMes = facturas
                .GroupBy(f => f.FechaEmision.Month)
                .Select(g => new { Mes = meses[g.Key - 1], Total = g.Sum(f => f.Monto) })
                .OrderBy(x => Array.IndexOf(meses, x.Mes))
                .ToList();

            var averiasPorTipo = averias
                .GroupBy(a => a.Tipo ?? "Otros")
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(6)
                .ToList();

            ViewBag.FacturasMesJson = Newtonsoft.Json.JsonConvert.SerializeObject(facturasPorMes);
            ViewBag.AveriasTipoJson = Newtonsoft.Json.JsonConvert.SerializeObject(averiasPorTipo);

            return View();
        }

        // GET: Admin/ReportesPDF
        public ActionResult ReportesPDF(string tipo)
        {
            var averias = _db.Averias
                .Include("Usuario")
                .Include("NISE")
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            return View(averias);
        }

        // ═══════════════════════════════════════════════════════════
        // ACTIVIDAD DE USO
        // ═══════════════════════════════════════════════════════════
        public ActionResult Actividad(DateTime? desde, DateTime? hasta)
        {
            var d = desde ?? DateTime.Now.AddDays(-7);
            var h = hasta ?? DateTime.Now;

            var actividad = _db.ActividadUsuario
                .Include("Usuario")
                .Where(a => a.Fecha >= d && a.Fecha <= h)
                .OrderByDescending(a => a.Fecha)
                .Take(500)
                .ToList();

            var porSeccion = actividad
                .GroupBy(a => a.Seccion ?? "Sin sección")
                .Select(g => new { Seccion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var porDia = new List<int>();
            var diasLabels = new List<string>();
            var hoy = DateTime.Now.Date;
            for (int i = 6; i >= 0; i--)
            {
                var dia = hoy.AddDays(-i);
                porDia.Add(_db.ActividadUsuario.Count(a => DbFunctions.TruncateTime(a.Fecha) == dia));
                diasLabels.Add(dia.ToString("ddd", new System.Globalization.CultureInfo("es-CR")));
            }

            ViewBag.Desde = d.ToString("yyyy-MM-dd");
            ViewBag.Hasta = h.ToString("yyyy-MM-dd");
            ViewBag.Actividad = actividad;
            ViewBag.TotalActividad = actividad.Count;
            ViewBag.PorSeccionJson = Newtonsoft.Json.JsonConvert.SerializeObject(porSeccion);
            ViewBag.PorDiaJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { labels = diasLabels, data = porDia });

            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // CONFIGURACIÓN
        // ═══════════════════════════════════════════════════════════
        public ActionResult Configuracion()
        {
            ViewBag.TotalUsuarios = _db.Usuarios.Count();
            ViewBag.TotalNISEs = _db.NISEs.Count();
            ViewBag.TotalFacturas = _db.Facturas.Count();
            ViewBag.TotalAverias = _db.Averias.Count();
            ViewBag.TotalTramites = _db.Tramites.Count();
            ViewBag.TotalNotificaciones = _db.Notificaciones.Count();
            ViewBag.VersionApp = "1.0.0-prototipo";
            ViewBag.UltimaActualizacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}