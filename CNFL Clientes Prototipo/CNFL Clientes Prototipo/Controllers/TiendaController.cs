using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class TiendaController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ============================================================
        // CATÁLOGO COMPLETO DE PRODUCTOS
        // ============================================================
        public static List<ProductoTiendaDto> GetCatalogo(string categoriaId)
        {
            var todos = new List<ProductoTiendaDto>
            {
                // ═══════ SUPRESORES ═══════
                new ProductoTiendaDto { Id="sup-120", Categoria="supresores", Nombre="Supresor de picos 120V", Descripcion="Protege tus electrodomésticos de variaciones de voltaje.", Precio=18500, Icono="🔌" },
                new ProductoTiendaDto { Id="sup-240", Categoria="supresores", Nombre="Supresor de picos 240V", Descripcion="Ideal para aires acondicionados y equipos de alto consumo.", Precio=24900, Icono="⚡" },
                new ProductoTiendaDto { Id="base-mult", Categoria="supresores", Nombre="Base múltiple con supresor", Descripcion="6 tomacorrientes + USB, protección integrada.", Precio=15900, Icono="🔋" },
                new ProductoTiendaDto { Id="sup-tri", Categoria="supresores", Nombre="Supresor industrial trifásico", Descripcion="Para tableros y equipos industriales.", Precio=89500, Icono="🛡️" },

                // ═══════ CARGADORES ═══════
                new ProductoTiendaDto { Id="carg-7kw", Categoria="cargadores", Nombre="Cargador semirápidos 7 kW", Descripcion="Instalación residencial para vehículos eléctricos.", Precio=425000, Icono="🔌" },
                new ProductoTiendaDto { Id="carg-11kw", Categoria="cargadores", Nombre="Cargador semirápidos 11 kW", Descripcion="Para uso comercial. Carga en 3-4 horas.", Precio=680000, Icono="⚡" },
                new ProductoTiendaDto { Id="carg-22kw", Categoria="cargadores", Nombre="Cargador semirápidos 22 kW", Descripcion="Instalación rápida para flotas o estaciones.", Precio=1250000, Icono="🚗" },

                // ═══════ SOLUCIONES ENERGÉTICAS ═══════
                new ProductoTiendaDto { Id="sol-audit", Categoria="soluciones-energeticas", Nombre="Auditoría energética completa", Descripcion="Diagnóstico profesional de consumo eléctrico.", Precio=150000, Icono="🔧" },
                new ProductoTiendaDto { Id="sol-panel", Categoria="soluciones-energeticas", Nombre="Panel solar residencial", Descripcion="Sistema fotovoltaico llave en mano.", Precio=2500000, Icono="☀️" },
                new ProductoTiendaDto { Id="sol-domotica", Categoria="soluciones-energeticas", Nombre="Kit domótica básico", Descripcion="Controlá luces y enchufes desde tu celular.", Precio=185000, Icono="🏡" },

                // ═══════ BIENES INMUEBLES ═══════
                new ProductoTiendaDto { Id="bien-local", Categoria="bienes", Nombre="Local comercial en San José", Descripcion="Espacio de 80 m² con servicio eléctrico trifásico.", Precio=0, Icono="🏢" },
                new ProductoTiendaDto { Id="bien-casa", Categoria="bienes", Nombre="Casa residencial en Escazú", Descripcion="3 habitaciones, patio, acometida monofásica.", Precio=0, Icono="🏠" },
                new ProductoTiendaDto { Id="bien-terreno", Categoria="bienes", Nombre="Terreno en zona industrial", Descripcion="1.500 m² con disponibilidad eléctrica.", Precio=0, Icono="🌳" },

                // ═══════ TIENDA CNFL ═══════
                new ProductoTiendaDto { Id="tie-led", Categoria="tienda", Nombre="Bombillos LED", Descripcion="Ahorro energético y mayor duración.", Precio=2500, Icono="💡" },
                new ProductoTiendaDto { Id="tie-ext", Categoria="tienda", Nombre="Extensiones y tomacorrientes", Descripcion="Todo para tu hogar con calidad certificada.", Precio=3900, Icono="🔌" },
                new ProductoTiendaDto { Id="tie-termo", Categoria="tienda", Nombre="Termostatos inteligentes", Descripcion="Controlá el consumo de tu A/C desde la app.", Precio=48000, Icono="🌡️" },
                new ProductoTiendaDto { Id="tie-ac", Categoria="tienda", Nombre="Aire acondicionado Inverter 12.000 BTU", Descripcion="Eficiencia energética A++.", Precio=289000, Icono="❄️" },
                new ProductoTiendaDto { Id="tie-fridge", Categoria="tienda", Nombre="Refrigeradora eficiente 400L", Descripcion="Consumo optimizado, diseño moderno.", Precio=485000, Icono="🧊" },

                // ═══════ SEGUROS ═══════
                new ProductoTiendaDto { Id="seg-basico", Categoria="seguro-hogar", Nombre="Seguro Básico Hogar", Descripcion="Cobertura contra incendio y rayo. Hasta ₡25 millones.", Precio=4900, Icono="🏠" },
                new ProductoTiendaDto { Id="seg-amplio", Categoria="seguro-hogar", Nombre="Seguro Amplio Hogar", Descripcion="Incendio, rayo, cortocircuito y robo.", Precio=9500, Icono="🛡️" },
                new ProductoTiendaDto { Id="seg-premium", Categoria="seguro-hogar", Nombre="Seguro Premium Hogar", Descripcion="Cobertura total + responsabilidad civil.", Precio=15900, Icono="💎" },

                // ═══════ CNFL TE ASISTE ═══════
                new ProductoTiendaDto { Id="asis-vial", Categoria="asiste", Nombre="Asistencia Vial", Descripcion="Grúa, cambio de llanta, paso de corriente.", Precio=3900, Icono="🚗" },
                new ProductoTiendaDto { Id="asis-med", Categoria="asiste", Nombre="Asistencia Médica", Descripcion="Consulta médica telefónica y descuentos.", Precio=5500, Icono="🏥" },
                new ProductoTiendaDto { Id="asis-hogar", Categoria="asiste", Nombre="Asistencia Hogar", Descripcion="Electricista, plomero y cerrajero a domicilio.", Precio=6900, Icono="🏠" },

                // ═══════ INGENIERÍA ELÉCTRICA ═══════
                new ProductoTiendaDto { Id="sri-diseno", Categoria="sri", Nombre="Diseño de instalaciones eléctricas", Descripcion="Planos, memorias de cálculo y especificaciones.", Precio=0, Icono="📐" },
                new ProductoTiendaDto { Id="sri-carga", Categoria="sri", Nombre="Estudios de carga y demanda", Descripcion="Análisis técnico para nuevos servicios.", Precio=0, Icono="⚡" },
                new ProductoTiendaDto { Id="sri-audit", Categoria="sri", Nombre="Auditorías eléctricas", Descripcion="Diagnóstico y optimización de consumos.", Precio=0, Icono="🔍" },

                // ═══════ MOVILIDAD ELÉCTRICA ═══════
                new ProductoTiendaDto { Id="mov-cargador", Categoria="movilidad", Nombre="Cargador residencial 7kW", Descripcion="Instalación certificada para autos eléctricos.", Precio=615000, Icono="🔋" },
                new ProductoTiendaDto { Id="mov-estacion", Categoria="movilidad", Nombre="Estación de carga pública", Descripcion="Puntos de recarga en vía pública.", Precio=0, Icono="🅿️" },

                // ═══════ ALQUILERES ═══════
                new ProductoTiendaDto { Id="alq-local", Categoria="alquileres", Nombre="Alquiler de local", Descripcion="Espacios comerciales en zonas estratégicas.", Precio=0, Icono="🔑" },
                new ProductoTiendaDto { Id="alq-bodega", Categoria="alquileres", Nombre="Alquiler de bodega", Descripcion="Bodegas con suministro eléctrico industrial.", Precio=0, Icono="📦" },

                // ═══════ REPARACIÓN ═══════
                new ProductoTiendaDto { Id="rep-falla", Categoria="reparacion", Nombre="Reparación de fallas eléctricas", Descripcion="Diagnóstico y reparación de cortocircuitos.", Precio=35000, Icono="🛠️" },
                new ProductoTiendaDto { Id="rep-mant", Categoria="reparacion", Nombre="Mantenimiento preventivo", Descripcion="Revisión periódica de instalaciones.", Precio=45000, Icono="🔧" },
                new ProductoTiendaDto { Id="rep-inspeccion", Categoria="reparacion", Nombre="Inspección y certificación", Descripcion="Certificación de instalaciones eléctricas.", Precio=65000, Icono="📋" },

                // ═══════ INTERNET 5G ═══════
                new ProductoTiendaDto { Id="int-basico", Categoria="internet", Nombre="Internet 5G Residencial Básico", Descripcion="100 Mbps de velocidad simétrica.", Precio=18900, Icono="📡" },
                new ProductoTiendaDto { Id="int-plus", Categoria="internet", Nombre="Internet 5G Residencial Plus", Descripcion="300 Mbps + router WiFi 6.", Precio=28500, Icono="🚀" },
                new ProductoTiendaDto { Id="int-emp", Categoria="internet", Nombre="Internet 5G Empresarial", Descripcion="500 Mbps + IP estática + soporte 24/7.", Precio=65000, Icono="💼" },

                // ═══════ VIDEOVIGILANCIA ═══════
                new ProductoTiendaDto { Id="vid-cam", Categoria="videovigilancia", Nombre="Cámara IP 2MP WiFi", Descripcion="Video HD con visión nocturna.", Precio=89000, Icono="🎥" },
                new ProductoTiendaDto { Id="vid-kit", Categoria="videovigilancia", Nombre="Kit videovigilancia 4 cámaras", Descripcion="DVR + 4 cámaras + instalación.", Precio=385000, Icono="📹" },

                // ═══════ AMBIENTALES ═══════
                new ProductoTiendaDto { Id="amb-huella", Categoria="ambientales", Nombre="Huella de carbono", Descripcion="Medición y certificación de emisiones.", Precio=0, Icono="📊" },
                new ProductoTiendaDto { Id="amb-residuos", Categoria="ambientales", Nombre="Gestión de residuos", Descripcion="Planes de manejo y disposición.", Precio=0, Icono="♻️" },
                new ProductoTiendaDto { Id="amb-reforest", Categoria="ambientales", Nombre="Compensación ambiental", Descripcion="Programas de reforestación.", Precio=0, Icono="🌱" },

                // ═══════ CALIBRACIÓN ═══════
                new ProductoTiendaDto { Id="cal-med", Categoria="calibracion", Nombre="Calibración de medidores", Descripcion="Verificación y ajuste de medidores.", Precio=35000, Icono="🔬" },
                new ProductoTiendaDto { Id="cal-equipo", Categoria="calibracion", Nombre="Calibración de equipos", Descripcion="Multímetros, pinzas, analizadores.", Precio=50000, Icono="⚙️" },

                // ═══════ TALLER ANONOS ═══════
                new ProductoTiendaDto { Id="ano-trafo", Categoria="anonos", Nombre="Reparación de transformadores", Descripcion="Servicio especializado de distribución.", Precio=0, Icono="🔧" },
                new ProductoTiendaDto { Id="ano-motor", Categoria="anonos", Nombre="Rebobinado de motores", Descripcion="Motores eléctricos hasta 500 HP.", Precio=0, Icono="⚙️" },
                new ProductoTiendaDto { Id="ano-torno", Categoria="anonos", Nombre="Fabricación de piezas", Descripcion="Torno y fresado de precisión.", Precio=0, Icono="🔩" },

                // ═══════ MARKETPLACE ═══════
                new ProductoTiendaDto { Id="mkt-bici", Categoria="marketplace", Nombre="Bicicleta eléctrica urbana", Descripcion="Vendida por cliente CNFL · 6 meses de uso.", Precio=285000, Icono="🚲" },
                new ProductoTiendaDto { Id="mkt-panel", Categoria="marketplace", Nombre="Panel solar 400W", Descripcion="Vendido por cliente CNFL · nuevo.", Precio=185000, Icono="☀️" }
            };

            if (string.IsNullOrWhiteSpace(categoriaId))
                return todos;

            return todos.Where(p => p.Categoria == categoriaId).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}