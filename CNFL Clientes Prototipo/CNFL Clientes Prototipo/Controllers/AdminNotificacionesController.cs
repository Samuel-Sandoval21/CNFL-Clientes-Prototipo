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
        // VISTA PRINCIPAL: Enviar notificaciones
        // ============================================================
        public ActionResult Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Cuenta");

            // Cargar todos los clientes (los que tienen rol "Cliente")
            var clientes = _db.Usuarios
                .Include("UsuarioRoles")
                .Where(u => u.Activo && u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"))
                .OrderBy(u => u.Nombre)
                .Select(u => new ClienteDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellidos,
                    Correo = u.Correo,
                    Telefono = u.Telefono
                })
                .ToList();

            ViewBag.Clientes = clientes;

            // Estadísticas
            ViewBag.TotalClientes = clientes.Count;
            ViewBag.ClientesConCorreo = clientes.Count(c => !string.IsNullOrEmpty(c.Correo));
            ViewBag.NotificacionesEnviadas = _db.Notificaciones.Count(n => n.Tipo == "Admin");

            // Últimas notificaciones enviadas por admin
            ViewBag.UltimasEnviadas = _db.Notificaciones
                .Include("Usuario")
                .Where(n => n.Tipo == "Admin")
                .OrderByDescending(n => n.Fecha)
                .Take(20)
                .ToList();

            return View();
        }

        // ============================================================
        // ENVIAR A UNO O VARIOS CLIENTES
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Enviar(int[] usuarioIds, string titulo, string mensaje, string tipo = "General")
        {
            if (!EsAdmin())
                return Json(new { success = false, message = "Sesión expirada." });

            if (usuarioIds == null || usuarioIds.Length == 0)
                return Json(new { success = false, message = "Seleccioná al menos un cliente." });

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(mensaje))
                return Json(new { success = false, message = "Título y mensaje son obligatorios." });

            try
            {
                int enviados = 0;
                int correosEnviados = 0;
                var errores = new List<string>();

                foreach (var usuarioId in usuarioIds)
                {
                    var usuario = _db.Usuarios.Find(usuarioId);
                    if (usuario == null) continue;

                    // 1. Crear notificación en la BD (para la app)
                    var notif = new Notificacion
                    {
                        UsuarioId = usuarioId,
                        Titulo = titulo,
                        Mensaje = mensaje,
                        Fecha = DateTime.Now,
                        Leida = false,
                        Tipo = "Admin",
                        Estado = tipo
                    };
                    _db.Notificaciones.Add(notif);
                    enviados++;

                    // 2. Enviar correo al cliente
                    if (!string.IsNullOrWhiteSpace(usuario.Correo))
                    {
                        try
                        {
                            string nombreCompleto = (usuario.Nombre + " " + usuario.Apellidos).Trim();
                            bool ok = EmailService.Enviar(
                                usuario.Correo,
                                "CNFL · " + titulo,
                                titulo,
                                mensaje,
                                nombreCompleto
                            );

                            if (ok) correosEnviados++;
                            else errores.Add("No se pudo enviar correo a " + usuario.Correo);
                        }
                        catch (Exception exMail)
                        {
                            errores.Add("Error correo " + usuario.Correo + ": " + exMail.Message);
                        }
                    }
                }

                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    enviados = enviados,
                    correos = correosEnviados,
                    message = "Se enviaron " + enviados + " notificaciones y " + correosEnviados + " correos.",
                    errores = errores
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ENVIAR A TODOS LOS CLIENTES
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EnviarATodos(string titulo, string mensaje, string tipo = "General")
        {
            if (!EsAdmin())
                return Json(new { success = false, message = "Sesión expirada." });

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(mensaje))
                return Json(new { success = false, message = "Título y mensaje son obligatorios." });

            try
            {
                var todosLosClientes = _db.Usuarios
                    .Include("UsuarioRoles")
                    .Where(u => u.Activo && u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"))
                    .ToList();

                int enviados = 0;
                int correosEnviados = 0;

                foreach (var usuario in todosLosClientes)
                {
                    var notif = new Notificacion
                    {
                        UsuarioId = usuario.UsuarioId,
                        Titulo = titulo,
                        Mensaje = mensaje,
                        Fecha = DateTime.Now,
                        Leida = false,
                        Tipo = "Admin",
                        Estado = tipo
                    };
                    _db.Notificaciones.Add(notif);
                    enviados++;

                    if (!string.IsNullOrWhiteSpace(usuario.Correo))
                    {
                        try
                        {
                            string nombreCompleto = (usuario.Nombre + " " + usuario.Apellidos).Trim();
                            bool ok = EmailService.Enviar(
                                usuario.Correo,
                                "CNFL · " + titulo,
                                titulo,
                                mensaje,
                                nombreCompleto
                            );

                            if (ok) correosEnviados++;
                        }
                        catch { /* continuar con los demás */ }
                    }
                }

                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    enviados = enviados,
                    correos = correosEnviados,
                    message = "Se enviaron " + enviados + " notificaciones y " + correosEnviados + " correos a todos los clientes."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // NOTIFICAR TRÁMITE ESPECÍFICO
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NotificarTramite(int tramiteId, string titulo, string mensaje)
        {
            if (!EsAdmin())
                return Json(new { success = false, message = "Sesión expirada." });

            var tramite = _db.Tramites.Include("Usuario").FirstOrDefault(t => t.TramiteId == tramiteId);
            if (tramite == null)
                return Json(new { success = false, message = "Trámite no encontrado." });

            return EnviarConMensajePersonalizado(tramite.UsuarioId, titulo, mensaje, "Tramite");
        }

        // ============================================================
        // NOTIFICAR AVERÍA ESPECÍFICA
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NotificarAveria(int averiaId, string titulo, string mensaje)
        {
            if (!EsAdmin())
                return Json(new { success = false, message = "Sesión expirada." });

            var averia = _db.Averias.Include("Usuario").FirstOrDefault(a => a.AveriaId == averiaId);
            if (averia == null)
                return Json(new { success = false, message = "Avería no encontrada." });

            return EnviarConMensajePersonalizado(averia.UsuarioId, titulo, mensaje, "Averia");
        }

        // ============================================================
        // HELPER: Enviar notificación + correo
        // ============================================================
        private JsonResult EnviarConMensajePersonalizado(int usuarioId, string titulo, string mensaje, string tipo)
        {
            try
            {
                var usuario = _db.Usuarios.Find(usuarioId);
                if (usuario == null)
                    return Json(new { success = false, message = "Usuario no encontrado." });

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
                    string nombreCompleto = (usuario.Nombre + " " + usuario.Apellidos).Trim();
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
                    success = true,
                    correo = correoOk,
                    message = correoOk
                        ? "Notificación enviada y correo enviado a " + usuario.Correo
                        : "Notificación creada pero no se pudo enviar el correo"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // AJAX: Buscar clientes
        // ============================================================
        [HttpGet]
        public JsonResult BuscarClientes(string q = "")
        {
            if (!EsAdmin())
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

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

            return Json(new { success = true, clientes = clientes }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // DTO para clientes
    public class ClienteDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}