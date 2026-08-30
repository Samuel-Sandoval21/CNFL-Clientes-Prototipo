using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        // GET: /Clientes/Inicio
        public ActionResult Inicio()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return View();
        }

        // GET: /Clientes/Tramites
        public ActionResult Tramites()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Pagos
        public ActionResult Pagos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Tienda
        public ActionResult Tienda()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Reportes
        public ActionResult Reportes()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Perfil
        public ActionResult Perfil()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Chat
        public ActionResult Chat()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/DetalleFactura
        public ActionResult DetalleFactura(int id = 1)
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            ViewBag.FacturaId = id;
            return View();
        }

        // GET: /Clientes/Sinpe
        public ActionResult Sinpe(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/Iban
        public ActionResult Iban(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/Tarjeta
        public ActionResult Tarjeta(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/EditarDatos
        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Suscripciones
        public ActionResult Suscripciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ServiciosContratados
        public ActionResult ServiciosContratados()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/HistorialCompras
        public ActionResult HistorialCompras()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Calculadora
        public ActionResult Calculadora()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Notificaciones
        public ActionResult Notificaciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ReportarAlumbrado
        public ActionResult ReportarAlumbrado()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ReportarPropia
        public ActionResult ReportarPropia()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ReportarAjena
        public ActionResult ReportarAjena()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }
    }
}