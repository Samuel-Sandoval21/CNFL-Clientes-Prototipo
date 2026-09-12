using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class TramitesController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ============================================================
        // CATÁLOGO COMPLETO DE TRÁMITES (según diagrama CNFL)
        // ============================================================
        private static readonly List<TramiteCatalogo> _catalogo = new List<TramiteCatalogo>
        {
            // ══════════════════════════════════════════════════════════
            // CAMBIOS Y MODIFICACIONES
            // ══════════════════════════════════════════════════════════
            new TramiteCatalogo {
                Id = "cambio-conexion-voltaje",
                Nombre = "Cambio de conexión de voltaje",
                Categoria = "Cambios y Modificaciones",
                Icono = "⚡",
                Descripcion = "Modificá el voltaje de tu conexión eléctrica."
            },
            new TramiteCatalogo {
                Id = "cambio-nombre-abonado",
                Nombre = "Cambio de Nombre de Abonado",
                Categoria = "Cambios y Modificaciones",
                Icono = "👤",
                Descripcion = "Actualizá el titular del servicio eléctrico."
            },
            new TramiteCatalogo {
                Id = "cambio-servicio-provisional",
                Nombre = "Cambio de Servicio Provisional a Definitivo",
                Categoria = "Cambios y Modificaciones",
                Icono = "🔄",
                Descripcion = "Convertí un servicio provisional en definitivo."
            },
            new TramiteCatalogo {
                Id = "desconexion-reconexion",
                Nombre = "Desconexión y Reconexión",
                Categoria = "Cambios y Modificaciones",
                Icono = "🔌",
                Descripcion = "Solicitá desconexión o reconexión del servicio."
            },

            // ══════════════════════════════════════════════════════════
            // DISEÑO E INGENIERÍA
            // ══════════════════════════════════════════════════════════
            new TramiteCatalogo {
                Id = "diseno-red-electrica",
                Nombre = "Solicitud de Diseño de Red Eléctrica y DER",
                Categoria = "Diseño e Ingeniería",
                Icono = "📐",
                Descripcion = "Solicitá el diseño de red eléctrica y recursos energéticos distribuidos."
            },
            new TramiteCatalogo {
                Id = "transformador-temporal",
                Nombre = "Conexión de Transformador Temporal",
                Categoria = "Diseño e Ingeniería",
                Icono = "🔋",
                Descripcion = "Solicitá un transformador temporal para obra."
            },
            new TramiteCatalogo {
                Id = "traslado-medidor",
                Nombre = "Traslado de Medidor",
                Categoria = "Diseño e Ingeniería",
                Icono = "📦",
                Descripcion = "Solicitá el traslado del medidor a otra ubicación."
            },

            // ══════════════════════════════════════════════════════════
            // TARIFAS Y RECLAMOS
            // ══════════════════════════════════════════════════════════
            new TramiteCatalogo {
                Id = "tarifa-residencial-horaria",
                Nombre = "Ingreso a Tarifa Residencial Horaria",
                Categoria = "Tarifas y Reclamos",
                Icono = "⏰",
                Descripcion = "Solicitá ingreso a la tarifa residencial horaria."
            },
            new TramiteCatalogo {
                Id = "reclamo-responsabilidad-civil",
                Nombre = "Reclamo por Daños con Responsabilidad Civil",
                Categoria = "Tarifas y Reclamos",
                Icono = "⚠️",
                Descripcion = "Reclamo por daños en equipos y artefactos eléctricos."
            },

            // ══════════════════════════════════════════════════════════
            // SERVICIOS NUEVOS
            // ══════════════════════════════════════════════════════════
            new TramiteCatalogo {
                Id = "servicio-nuevo-mono-tri",
                Nombre = "Servicio Nuevo Monofásico o Trifásico",
                Categoria = "Servicios Nuevos",
                Icono = "⚡",
                Descripcion = "Solicitá un servicio eléctrico nuevo (monofásico o trifásico)."
            },
            new TramiteCatalogo {
                Id = "suministro-inmuebles",
                Nombre = "Suministro Eléctrico para Inmuebles",
                Categoria = "Servicios Nuevos",
                Icono = "🏠",
                Descripcion = "Solicitá suministro eléctrico para un inmueble."
            },
            new TramiteCatalogo {
                Id = "suministro-especial",
                Nombre = "Suministro Eléctrico Especial",
                Categoria = "Servicios Nuevos",
                Icono = "🔌",
                Descripcion = "Solicitá suministro eléctrico especial."
            },
            new TramiteCatalogo {
                Id = "alumbrado-publico",
                Nombre = "Solicitud de Alumbrado Público",
                Categoria = "Servicios Nuevos",
                Icono = "💡",
                Descripcion = "Solicitá instalación de alumbrado público."
            },

            // ══════════════════════════════════════════════════════════
            // TRASPASOS Y SUMINISTROS ESPECIALES
            // ══════════════════════════════════════════════════════════
            new TramiteCatalogo {
                Id = "traspaso-servicio",
                Nombre = "Traspaso de Servicio Eléctrico",
                Categoria = "Traspasos y Suministros",
                Icono = "🔄",
                Descripcion = "Traspasá el servicio eléctrico a otro titular."
            },
            new TramiteCatalogo {
                Id = "carga-fija",
                Nombre = "Servicio Especial de Carga Fija",
                Categoria = "Traspasos y Suministros",
                Icono = "🔋",
                Descripcion = "Solicitá servicio especial para carga fija."
            },
            new TramiteCatalogo {
                Id = "eventos-temporal",
                Nombre = "Servicio Especial Temporal para Eventos",
                Categoria = "Traspasos y Suministros",
                Icono = "🎪",
                Descripcion = "Solicitá servicio temporal para eventos especiales."
            }
        };

        // GET: Tramites
        public ActionResult Index()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Catalogo = _catalogo;

            var historial = _db.Tramites
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaSolicitud)
                .Take(5)
                .Select(t => new TramiteResumen
                {
                    TramiteId = t.TramiteId,
                    Tipo = t.Tipo,
                    Estado = t.Estado,
                    FechaSolicitud = t.FechaSolicitud,
                    NumeroReferencia = t.NumeroReferencia
                })
                .ToList();

            ViewBag.Historial = historial;

            ViewBag.TotalTramites = _db.Tramites.Count(t => t.UsuarioId == usuarioId);
            ViewBag.TramitesPendientes = _db.Tramites
                .Count(t => t.UsuarioId == usuarioId && t.Estado != "Resuelto");

            return View();
        }

        // GET: Tramites/Detalle?id=cambio-conexion-voltaje
        public ActionResult Detalle(string id)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var tramite = _catalogo.FirstOrDefault(t => t.Id == id);
            if (tramite == null)
                return RedirectToAction("Index");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            ViewBag.TramiteId = tramite.Id;

            return View(tramite);
        }

        // POST: Tramites/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Solicitar(string tramiteId, string niseId, string descripcion, string datosFormulario)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var tramite = _catalogo.FirstOrDefault(t => t.Id == tramiteId);
            if (tramite == null)
                return Json(new { success = false, message = "Trámite no válido." });

            var anio = DateTime.Now.Year;
            var count = _db.Tramites.Count() + 1;
            var numeroReferencia = $"TRM-{anio}-{count:D4}";

            string numeroNise = "N/A";
            if (!string.IsNullOrWhiteSpace(niseId))
            {
                int niseIdInt;
                if (int.TryParse(niseId, out niseIdInt))
                {
                    var nise = _db.NISEs.FirstOrDefault(n => n.NiseId == niseIdInt);
                    if (nise != null) numeroNise = nise.NumeroNise;
                }
            }

            var datosCompletos = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(datosFormulario))
            {
                try
                {
                    var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(datosFormulario);
                    if (parsed != null) datosCompletos = parsed;
                }
                catch { }
            }
            datosCompletos["NISE"] = numeroNise;

            var nuevoTramite = new Tramite
            {
                UsuarioId = usuarioId.Value,
                Tipo = tramite.Nombre,
                Categoria = tramite.Categoria,
                Estado = "Solicitado",
                FechaSolicitud = DateTime.Now,
                Descripcion = string.IsNullOrWhiteSpace(descripcion)
                    ? $"Trámite sobre NISE {numeroNise}."
                    : $"NISE {numeroNise}. {descripcion}",
                NumeroReferencia = numeroReferencia,
                DatosFormulario = JsonConvert.SerializeObject(datosCompletos)
            };

            _db.Tramites.Add(nuevoTramite);
            _db.SaveChanges();

            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId.Value,
                Titulo = "Trámite solicitado",
                Mensaje = $"Su trámite {numeroReferencia} ({tramite.Nombre}) fue ingresado correctamente.",
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Tramite"
            };
            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Trámite solicitado exitosamente.",
                referencia = numeroReferencia
            });
        }

        // GET: Tramites/MisTramites
        public ActionResult MisTramites()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var tramites = _db.Tramites
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaSolicitud)
                .ToList();

            return View(tramites);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ============================================================
    // DTO DEL CATÁLOGO
    // ============================================================
    public class TramiteCatalogo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
    }

    // ============================================================
    // DTO DE RESUMEN (para el historial)
    // ============================================================
    public class TramiteResumen
    {
        public int TramiteId { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string NumeroReferencia { get; set; }
    }
}