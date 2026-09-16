using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    // ═══════════════════════════════════════════════════════════
    // DTOs del DASHBOARD (Cliente)
    // ═══════════════════════════════════════════════════════════

    public class AveriaResumenDto
    {
        public int AveriaId { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaReporte { get; set; }
    }

    public class TramiteResumenDto
    {
        public int TramiteId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string NumeroReferencia { get; set; }
        public string Descripcion { get; set; }
    }

    public class NotificacionResumenDto
    {
        public int NotificacionId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
    }

    public class DistribucionNiseDto
    {
        public string label { get; set; }
        public decimal value { get; set; }
        public string color { get; set; }
    }

    public class ConsumoMensualDto
    {
        public string mes { get; set; }
        public decimal monto { get; set; }
        public double kwh { get; set; }
    }

    public class ActividadSemanalDto
    {
        public string dia { get; set; }
        public int facturas { get; set; }
        public int reportes { get; set; }
        public int tramites { get; set; }
        public int perfil { get; set; }
    }

    public class SeccionTopDto
    {
        public string nombre { get; set; }
        public int visitas { get; set; }
    }

    public class BannerDto
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Icono { get; set; }
        public string CategoriaId { get; set; }
        public string ColorInicio { get; set; }
        public string ColorFin { get; set; }
    }

    /// <summary>DTO con las métricas de uso agregadas del cliente.</summary>
    public class MetricasUsoDto
    {
        public int TotalSesiones { get; set; }
        public int TiempoTotalMinutos { get; set; }
        public int PromedioMinutosPorDia { get; set; }
        public string SeccionMasVisitada { get; set; }
        public int TotalDescargas { get; set; }
        public int DescargasPDF { get; set; }
        public int DescargasExcel { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // DTOs del ADMIN
    // ═══════════════════════════════════════════════════════════

    /// <summary>DTO con datos de un cliente para el panel de administración.</summary>
    public class ClienteAdminDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int TotalNISEs { get; set; }
        public int FacturasPendientes { get; set; }
        public decimal MontoPendiente { get; set; }
        public int AveriasActivas { get; set; }
    }

    /// <summary>DTO con tiempo de uso de la app por cliente (Admin → Actividad).</summary>
    public class TiempoUsoClienteDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int MinutosTotales { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // DTOs para Registro
    // ═══════════════════════════════════════════════════════════

    /// <summary>DTO simple para listar actividades económicas en el registro.</summary>
    public class ActividadEconomicaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    /// <summary>DTO para cascada de provincia → cantón → distrito.</summary>
    public class UbicacionCostaRica
    {
        public static readonly Dictionary<string, Dictionary<string, List<string>>> Catalogo =
            new Dictionary<string, Dictionary<string, List<string>>>
        {
            { "San José", new Dictionary<string, List<string>>
                {
                    { "San José", new List<string> { "Carmen", "Merced", "Hospital", "Catedral", "Zapote", "San Francisco de Dos Ríos", "Uruca", "Mata Redonda", "Pavas", "Hatillo", "San Sebastián" } },
                    { "Escazú", new List<string> { "Escazú", "San Antonio", "San Rafael" } },
                    { "Desamparados", new List<string> { "Desamparados", "San Miguel", "San Juan de Dios", "San Rafael Arriba", "San Antonio", "Frailes", "Patarrá", "San Cristóbal", "Rosario", "Damas", "San Rafael Abajo", "Gravilias", "Los Guido" } },
                    { "Puriscal", new List<string> { "Santiago", "Mercedes Sur", "Barbacoas", "Grifo Alto", "San Rafael", "Candelarita", "Desamparaditos", "San Antonio", "Chires" } },
                    { "Tarrazú", new List<string> { "San Marcos", "San Lorenzo", "San Carlos" } },
                    { "Aserrí", new List<string> { "Aserrí", "Tarbaca", "Vuelta de Jorco", "San Gabriel", "Legua", "Monterrey", "Salitrillos" } },
                    { "Mora", new List<string> { "Colón", "Guayabo", "Tabarcia", "Piedras Negras", "Picagres", "Jaris", "Quitirrisí" } },
                    { "Goicoechea", new List<string> { "Guadalupe", "San Francisco", "Calle Blancos", "Mata de Plátano", "Ipís", "Rancho Redondo", "Purral" } },
                    { "Santa Ana", new List<string> { "Santa Ana", "Salitral", "Pozos", "Uruca", "Piedades", "Brasil" } },
                    { "Alajuelita", new List<string> { "Alajuelita", "San Josecito", "San Antonio", "Concepción", "San Felipe" } },
                    { "Vásquez de Coronado", new List<string> { "San Isidro", "San Rafael", "Dulce Nombre de Jesús", "Patalillo", "Cascajal" } },
                    { "Acosta", new List<string> { "San Ignacio", "Guaitil", "Palmichal", "Cangrejal", "Sabanillas" } },
                    { "Tibás", new List<string> { "San Juan", "Cinco Esquinas", "Anselmo Llorente", "León XIII", "Colima" } },
                    { "Moravia", new List<string> { "San Vicente", "San Jerónimo", "La Trinidad" } },
                    { "Montes de Oca", new List<string> { "San Pedro", "Sabanilla", "Mercedes", "San Rafael" } },
                    { "Turrubares", new List<string> { "San Pablo", "San Pedro", "San Juan de Mata", "San Luis", "Carara" } },
                    { "Dota", new List<string> { "Santa María", "Jardín", "Copey" } },
                    { "Curridabat", new List<string> { "Curridabat", "Granadilla", "Sánchez", "Tirrases" } },
                    { "Pérez Zeledón", new List<string> { "San Isidro de El General", "El General", "Daniel Flores", "Rivas", "San Pedro", "Platanares", "Pejibaye", "Cajón", "Barú", "Río Nuevo", "Páramo" } },
                    { "León Cortés", new List<string> { "San Pablo", "San Andrés", "Llano Bonito", "San Isidro", "Santa Cruz", "San Antonio" } }
                }
            },
            { "Alajuela", new Dictionary<string, List<string>>
                {
                    { "Alajuela", new List<string> { "Alajuela", "San José", "Carrizal", "San Antonio", "Guácima", "San Isidro", "Sabanilla", "San Rafael", "Río Segundo", "Desamparados", "Turrúcares", "Tambor", "Garita", "Sarapiquí" } },
                    { "San Ramón", new List<string> { "San Ramón", "Santiago", "San Juan", "Piedades Norte", "Piedades Sur", "San Rafael", "San Isidro", "Ángeles", "Alfaro", "Volio", "Concepción", "Zapotal", "Peñas Blancas", "San Lorenzo" } },
                    { "Grecia", new List<string> { "Grecia", "San Isidro", "San José", "San Roque", "Tacares", "Puente de Piedra", "Bolívar" } },
                    { "San Mateo", new List<string> { "San Mateo", "Desmonte", "Jesús María", "Labrador" } },
                    { "Atenas", new List<string> { "Atenas", "Jesús", "Mercedes", "San Isidro", "Concepción", "San José", "Santa Eulalia", "Escobal" } },
                    { "Naranjo", new List<string> { "Naranjo", "San Miguel", "San José", "Cirrí Sur", "San Jerónimo", "San Juan", "El Rosario", "Palmitos" } },
                    { "Palmares", new List<string> { "Palmares", "Zaragoza", "Buenos Aires", "Santiago", "Candelaria", "Esquipulas", "La Granja" } },
                    { "Poás", new List<string> { "San Pedro", "San Juan", "San Rafael", "Carrillos", "Sabana Redonda" } },
                    { "Orotina", new List<string> { "Orotina", "El Mastate", "Hacienda Vieja", "Coyolar", "La Ceiba" } },
                    { "San Carlos", new List<string> { "Quesada", "Florencia", "Buenavista", "Aguas Zarcas", "Venecia", "Pital", "La Fortuna", "La Tigra", "La Palmera", "Venado", "Cutris", "Monterrey", "Pocosol" } },
                    { "Zarcero", new List<string> { "Zarcero", "Laguna", "Tapesco", "Guadalupe", "Palmira", "Zapote", "Brisas" } },
                    { "Sarchí", new List<string> { "Sarchí Norte", "Sarchí Sur", "Toro Amarillo", "San Pedro", "Rodríguez" } },
                    { "Upala", new List<string> { "Upala", "Aguas Claras", "San José", "Bijagua", "Delicias", "Dos Ríos", "Yolillal", "Canalete" } },
                    { "Los Chiles", new List<string> { "Los Chiles", "Caño Negro", "El Amparo", "San Jorge" } },
                    { "Guatuso", new List<string> { "San Rafael", "Buenavista", "Cote", "Katira" } },
                    { "Río Cuarto", new List<string> { "Río Cuarto", "Santa Rita", "Santa Isabel" } }
                }
            },
            { "Cartago", new Dictionary<string, List<string>>
                {
                    { "Cartago", new List<string> { "Oriental", "Occidental", "Carmen", "San Nicolás", "Aguacaliente", "Guadalupe", "Corralillo", "Tierra Blanca", "Dulce Nombre", "Llano Grande", "Quebradilla" } },
                    { "Paraíso", new List<string> { "Paraíso", "Santiago", "Orosi", "Cachí", "Los Llanos de Santa Lucía" } },
                    { "La Unión", new List<string> { "Tres Ríos", "San Diego", "San Juan", "San Rafael", "Concepción", "Dulce Nombre", "San Ramón", "Río Azul" } },
                    { "Jiménez", new List<string> { "Juan Viñas", "Tucurrique", "Pejibaye" } },
                    { "Turrialba", new List<string> { "Turrialba", "Jesús", "Santa Cruz", "Santa Teresita", "Pavones", "Tuis", "Tayutic", "Santa Rosa", "Tres Equis", "La Isabel", "Chirripó" } },
                    { "Alvarado", new List<string> { "Pacayas", "Cervantes", "Capellades" } },
                    { "Oreamuno", new List<string> { "San Rafael", "Cot", "Potrero Cerrado", "Cipreses", "Santa Rosa" } },
                    { "El Guarco", new List<string> { "El Tejar", "San Isidro", "Tobosi", "Patio de Agua" } }
                }
            },
            { "Heredia", new Dictionary<string, List<string>>
                {
                    { "Heredia", new List<string> { "Heredia", "Mercedes", "San Francisco", "Ulloa", "Varablanca" } },
                    { "Barva", new List<string> { "Barva", "San Pedro", "San Pablo", "San Roque", "Santa Lucía", "San José de la Montaña" } },
                    { "Santo Domingo", new List<string> { "Santo Domingo", "San Vicente", "San Miguel", "Paracito", "Santo Tomás", "Santa Rosa", "Tures", "Paraíso" } },
                    { "Santa Bárbara", new List<string> { "Santa Bárbara", "San Pedro", "San Juan", "Jesús", "Santo Domingo", "Purabá" } },
                    { "San Rafael", new List<string> { "San Rafael", "San Josecito", "Santiago", "Los Ángeles", "Concepción" } },
                    { "San Isidro", new List<string> { "San Isidro", "San José", "Concepción", "San Francisco" } },
                    { "Belén", new List<string> { "San Antonio", "La Ribera", "La Asunción" } },
                    { "Flores", new List<string> { "San Joaquín", "Barrantes", "Llorente" } },
                    { "San Pablo", new List<string> { "San Pablo", "Rincón de Sabanilla" } },
                    { "Sarapiquí", new List<string> { "Puerto Viejo", "La Virgen", "Las Horquetas", "Llanuras del Gaspar", "Cureña" } }
                }
            },
            { "Guanacaste", new Dictionary<string, List<string>>
                {
                    { "Liberia", new List<string> { "Liberia", "Cañas Dulces", "Mayorga", "Nacascolo", "Curubandé" } },
                    { "Nicoya", new List<string> { "Nicoya", "Mansión", "San Antonio", "Quebrada Honda", "Sámara", "Nosara", "Belén de Nosarita" } },
                    { "Santa Cruz", new List<string> { "Santa Cruz", "Bolsón", "Veintisiete de Abril", "Tempate", "Cartagena", "Cuajiniquil", "Diriá", "Cabo Velas", "Tamarindo" } },
                    { "Bagaces", new List<string> { "Bagaces", "Fortuna", "Mogote", "Río Naranjo" } },
                    { "Carrillo", new List<string> { "Filadelfia", "Palmira", "Sardinal", "Belén" } },
                    { "Cañas", new List<string> { "Cañas", "Palmira", "San Miguel", "Bebedero", "Porozal" } },
                    { "Abangares", new List<string> { "Las Juntas", "Sierra", "San Juan", "Colorado" } },
                    { "Tilarán", new List<string> { "Tilarán", "Quebrada Grande", "Tronadora", "Santa Rosa", "Líbano", "Tierras Morenas", "Arenal" } },
                    { "Nandayure", new List<string> { "Carmona", "Santa Rita", "Zapotal", "San Pablo", "Porvenir", "Bejuco" } },
                    { "La Cruz", new List<string> { "La Cruz", "Santa Cecilia", "La Garita", "Santa Elena" } },
                    { "Hojancha", new List<string> { "Hojancha", "Monte Romo", "Puerto Carrillo", "Huacas" } }
                }
            },
            { "Puntarenas", new Dictionary<string, List<string>>
                {
                    { "Puntarenas", new List<string> { "Puntarenas", "Pitahaya", "Chomes", "Lepanto", "Paquera", "Manzanillo", "Guacimal", "Barranca", "Monte Verde", "Isla del Coco", "Cóbano", "Chacarita", "Chira", "Acapulco", "El Roble", "Arancibia" } },
                    { "Esparza", new List<string> { "Espíritu Santo", "San Juan Grande", "Macacona", "San Rafael", "San Jerónimo", "Caldera" } },
                    { "Buenos Aires", new List<string> { "Buenos Aires", "Volcán", "Potrero Grande", "Boruca", "Pilas", "Colinas", "Chánguena", "Biolley", "Brunka" } },
                    { "Montes de Oro", new List<string> { "Miramar", "La Unión", "San Isidro" } },
                    { "Osa", new List<string> { "Puerto Cortés", "Palmar", "Sierpe", "Bahía Ballena", "Piedras Blancas", "Bahía Drake" } },
                    { "Quepos", new List<string> { "Quepos", "Savegre", "Naranjito" } },
                    { "Golfito", new List<string> { "Golfito", "Puerto Jiménez", "Guaycará", "Pavón" } },
                    { "Coto Brus", new List<string> { "San Vito", "Sabalito", "Aguabuena", "Limoncito", "Pittier", "Gutiérrez Braun" } },
                    { "Parrita", new List<string> { "Parrita" } },
                    { "Corredores", new List<string> { "Corredor", "La Cuesta", "Canoas", "Laurel" } },
                    { "Garabito", new List<string> { "Jacó", "Tárcoles", "Lagunillas" } },
                    { "Monteverde", new List<string> { "Monteverde" } }
                }
            },
            { "Limón", new Dictionary<string, List<string>>
                {
                    { "Limón", new List<string> { "Limón", "Valle La Estrella", "Río Blanco", "Matama" } },
                    { "Pococí", new List<string> { "Guápiles", "Jiménez", "Rita", "Roxana", "Cariari", "Colorado", "La Colonia" } },
                    { "Siquirres", new List<string> { "Siquirres", "Pacuarito", "Florida", "Germania", "El Cairo", "Alegría", "Reventazón" } },
                    { "Talamanca", new List<string> { "Bratsi", "Sixaola", "Cahuita", "Telire" } },
                    { "Matina", new List<string> { "Matina", "Batán", "Carrandi" } },
                    { "Guácimo", new List<string> { "Guácimo", "Mercedes", "Pocora", "Río Jiménez", "Duacarí" } }
                }
            }
        };
    }
}