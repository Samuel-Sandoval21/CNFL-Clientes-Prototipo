# CNFL Clientes · Prototipo App Móvil

Prototipo funcional de una aplicación móvil para la **Compañía Nacional de Fuerza y Luz (CNFL)** de Costa Rica. Incluye módulo de **Cliente** y módulo de **Administrador**, ambos con diseño tipo app móvil (iOS/Android), notificaciones push reales, envío de correos, exportación a Excel/PDF y persistencia en SQL Server.

---

## 📱 Vista general

| Módulo | Descripción |
|---|---|
| **Cliente** | Gestión de servicios, facturas, trámites, reportes, marketplace, alertas y cuenta |
| **Admin** | Panel de control con métricas, gestión de clientes, averías, trámites, reportes y actividad |

Ambos módulos están diseñados como **app móvil real**: se visualizan dentro de un marco de iPhone/Android en desktop y ocupan toda la pantalla en móvil real.

---

## 🎯 Funcionalidades principales

### 🔐 Autenticación

- Login con cédula, correo o nombre de usuario.
- **Detección automática de rol** (Cliente / Administrador) al iniciar sesión.
- Registro con validación de cédula única, correo único y nombre de usuario único.
- Recuperación de contraseña con correo electrónico.
- Login biométrico (Face ID / Huella) preparado.
- Roles: **Cliente** y **Administrador**.

---

### 👤 Módulo Cliente

#### 🏠 Dashboard (Inicio)

- Saludo dinámico según hora (Buenos días / tardes / noches).
- Hero card con mensaje de bienvenida.
- Facturas pendientes con monto, NISE y fecha de vencimiento.
- Gráfica de consumo kWh con 3 modos: **últimos 6 meses · 31 días · lecturas**.
- Resumen del mes (consumo actual, anterior, promedio, próxima lectura).
- **Contáctanos funcional** (teléfono, correo, web, oficinas).

#### 🔌 Mis Servicios

- Lista de NISEs con estado (Normal / Prevención / Incidente).
- Historial de consumo (kWh y ₡).
- Consulta al medidor AMI (última lectura, voltaje, costo estimado).
- Reportes con 3 tipos de avería: alumbrado público, eléctrica propia, eléctrica ajena.
- **Mapa GIS** con Leaflet + OpenStreetMap mostrando:
  - Averías por color según clientes afectados.
  - Estaciones de recarga eléctrica.
  - Puntos verdes.
- Calculadora energética con persistencia en `localStorage`.

#### 📄 Trámites

- **Catálogo de 16 trámites**:
  - Cambio de conexión de voltaje
  - Cambio de nombre de abonado
  - Cambio de servicio provisional a definitivo
  - Desconexión y reconexión
  - Solicitud diseño de red eléctrica y DER
  - Ingreso a tarifa residencial horaria
  - Reclamo por daños con Responsabilidad Civil
  - Solicitud de conexión de transformador temporal
  - Solicitud de Servicio Nuevo Monofásico o Trifásico
  - Solicitud de Traslado de Medidor
  - Solicitud de Traspaso de Servicio Eléctrico
  - Solicitud de Suministro Eléctrico para Inmuebles
  - Solicitud de Suministro Eléctrico Especial
  - Solicitud de Alumbrado Público
  - Solicitud Servicio Especial de Carga Fija
  - Solicitud Servicio Especial Temporal para Eventos
- Modal con formulario para cada trámite.
- Tracker visual: **Iniciado → En Proceso → Resuelto**.
- Notificación push + correo al iniciar.

#### 🛍️ Productos y Servicios

- **Servicios Hogar 360°**: eficiencia energética, alarmas, cargadores eléctricos, domótica, mantenimiento de barras, venta e instalación, consumo verde y DERs.
- **Marketplace**: 7 categorías (Línea Blanca, Tecnología, Entretenimiento, A/C, Electrodomésticos, Audio, Herramientas) con 14 productos.
- **Subscripciones**: Seguros Hogar/Incendio, Asistencia, Videovigilancia.
- **Historial de compras** con subscripciones activas, servicios contratados, créditos abiertos y productos comprados.

#### 🛒 Carrito / Tienda / Mis Compras

