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
            new Usuario { Nombre = "Katherine", Apellidos = "Villalobos", Cedula = "1-2345-6789", Telefono = "8888-7777", Correo = "k.villalobos@correo.cr", Username = "cliente", Password = "123456", Rol = "Cliente", NISEs = new List<string> { "NISE 4021", "NISE 7788" } }
        };

        public ActionResult Index()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // Vista Principal de Cuenta
        public ActionResult Cuenta()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            var user = _usuarios.Find(u => u.Username == Session["Username"]);
            return View(user);
        }

        // Acción para "Historial de compras"
        public ActionResult HistorialCompras()
        {
            return View();
        }

        // Acción para "Suscripciones"
        public ActionResult Suscripciones()
        {
            return View();
        }

        // Acción para "Servicios contratados"
        public ActionResult ServiciosContratados()
        {
            return View();
        }

        // Acción para "Editar mis datos"
        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            var user = _usuarios.Find(u => u.Username == Session["Username"]);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditarDatos(Usuario modelo)
        {
            var user = _usuarios.Find(u => u.Username == Session["Username"]);
            if (user != null)
            {
                user.Nombre = modelo.Nombre;
                user.Apellidos = modelo.Apellidos;
                user.Telefono = modelo.Telefono;
                user.Correo = modelo.Correo;
                Session["Usuario"] = modelo.Nombre + " " + modelo.Apellidos;
                Session["Correo"] = modelo.Correo;
            }
            return RedirectToAction("Cuenta", "Cuenta");
        }

        // Acción para "Calculadora energética"
        public ActionResult Calculadora()
        {
            return View();
        }

        // Acción para "Chat / WhatsApp"
        public ActionResult Chat()
        {
            return View();
        }

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
                Session["Usuario"] = user.Nombre + " " + user.Apellidos;
                Session["Username"] = user.Username;
                Session["Rol"] = user.Rol;
                Session["Correo"] = user.Correo;
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
        public ActionResult Registro(string nombre, string apellidos, string cedula, string telefono, string correo)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(cedula) && !string.IsNullOrEmpty(correo))
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Cedula = cedula,
                    Telefono = telefono,
                    Correo = correo,
                    Username = nombre.ToLower().Replace(" ", ""),
                    Password = "123456",
                    Rol = "Cliente",
                    NISEs = new List<string> { "NISE 4021", "NISE 7788" }
                };
                _usuarios.Add(nuevoUsuario);
                return RedirectToAction("Login", "Cuenta");
            }
            ViewBag.Error = "Debe completar todos los campos";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Cuenta");
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