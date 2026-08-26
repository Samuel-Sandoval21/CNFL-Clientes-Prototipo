# CNFL-Clientes-Prototipo
Prototipo funcional para la aplicación móvil CNFL Clientes - TCU Samuel Sandoval Ramírez
# CNFL Clientes - Prototipo Funcional

## 📱 Prototipo de aplicación móvil para la Compañía Nacional de Fuerza y Luz (CNFL)

Este repositorio contiene el prototipo funcional desarrollado como parte del Trabajo Comunal Universitario (TCU) de **Samuel Sandoval Ramírez**, estudiante de **Bachillerato en Ingeniería en Sistemas de Computación** en la Universidad Fidélitas.

---

## 🎯 Objetivo del proyecto

Desarrollar un prototipo funcional de la aplicación móvil **CNFL Clientes**, con enfoque en el ciclo **pago–corte–reconexión**, mejorando la experiencia del usuario, la accesibilidad y la integración de servicios no regulados.

---

## 🛠️ Tecnologías utilizadas

| Componente | Tecnología |
|------------|------------|
| **IDE** | Visual Studio 2026 |
| **Framework** | .NET Framework 4.8.1 |
| **Lenguaje** | C# |
| **Arquitectura** | MVC (Modelo-Vista-Controlador) |
| **Frontend** | HTML, CSS, JavaScript |
| **Control de versiones** | Git / GitHub |
| **Base de datos** | Simulada (sin conexión real) |

---

## 📦 Estructura del proyecto


CNFL.Clientes.Prototipo/
├── Controllers/
│ ├── HomeController.cs
│ ├── AccountController.cs
│ ├── ConsultasController.cs
│ ├── SolicitudesController.cs
│ └── ContactoController.cs
├── Models/
│ ├── Usuario.cs
│ ├── Factura.cs
│ ├── Averia.cs
│ ├── Nise.cs
│ └── ...
├── Views/
│ ├── Home/
│ ├── Account/
│ ├── Consultas/
│ ├── Solicitudes/
│ └── Shared/
├── Services/
│ └── DatosSimuladosService.cs
├── wwwroot/
│ ├── css/
│ │ ├── style.css
│ │ └── accesibilidad.css
│ ├── js/
│ │ ├── main.js
│ │ └── accesibilidad.js
│ └── images/
└── README.md



---

## 🚀 Funcionalidades implementadas

| # | Funcionalidad | Estado |
|---|---------------|--------|
| 1 | Login seguro | ✅ |
| 2 | Registro de usuarios | ✅ |
| 3 | Home sin login (informativo) | ✅ |
| 4 | Menú fijo al hacer scroll | ✅ |
| 5 | Accesibilidad (contraste, fuente) | ✅ |
| 6 | Notificaciones push | ✅ |
| 7 | Pago dentro de la app | ✅ |
| 8 | Reporte de averías con foto | ✅ |
| 9 | Estado del reporte (tracking) | ✅ |
| 10 | Consulta de facturas | ✅ |
| 11 | Historial de consumo (gráfica) | ✅ |
| 12 | Perfil del cliente | ✅ |
| 13 | Reconexión | ✅ |
| 14 | Suscripción de servicios no regulados | ✅ |
| 15 | Consumo por franja horaria | ✅ |

---

## 🔧 Cómo ejecutar el proyecto

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/tu-usuario/CNFL-Clientes-Prototipo.git
