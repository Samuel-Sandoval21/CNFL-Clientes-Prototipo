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
        private static List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "Admin", Apellidos = "Sistema", Cedula = "1-0000-0000", Telefono = "0000-0000", Correo = "admin@cnfl.go.cr", NISE = "000000000", UserName = "admin", Contraseña = "12345$", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Katherine", Apellidos = "Villalobos", Cedula = "1-2345-6789", Telefono = "8888-7777", Correo = "k.villalobos@correo.cr", NISE = "402112345", UserName = "cliente", Contraseña = "123456", Rol = "Cliente" }
        };

        // DATOS PARA VALIDACIÓN DE CÉDULA (TSE SIMULADO)
        private static Dictionary<string, (string Nombre, string Apellidos, List<string> NISEs)> _datosTSE = new Dictionary<string, (string, string, List<string>)>
        {
            { "1-2345-6789", ("Katherine", "Villalobos", new List<string> { "402112345", "402198765" }) },
            { "1-1234-5678", ("Juan", "Pérez", new List<string> { "123456789" }) },
            { "1-8765-4321", ("María", "Gómez", new List<string> { "987654321" }) },
            { "1-5555-6666", ("Carlos", "Rodríguez", new List<string> { "456789123" }) },
            { "1-1111-2222", ("Ana", "Mora", new List<string> { "111222333" }) }
        };

        public static List<Usuario> ObtenerUsuarios()
        {
            return _usuarios;
        }

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
                    nises = datos.NISEs
                });
            }

            return Json(new { success = false, message = "Cédula no encontrada en el sistema TSE" });
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
                var usuario = _usuarios.FirstOrDefault(u => u.UserName == model.UserName && u.Contraseña == model.Contraseña);

                if (usuario != null)
                {
                    Session["Id"] = usuario.Id;
                    Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["Correo"] = usuario.Correo;
                    Session["Rol"] = usuario.Rol;
                    Session["NISE"] = usuario.NISE;

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
                    Rol = "Cliente"
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
    }
}