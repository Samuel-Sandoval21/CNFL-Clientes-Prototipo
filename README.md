# CNFL Clientes · Prototipo App Móvil

Prototipo funcional de una aplicación móvil para la **Compañía Nacional de Fuerza y Luz (CNFL)** de Costa Rica. Incluye módulo de **Cliente** y módulo de **Administrador**, ambos con diseño tipo app móvil (iOS/Android), notificaciones push reales, envío de correos, exportación a Excel/PDF con gráficos y persistencia en SQL Server.

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

#### 📈 Reportes (rediseñado)

- **Reporte general unificado** con un solo origen de datos (`ReporteClienteViewModel`) que alimenta la pantalla, el Excel y el PDF.
- **Filtros por cliente y período** (todo / 7 días / 30 días / 90 días / año en curso).
- **Secciones**:
  1. Resumen general del sistema (clientes, NISEs, facturas, servicios al día/con deuda, averías, trámites abiertos, facturado, cobrado, pendiente, porcentaje).
  2. Clientes con mayor saldo pendiente.
  3. Detalle del cliente seleccionado con estado de cuenta (Al día / Con pendientes / Con vencidas).
  4. Servicios (NISE) con saldo por servicio.
  5. Facturas con badges de estado (Pagada / Pendiente / Vencida) y días de atraso.
  6. Averías con promedio de días de resolución.
  7. Trámites con estado actual.
  8. Compras con método de pago y ticket promedio.
  9. Actividad en la app (sesiones, minutos, secciones más usadas).
- **Gráficos interactivos** con Chart.js:
  - Facturado vs Cobrado (últimos 6 meses, sistema).
  - Facturado vs Pagado (últimos 12 meses, por cliente).
- **Exportación real a Excel (EPPlus)**:
  - 7 hojas: Resumen, Cliente, Facturas, Averías, Trámites, Compras, Actividad.
  - Banners de encabezado, tablas con auto-filtro, filas alternadas, bordes, encabezado congelado.
  - Estados con color (verde / rojo / ámbar).
  - **Gráficos nativos de Excel** (facturado vs cobrado, tendencia de consumo, ventas por mes).
  - Configuración de impresión A4 horizontal lista.
- **Exportación real a PDF (iTextSharp)**:
  - Banner de portada con período y fecha de generación.
  - Tarjetas de KPI por sección.
  - Gráficos generados con `System.Drawing` insertados como imágenes.
  - Tablas con encabezado repetido por página.
  - Pie de página con número de página.
  - Búsqueda automática de fuente con símbolo `₡` (Segoe UI / Arial / Calibri) o fallback a `CRC `.

#### ⏱️ Actividad de uso

- Top 10 usuarios con más minutos de uso.
- Gráficos por sección y por día.

#### 🔔 Alertas

- Notificaciones del sistema para el admin.
- Banners de prioridad (crítico, advertencia, ok).

#### 👤 Cuenta (Admin)

- Perfil con avatar (iniciales o foto subida).
- **Subida de foto de perfil** con preview instantáneo, validación de tipo y tamaño (máx 4 MB).
- Guardado en `~/Content/img/admin/` y persistencia en `Session["AdminFoto"]`.
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
- **Modo oscuro** persistente en cliente y admin.
- **Zoom** persistente para accesibilidad.
- **Logo adaptativo** según tema claro/oscuro (`logo-cnfl.png` / `logo-cnfl-dark.png`).
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
| **Excel** | EPPlus 4.5.3.3 (con gráficos nativos) |
| **PDF** | iTextSharp 5.5.13 + System.Drawing (gráficos como imagen) |

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
│   │   ├── logo-cnfl.png
│   │   ├── logo-cnfl-dark.png    # Logo para tema oscuro
│   │   └── admin/                # Fotos de perfil del admin
│   ├── bootstrap-grid.css
│   ├── bootstrap-grid.css.map
│   ├── bootstrap-grid.min.css
│   ├── bootstrap-grid.min.css.map
│   ├── bootstrap-grid.rtl.css
│   └── Site.css
│
├── Controllers/
│   ├── AdminController.cs
│   ├── AdminNotificacionesController.cs
│   ├── AdminReportesController.cs
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
│   ├── UsuarioRol.cs
│   │
│   └── ViewModels/
│       ├── ReporteClienteViewModel.cs    # Reporte unificado (Admin)
│       └── ReporteVentasViewModel.cs     # KPIs de ventas
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
│   │   ├── Alertas.cshtml
│   │   ├── Averias.cshtml
│   │   ├── Clientes.cshtml
│   │   ├── Configuracion.cshtml
│   │   ├── Cuenta.cshtml            # Con subida de foto
│   │   ├── Dashboard.cshtml
│   │   ├── DetalleAveria.cshtml
│   │   ├── DetalleCliente.cshtml
│   │   ├── Reportes.cshtml          # Rediseñado, dashboard unificado
│   │   └── Tramites.cshtml
│   ├── AdminNotificaciones/
│   │   └── Index.cshtml             # Enviar notificaciones + historial
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