- Carrito funcional con persistencia en `localStorage`.
- Subtotal + IVA 13% + Total.
- **5 métodos de pago**:
  - 💡 Cargar a factura eléctrica
  - 💳 Tarjeta de crédito/débito (Tilopay)
  - 📱 SINPE Móvil
  - 🏦 Transferencia IBAN
  - 🔐 Token bancario (BN, BCR, BAC, Popular y más)
- Confirmación de compra con notificación push.

#### 👤 Cuenta

- Foto de perfil con preview (subida y guardado en servidor).
- Edición de datos personales y contacto.
- Factura electrónica con checkbox + buscador de actividad económica por código o nombre.
- Seguridad: cambio de contraseña.
- Cerrar sesión.

#### 🔔 Alertas

- **4 categorías**:
  1. Averías (Reportada, En proceso, Finalizada + hora estimada)
  2. Suspensiones programadas por NISE (Programada, En Proceso)
  3. Facturas por vencer / Medio de pago vencido
  4. Eventos (Voltaje fuera de rango, consumo fuera de límites)
- Notificaciones push del navegador con sonido generado por Web Audio API.
- Envío de correo real vía SMTP.
- Configuración por tipo de alerta.

---

### 🛠️ Módulo Admin

#### 📊 Dashboard

- Métricas principales: clientes, NISEs, monto pendiente, ingresos del mes.
- Métricas secundarias: averías abiertas/resueltas, trámites abiertos, tiempo promedio.
- Gráficos interactivos con **Chart.js**:
  - Averías por estado (doughnut).
  - Averías últimos 7 días (line).
- Banner de bienvenida con saludo dinámico.

#### 👥 Clientes

- Búsqueda por nombre, cédula o correo.
- Lista mobile con avatar, chips de estado y estadísticas.
- Vista de detalle por cliente (NISEs, facturas, averías, trámites).

#### ⚡ Averías

- Filtros por estado (Todas, Ingresadas, En revisión, Resueltas).
- Cards con NISE, fecha y descripción.
- Estadísticas arriba: abiertas, en proceso, resueltas.

#### 📋 Trámites

- Filtros por estado (Todos, Solicitados, En proceso, Completados).
- Vista con referencia, tipo, cliente y fecha.

#### 📈 Reportes

- Resumen financiero: facturado, cobrado, pendiente.
- Gráfico de facturación últimos 6 meses.
- **Exportación real a Excel**:
  - 10 hojas con datos de Resumen, Clientes, NISEs, Facturas, Averías, Trámites, Actividad.
  - 3 hojas adicionales con datos listos para insertar gráficos manualmente en Excel.
- **Exportación real a PDF**:
  - 6 secciones de tablas: Resumen General, Clientes, NISEs, Facturas, Averías, Trámites.
  - Estilos con paleta CNFL y encabezados.

#### ⏱️ Actividad de uso

- Top 10 usuarios con más minutos de uso.
- Gráficos por sección y por día.

#### 🔔 Alertas

- Notificaciones del sistema para el admin.
- Banners de prioridad (crítico, advertencia, ok).

#### 👤 Cuenta (Admin)

- Perfil con iniciales del administrador.
- Accesos rápidos: Clientes, Actividad de uso, Configuración, Alertas.
- Cerrar sesión.

#### ⚙️ Configuración

- Info del sistema (versión, framework, BD).
- Estadísticas generales.

---

## 🌐 Internacionalización

Soporte para **5 idiomas**:

- 🇨🇷 Español (Costa Rica)
- 🇺🇸 English (US)
- 🇫🇷 Français
- 🇧🇷 Português (BR)
- 🇨🇳 中文 (简体)

Persistido en `localStorage`.

---

## 🎨 Diseño

- **Mobile-first**: marco de celular en desktop, pantalla completa en móvil.
- **Paleta CNFL**:
  - Azul `#1a2b6b` (confianza / navegación)
  - Azul oscuro `#001482` (seguridad)
  - Naranja `#ff692d` (CTA / conversión)
  - Verde `#00a651` (éxito / ahorro)
  - Dorado `#c98a00` (prevención)
  - Rojo `#e53935` (incidente)
  - Morado `#5b3fbf` (acento admin)
- **Bottom navigation** (no sidebar).
- **Modo oscuro** persistente.
- **Zoom** persistente para accesibilidad.
- **Componentes reutilizables**: cards, chips de estado, trackers, modales.

---

## 🏗️ Arquitectura técnica

### Stack

