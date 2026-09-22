using CNFL_Clientes_Prototipo.Data;
using CNFL_Clientes_Prototipo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ============================================================
        // PERFIL Y DATOS DEL USUARIO
        // ============================================================

        public ActionResult MiPerfil()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .Include("Suscripciones")
                .Include("ActividadEconomica")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null) return HttpNotFound();

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            var niseIds = nises.Select(n => n.NiseId).ToList();

            ViewBag.NISEs = nises;
            ViewBag.UltimosPagos = _db.Pagos
                .Include("Factura")
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(5)
                .ToList();

            ViewBag.AveriasActivas = _db.Averias
                .Include("NISE")
                .Where(a => a.UsuarioId == usuarioId
                         && a.Estado != "Problema resuelto"
                         && a.Estado != "Resuelto")
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            return View(usuario);
        }

        public ActionResult EditarPerfil()
        {
            return RedirectToAction("EditarDatos");
        }

        public ActionResult EditarDatos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("ActividadEconomica")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null) return HttpNotFound();

            ViewBag.ActividadesEconomicas = new SelectList(
                _db.ActividadesEconomicas.OrderBy(a => a.Codigo).ToList(),
                "Id", "Nombre", usuario.ActividadEconomicaId);

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDatos(Usuario model, string NombreCompleto,
            bool FacturaElectronica = false, int? ActividadEconomicaId = null)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null) return HttpNotFound();

            if (!string.IsNullOrWhiteSpace(NombreCompleto))
            {
                var partes = NombreCompleto.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 3)
                {
                    usuario.Apellidos = string.Join(" ", partes.Skip(partes.Length - 2));
                    usuario.Nombre = string.Join(" ", partes.Take(partes.Length - 2));
                }
                else if (partes.Length == 2)
                {
                    usuario.Nombre = partes[0];
                    usuario.Apellidos = partes[1];
                }
                else
                {
                    usuario.Nombre = NombreCompleto.Trim();
                    usuario.Apellidos = "";
                }
            }

            usuario.Correo = model.Correo;
            usuario.CorreoSecundario = model.CorreoSecundario;
            usuario.Telefono = model.Telefono;
            usuario.TelefonoSecundario = model.TelefonoSecundario;

            usuario.FacturaElectronica = FacturaElectronica;
            usuario.ActividadEconomicaId = (FacturaElectronica && ActividadEconomicaId.HasValue)
                ? ActividadEconomicaId
                : (int?)null;

            _db.SaveChanges();
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;

            TempData["Mensaje"] = "Datos actualizados correctamente.";
            return RedirectToAction("MiPerfil");
        }

        // ============================================================
        // FACTURAS Y PAGOS
        // ============================================================

        public ActionResult MisFacturas()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var nisesIds = _db.NISEs
                .Where(n => n.UsuarioId == usuarioId)
                .Select(n => n.NiseId)
                .ToList();

            var facturas = _db.Facturas
                .Include("NISE")
                .Where(f => nisesIds.Contains(f.NiseId))
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            return View(facturas);
        }

        public ActionResult Pagos()
        {
            return RedirectToAction("MisFacturas");
        }

        [HttpPost]
        public JsonResult PagarFactura(int facturaId)
        {
            var factura = _db.Facturas.Find(facturaId);
            if (factura != null && !factura.Pagada)
            {
                factura.Pagada = true;
                _db.SaveChanges();

                var notificacion = new Notificacion
                {
                    UsuarioId = _db.NISEs.Find(factura.NiseId).UsuarioId,
                    Titulo = "Pago realizado",
                    Mensaje = "Se ha realizado el pago de la factura " + factura.NumeroFactura + " por ₡" + factura.Monto.ToString("N0") + ".",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Pago",
                    Estado = "Pagada"
                };
                _db.Notificaciones.Add(notificacion);
                _db.SaveChanges();

                return Json(new { success = true, message = "Pago realizado exitosamente." });
            }
            return Json(new { success = false, message = "La factura no existe o ya fue pagada." });
        }

        // ============================================================
        // NOTIFICACIONES
        // ============================================================

        public ActionResult MisNotificaciones()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var notificaciones = _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToList();

            foreach (var not in notificaciones.Where(n => !n.Leida))
            {
                not.Leida = true;
            }
            _db.SaveChanges();

            return View(notificaciones);
        }

        // ============================================================
        // DASHBOARD
        // ============================================================

        public ActionResult Dashboard()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            if (usuario == null) return HttpNotFound();

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            var niseIds = nises.Select(n => n.NiseId).ToList();

            var facturas = _db.Facturas
                .Include("NISE")
                .Where(f => niseIds.Contains(f.NiseId))
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            var facturasPendientes = facturas.Where(f => !f.Pagada).OrderBy(f => f.FechaVencimiento).ToList();
            var facturaActual = facturasPendientes.FirstOrDefault();
            var totalPendiente = facturasPendientes.Sum(f => (decimal?)f.Monto) ?? 0m;

            var averiasActivas = _db.Averias
                .Where(a => a.UsuarioId == usuarioId
                         && a.Estado != "Problema resuelto"
                         && a.Estado != "Resuelto")
                .OrderByDescending(a => a.FechaReporte)
                .Select(a => new AveriaResumenDto
                {
                    AveriaId = a.AveriaId,
                    Tipo = a.Tipo,
                    Estado = a.Estado,
                    FechaReporte = a.FechaReporte
                })
                .ToList();

            var tramitesActivos = _db.Tramites
                .Where(t => t.UsuarioId == usuarioId && t.Estado != "Resuelto")
                .OrderByDescending(t => t.FechaSolicitud)
                .Select(t => new TramiteResumenDto
                {
                    TramiteId = t.TramiteId,
                    Tipo = t.Tipo,
                    Categoria = t.Categoria,
                    Estado = t.Estado,
                    FechaSolicitud = t.FechaSolicitud,
                    NumeroReferencia = t.NumeroReferencia,
                    Descripcion = t.Descripcion
                })
                .ToList();

            var notificaciones = _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .OrderByDescending(n => n.Fecha)
                .Take(3)
                .Select(n => new NotificacionResumenDto
                {
                    NotificacionId = n.NotificacionId,
                    Titulo = n.Titulo,
                    Mensaje = n.Mensaje,
                    Fecha = n.Fecha,
                    Tipo = n.Tipo
                })
                .ToList();

            ViewBag.Nombre = usuario.Nombre;
            ViewBag.TotalNISEs = nises.Count;
            ViewBag.TotalFacturasPendientes = facturasPendientes.Count;
            ViewBag.MontoPendiente = totalPendiente;
            ViewBag.TotalAveriasActivas = averiasActivas.Count;
            ViewBag.TotalTramitesActivos = tramitesActivos.Count;

            ViewBag.FacturaActual = facturaActual;
            ViewBag.AveriasActivas = averiasActivas;
            ViewBag.TramitesActivos = tramitesActivos;
            ViewBag.Notificaciones = notificaciones;

            Session["FotoPerfil"] = usuario.FotoPerfil;
            Session["Correo"] = usuario.Correo;
            Session["Telefono"] = usuario.Telefono;
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;

            return View();
        }

        // ============================================================
        // MIS SERVICIOS
        // ============================================================

        public ActionResult MisServicios()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = nises;

            var facturasPorNise = new Dictionary<int, List<Factura>>();
            foreach (var n in nises)
            {
                facturasPorNise[n.NiseId] = _db.Facturas
                    .Where(f => f.NiseId == n.NiseId)
                    .OrderByDescending(f => f.FechaEmision)
                    .ToList();
            }

            ViewBag.FacturasPorNise = facturasPorNise;
            return View();
        }

        // ============================================================
        // HISTORIAL DE CONSUMO
        // ============================================================

        public ActionResult HistorialConsumo()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            var niseIds = nises.Select(n => n.NiseId).ToList();

            var facturas = _db.Facturas
                .Where(f => niseIds.Contains(f.NiseId))
                .OrderByDescending(f => f.FechaEmision)
                .Take(12)
                .OrderBy(f => f.FechaEmision)
                .ToList();

            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var historial = facturas.Select(f => new
            {
                mes = meses[f.FechaEmision.Month - 1] + " " + f.FechaEmision.Year.ToString().Substring(2),
                kwh = Math.Round((double)f.Monto / 78.0, 0),
                monto = f.Monto
            }).ToList();

            ViewBag.HistorialJson = Newtonsoft.Json.JsonConvert.SerializeObject(historial);
            ViewBag.Promedio = historial.Any() ? Math.Round(historial.Average(h => h.kwh), 0) : 0;
            ViewBag.Maximo = historial.Any() ? historial.Max(h => h.kwh) : 0;
            ViewBag.Ultimo = historial.Any() ? historial.Last().kwh : 0;
            return View();
        }

        // ============================================================
        // CONSULTA AL MEDIDOR (AMI)
        // ============================================================

        public ActionResult ConsultaMedidor()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var nise = _db.NISEs.FirstOrDefault(n => n.UsuarioId == usuarioId);

            ViewBag.UltimaFecha = DateTime.Now.AddHours(-3);
            ViewBag.UltimaLectura = 742.0;
            ViewBag.LecturaFacturada = 620.0;
            ViewBag.ConsumoMes = 198.0;
            ViewBag.CostoEstimado = 21300m;
            ViewBag.PromedioMensual = 205.0;
            ViewBag.VoltajeActual = 119.8;
            ViewBag.NISEActual = nise;
            return View();
        }

        // ============================================================
        // MAPA GIS
        // ============================================================

        public ActionResult MapaGIS()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        [HttpGet]
        public JsonResult MapaGISData()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var averias = _db.Averias
                .Where(a => a.Latitud != null && a.Longitud != null)
                .OrderByDescending(a => a.FechaReporte)
                .Take(50)
                .Select(a => new
                {
                    id = a.AveriaId,
                    tipo = a.Tipo ?? "Sin tipo",
                    estado = a.Estado ?? "Ingresado",
                    lat = a.Latitud,
                    lng = a.Longitud,
                    direccion = a.Direccion ?? "Sin dirección"
                })
                .ToList();

            return Json(new { success = true, data = averias }, JsonRequestBehavior.AllowGet);
        }

        // ============================================================
        // ALERTAS
        // ============================================================

        public ActionResult Alertas()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var alertas = _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .Take(50)
                .ToList();

            ViewBag.Alertas = alertas;
            return View();
        }

        // ============================================================
        // PUSH
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NotificarPush(string titulo, string mensaje)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Sesión expirada." });

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(mensaje))
                return Json(new { success = false, message = "Título y mensaje son obligatorios." });

            try
            {
                var notif = new Notificacion
                {
                    UsuarioId = usuarioId.Value,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Push",
                    Estado = "Enviada"
                };
                _db.Notificaciones.Add(notif);
                _db.SaveChanges();

                EnviarCorreoReal(usuarioId.Value, titulo, mensaje);

                return Json(new { success = true, id = notif.NotificacionId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // CREAR TRÁMITE COMPLETO (con documentos adjuntos)
        // ============================================================
        [HttpPost]
        public JsonResult CrearTramiteCompleto()
        {
            try
            {
                var usuarioId = Session["UsuarioId"] as int?;
                if (usuarioId == null)
                    return Json(new { success = false, message = "Sesión expirada." });

                var tipo = Request.Form["tipo"];
                var descripcion = Request.Form["descripcion"] ?? "";
                var nise = Request.Form["nise"] ?? "";
                var telefono = Request.Form["telefono"] ?? "";

                if (string.IsNullOrWhiteSpace(tipo))
                    return Json(new { success = false, message = "Falta el tipo de trámite." });

                var numeroRef = "TR-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var tramite = new Tramite
                {
                    UsuarioId = usuarioId.Value,
                    Tipo = tipo,
                    Categoria = "General",
                    Estado = "Iniciado",
                    FechaSolicitud = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Descripcion = descripcion,
                    NumeroReferencia = numeroRef,
                    DatosFormulario = "{}"
                };

                _db.Tramites.Add(tramite);
                _db.SaveChanges();

                var archivos = Request.Files;
                var nombresArchivos = Request.Form.GetValues("nombresArchivos") ?? new string[0];
                int guardados = 0;

                if (archivos != null && archivos.Count > 0)
                {
                    var carpeta = Server.MapPath("~/Content/uploads/tramites/");
                    if (!System.IO.Directory.Exists(carpeta))
                        System.IO.Directory.CreateDirectory(carpeta);

                    for (int i = 0; i < archivos.Count; i++)
                    {
                        var archivo = archivos[i];
                        if (archivo == null || archivo.ContentLength == 0) continue;

                        var nombreRequisito = i < nombresArchivos.Length ? nombresArchivos[i] : "Documento";
                        var extension = System.IO.Path.GetExtension(archivo.FileName).ToLower();

                        var extensionesValidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        if (Array.IndexOf(extensionesValidas, extension) < 0) continue;

                        var nombreGuardar = "tramite_" + tramite.TramiteId + "_" + i + "_" + DateTime.Now.Ticks + extension;
                        var rutaCompleta = System.IO.Path.Combine(carpeta, nombreGuardar);
                        archivo.SaveAs(rutaCompleta);

                        var doc = new TramiteDocumento
                        {
                            TramiteId = tramite.TramiteId,
                            NombreRequisito = nombreRequisito,
                            NombreArchivo = archivo.FileName,
                            RutaArchivo = "/Content/uploads/tramites/" + nombreGuardar,
                            TamanoBytes = archivo.ContentLength,
                            FechaSubida = DateTime.Now
                        };

                        _db.TramiteDocumentos.Add(doc);
                        guardados++;
                    }

                    _db.SaveChanges();
                }

                var notif = new Notificacion
                {
                    UsuarioId = usuarioId.Value,
                    Titulo = "Trámite iniciado",
                    Mensaje = tipo + " · " + numeroRef + " · Adjuntaste " + guardados + " documento(s).",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Tramite",
                    Estado = "Iniciado"
                };
                _db.Notificaciones.Add(notif);
                _db.SaveChanges();

                EnviarCorreoReal(usuarioId.Value,
                    "Trámite iniciado · " + tipo,
                    "Tu trámite <b>" + tipo + "</b> fue recibido con el número de referencia <b>" + numeroRef + "</b>. Adjuntaste " + guardados + " documento(s).");

                return Json(new { success = true, id = numeroRef, tramiteId = tramite.TramiteId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // DETALLE DEL TRÁMITE (CLIENTE)
        // ============================================================
        public ActionResult DetalleTramite(int id = 0)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var tramite = _db.Tramites
                .Include("TramiteDocumentos")
                .FirstOrDefault(t => t.TramiteId == id && t.UsuarioId == usuarioId);

            if (tramite == null) return HttpNotFound();

            ViewBag.Tramite = tramite;
            return View();
        }

        // ============================================================
        // REGISTRAR ACTIVIDAD (AJAX con tiempo real) ← NUEVO
        // ============================================================
        [HttpPost]
        public JsonResult RegistrarActividad(string seccion, string accion, int segundos, string detalle = null)
        {
            try
            {
                var usuarioId = Session["UsuarioId"] as int?;
                if (usuarioId == null)
                    return Json(new { ok = false, mensaje = "Sesión expirada." });

                // Excluir admins
                var esAdmin = _db.UsuarioRoles
                    .Any(ur => ur.UsuarioId == usuarioId.Value && ur.Rol.NombreRol == "Admin");
                if (esAdmin)
                    return Json(new { ok = false, mensaje = "Admin no se registra." });

                if (string.IsNullOrWhiteSpace(seccion))
                    seccion = "Desconocida";

                if (segundos <= 0 || segundos > 7200) segundos = 30;

                var actividad = new ActividadUsuario
                {
                    UsuarioId = usuarioId.Value,
                    Seccion = seccion,
                    Accion = string.IsNullOrWhiteSpace(accion) ? "Vista" : accion,
                    Detalle = detalle,
                    DuracionSegundos = segundos,
                    Fecha = DateTime.Now
                };

                _db.ActividadUsuario.Add(actividad);
                _db.SaveChanges();

                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = ex.Message });
            }
        }

        // ============================================================
        // ENVÍO DE CORREO REAL (SMTP)
        // ============================================================

        private void EnviarCorreoReal(int usuarioId, string titulo, string mensaje)
        {
            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Correo)) return;

            try
            {
                var smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var smtpUser = System.Configuration.ConfigurationManager.AppSettings["SmtpUser"] ?? "";
                var smtpPass = System.Configuration.ConfigurationManager.AppSettings["SmtpPass"] ?? "";
                var smtpFrom = System.Configuration.ConfigurationManager.AppSettings["SmtpFrom"] ?? smtpUser;

                if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                    return;

                using (var smtp = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);

                    var mail = new System.Net.Mail.MailMessage
                    {
                        From = new System.Net.Mail.MailAddress(smtpFrom, "CNFL"),
                        Subject = "CNFL · " + titulo,
                        IsBodyHtml = true,
                        Body = @"
                            <div style='font-family: Arial, sans-serif; max-width: 560px; margin: 0 auto; padding: 24px; background: #f5f6fa; border-radius: 16px;'>
                                <div style='text-align: center; margin-bottom: 20px;'>
                                    <h1 style='color: #1E23E6; font-size: 22px; margin: 0;'>CNFL</h1>
                                </div>
                                <div style='background: #fff; border-radius: 14px; padding: 24px; box-shadow: 0 4px 14px rgba(16,20,40,.06);'>
                                    <h2 style='color: #0E1116; font-size: 18px; margin: 0 0 12px;'>" + titulo + @"</h2>
                                    <p style='color: #727A86; font-size: 14px; line-height: 1.5; margin: 0 0 20px;'>" + mensaje + @"</p>
                                    <a href='http://localhost:44387/Clientes/Alertas'
                                       style='display: inline-block; background: #FF692D; color: #fff; text-decoration: none; padding: 12px 22px; border-radius: 10px; font-weight: 700; font-size: 14px;'>
                                        Ver en la app
                                    </a>
                                </div>
                                <p style='color: #aab0be; font-size: 11px; text-align: center; margin-top: 18px; line-height: 1.5;'>
                                    Este correo fue enviado automáticamente por CNFL.<br>
                                    Si no solicitaste esta notificación, ignoralo.
                                </p>
                            </div>"
                    };
                    mail.To.Add(usuario.Correo);

                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error enviando correo: " + ex.Message);
            }
        }

        // ============================================================
        // TRÁMITES (INDEX)
        // ============================================================

        public ActionResult Tramites()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var activos = _db.Tramites
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaSolicitud)
                .ToList();

            ViewBag.TramitesActivos = activos;
            return View();
        }

        // ============================================================
        // PRODUCTOS Y SERVICIOS
        // ============================================================

        public ActionResult ProductosServicios()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId.Value);
            if (usuario != null)
            {
                Session["Correo"] = usuario.Correo;
                Session["Telefono"] = usuario.Telefono;
                Session["FotoPerfil"] = usuario.FotoPerfil;
            }

            var nise = _db.NISEs
                .Where(n => n.UsuarioId == usuarioId.Value)
                .Select(n => n.NumeroNise)
                .FirstOrDefault();

            Session["Nise"] = nise ?? "";

            return View();
        }

        // ============================================================
        // TIENDA / CARRITO / COMPRAS / PAGOS
        // ============================================================

        public ActionResult Tienda()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        public ActionResult DetalleProducto(int id = 0)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            ViewBag.ProductoId = id;
            return View();
        }

        public ActionResult MisCompras()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        public ActionResult MetodosPago()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // ============================================================
        // REPORTES / AVERÍAS
        // ============================================================

        public ActionResult Reportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        public ActionResult EstadoAveria(int id = 0)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var averia = _db.Averias.FirstOrDefault(a => a.AveriaId == id && a.UsuarioId == usuarioId);

            ViewBag.AveriaId = id;
            ViewBag.Averia = averia;
            return View();
        }

        public ActionResult HistorialReportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var averias = _db.Averias
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaReporte)
                .Take(50)
                .ToList();

            ViewBag.Averias = averias;
            return View();
        }

        public ActionResult Calculadora()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // ============================================================
        // CUENTA
        // ============================================================

        public ActionResult Cuenta()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("ActividadEconomica")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null) return HttpNotFound();

            ViewBag.NISEs = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.ActividadesEconomicas = _db.ActividadesEconomicas
                .OrderBy(a => a.Codigo)
                .Select(a => new ActividadEconomicaDto
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre
                })
                .ToList();
            ViewBag.ActividadActualId = usuario.ActividadEconomicaId;

            Session["FotoPerfil"] = usuario.FotoPerfil;
            Session["Correo"] = usuario.Correo;
            Session["Telefono"] = usuario.Telefono;
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarCuenta(Usuario model, string NombreCompleto,
            bool FacturaElectronica = false, int? ActividadEconomicaId = null,
            HttpPostedFileBase FotoPerfil = null)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null) return HttpNotFound();

            if (!string.IsNullOrWhiteSpace(NombreCompleto))
            {
                var partes = NombreCompleto.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 3)
                {
                    usuario.Apellidos = string.Join(" ", partes.Skip(partes.Length - 2));
                    usuario.Nombre = string.Join(" ", partes.Take(partes.Length - 2));
                }
                else if (partes.Length == 2)
                {
                    usuario.Nombre = partes[0];
                    usuario.Apellidos = partes[1];
                }
                else
                {
                    usuario.Nombre = NombreCompleto.Trim();
                    usuario.Apellidos = "";
                }
            }

            usuario.Correo = model.Correo;
            usuario.Telefono = model.Telefono;
            usuario.CorreoSecundario = model.CorreoSecundario;
            usuario.TelefonoSecundario = model.TelefonoSecundario;

            usuario.FacturaElectronica = FacturaElectronica;
            usuario.ActividadEconomicaId = (FacturaElectronica && ActividadEconomicaId.HasValue)
                ? ActividadEconomicaId
                : (int?)null;

            if (FotoPerfil != null && FotoPerfil.ContentLength > 0)
            {
                var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
                if (Array.IndexOf(tiposPermitidos, FotoPerfil.ContentType) >= 0)
                {
                    var extension = System.IO.Path.GetExtension(FotoPerfil.FileName).ToLower();
                    var nombreArchivo = "perfil_" + usuario.UsuarioId + "_" + DateTime.Now.Ticks + extension;

                    var carpeta = Server.MapPath("~/Content/uploads/perfiles/");
                    if (!System.IO.Directory.Exists(carpeta))
                        System.IO.Directory.CreateDirectory(carpeta);

                    var rutaCompleta = System.IO.Path.Combine(carpeta, nombreArchivo);
                    FotoPerfil.SaveAs(rutaCompleta);

                    if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                    {
                        var rutaAnterior = Server.MapPath(usuario.FotoPerfil);
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            try { System.IO.File.Delete(rutaAnterior); } catch { }
                        }
                    }

                    usuario.FotoPerfil = "/Content/uploads/perfiles/" + nombreArchivo;
                }
            }

            _db.SaveChanges();
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
            Session["FotoPerfil"] = usuario.FotoPerfil;
            Session["Correo"] = usuario.Correo;
            Session["Telefono"] = usuario.Telefono;

            TempData["Mensaje"] = "Datos actualizados correctamente.";
            return RedirectToAction("Cuenta");
        }

        [HttpPost]
        public JsonResult SubirFotoPerfil(HttpPostedFileBase fotoPerfil)
        {
            try
            {
                var usuarioId = Session["UsuarioId"] as int?;
                if (usuarioId == null)
                    return Json(new { ok = false, mensaje = "Sesión expirada." });

                if (fotoPerfil == null || fotoPerfil.ContentLength == 0)
                    return Json(new { ok = false, mensaje = "No se seleccionó ninguna imagen." });

                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = System.IO.Path.GetExtension(fotoPerfil.FileName).ToLower();

                if (Array.IndexOf(extensionesPermitidas, extension) < 0)
                    return Json(new { ok = false, mensaje = "Formato no permitido." });

                if (fotoPerfil.ContentLength > 4 * 1024 * 1024)
                    return Json(new { ok = false, mensaje = "La imagen no puede pesar más de 4 MB." });

                var carpeta = Server.MapPath("~/Content/uploads/perfiles/");
                if (!System.IO.Directory.Exists(carpeta))
                    System.IO.Directory.CreateDirectory(carpeta);

                var nombreArchivo = "perfil_" + usuarioId.Value + "_" + DateTime.Now.Ticks + extension;
                var rutaCompleta = System.IO.Path.Combine(carpeta, nombreArchivo);
                fotoPerfil.SaveAs(rutaCompleta);

                var rutaRelativa = "/Content/uploads/perfiles/" + nombreArchivo;

                var usuario = _db.Usuarios.Find(usuarioId.Value);
                if (usuario == null)
                    return Json(new { ok = false, mensaje = "Usuario no encontrado." });

                if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                {
                    var rutaAnterior = Server.MapPath(usuario.FotoPerfil);
                    if (System.IO.File.Exists(rutaAnterior))
                    {
                        try { System.IO.File.Delete(rutaAnterior); } catch { }
                    }
                }

                usuario.FotoPerfil = rutaRelativa;
                _db.SaveChanges();

                Session["FotoPerfil"] = rutaRelativa;

                return Json(new { ok = true, mensaje = "Foto actualizada.", ruta = rutaRelativa });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error: " + ex.Message });
            }
        }

        // ============================================================
        // AJAX: Provincia → Cantón → Distrito
        // ============================================================

        [HttpGet]
        public JsonResult GetCantones(string provincia)
        {
            if (string.IsNullOrWhiteSpace(provincia) ||
                !UbicacionCostaRica.Catalogo.ContainsKey(provincia))
                return Json(new string[0], JsonRequestBehavior.AllowGet);

            return Json(UbicacionCostaRica.Catalogo[provincia].Keys.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDistritos(string provincia, string canton)
        {
            if (string.IsNullOrWhiteSpace(provincia) ||
                string.IsNullOrWhiteSpace(canton) ||
                !UbicacionCostaRica.Catalogo.ContainsKey(provincia) ||
                !UbicacionCostaRica.Catalogo[provincia].ContainsKey(canton))
                return Json(new string[0], JsonRequestBehavior.AllowGet);

            return Json(UbicacionCostaRica.Catalogo[provincia][canton], JsonRequestBehavior.AllowGet);
        }

        // ============================================================
        // DISPOSE
        // ============================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}