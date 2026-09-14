using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ============================================================
        // PERFIL Y DATOS DEL USUARIO
        // ============================================================

        // GET: Clientes/MiPerfil
        public ActionResult MiPerfil()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .Include("Suscripciones")
                .Include("ActividadEconomica")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            var nises = _db.NISEs
                .Where(n => n.UsuarioId == usuarioId)
                .ToList();

            var niseIds = nises.Select(n => n.NiseId).ToList();
            var facturas = _db.Facturas
                .Where(f => niseIds.Contains(f.NiseId))
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            var suscripciones = _db.Suscripciones
                .Where(s => s.UsuarioId == usuarioId && s.Activa)
                .ToList();

            var pagos = _db.Pagos
                .Include("Factura")
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(5)
                .ToList();

            var averiasActivas = _db.Averias
                .Include("NISE")
                .Where(a => a.UsuarioId == usuarioId
                         && a.Estado != "Problema resuelto"
                         && a.Estado != "Resuelto")
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            ViewBag.TotalNISEs = nises.Count;
            ViewBag.TotalFacturasPendientes = facturas.Count(f => !f.Pagada);
            ViewBag.MontoPendiente = facturas.Where(f => !f.Pagada).Sum(f => (decimal?)f.Monto) ?? 0m;
            ViewBag.TotalSuscripciones = suscripciones.Count;
            ViewBag.TotalAveriasActivas = averiasActivas.Count;
            ViewBag.NotificacionesNoLeidas = _db.Notificaciones
                .Count(n => n.UsuarioId == usuarioId && !n.Leida);

            ViewBag.NISEs = nises;
            ViewBag.UltimasFacturas = facturas.Take(3).ToList();
            ViewBag.Suscripciones = suscripciones;
            ViewBag.UltimosPagos = pagos;
            ViewBag.AveriasActivas = averiasActivas;

            return View(usuario);
        }

        // GET: Clientes/EditarPerfil
        public ActionResult EditarPerfil()
        {
            return RedirectToAction("EditarDatos");
        }

        // GET: Clientes/EditarDatos
        public ActionResult EditarDatos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .Include("Suscripciones")
                .Include("ActividadEconomica")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // POST: Clientes/EditarDatos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDatos(Usuario model, string NombreCompleto)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null)
                return HttpNotFound();

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
            usuario.ActividadEconomicaId = model.ActividadEconomicaId;

            _db.SaveChanges();

            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;

            TempData["Mensaje"] = "Datos actualizados correctamente.";
            return RedirectToAction("MiPerfil");
        }

        // ============================================================
        // FACTURAS Y PAGOS
        // ============================================================

        // GET: Clientes/MisFacturas
        public ActionResult MisFacturas()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null)
                return HttpNotFound();

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

        // GET: Clientes/Pagos
        public ActionResult Pagos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

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

        // POST: Clientes/PagarFactura
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
                    Mensaje = $"Se ha realizado el pago de la factura {factura.NumeroFactura} por ₡{factura.Monto:N0}.",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Factura"
                };
                _db.Notificaciones.Add(notificacion);
                _db.SaveChanges();

                return Json(new { success = true, message = "Pago realizado exitosamente." });
            }
            return Json(new { success = false, message = "La factura no existe o ya fue pagada." });
        }

        // ============================================================
        // NOTIFICACIONES Y SUSCRIPCIONES
        // ============================================================

        // GET: Clientes/MisNotificaciones
        public ActionResult MisNotificaciones()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

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

        // GET: Clientes/MisSuscripciones
        public ActionResult MisSuscripciones()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var suscripciones = _db.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .ToList();

            return View(suscripciones);
        }

        // POST: Clientes/SuscribirServicio
        [HttpPost]
        public JsonResult SuscribirServicio(string servicio, decimal? montoMensual)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var suscripcion = new Suscripcion
            {
                UsuarioId = usuarioId.Value,
                Servicio = servicio,
                FechaInicio = DateTime.Now,
                Activa = true,
                MontoMensual = montoMensual
            };

            _db.Suscripciones.Add(suscripcion);
            _db.SaveChanges();

            return Json(new { success = true, message = "Suscripción activada correctamente." });
        }

        // ============================================================
        // DASHBOARD
        // ============================================================

        // GET: Clientes/Dashboard
        public ActionResult Dashboard()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            // ─── NISEs y facturas ───
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

            // ─── Averías activas ───
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

            // ─── Trámites activos ───
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

            // ─── Notificaciones ───
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

            // ═══════════════════════════════════════════════════════════
            // GRÁFICOS
            // ═══════════════════════════════════════════════════════════
            var distribucionNise = new List<DistribucionNiseDto>();
            var coloresNise = new[] { "#1E23E6", "#FF692D", "#64B95A", "#64B9CD", "#F5A623" };
            int colorIdx = 0;
            foreach (var nise in nises)
            {
                var montoNise = facturas.Where(f => f.NiseId == nise.NiseId).Sum(f => (decimal?)f.Monto) ?? 0m;
                distribucionNise.Add(new DistribucionNiseDto
                {
                    label = nise.NumeroNise,
                    value = montoNise,
                    color = coloresNise[colorIdx % coloresNise.Length]
                });
                colorIdx++;
            }

            var meses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var consumoMensual = facturas
                .OrderByDescending(f => f.FechaEmision)
                .Take(6)
                .OrderBy(f => f.FechaEmision)
                .Select(f => new ConsumoMensualDto
                {
                    mes = meses[f.FechaEmision.Month - 1],
                    monto = f.Monto,
                    kwh = Math.Round((double)f.Monto / 78.0, 0)
                })
                .ToList();

            // ─── Actividad semanal REAL desde BD ───
            var hace7dias = DateTime.Now.AddDays(-7);
            var actividadReal = _db.ActividadUsuario
                .Where(a => a.UsuarioId == usuarioId && a.Fecha >= hace7dias)
                .ToList();

            var diasSemana = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
            var actividadSemana = new List<ActividadSemanalDto>();
            for (int i = 6; i >= 0; i--)
            {
                var dia = DateTime.Now.AddDays(-i).Date;
                var delDia = actividadReal.Where(a => a.Fecha.Date == dia).ToList();
                actividadSemana.Add(new ActividadSemanalDto
                {
                    dia = diasSemana[(int)dia.DayOfWeek],
                    facturas = delDia.Count(a => a.Seccion == "Facturas"),
                    reportes = delDia.Count(a => a.Seccion == "Reportes"),
                    tramites = delDia.Count(a => a.Seccion == "Trámites"),
                    perfil = delDia.Count(a => a.Seccion == "Perfil" || a.Seccion == "Dashboard")
                });
            }

            // ─── Secciones más visitadas REALES ───
            var seccionesTop = actividadReal
                .GroupBy(a => a.Seccion)
                .Select(g => new SeccionTopDto { nombre = g.Key, visitas = g.Count() })
                .OrderByDescending(s => s.visitas)
                .Take(6)
                .ToList();

            if (seccionesTop.Count == 0)
                seccionesTop.Add(new SeccionTopDto { nombre = "Sin actividad aún", visitas = 0 });

            // ═══════════════════════════════════════════════════════════
            // MÉTRICAS DE USO
            // ═══════════════════════════════════════════════════════════
            var tiempoTotalSegundos = actividadReal
                .Where(a => a.DuracionSegundos.HasValue)
                .Sum(a => a.DuracionSegundos.Value);

            var seccionTop = actividadReal
                .GroupBy(a => a.Seccion)
                .Select(g => new { Seccion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            var descargas = _db.DescargasUsuario
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.Fecha)
                .ToList();

            var metricas = new MetricasUsoDto
            {
                TotalSesiones = actividadReal.Count,
                TiempoTotalMinutos = tiempoTotalSegundos / 60,
                PromedioMinutosPorDia = tiempoTotalSegundos > 0 ? (tiempoTotalSegundos / 60) / 7 : 0,
                SeccionMasVisitada = seccionTop != null ? seccionTop.Seccion : "Sin datos",
                TotalDescargas = descargas.Count,
                DescargasPDF = descargas.Count(d => d.Tipo == "PDF"),
                DescargasExcel = descargas.Count(d => d.Tipo == "Excel")
            };

            // ═══════════════════════════════════════════════════════════
            // ViewBags
            // ═══════════════════════════════════════════════════════════
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

            ViewBag.DistribucionJson = Newtonsoft.Json.JsonConvert.SerializeObject(distribucionNise);
            ViewBag.ConsumoJson = Newtonsoft.Json.JsonConvert.SerializeObject(consumoMensual);
            ViewBag.ActividadJson = Newtonsoft.Json.JsonConvert.SerializeObject(actividadSemana);
            ViewBag.SeccionesJson = Newtonsoft.Json.JsonConvert.SerializeObject(seccionesTop);

            ViewBag.Metricas = metricas;
            ViewBag.Descargas = descargas;

            return View();
        }

        // ============================================================
        // REPORTES Y TIENDA
        // ============================================================

        // GET: Clientes/Reportes
        public ActionResult Reportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/Tienda
        public ActionResult Tienda()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var banners = new List<BannerDto>
            {
                new BannerDto { Titulo = "Servicios Hogar 360", Subtitulo = "Eficiencia energética, domótica, acometidas y más", Icono = "🏡", CategoriaId = "hogar360", ColorInicio = "#0033A0", ColorFin = "#2a4fd6" },
                new BannerDto { Titulo = "Internet Fijo 5G", Subtitulo = "Navegación fluida, streaming y gaming sin límites", Icono = "📶", CategoriaId = "internet", ColorInicio = "#1E23E6", ColorFin = "#64B9CD" },
                new BannerDto { Titulo = "Seguro de Hogar", Subtitulo = "Protegé tu vivienda contra incendio y otros riesgos", Icono = "🛡️", CategoriaId = "seguro-hogar", ColorInicio = "#0033A0", ColorFin = "#64B95A" },
                new BannerDto { Titulo = "Tienda CNFL", Subtitulo = "Electrodomésticos, tecnología y línea blanca a crédito", Icono = "🛒", CategoriaId = "tienda", ColorInicio = "#FF692D", ColorFin = "#F5A623" },
                new BannerDto { Titulo = "CNFL Te Asiste", Subtitulo = "Asistencias para el hogar cuando más las necesitás", Icono = "🤝", CategoriaId = "asiste", ColorInicio = "#64B95A", ColorFin = "#0033A0" }
            };

            ViewBag.Banners = banners;
            return View();
        }

        // ============================================================
        // GET: Clientes/DetalleProducto?id=tienda
        // ⭐ AHORA CARGA LOS PRODUCTOS DESDE TiendaController
        // ============================================================
        public ActionResult DetalleProducto(string id)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Tienda");

            ViewBag.CategoriaId = id;

            var titulos = new Dictionary<string, string>
            {
                { "tienda", "Tienda CNFL" },
                { "supresores", "Supresores y Bases" },
                { "cargadores", "Cargadores Semirápidos" },
                { "bienes", "Bienes Inmuebles CNFL" },
                { "asiste", "CNFL Te Asiste" },
                { "internet", "Internet Fijo 5G" },
                { "seguro-hogar", "Seguro de Hogar" },
                { "sri", "Ingeniería Eléctrica (SIE)" },
                { "ambientales", "Servicios Ambientales" },
                { "calibracion", "Calibración" },
                { "anonos", "Taller Anonos" },
                { "reparacion", "Reparación y Mantenimiento" },
                { "hogar360", "Servicios Hogar 360" },
                { "videovigilancia", "Videovigilancia" },
                { "movilidad", "Movilidad Eléctrica" },
                { "alquileres", "Alquileres" },
                { "soluciones-energeticas", "Soluciones Energéticas Integrales" },
                { "marketplace", "Marketplace" }
            };

            var subtitulos = new Dictionary<string, string>
            {
                { "tienda", "Comprá productos del hogar" },
                { "supresores", "Protección en cada partido" },
                { "cargadores", "Energía lista para cada jugada" },
                { "bienes", "Locales y propiedades" },
                { "asiste", "Asistencias y seguros" },
                { "internet", "Viví la velocidad" },
                { "seguro-hogar", "Contra incendio y rayo" },
                { "sri", "Soluciones profesionales" },
                { "ambientales", "Sostenibilidad y control" },
                { "calibracion", "Equipos certificados" },
                { "anonos", "Reparación especializada" },
                { "reparacion", "Servicio técnico" },
                { "hogar360", "Domótica, acometidas y eficiencia energética" },
                { "videovigilancia", "Seguridad para tu hogar o negocio" },
                { "movilidad", "Vehículos y estaciones de carga eléctrica" },
                { "alquileres", "Espacios en alquiler de la CNFL" },
                { "soluciones-energeticas", "Diagnóstico y diseño energético a medida" },
                { "marketplace", "Comprá y vendé entre clientes CNFL" }
            };

            ViewBag.Titulo = titulos.ContainsKey(id) ? titulos[id] : "Producto";
            ViewBag.Subtitulo = subtitulos.ContainsKey(id) ? subtitulos[id] : "";

            // ⭐ CLAVE: Cargar el catálogo de productos de esa categoría
            ViewBag.Productos = TiendaController.GetCatalogo(id);

            return View();
        }

        // ============================================================
        // REPORTES DE AVERÍAS — FLUJO COMPLETO
        // ============================================================

        // GET: Clientes/TiposReportes
        public ActionResult TiposReportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/TiposReportesDetalle
        public ActionResult TiposReportesDetalle(string tipo)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Tipo = string.IsNullOrWhiteSpace(tipo) ? "Avería" : tipo;
            return View();
        }

        // GET: Clientes/ReportarAveria
        public ActionResult ReportarAveria(string tipo)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            ViewBag.Tipo = tipo ?? "";
            return View();
        }

        // POST: Clientes/ReportarAveria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportarAveria(Averia averia)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (ModelState.IsValid)
            {
                averia.UsuarioId = usuarioId.Value;
                averia.FechaReporte = DateTime.Now;
                averia.Estado = "Ingresado";
                _db.Averias.Add(averia);
                _db.SaveChanges();

                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId.Value,
                    Titulo = "Avería reportada",
                    Mensaje = $"Su reporte de avería #{averia.AveriaId} ha sido ingresado correctamente.",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Averia"
                };
                _db.Notificaciones.Add(notificacion);
                _db.SaveChanges();

                ViewBag.Mensaje = "Avería reportada exitosamente. Número de seguimiento: #" + averia.AveriaId;
            }

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            return View(averia);
        }

        // GET: Clientes/EstadoAveria/5
        public ActionResult EstadoAveria(int id)
        {
            var averia = _db.Averias
                .Include("NISE")
                .FirstOrDefault(a => a.AveriaId == id);

            if (averia == null)
                return HttpNotFound();

            return View(averia);
        }

        // GET: Clientes/ConsultarAveria
        public ActionResult ConsultarAveria()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/GenerarComprobante
        public ActionResult GenerarComprobante()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/MapaAverias
        public ActionResult MapaAverias()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/MapaAveriasData
        [HttpGet]
        public JsonResult MapaAveriasData()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "No autenticado." }, JsonRequestBehavior.AllowGet);

            var averias = _db.Averias
                .Include("NISE")
                .Where(a => a.Latitud != null && a.Longitud != null)
                .OrderByDescending(a => a.FechaReporte)
                .Take(50)
                .ToList();

            var data = averias.Select(a => new ReporteMapa
            {
                AveriaId = a.AveriaId,
                Titulo = "Avería #" + a.AveriaId,
                Tipo = a.Tipo ?? "Sin tipo",
                Estado = a.Estado ?? "Ingresado",
                Direccion = a.Direccion ?? "Sin dirección",
                NiseNumero = a.NISE != null ? a.NISE.NumeroNise : "N/A",
                Latitud = a.Latitud ?? 0,
                Longitud = a.Longitud ?? 0,
                FechaReporte = a.FechaReporte
            }).ToList();

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: Clientes/HistorialReportes
        public ActionResult HistorialReportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var averias = _db.Averias
                .Include("NISE")
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            return View(averias);
        }

        // ============================================================
        // ALUMBRADO PÚBLICO
        // ============================================================

        // GET: Clientes/ReportarAlumbrado
        public ActionResult ReportarAlumbrado()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // POST: Clientes/ReportarAlumbrado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportarAlumbrado(string NISE, string TipoProblema, string Direccion, string Descripcion)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (string.IsNullOrWhiteSpace(Direccion) || string.IsNullOrWhiteSpace(TipoProblema))
            {
                ViewBag.Error = "Debe completar la dirección y el tipo de problema.";
                return View();
            }

            var averia = new Averia
            {
                UsuarioId = usuarioId.Value,
                Tipo = "Iluminación pública",
                Descripcion = $"Tipo: {TipoProblema}. Dirección: {Direccion}. Detalle: {Descripcion}",
                FechaReporte = DateTime.Now,
                Estado = "Ingresado"
            };

            _db.Averias.Add(averia);
            _db.SaveChanges();

            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId.Value,
                Titulo = "Reporte de alumbrado enviado",
                Mensaje = $"Su reporte de alumbrado #{averia.AveriaId} fue registrado. Gracias por su colaboración.",
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Averia"
            };
            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();

            ViewBag.Mensaje = "Reporte de alumbrado enviado correctamente. Número de seguimiento: #" + averia.AveriaId;
            return View();
        }

        // ============================================================
        // CALCULADORA DE CONSUMO
        // ============================================================

        // GET: Clientes/Calculadora
        public ActionResult Calculadora()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // ============================================================
        // CHATBOT
        // ============================================================

        // GET: Clientes/Chatbot
        public ActionResult Chatbot()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // ============================================================
        // HISTORIAL DE PAGOS
        // ============================================================

        // GET: Clientes/HistorialPagos
        public ActionResult HistorialPagos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var pagos = _db.Pagos
                .Include("Factura")
                .Include("Factura.NISE")
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();

            return View(pagos);
        }

        // ============================================================
        // DISPOSE
        // ============================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }
}