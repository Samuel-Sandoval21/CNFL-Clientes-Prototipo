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
        // Lista de usuarios registrados (simulada)
        private static List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "Admin", Apellidos = "Sistema", Cedula = "1-0000-0000", Telefono = "0000-0000", Correo = "admin@cnfl.go.cr", NISE = "000000000", UserName = "admin", Contraseña = "12345$", Rol = "Admin" },
            new Usuario { Id = 2, Nombre = "Katherine", Apellidos = "Villalobos", Cedula = "1-2345-6789", Telefono = "8888-7777", Correo = "k.villalobos@correo.cr", NISE = "402112345", UserName = "cliente", Contraseña = "123456", Rol = "Cliente" }
        };

        public static List<Usuario> ObtenerUsuarios()
        {
            return _usuarios;
        }

        // ==========================================================
        // GET: /Cuenta (Redirige según sesión)
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

        // ==========================================================
        // GET: /Cuenta/Login
        // ==========================================================
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

        // ==========================================================
        // POST: /Cuenta/Login
        // ==========================================================
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

        // ==========================================================
        // GET: /Cuenta/Registro
        // ==========================================================
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

        // ==========================================================
        // POST: /Cuenta/Registro
        // ==========================================================
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

        // ==========================================================
        // GET: /Cuenta/Logout
        // ==========================================================
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        // ==========================================================
        // GET: /Cuenta/Cuenta (REDIRIGE A PERFIL DEL CLIENTE)
        // ==========================================================
        public ActionResult Cuenta()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Perfil", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/MisDatos (REDIRIGE A EDITAR DATOS)
        // ==========================================================
        public ActionResult MisDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("EditarDatos", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/Suscripciones (REDIRIGE A SUSCRIPCIONES)
        // ==========================================================
        public ActionResult Suscripciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("Suscripciones", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/ServiciosContratados (REDIRIGE)
        // ==========================================================
        public ActionResult ServiciosContratados()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("ServiciosContratados", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/Calculadora (REDIRIGE)
        // ==========================================================
        public ActionResult Calculadora()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("Calculadora", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/Chat (REDIRIGE)
        // ==========================================================
        public ActionResult Chat()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("Chat", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/HistorialCompras (REDIRIGE)
        // ==========================================================
        public ActionResult HistorialCompras()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("HistorialCompras", "Clientes");
        }

        // ==========================================================
        // GET: /Cuenta/EditarDatos (REDIRIGE)
        // ==========================================================
        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login");
            return RedirectToAction("EditarDatos", "Clientes");
        }
    }

    // ===== MODELOS =====
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