using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        // ==========================================================
        // USAR BASE DE DATOS
        // ==========================================================
        private readonly CNFLDbContext _db = new CNFLDbContext();

        // ==========================================================
        // DATOS TSE SIMULADO (SOLO PARA AUTOCOMPLETAR)
        // ==========================================================
        private Dictionary<string, (string Nombre, string Apellidos, List<string> NISEs, DateTime FechaNacimiento)> DatosTSE
        {
            get
            {
                var key = "DatosTSE";
                if (HttpRuntime.Cache[key] == null)
                {
                    var datos = new Dictionary<string, (string, string, List<string>, DateTime)>
                    {
                        { "2-0874-0716", ("Samuel", "Sandoval Ramírez", new List<string> { "402112345", "402198765", "123456789" }, new DateTime(2001, 2, 21)) },
                        { "1-2345-6789", ("Katherine", "Villalobos", new List<string> { "402112345", "402198765" }, new DateTime(1990, 5, 15)) },
                        { "1-1234-5678", ("Juan", "Pérez Rodríguez", new List<string> { "123456789" }, new DateTime(1985, 3, 10)) },
                        { "1-8765-4321", ("María", "Gómez Fernández", new List<string> { "987654321" }, new DateTime(1992, 8, 22)) },
                        { "1-5555-6666", ("Carlos", "Rodríguez Mora", new List<string> { "456789123" }, new DateTime(1988, 11, 5)) },
                        { "1-1111-2222", ("Ana", "Mora Solís", new List<string> { "111222333", "444555666" }, new DateTime(1995, 12, 1)) }
                    };

                    HttpRuntime.Cache.Insert(key, datos, null, DateTime.Now.AddDays(30), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                return HttpRuntime.Cache[key] as Dictionary<string, (string, string, List<string>, DateTime)>;
            }
        }

        // ==========================================================
        // MÉTODOS DE VALIDACIÓN (AJAX)
        // ==========================================================

        [HttpPost]
        public JsonResult ValidarCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
            {
                return Json(new { success = false, message = "Ingrese una cédula" });
            }

            // Verificar formato
            bool formatoValido = System.Text.RegularExpressions.Regex.IsMatch(cedula, @"^\d{1}-\d{4}-\d{4}$") ||
                                 System.Text.RegularExpressions.Regex.IsMatch(cedula, @"^\d{9,10}$");
            if (!formatoValido)
            {
                return Json(new { success = false, message = "❌ Formato inválido. Use 1-2345-6789" });
            }

            // Verificar si ya está registrada en BD
            var existeEnBD = _db.Usuarios.Any(u => u.Cedula == cedula);
            if (existeEnBD)
            {
                return Json(new { success = false, message = "❌ Cédula ya registrada" });
            }

            // Buscar en TSE simulado para autocompletar
            if (DatosTSE.TryGetValue(cedula, out var datos))
            {
                return Json(new
                {
                    success = true,
                    nombre = datos.Nombre,
                    apellidos = datos.Apellidos,
                    nises = datos.NISEs,
                    fechaNacimiento = datos.FechaNacimiento.ToString("yyyy-MM-dd")
                });
            }

            // Si no existe en TSE, permitir registro manual
            return Json(new
            {
                success = true,
                nombre = "",
                apellidos = "",
                nises = new List<string>(),
                fechaNacimiento = ""
            });
        }

        [HttpPost]
        public JsonResult ValidarUsuario(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "Ingrese un usuario" });
            }

            var existe = _db.Usuarios.Any(u => u.UserName == userName);
            return Json(new { success = !existe, message = existe ? "❌ Usuario no disponible" : "✅ Usuario disponible" });
        }

        [HttpPost]
        public JsonResult ValidarFormatoCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
            {
                return Json(new { success = false, message = "Ingrese una cédula" });
            }

            bool esValida = System.Text.RegularExpressions.Regex.IsMatch(cedula, @"^\d{1}-\d{4}-\d{4}$") ||
                            System.Text.RegularExpressions.Regex.IsMatch(cedula, @"^\d{9,10}$");

            return Json(new { success = esValida, message = esValida ? "✅ Formato válido" : "❌ Formato inválido (use 1-2345-6789)" });
        }

        [HttpPost]
        public JsonResult ValidarFormatoTelefono(string telefono)
        {
            if (string.IsNullOrEmpty(telefono))
            {
                return Json(new { success = false, message = "Ingrese un teléfono" });
            }

            bool esValida = System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^\d{4}-\d{4}$") ||
                            System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^\d{8}$");

            return Json(new { success = esValida, message = esValida ? "✅ Formato válido" : "❌ Formato inválido (use 8888-8888)" });
        }

        [HttpPost]
        public JsonResult ValidarFormatoNISE(string nise)
        {
            if (string.IsNullOrEmpty(nise))
            {
                return Json(new { success = false, message = "Ingrese un NISE" });
            }

            bool esValida = System.Text.RegularExpressions.Regex.IsMatch(nise, @"^\d{9}$");

            // Si el NISE no está en TSE pero el usuario lo ingresa manualmente, permitirlo
            if (!DatosTSE.Values.Any(d => d.NISEs.Contains(nise)))
            {
                return Json(new { success = esValida, message = esValida ? "✅ NISE válido" : "❌ El NISE debe tener 9 dígitos" });
            }

            return Json(new
            {
                success = esValida,
                message = esValida ? "✅ NISE válido" : "❌ El NISE debe tener 9 dígitos"
            });
        }

        [HttpPost]
        public JsonResult RecuperarContraseña(string correo)
        {
            if (string.IsNullOrEmpty(correo))
            {
                return Json(new { success = false, message = "Ingrese un correo electrónico" });
            }

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo);
            if (usuario != null)
            {
                return Json(new { success = true, message = "✅ Se ha enviado un enlace de recuperación a su correo" });
            }

            return Json(new { success = false, message = "❌ Correo no encontrado en el sistema" });
        }

        // ==========================================================
        // RECUPERAR CONTRASEÑA - VISTA
        // ==========================================================
        public ActionResult RecuperarClave()
        {
            if (Session["Rol"] != null)
            {
                if (Session["Rol"].ToString() == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Inicio", "Clientes");
            }

            return View();
        }

        [HttpPost]
        public ActionResult RecuperarClave(string correo, string nuevaClave, string confirmarClave)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(nuevaClave) || string.IsNullOrEmpty(confirmarClave))
            {
                ViewBag.Error = "❌ Todos los campos son obligatorios.";
                return View();
            }

            if (nuevaClave.Length < 6)
            {
                ViewBag.Error = "❌ La contraseña debe tener al menos 6 caracteres.";
                return View();
            }

            if (nuevaClave != confirmarClave)
            {
                ViewBag.Error = "❌ Las contraseñas no coinciden.";
                return View();
            }

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo);
            if (usuario == null)
            {
                ViewBag.Error = "❌ Correo no encontrado en el sistema.";
                return View();
            }

            usuario.Contraseña = nuevaClave;
            _db.SaveChanges();

            TempData["Mensaje"] = "✅ ¡Contraseña actualizada exitosamente! Ahora puedes iniciar sesión.";

            return RedirectToAction("Login");
        }

        // ==========================================================
        // ACCIONES DE VISTA
        // ==========================================================

        public ActionResult Index()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Inicio", "Clientes");
        }

        public ActionResult Login(string returnUrl = "")
        {
            if (Session["Rol"] != null)
            {
                if (Session["Rol"].ToString() == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Inicio", "Clientes");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var usuario = _db.Usuarios.FirstOrDefault(u =>
                    (u.UserName == model.UserName || u.Cedula == model.UserName) &&
                    u.Contraseña == model.Contraseña &&
                    u.Activo == true);

                if (usuario != null)
                {
                    Session["Id"] = usuario.Id;
                    Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["NombreCompleto"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["Correo"] = usuario.Correo;
                    Session["Rol"] = usuario.RolId == 2 ? "Admin" : "Cliente";
                    Session["RolId"] = usuario.RolId;
                    Session["NISE"] = usuario.NISE;
                    Session["Cedula"] = usuario.Cedula;
                    Session["Telefono"] = usuario.Telefono;
                    Session["UserName"] = usuario.UserName;
                    Session["FechaNacimiento"] = usuario.FechaNacimiento.ToString("dd/MM/yyyy");

                    if (usuario.RolId == 2)
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Inicio", "Clientes");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "❌ Usuario o contraseña incorrectos");
                }
            }

            return View(model);
        }

        public ActionResult Registro()
        {
            if (Session["Rol"] != null)
            {
                if (Session["Rol"].ToString() == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Inicio", "Clientes");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Registro(RegistroViewModel model)
        {
            // ==========================================================
            // LOG DE DEPURACIÓN
            // ==========================================================
            System.Diagnostics.Debug.WriteLine("=== REGISTRO POST ===");
            System.Diagnostics.Debug.WriteLine($"Nombre: {model.Nombre}");
            System.Diagnostics.Debug.WriteLine($"Apellidos: {model.Apellidos}");
            System.Diagnostics.Debug.WriteLine($"Cedula: {model.Cedula}");
            System.Diagnostics.Debug.WriteLine($"NISE: {model.NISE}");
            System.Diagnostics.Debug.WriteLine($"UserName: {model.UserName}");
            System.Diagnostics.Debug.WriteLine($"Correo: {model.Correo}");
            System.Diagnostics.Debug.WriteLine($"CorreoSecundario: {model.CorreoSecundario}");
            System.Diagnostics.Debug.WriteLine($"Telefono: {model.Telefono}");
            System.Diagnostics.Debug.WriteLine($"TelefonoSecundario: {model.TelefonoSecundario}");
            System.Diagnostics.Debug.WriteLine($"Sexo: {model.Sexo}");
            System.Diagnostics.Debug.WriteLine($"SexoPersonalizado: {model.SexoPersonalizado}");
            System.Diagnostics.Debug.WriteLine($"Direccion: {model.Direccion}");
            System.Diagnostics.Debug.WriteLine($"AceptaPolitica: {model.AceptaPolitica}");
            System.Diagnostics.Debug.WriteLine($"AceptaConsentimiento: {model.AceptaConsentimiento}");

            // ==========================================================
            // VALIDACIONES
            // ==========================================================

            // 1. NOMBRE
            if (string.IsNullOrEmpty(model.Nombre))
            {
                ModelState.AddModelError("", "❌ El nombre es obligatorio.");
                return View(model);
            }

            // 2. APELLIDOS
            if (string.IsNullOrEmpty(model.Apellidos))
            {
                ModelState.AddModelError("", "❌ Los apellidos son obligatorios.");
                return View(model);
            }

            // 3. CÉDULA - VERIFICAR FORMATO Y BD
            if (string.IsNullOrEmpty(model.Cedula))
            {
                ModelState.AddModelError("", "❌ La cédula es obligatoria.");
                return View(model);
            }

            bool cedulaValida = System.Text.RegularExpressions.Regex.IsMatch(model.Cedula, @"^\d{1}-\d{4}-\d{4}$") ||
                                System.Text.RegularExpressions.Regex.IsMatch(model.Cedula, @"^\d{9,10}$");
            if (!cedulaValida)
            {
                ModelState.AddModelError("", "❌ Formato de cédula inválido. Use 1-2345-6789");
                return View(model);
            }

            // Verificar que la cédula no esté registrada en BD
            if (_db.Usuarios.Any(u => u.Cedula == model.Cedula))
            {
                ModelState.AddModelError("", "❌ La cédula ya está registrada.");
                return View(model);
            }

            // 4. NISE
            if (string.IsNullOrEmpty(model.NISE))
            {
                ModelState.AddModelError("", "❌ El NISE es obligatorio.");
                return View(model);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.NISE, @"^\d{9}$"))
            {
                ModelState.AddModelError("", "❌ El NISE debe tener 9 dígitos.");
                return View(model);
            }

            // 5. FECHA DE NACIMIENTO
            if (model.FechaNacimiento == null || model.FechaNacimiento == DateTime.MinValue)
            {
                ModelState.AddModelError("", "❌ La fecha de nacimiento es obligatoria.");
                return View(model);
            }

            // 6. CORREO PRINCIPAL
            if (string.IsNullOrEmpty(model.Correo))
            {
                ModelState.AddModelError("", "❌ El correo principal es obligatorio.");
                return View(model);
            }

            if (!model.Correo.Contains("@") || !model.Correo.Contains("."))
            {
                ModelState.AddModelError("", "❌ Ingrese un correo electrónico válido.");
                return View(model);
            }

            if (_db.Usuarios.Any(u => u.Correo == model.Correo))
            {
                ModelState.AddModelError("", "❌ El correo principal ya está registrado.");
                return View(model);
            }

            // 7. CORREO SECUNDARIO (opcional)
            if (!string.IsNullOrEmpty(model.CorreoSecundario))
            {
                if (!model.CorreoSecundario.Contains("@") || !model.CorreoSecundario.Contains("."))
                {
                    ModelState.AddModelError("", "❌ Ingrese un correo secundario válido.");
                    return View(model);
                }
            }

            // 8. TELÉFONO PRINCIPAL
            if (string.IsNullOrEmpty(model.Telefono))
            {
                ModelState.AddModelError("", "❌ El teléfono principal es obligatorio.");
                return View(model);
            }

            bool telefonoValido = System.Text.RegularExpressions.Regex.IsMatch(model.Telefono, @"^\d{4}-\d{4}$") ||
                                  System.Text.RegularExpressions.Regex.IsMatch(model.Telefono, @"^\d{8}$");
            if (!telefonoValido)
            {
                ModelState.AddModelError("", "❌ Formato de teléfono inválido. Use 8888-8888");
                return View(model);
            }

            // 9. TELÉFONO SECUNDARIO (opcional)
            if (!string.IsNullOrEmpty(model.TelefonoSecundario))
            {
                bool telefonoSecValido = System.Text.RegularExpressions.Regex.IsMatch(model.TelefonoSecundario, @"^\d{4}-\d{4}$") ||
                                          System.Text.RegularExpressions.Regex.IsMatch(model.TelefonoSecundario, @"^\d{8}$");
                if (!telefonoSecValido)
                {
                    ModelState.AddModelError("", "❌ Formato de teléfono secundario inválido. Use 8888-8888");
                    return View(model);
                }
            }

            // 10. SEXO
            if (string.IsNullOrEmpty(model.Sexo))
            {
                ModelState.AddModelError("", "❌ Seleccione su sexo.");
                return View(model);
            }

            if (model.Sexo == "Personalizado" && string.IsNullOrEmpty(model.SexoPersonalizado))
            {
                ModelState.AddModelError("", "❌ Especifique su sexo.");
                return View(model);
            }

            // 11. DIRECCIÓN
            if (string.IsNullOrEmpty(model.Direccion))
            {
                ModelState.AddModelError("", "❌ La dirección es obligatoria.");
                return View(model);
            }

            if (model.Direccion.Length < 5)
            {
                ModelState.AddModelError("", "❌ Ingrese una dirección más detallada.");
                return View(model);
            }

            // 12. USUARIO
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("", "❌ El usuario es obligatorio.");
                return View(model);
            }

            if (model.UserName.Length < 3)
            {
                ModelState.AddModelError("", "❌ El usuario debe tener al menos 3 caracteres.");
                return View(model);
            }

            if (_db.Usuarios.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("", "❌ El usuario ya existe. Por favor, elige otro.");
                return View(model);
            }

            // 13. CONTRASEÑA
            if (string.IsNullOrEmpty(model.Contraseña))
            {
                ModelState.AddModelError("", "❌ La contraseña es obligatoria.");
                return View(model);
            }

            if (model.Contraseña.Length < 6)
            {
                ModelState.AddModelError("", "❌ La contraseña debe tener al menos 6 caracteres.");
                return View(model);
            }

            // 14. ACEPTACIÓN
            if (!model.AceptaPolitica)
            {
                ModelState.AddModelError("", "❌ Debe aceptar la Política de Privacidad.");
                return View(model);
            }

            if (!model.AceptaConsentimiento)
            {
                ModelState.AddModelError("", "❌ Debe aceptar el Consentimiento Informado.");
                return View(model);
            }

            // ==========================================================
            // CREAR USUARIO EN BASE DE DATOS
            // ==========================================================
            var nuevoUsuario = new Usuario
            {
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                Cedula = model.Cedula,
                Telefono = model.Telefono,
                TelefonoSecundario = model.TelefonoSecundario,
                Correo = model.Correo,
                CorreoSecundario = model.CorreoSecundario,
                Sexo = model.Sexo,
                SexoPersonalizado = model.SexoPersonalizado,
                Direccion = model.Direccion,
                NISE = model.NISE,
                UserName = model.UserName,
                Contraseña = model.Contraseña,
                RolId = 1,
                FechaNacimiento = model.FechaNacimiento,
                AceptaPolitica = model.AceptaPolitica,
                AceptaConsentimiento = model.AceptaConsentimiento,
                FechaRegistro = DateTime.Now,
                Activo = true
            };

            _db.Usuarios.Add(nuevoUsuario);
            _db.SaveChanges();

            // ==========================================================
            // CREAR REGISTRO EN CLIENTES
            // ==========================================================
            var nuevoCliente = new Cliente
            {
                UsuarioId = nuevoUsuario.Id,
                Direccion = model.Direccion,
                Provincia = "Pendiente",
                Canton = "Pendiente",
                Distrito = "Pendiente"
            };
            _db.Clientes.Add(nuevoCliente);
            _db.SaveChanges();

            // ==========================================================
            // CREAR NISE ASOCIADO
            // ==========================================================
            var nuevoNISE = new NISE
            {
                ClienteId = nuevoCliente.Id,
                Numero = model.NISE,
                Direccion = "Principal",
                Activo = true
            };
            _db.NISEs.Add(nuevoNISE);
            _db.SaveChanges();

            System.Diagnostics.Debug.WriteLine($"✅ USUARIO REGISTRADO EN BD: {model.UserName}");
            System.Diagnostics.Debug.WriteLine($"ID: {nuevoUsuario.Id}");

            // ==========================================================
            // REDIRIGIR AL LOGIN CON MENSAJE
            // ==========================================================
            TempData["Mensaje"] = "✅ ¡Registro exitoso! Ya puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult CerrarSesion()
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                var cookie = new HttpCookie(".ASPXAUTH");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Cuenta()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Perfil", "Clientes");
        }

        public ActionResult MisDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("EditarDatos", "Clientes");
        }

        public ActionResult Suscripciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("Suscripciones", "Clientes");
        }

        public ActionResult ServiciosContratados()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("ServiciosContratados", "Clientes");
        }

        public ActionResult Calculadora()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("Calculadora", "Clientes");
        }

        public ActionResult Chat()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("Chat", "Clientes");
        }

        public ActionResult HistorialCompras()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("HistorialCompras", "Clientes");
        }

        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            return RedirectToAction("EditarDatos", "Clientes");
        }

        // ==========================================================
        // MÉTODO PARA VERIFICAR USUARIOS (DEBUG)
        // ==========================================================
        public JsonResult GetUsuarios()
        {
            return Json(_db.Usuarios.ToList(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ==========================================================
    // MODELOS DE VISTA
    // ==========================================================
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Contraseña { get; set; }
    }

    public class RegistroViewModel
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Correo { get; set; }
        public string CorreoSecundario { get; set; }
        public string Sexo { get; set; }
        public string SexoPersonalizado { get; set; }
        public string Direccion { get; set; }
        public string NISE { get; set; }
        public string UserName { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool AceptaPolitica { get; set; }
        public bool AceptaConsentimiento { get; set; }
    }
}