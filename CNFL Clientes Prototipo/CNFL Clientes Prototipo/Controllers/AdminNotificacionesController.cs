using CNFL_Clientes_Prototipo.Data;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminNotificacionesController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        private bool EsAdmin()
        {
            return Session["AdminLogueado"] != null && (bool)Session["AdminLogueado"];
        }

        // ============================================================
        // VISTA PRINCIPAL: Enviar notificaciones + Historial
        // ============================================================
        public ActionResult Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Cuenta");

            // Clientes (rol "Cliente" y activos)
            var clientes = _db.Usuarios
                .Include("UsuarioRoles")
                .Where(u => u.Activo && u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"))
                .OrderBy(u => u.Nombre)
                .ToList();

            ViewBag.Usuarios = clientes;

            // Notificaciones enviadas por admin (historial)
            var notificaciones = _db.Notificaciones
                .Where(n => n.Tipo == "Admin")
                .OrderByDescending(n => n.Fecha)
                .Take(50)
                .ToList();

            ViewBag.Notificaciones = notificaciones;

            // Estadísticas
            ViewBag.TotalClientes = clientes.Count;
            ViewBag.TotalNotificaciones = _db.Notificaciones.Count(n => n.Tipo == "Admin");
            ViewBag.TotalLeidas = _db.Notificaciones.Count(n => n.Tipo == "Admin" && n.Leida);
            ViewBag.TotalNoLeidas = _db.Notificaciones.Count(n => n.Tipo == "Admin" && !n.Leida);

            return View();
        }

        // ============================================================
        // ENVIAR: Todos o seleccionados
        // ============================================================
        [HttpPost]
        public JsonResult EnviarNotificacion(string titulo, string mensaje, string tipo = "General", string scope = "todos", string usuariosIds = "")
        {
            if (!EsAdmin())
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(mensaje))
                return Json(new { ok = false, mensaje = "Título y mensaje son obligatorios." });

            try
            {
                List<Usuario> destinatarios;

                if (scope == "seleccion" && !string.IsNullOrWhiteSpace(usuariosIds))
                {
                    var ids = usuariosIds
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();

                    destinatarios = _db.Usuarios.Where(u => ids.Contains(u.UsuarioId)).ToList();
                }
                else
                {
                    // Todos los clientes activos con rol Cliente
                    destinatarios = _db.Usuarios
                        .Include("UsuarioRoles")
                        .Where(u => u.Activo && u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"))
                        .ToList();
                }

                if (!destinatarios.Any())
                    return Json(new { ok = false, mensaje = "No hay destinatarios válidos." });

                int enviados = 0;
                int correosOk = 0;
                var errores = new List<string>();

                foreach (var u in destinatarios)
                {
                    var notif = new Notificacion
                    {
                        UsuarioId = u.UsuarioId,
                        Titulo = titulo,
                        Mensaje = mensaje,
                        Fecha = DateTime.Now,
                        Leida = false,
                        Tipo = "Admin",
                        Estado = tipo
                    };
                    _db.Notificaciones.Add(notif);
                    enviados++;

                    if (!string.IsNullOrWhiteSpace(u.Correo))
                    {
                        try
                        {
                            var nombreCompleto = (u.Nombre + " " + u.Apellidos).Trim();
                            bool ok = EmailService.Enviar(
                                u.Correo,
                                "CNFL · " + titulo,
                                titulo,
                                mensaje,
                                nombreCompleto
                            );

                            if (ok) correosOk++;
                        }
                        catch (Exception exMail)
                        {
                            errores.Add("Error correo " + u.Correo + ": " + exMail.Message);
                        }
                    }
                }

                _db.SaveChanges();

                return Json(new
                {
                    ok = true,
                    enviados = enviados,
                    correos = correosOk,
                    mensaje = "Se enviaron " + enviados + " notificaciones y " + correosOk + " correos."
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error: " + ex.Message });
            }
        }

        // ============================================================
        // NOTIFICAR TRÁMITE ESPECÍFICO
        // ============================================================
        [HttpPost]
        public JsonResult NotificarTramite(int tramiteId, string titulo, string mensaje)
        {
            if (!EsAdmin())
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            var tramite = _db.Tramites
                .Include("Usuario")
                .FirstOrDefault(t => t.TramiteId == tramiteId);

            if (tramite == null)
                return Json(new { ok = false, mensaje = "Trámite no encontrado." });

            return EnviarPersonalizado(tramite.UsuarioId, titulo, mensaje, "Tramite");
        }

        // ============================================================
        // NOTIFICAR AVERÍA ESPECÍFICA
        // ============================================================
        [HttpPost]
        public JsonResult NotificarAveria(int averiaId, string titulo, string mensaje)
        {
            if (!EsAdmin())
                return Json(new { ok = false, mensaje = "Sesión expirada." });

            var averia = _db.Averias
                .Include("Usuario")
                .FirstOrDefault(a => a.AveriaId == averiaId);

            if (averia == null)
                return Json(new { ok = false, mensaje = "Avería no encontrada." });

            return EnviarPersonalizado(averia.UsuarioId, titulo, mensaje, "Averia");
        }

        // ============================================================
        // HELPER: Enviar notificación a un solo usuario
        // ============================================================
        private JsonResult EnviarPersonalizado(int usuarioId, string titulo, string mensaje, string tipo)
        {
            try
            {
                var usuario = _db.Usuarios.Find(usuarioId);
                if (usuario == null)
                    return Json(new { ok = false, mensaje = "Usuario no encontrado." });

                var notif = new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = tipo,
                    Estado = "Enviada"
                };
                _db.Notificaciones.Add(notif);
                _db.SaveChanges();

                bool correoOk = false;
                if (!string.IsNullOrWhiteSpace(usuario.Correo))
                {
                    var nombreCompleto = (usuario.Nombre + " " + usuario.Apellidos).Trim();
                    correoOk = EmailService.Enviar(
                        usuario.Correo,
                        "CNFL · " + titulo,
                        titulo,
                        mensaje,
                        nombreCompleto
                    );
                }

                return Json(new
                {
                    ok = true,
                    correo = correoOk,
                    mensaje = correoOk
                        ? "Notificación enviada y correo a " + usuario.Correo
                        : "Notificación creada pero no se pudo enviar correo"
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = ex.Message });
            }
        }

        // ============================================================
        // AJAX: Buscar clientes
        // ============================================================
        [HttpGet]
        public JsonResult BuscarClientes(string q = "")
        {
            if (!EsAdmin())
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);

            var query = _db.Usuarios
                .Include("UsuarioRoles")
                .Where(u => u.Activo && u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"));

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(q) ||
                    u.Apellidos.ToLower().Contains(q) ||
                    u.Cedula.Contains(q) ||
                    u.Correo.ToLower().Contains(q));
            }

            var clientes = query
                .OrderBy(u => u.Nombre)
                .Take(50)
                .Select(u => new
                {
                    id = u.UsuarioId,
                    nombre = u.Nombre + " " + u.Apellidos,
                    correo = u.Correo,
                    cedula = u.Cedula
                })
                .ToList();

            return Json(new { ok = true, clientes = clientes }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // DTO para clientes (por si lo usás en otra vista)
    public class ClienteDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}