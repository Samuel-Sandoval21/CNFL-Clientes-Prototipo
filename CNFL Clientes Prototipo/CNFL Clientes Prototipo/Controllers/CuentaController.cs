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
        private static List<Usuario> _usuarios = new List<Usuario>()
        {
            new Usuario { Nombre = "Admin", Apellidos = "Sistema", Cedula = "1-0000-0000", Telefono = "8888-8888", Correo = "admin@cnfl.go.cr", Username = "admin", Password = "12345$", Rol = "Admin", NISEs = new List<string> { "NISE 0000", "NISE 1111" } },
            new Usuario { Nombre = "Katherine", Apellidos = "Villalobos", Cedula = "1-2345-6789", Telefono = "8888-7777", Correo = "k.villalobos@correo.cr", Username = "cliente", Password = "123456", Rol = "Cliente", NISEs = new List<string> { "NISE 4021", "NISE 7788" } }
        };

        // GET: /Cuenta/Index (Home informativa - ACCESO LIBRE)
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Cuenta/Cuenta (Perfil de Cuenta - SOLO LOGUEADO)
        public ActionResult Cuenta()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");

            // Usa los datos de la sesión actualizada
            var user = new Usuario
            {
                Nombre = Session["Nombre"]?.ToString() ?? "Usuario",
                Apellidos = Session["Apellidos"]?.ToString() ?? "",
                Cedula = Session["Cedula"]?.ToString() ?? "",
                Telefono = Session["Telefono"]?.ToString() ?? "",
                Correo = Session["Correo"]?.ToString() ?? "",
                Username = Session["Username"]?.ToString() ?? "",
                Rol = Session["Rol"]?.ToString() ?? "Cliente",
                NISEs = Session["NISEs"] != null ? new List<string>(Session["NISEs"].ToString().Split(',')) : new List<string>()
            };

            return View(user);
        }

        public ActionResult HistorialCompras() => View();
        public ActionResult Suscripciones() => View();
        public ActionResult ServiciosContratados() => View();

        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");

            var user = new Usuario
            {
                Nombre = Session["Nombre"]?.ToString() ?? "",
                Apellidos = Session["Apellidos"]?.ToString() ?? "",
                Cedula = Session["Cedula"]?.ToString() ?? "",
                Telefono = Session["Telefono"]?.ToString() ?? "",
                Correo = Session["Correo"]?.ToString() ?? "",
                Username = Session["Username"]?.ToString() ?? ""
            };
            return View(user);
        }

        [HttpPost]
        public ActionResult EditarDatos(Usuario modelo)
        {
            var username = Session["Username"]?.ToString();
            var user = _usuarios.Find(u => u.Username == username);

            if (user != null)
            {
                user.Nombre = modelo.Nombre;
                user.Apellidos = modelo.Apellidos;
                user.Telefono = modelo.Telefono;
                user.Correo = modelo.Correo;

                // Actualizar TODA la sesión
                Session["Nombre"] = modelo.Nombre;
                Session["Apellidos"] = modelo.Apellidos;
                Session["Telefono"] = modelo.Telefono;
                Session["Correo"] = modelo.Correo;
                Session["Usuario"] = modelo.Nombre + " " + modelo.Apellidos;
            }
            return RedirectToAction("Cuenta", "Cuenta");
        }

        public ActionResult Calculadora() => View();
        public ActionResult Chat() => View();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            var user = _usuarios.Find(u => u.Username == usuario && u.Password == contrasena);
            if (user != null)
            {
                // Guardar TODOS los datos en sesión
                Session["Usuario"] = user.Nombre + " " + user.Apellidos;
                Session["Username"] = user.Username;
                Session["Rol"] = user.Rol;
                Session["Correo"] = user.Correo;
                Session["Nombre"] = user.Nombre;
                Session["Apellidos"] = user.Apellidos;
                Session["Cedula"] = user.Cedula;
                Session["Telefono"] = user.Telefono;
                Session["NISEs"] = string.Join(", ", user.NISEs);

                if (user.Rol == "Admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Cuenta");
            }
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro(string nombre, string apellidos, string cedula, string telefono, string correo, string usuario, string contrasena)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(cedula) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(contrasena))
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Cedula = cedula,
                    Telefono = telefono,
                    Correo = correo,
                    Username = usuario,
                    Password = contrasena,
                    Rol = "Cliente",
                    NISEs = new List<string> { "NISE 4021", "NISE 7788" }
                };
                _usuarios.Add(nuevoUsuario);

                // 🚀 AUTO-LOGIN: Inicia sesión automáticamente con los datos guardados
                Session["Usuario"] = nombre + " " + apellidos;
                Session["Username"] = usuario;
                Session["Rol"] = "Cliente";
                Session["Correo"] = correo;
                Session["Nombre"] = nombre;
                Session["Apellidos"] = apellidos;
                Session["Cedula"] = cedula;
                Session["Telefono"] = telefono;
                Session["NISEs"] = "NISE 4021, NISE 7788";

                // Redirige directo al Dashboard (YA LOGUEADO)
                return RedirectToAction("Index", "Cuenta");
            }
            ViewBag.Error = "Debe completar todos los campos";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Cuenta"); // Vuelve a la página de inicio informativa
        }

        public static List<Usuario> ObtenerUsuarios()
        {
            return _usuarios;
        }
    }

    public class Usuario
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public List<string> NISEs { get; set; } = new List<string>();
    }
}