| Capa | Tecnología |
|---|---|
| **Backend** | ASP.NET MVC 4.8.1 · .NET Framework 4.8.1 |
| **Base de datos** | SQL Server 2022 (LocalDB) |
| **ORM** | Entity Framework 6 |
| **Frontend** | Razor Views · CSS3 · Vanilla JS |
| **Gráficos** | Chart.js 4.4.1 (CDN) |
| **Mapas** | Leaflet 1.9.4 + OpenStreetMap |
| **Notificaciones** | Web Notifications API + Web Audio API |
| **Correo** | SMTP (Gmail / Office 365 / otro) |
| **Excel** | ClosedXML 0.105.1 |
| **PDF** | iTextSharp 5.5.13 |

### Estructura del proyecto

```text
CNFL Clientes Prototipo/
│
├── Index.html                  # Página lanzadora (raíz)
├── README.md                   # Este archivo
├── .gitignore
├── CNFL Clientes Prototipo.sln
│
├── App_Data/
├── App_Start/
│   ├── BundleConfig.cs
│   ├── FilterConfig.cs
│   └── RouteConfig.cs
│
├── Content/
│   ├── admin/
│   │   ├── admin.css
│   │   ├── averias.css
│   │   ├── clientes.css
│   │   ├── dashboard.css
│   │   ├── detalle-averia.css
│   │   ├── detalle-cliente.css
│   │   └── reportes.css
│   ├── cliente/
│   │   ├── app.css
│   │   ├── calculadora.css
│   │   ├── carrito.css
│   │   ├── chat.css
│   │   ├── cuenta.css
│   │   ├── dashboard.css
│   │   ├── detalle-tramite.css
│   │   ├── editar-datos.css
│   │   ├── estado-averia.css
│   │   ├── historial-compras.css
│   │   ├── mis-facturas.css
│   │   ├── reportes.css
│   │   ├── servicios-contratados.css
│   │   ├── suscripciones.css
│   │   ├── tienda.css
│   │   └── tramites.css
│   ├── img/
│   │   └── logo-cnfl.png
│   ├── bootstrap-grid.css
│   ├── bootstrap-grid.css.map
│   ├── bootstrap-grid.min.css
│   ├── bootstrap-grid.min.css.map
│   ├── bootstrap-grid.rtl.css
│   └── Site.css
│
├── Controllers/
│   ├── AdminController.cs
│   ├── CarritoController.cs
│   ├── ClientesController.cs
│   ├── CuentaController.cs
│   ├── HomeController.cs
│   ├── PagosController.cs
│   ├── TiendaController.cs
│   └── TramitesController.cs
│
├── Data/
│   └── CNFLDbContext.cs
│
├── Filters/
│   └── SessionAuthorizeAttribute.cs
│
├── Models/
│   ├── ActividadEconomica.cs
│   ├── ActividadUsuario.cs
│   ├── Averia.cs
│   ├── DashboardDtos.cs
│   ├── DescargaUsuario.cs
│   ├── Factura.cs
│   ├── NISE.cs
│   ├── Notificacion.cs
│   ├── Pago.cs
│   ├── Tramite.cs
│   ├── Usuario.cs
│   └── UsuarioRol.cs
│
├── Repositories/
│   ├── AveriaRepository.cs
│   ├── FacturaRepository.cs
│   └── NotificacionRepository.cs
│
├── Scripts/
│   ├── admin/
│   │   ├── admin.js
│   │   ├── averias.js
│   │   ├── clientes.js
│   │   └── reportes.js
│   ├── cliente/
│   │   ├── app.js
│   │   ├── calculadora.js
│   │   ├── carrito.js
│   │   ├── chat.js
│   │   ├── cuenta.js
│   │   ├── dashboard.js
│   │   ├── detalle-tramite.js
│   │   ├── editar-datos.js
│   │   ├── estado-averia.js
│   │   ├── mis-facturas.js
│   │   ├── perfil.js
│   │   ├── recuperar-clave.js
│   │   ├── reportar-averia.js
│   │   ├── reportes.js
│   │   ├── servicios-contratados.js
│   │   ├── suscripciones.js
│   │   ├── tienda.js
│   │   └── tramites.js
│   ├── bootstrap.bundle.js
│   ├── bootstrap.bundle.js.map
│   ├── bootstrap.bundle.min.js
│   ├── bootstrap.bundle.min.js.map
│   ├── registro.js
│   └── Site.js
│
├── Services/
│   ├── DashboardService.cs
│   ├── NISEService.cs
│   ├── NotificationService.cs
│   ├── PaymentService.cs
│   ├── SuscripcionService.cs
│   ├── SuspensionService.cs
│   ├── TramiteService.cs
│   └── UsuarioService.cs
│
├── Views/
│   ├── Admin/
│   │   ├── Actividad.cshtml
│   │   ├── Alertas.cshtml
│   │   ├── Averias.cshtml
│   │   ├── Clientes.cshtml
│   │   ├── Configuracion.cshtml
│   │   ├── Cuenta.cshtml
│   │   ├── Dashboard.cshtml
│   │   ├── DetalleAveria.cshtml
│   │   ├── DetalleCliente.cshtml
│   │   ├── Reportes.cshtml
│   │   └── Tramites.cshtml
│   ├── Clientes/
│   │   ├── Alertas.cshtml
│   │   ├── Calculadora.cshtml
│   │   ├── ConsultaMedidor.cshtml
│   │   ├── Cuenta.cshtml
│   │   ├── Dashboard.cshtml
│   │   ├── DetalleProducto.cshtml
│   │   ├── EstadoAveria.cshtml
│   │   ├── HistorialConsumo.cshtml
│   │   ├── MisServicios.cshtml
│   │   ├── ProductosServicios.cshtml
│   │   ├── Reportes.cshtml
│   │   ├── Tienda.cshtml
│   │   └── Tramites.cshtml
│   ├── Cuenta/
│   │   ├── _ConsentimientoInformado.cshtml
│   │   ├── _PoliticaPrivacidad.cshtml
│   │   ├── Login.cshtml
│   │   ├── RecuperarClave.cshtml
│   │   └── Registro.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Shared/
│   │   ├── _EstilosBase.cshtml
│   │   ├── _LayoutAdmin.cshtml
│   │   ├── _LayoutApp.cshtml
│   │   ├── _LayoutCliente.cshtml
│   │   ├── _LayoutLogin.cshtml
│   │   ├── _LayoutPublico.cshtml
│   │   ├── _LogoCNFL.cshtml
│   │   └── Error.cshtml
│   ├── Tramites/
│   │   ├── Detalle.cshtml
│   │   ├── Index.cshtml
│   │   └── MisTramites.cshtml
│   ├── _ViewStart.cshtml
│   └── Web.config
│
├── favicon.ico
├── Global.asax
├── Global.asax.cs
├── packages.config
└── Web.config
```

