using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        // ==========================================================
        // DATOS DE USUARIOS REGISTRADOS
        // ==========================================================
        private static List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario {
                Id = 1,
                Nombre = "Admin",
                Apellidos = "Sistema",
                Cedula = "1-0000-0000",
                Telefono = "0000-0000",
                Correo = "admin@cnfl.go.cr",
                NISE = "000000000",
                UserName = "admin",
                Contraseña = "12345$",
                Rol = "Admin",
                FechaNacimiento = DateTime.Now.AddYears(-30)
            },
            new Usuario {
                Id = 2,
                Nombre = "Katherine",
                Apellidos = "Villalobos",
                Cedula = "1-2345-6789",
                Telefono = "8888-7777",
                Correo = "k.villalobos@correo.cr",
                NISE = "402112345",
                UserName = "cliente",
                Contraseña = "123456",
                Rol = "Cliente",
                FechaNacimiento = new DateTime(1990, 5, 15)
            },
            new Usuario {
                Id = 3,
                Nombre = "Samuel",
                Apellidos = "Sandoval Ramírez",
                Cedula = "2-0874-0716",
                Telefono = "8888-8888",
                Correo = "samuel@correo.cr",
                NISE = "402198765",
                UserName = "samuel",
                Contraseña = "123456",
                Rol = "Cliente",
                FechaNacimiento = new DateTime(2001, 2, 21)
            }
        };

        // ==========================================================
        // DATOS TSE SIMULADO
        // ==========================================================
        private static Dictionary<string, (string Nombre, string Apellidos, List<string> NISEs, DateTime FechaNacimiento)> _datosTSE =
            new Dictionary<string, (string, string, List<string>, DateTime)>
        {
            { "2-0874-0716", ("Samuel", "Sandoval Ramírez", new List<string> { "402112345", "402198765", "123456789" }, new DateTime(2001, 2, 21)) },
            { "1-2345-6789", ("Katherine", "Villalobos", new List<string> { "402112345", "402198765" }, new DateTime(1990, 5, 15)) },
            { "1-1234-5678", ("Juan", "Pérez Rodríguez", new List<string> { "123456789" }, new DateTime(1985, 3, 10)) },
            { "1-8765-4321", ("María", "Gómez Fernández", new List<string> { "987654321" }, new DateTime(1992, 8, 22)) },
            { "1-5555-6666", ("Carlos", "Rodríguez Mora", new List<string> { "456789123" }, new DateTime(1988, 11, 5)) },
            { "1-1111-2222", ("Ana", "Mora Solís", new List<string> { "111222333", "444555666" }, new DateTime(1995, 12, 1)) }
        };

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

            if (_datosTSE.TryGetValue(cedula, out var datos))
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

            return Json(new { success = false, message = "Cédula no encontrada en el sistema TSE" });
        }

        [HttpPost]
        public JsonResult ValidarUsuario(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "Ingrese un usuario" });
            }

            var existe = _usuarios.Any(u => u.UserName == userName);
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

            bool existeEnTSE = _datosTSE.Values.Any(d => d.NISEs.Contains(nise));

            return Json(new
            {
                success = esValida && existeEnTSE,
                message = esValida ? (existeEnTSE ? "✅ NISE válido" : "❌ NISE no asociado a su cédula") : "❌ Formato inválido (use 9 dígitos)"
            });
        }

        [HttpPost]
        public JsonResult RecuperarContraseña(string correo)
        {
            if (string.IsNullOrEmpty(correo))
            {
                return Json(new { success = false, message = "Ingrese un correo electrónico" });
            }

            var usuario = _usuarios.FirstOrDefault(u => u.Correo == correo);
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

            var usuario = _usuarios.FirstOrDefault(u => u.Correo == correo);
            if (usuario == null)
            {
                ViewBag.Error = "❌ Correo no encontrado en el sistema.";
                return View();
            }

            usuario.Contraseña = nuevaClave;
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
                // Buscar por UserName o por Cédula
                var usuario = _usuarios.FirstOrDefault(u =>
                    u.UserName == model.UserName && u.Contraseña == model.Contraseña);

                // Si no encuentra por UserName, buscar por Cédula
                if (usuario == null)
                {
                    usuario = _usuarios.FirstOrDefault(u =>
                        u.Cedula == model.UserName && u.Contraseña == model.Contraseña);
                }

                if (usuario != null)
                {
                    // ==========================================================
                    // GUARDAR TODOS LOS DATOS DEL USUARIO EN LA SESIÓN
                    // ==========================================================
                    Session["Id"] = usuario.Id;
                    Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["NombreCompleto"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["Correo"] = usuario.Correo;
                    Session["Rol"] = usuario.Rol;
                    Session["NISE"] = usuario.NISE;
                    Session["Cedula"] = usuario.Cedula;
                    Session["Telefono"] = usuario.Telefono;
                    Session["UserName"] = usuario.UserName;
                    Session["FechaNacimiento"] = usuario.FechaNacimiento.ToString("dd/MM/yyyy");

                    if (usuario.Rol == "Admin")
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
            if (ModelState.IsValid)
            {
                // ==========================================================
                // 1. VALIDACIÓN DE NISE
                // ==========================================================
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

                bool niseValido = _datosTSE.Values.Any(d => d.NISEs.Contains(model.NISE));
                if (!niseValido)
                {
                    ModelState.AddModelError("", "❌ El NISE ingresado no es válido o no está asociado a su cédula.");
                    return View(model);
                }

                // ==========================================================
                // 2. VALIDAR FECHA DE NACIMIENTO
                // ==========================================================
                if (model.FechaNacimiento == null || model.FechaNacimiento == DateTime.MinValue)
                {
                    ModelState.AddModelError("", "❌ La fecha de nacimiento es obligatoria.");
                    return View(model);
                }

                // ==========================================================
                // 3. VERIFICAR QUE EL USUARIO NO EXISTA
                // ==========================================================
                if (_usuarios.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("", "❌ El usuario ya existe. Por favor, elige otro.");
                    return View(model);
                }

                if (_usuarios.Any(u => u.Cedula == model.Cedula))
                {
                    ModelState.AddModelError("", "❌ La cédula ya está registrada.");
                    return View(model);
                }

                // ==========================================================
                // 4. CREAR EL NUEVO USUARIO
                // ==========================================================
                var nuevoUsuario = new Usuario
                {
                    Id = _usuarios.Count + 1,
                    Nombre = model.Nombre,
                    Apellidos = model.Apellidos,
                    Cedula = model.Cedula,
                    Telefono = model.Telefono,
                    Correo = model.Correo,
                    NISE = model.NISE,
                    UserName = model.UserName,
                    Contraseña = model.Contraseña,
                    Rol = "Cliente",
                    FechaNacimiento = model.FechaNacimiento
                };

                _usuarios.Add(nuevoUsuario);

                TempData["Mensaje"] = "✅ ¡Registro exitoso! Ya puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            return View(model);
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
    }

    // ==========================================================
    // MODELOS
    // ==========================================================
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string NISE { get; set; }
        public string UserName { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }

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
        public string Correo { get; set; }
        public string NISE { get; set; }
        public string UserName { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}