---

## 🚀 Instalación y ejecución

### Requisitos previos

- **Visual Studio 2026** (Community o superior).
- **.NET Framework 4.8.1**.
- **SQL Server 2022** (LocalDB o instancia completa).
- Navegador moderno (Chrome, Edge, Firefox).

### Paquetes NuGet requeridos

- `EntityFramework` 6.x
- `ClosedXML` 0.105.1 (Excel)
- `itextsharp` 5.5.13 (PDF)
- `Newtonsoft.Json`

### Pasos

1. **Clonar el repositorio**

   ```bash
   git clone https://github.com/Samuel-Sandoval21/CNFL.git
   cd CNFL
   ```

2. **Abrir la solución**
   - Doble clic en `CNFL Clientes Prototipo.sln` o abrir desde VS.

3. **Restaurar paquetes NuGet**
   - Clic derecho en la solución → **Restaurar paquetes NuGet**.

4. **Configurar la base de datos**
   - La cadena de conexión por defecto usa LocalDB:
     ```
     Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CNFL_Clientes;Integrated Security=True
     ```
   - Si la BD no existe, ejecutá los scripts SQL o ajustá la cadena en `Web.config`.

5. **Configurar SMTP** (opcional, para correos)

   ```xml
   <add key="SmtpHost" value="smtp.gmail.com" />
   <add key="SmtpPort" value="587" />
   <add key="SmtpUser" value="tucorreo@gmail.com" />
   <add key="SmtpPass" value="tu-contraseña-de-aplicacion" />
   <add key="SmtpFrom" value="tucorreo@gmail.com" />
   ```
   > En Gmail necesitás generar una **contraseña de aplicación**.

6. **Compilar y ejecutar**
   - `Ctrl + Shift + B` → Compilar.
   - `Ctrl + F5` → Ejecutar.

7. **Abrir en el navegador**
   - App: `http://localhost:44387`
   - Alternativa: doble clic en `Index.html` (raíz) para una página lanzadora.

---

## 👥 Usuarios de prueba

| Rol | Cédula | Correo | Contraseña |
|---|---|---|---|
| 👤 Cliente | `2-0874-0716` | `ssandoval40716@ufide.ac.cr` | `cliente123` |
| 🧑‍💼 Admin | `1-1180-0989` | `mimiranda@cnfl.go.cr` | `admin123` |

> ⚠️ Datos de ejemplo. Cambialos según tu base de datos.

---

## 🔐 Seguridad

- **Contraseñas**: en texto plano en este prototipo (en producción usar BCrypt).
- **Autorización**: `SessionAuthorizeAttribute` valida sesión y rol por controlador.
- **Anti-forgery**: todos los POST usan `@Html.AntiForgeryToken()`.
- **SMTP**: credenciales leídas de `Web.config`, no del código.
- **HTTPS**: forzar con `[RequireHttps]` en producción.

---

## 🧪 Pruebas realizadas

- ✅ Login con cédula, correo y nombre de usuario.
- ✅ Detección automática de rol (Cliente / Admin).
- ✅ Registro con validaciones de unicidad.
- ✅ Recuperación de contraseña.
- ✅ Cascada Provincia → Cantón → Distrito.
- ✅ Carrito con persistencia en `localStorage`.
- ✅ 5 métodos de pago.
- ✅ Notificaciones push con sonido.
- ✅ Envío de correo vía SMTP.
- ✅ Gráficos con Chart.js.
- ✅ Mapa GIS con Leaflet.
- ✅ Cambio de idioma (5 idiomas).
- ✅ Modo oscuro y zoom persistentes.
- ✅ Admin: gestión de clientes, averías, trámites.
- ✅ Admin: exportación real a Excel (10 hojas).
- ✅ Admin: exportación real a PDF (6 secciones).
- ✅ Contáctanos funcional en público y privado (teléfono, correo, web, oficinas).

---

## 📋 Pendientes / Mejoras futuras

- 🔲 Hashing de contraseñas con BCrypt.
- 🔲 Firebase Cloud Messaging para push multiplataforma.
- 🔲 App móvil nativa en Flutter / FlutterFlow / .NET MAUI.
- 🔲 Exponer el prototipo por API (arquitectura recomendada).
- 🔲 HTTPS obligatorio en producción.
- 🔲 Rate limiting en endpoints sensibles.
- 🔲 Logs con Serilog o NLog.
- 🔲 Migración a .NET moderno (.NET 8).

---

## 📚 Diagrama del proceso

```text
Cliente → Servicios → Facturas / Historial / AMI / Reportes / Mapa GIS / Calculadora
                    → Alertas → Push + correo
                    → Trámites → Formulario → Proceso CNFL → Notificar
                    → Productos y Servicios → Servicios / Marketplace / Subscripciones
                    → Carrito → Métodos de pago → Confirmación

Admin   → Dashboard → Métricas + gráficos
        → Clientes → Búsqueda + detalle
        → Averías → Filtros + cambio de estado
        → Trámites → Filtros + cambio de estado
        → Reportes → Rango de fechas + export Excel/PDF
        → Actividad → Log de uso por cliente
```

---

## 👨‍💻 Autor

- **Samuel Sandoval Ramírez**
- Cédula: 2-0874-0716
- Bachillerato en Ingeniería en Sistemas de Computación — Universidad Fidélitas
- GitHub: [@Samuel-Sandoval21](https://github.com/Samuel-Sandoval21)

**Supervisor CNFL:** Michael Miranda Guevara — Área de Inteligencia de Negocios
**Período:** 24 de agosto al 25 de setiembre de 2026

---

## 📄 Licencia

Prototipo académico desarrollado como parte del **Trabajo Comunal Universitario (TCU)**. Todos los derechos de la marca CNFL pertenecen a la Compañía Nacional de Fuerza y Luz de Costa Rica.

---

## 🙏 Agradecimientos

- **CNFL** por permitir el desarrollo del prototipo.
- **Katherine Villalobos** y **Daniel Rodríguez** por la revisión del diagrama de proceso.
- **Comunidad de desarrolladores** por las librerías open source (Leaflet, Chart.js, ClosedXML, iTextSharp, etc.).

---

**Última actualización:** Septiembre 2026