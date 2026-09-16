# CNFL Clientes · Prototipo App Móvil

Prototipo funcional de una aplicación móvil para la **Compañía Nacional de Fuerza y Luz (CNFL)** de Costa Rica. Incluye módulo de **Cliente** y módulo de **Administrador**, ambos con diseño tipo app móvil (iOS/Android), notificaciones push reales, envío de correos y persistencia en SQL Server.

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
- Registro con validación de cédula única, correo único y nombre de usuario único.
- Recuperación de contraseña con correo electrónico.
- Login biométrico (Face ID / Huella) preparado.
- Roles: **Cliente** y **Administrador**.

### 👤 Módulo Cliente

#### 🏠 Dashboard (Inicio)
- Saludo dinámico según hora (Buenos días / tardes / noches).
- Hero card con mensaje de bienvenida.
- Facturas pendientes con monto, NISE y fecha de vencimiento.
- Gráfica de consumo kWh con 3 modos: **últimos 6 meses · 31 días · lecturas**.
- Resumen del mes (consumo, promedio, próxima lectura).
- Contactános (teléfono, correo, web, oficinas).

#### 🔌 Mis Servicios
- Lista de NISEs con estado (Normal / Prevención / Incidente).
- Historial de consumo (kWh y ₡).
- Consulta al medidor AMI (última lectura, voltaje, costo estimado).
- Reportes con 3 tipos de avería: alumbrado público, eléctrica propia, eléctrica ajena.
- Mapa GIS con **Leaflet + OpenStreetMap** mostrando:
  - Averías por color según clientes afectados.
  - Estaciones de recarga eléctrica.
  - Puntos verdes.
- Calculadora energética con persistencia en `localStorage`.

#### 📄 Trámites
- **Catálogo de 16 trámites** según diagrama:
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
- **Servicios (Hogar 360°)**: eficiencia energética, alarmas, cargadores eléctricos, domótica, mantenimiento de barras, venta e instalación, consumo verde y DERs.
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
- Edición de datos personales, contacto y ubicación.
- Selects en cascada Provincia → Cantón → Distrito.
- Factura electrónica con checkbox + buscador de actividad económica por código o nombre.
- Seguridad: cambio de contraseña.
- Cerrar sesión.

#### 🔔 Alertas
- **4 categorías** según diagrama:
  1. Averías (Reportada, En proceso, Finalizada + hora estimada)
  2. Suspensiones programadas por NISE (Programada, En Proceso)
  3. Facturas por vencer / Medio de pago vencido
  4. Eventos (Voltaje fuera de rango, consumo fuera de límites)
- Notificaciones push del navegador con **sonido generado por Web Audio API**.
- Envío de correo real vía SMTP.
- Configuración por tipo de alerta.

### 🛠️ Módulo Admin

#### 📊 Dashboard
- Métricas principales: clientes, NISEs, monto pendiente, ingresos del mes.
- Métricas secundarias: averías abiertas/resueltas, trámites abiertos, tiempo promedio.
- Gráficos interactivos con **Chart.js**:
  - Averías por estado (doughnut).
  - Averías últimos 7 días (line).
- Tasa de resolución con barra de progreso.

#### 👥 Clientes
- Búsqueda por nombre, cédula o correo.
- Filtro por rol (Cliente / Admin / Todos).
- Lista mobile con chips de estado.
- Vista de detalle por cliente.

#### ⚡ Averías
- Filtros por estado, tipo y búsqueda.
- Vista de detalle con historial.
- Cambio de estado con notificación automática al cliente.

#### 📋 Trámites
- Filtros por estado y búsqueda.
- Vista con referencia, tipo, cliente y fecha.
- Cambio de estado con notificación al cliente.

#### 📈 Reportes
- Rango de fechas configurable.
- Métricas: facturado, cobrado, averías, trámites.
- Gráficos de facturación por mes y averías por tipo.
- Exportación a PDF y Excel (estructura lista).

#### ⏱️ Actividad de uso
- Log de actividad de los clientes en la app.
- Gráficos por sección y por día.
- Últimos 500 registros.

#### 🔧 Configuración
- Información del sistema (versión, framework, BD).
- Estadísticas generales.

#### ⚙️ Modal de configuración Admin
- Zoom del panel (70% - 140%, persistente).
- Modo oscuro (persistente).
- Notificaciones push del navegador.
- Cerrar sesión con confirmación.

---

## 🌐 Internacionalización

La app incluye soporte para **5 idiomas**:
- 🇨🇷 Español (Costa Rica)
- 🇺🇸 English (US)
- 🇫🇷 Français
- 🇧🇷 Português (BR)
- 🇨🇳 中文 (简体)

El idioma seleccionado se guarda en `localStorage` y se aplica automáticamente a los textos del layout.

---

## 🎨 Diseño

- **Mobile-first**: la app se ve como un celular real en desktop y ocupa toda la pantalla en móvil.
- **Paleta CNFL**:
  - Azul `#1E23E6` (confianza / navegación)
  - Azul oscuro `#001482` (seguridad)
  - Naranja `#FF692D` (CTA / conversión)
  - Verde `#64B95A` (éxito / ahorro)
  - Ámbar `#F5A623` (prevención)
  - Rojo `#E5484D` (incidente)
- **Bottom navigation** en lugar de sidebar (como app móvil real).
- **Modo oscuro** persistente.
- **Zoom** persistente para accesibilidad.
- **Componentes reutilizables**: cards, chips de estado, trackers, modales.

---

## 🏗️ Arquitectura técnica

### Stack
| Capa | Tecnología |
|---|---|
| **Backend** | ASP.NET MVC 5 · .NET Framework 4.8.1 |
| **Base de datos** | SQL Server (LocalDB) |
| **ORM** | Entity Framework 6 |
| **Frontend** | Razor Views · CSS3 · Vanilla JS |
| **Gráficos** | Chart.js 4.4.1 (CDN) |
| **Mapas** | Leaflet 1.9.4 + OpenStreetMap |
| **Notificaciones** | Web Notifications API + Web Audio API |
| **Correo** | SMTP (Gmail / Office 365 / cualquier proveedor) |

### Estructura del proyecto

```
CNFL_Clientes_Prototipo/
├── Controllers/
│   ├── AdminController.cs         # Panel administrador
│   ├── ClientesController.cs      # Módulo cliente
│   ├── CuentaController.cs        # Login/Registro/Recuperar
│   └── HomeController.cs          # Home público
├── Data/
│   └── CNFLDbContext.cs           # DbContext de EF
├── Filters/
│   └── SessionAuthorize.cs        # Filtro de autorización por rol
├── Models/
│   ├── Usuario.cs
│   ├── NISE.cs
│   ├── Factura.cs
│   ├── Averia.cs
│   ├── Tramite.cs
│   ├── Notificacion.cs
│   ├── ActividadUsuario.cs
│   ├── DescargaUsuario.cs
│   ├── DashboardDtos.cs           # DTOs compartidos
│   └── ClienteAdminDto.cs         # DTO para admin
├── Views/
│   ├── Admin/                     # Vistas del admin
│   │   ├── Dashboard.cshtml
│   │   ├── Clientes.cshtml
│   │   ├── Averias.cshtml
│   │   ├── Tramites.cshtml
│   │   ├── Reportes.cshtml
│   │   ├── Actividad.cshtml
│   │   └── Configuracion.cshtml
│   ├── Clientes/                  # Vistas del cliente
│   │   ├── Dashboard.cshtml
│   │   ├── MisServicios.cshtml
│   │   ├── HistorialConsumo.cshtml
│   │   ├── ConsultaMedidor.cshtml
│   │   ├── MapaGIS.cshtml
│   │   ├── Calculadora.cshtml
│   │   ├── Reportes.cshtml
│   │   ├── EstadoAveria.cshtml
│   │   ├── HistorialReportes.cshtml
│   │   ├── Tramites.cshtml
│   │   ├── ProductosServicios.cshtml
│   │   ├── Tienda.cshtml
│   │   ├── DetalleProducto.cshtml
│   │   ├── MisCompras.cshtml
│   │   ├── MetodosPago.cshtml
│   │   ├── Alertas.cshtml
│   │   ├── MisFacturas.cshtml
│   │   └── Cuenta.cshtml
│   ├── Cuenta/                    # Login, Registro, Recuperar
│   │   ├── Login.cshtml
│   │   ├── Registro.cshtml
│   │   └── RecuperarClave.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _EstilosBase.cshtml    # Estilos compartidos
│       ├── _LayoutAdmin.cshtml    # Layout admin (mobile)
│       ├── _LayoutCliente.cshtml  # Layout cliente (mobile)
│       └── _LayoutPublico.cshtml  # Layout público (sin login)
├── Content/
│   ├── img/
│   │   ├── logo-cnfl.png
│   │   └── logo-cnfl.jpg
│   └── uploads/
│       └── perfiles/              # Fotos de perfil subidas
├── Scripts/                        # (Opcional) JS externos
├── Web.config
├── packages.config
└── README.md
```

---

## 🚀 Instalación y ejecución

### Requisitos previos
- **Visual Studio 2022** (Community o superior).
- **.NET Framework 4.8.1**.
- **SQL Server LocalDB** (viene con VS).
- Navegador moderno (Chrome, Edge, Firefox).

### Pasos

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Samuel-Sandoval21/CNFL.git
   cd CNFL
   ```

2. **Abrir la solución**
   - Doble clic en `CNFL_Clientes_Prototipo.sln` o abrir desde VS.

3. **Restaurar paquetes NuGet**
   - Clic derecho en la solución → **Restaurar paquetes NuGet**.

4. **Configurar la base de datos**
   - La cadena de conexión por defecto usa LocalDB:
     ```
     Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CNFL_Clientes;Integrated Security=True
     ```
   - Si la BD no existe, se puede crear ejecutando los scripts SQL (no incluidos) o ajustar la cadena en `Web.config`.

5. **Configurar SMTP** (opcional, para envío de correos)
   - En `Web.config`, dentro de `<appSettings>`:
     ```xml
     <add key="SmtpHost" value="smtp.gmail.com" />
     <add key="SmtpPort" value="587" />
     <add key="SmtpUser" value="tucorreo@gmail.com" />
     <add key="SmtpPass" value="tu-contraseña-de-aplicacion" />
     <add key="SmtpFrom" value="tucorreo@gmail.com" />
     ```
   - **Nota**: Si usás Gmail, necesitás generar una **contraseña de aplicación** (no tu contraseña normal).

6. **Compilar y ejecutar**
   - `Ctrl + Shift + B` → Compilar.
   - `Ctrl + F5` → Ejecutar.

7. **Abrir en el navegador**
   - La app se abre en `http://localhost:44387` (el puerto puede variar).

---

## 👥 Usuarios de prueba

Si cargaste datos de ejemplo en la BD:

| Rol | Cédula | Correo | Contraseña |
|---|---|---|---|
| Cliente | `2-0874-0716` | `ssandoval40716@ufide.ac.cr` | `123456` |
| Admin | `1-1111-1111` | `admin@cnfl.go.cr` | `admin123` |

> ⚠️ Estos son datos de ejemplo. Cambialos según tu base de datos.

---

## 🔐 Seguridad

- **Contraseñas**: almacenadas en texto plano en este prototipo (en producción usar `BCrypt` o `PasswordHasher`).
- **Autorización**: filtro `SessionAuthorize` que valida sesión y rol por controlador.
- **Anti-forgery**: todos los POST usan `@Html.AntiForgeryToken()`.
- **SMTP**: las credenciales se leen desde `Web.config`, no del código.
- **HTTPS**: en producción, forzar HTTPS con `[RequireHttps]`.

---

## 🧪 Pruebas realizadas

- ✅ Login con cédula, correo y nombre de usuario.
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
- ✅ Modo oscuro persistente.
- ✅ Zoom persistente.
- ✅ Cierre de sesión con confirmación.
- ✅ Admin: gestión de clientes, averías y trámites.
- ✅ Admin: exportación de reportes (estructura lista).

---

## 📋 Pendientes / Mejoras futuras

- 🔲 Exportación real a **PDF** con iTextSharp o Rotativa.
- 🔲 Exportación real a **Excel** con EPPlus o ClosedXML.
- 🔲 **Hashing de contraseñas** con BCrypt.
- 🔲 **Firebase Cloud Messaging** para push reales multiplataforma.
- 🔲 **Backend con Supabase / Firebase** para sincronización.
- 🔲 App nativa en **React Native + Expo** (según diagrama del proyecto).
- 🔲 **HTTPS obligatorio** en producción.
- 🔲 **Rate limiting** en endpoints sensibles.
- 🔲 **Logs** con Serilog o NLog.

---

## 📚 Diagrama del proceso

El prototipo sigue el **Diagrama Proceso App CNFL - Propuesto**:

```
Cliente → Servicios → Facturas / Historial / AMI / Reportes / Mapa GIS / Calculadora
                    → Alertas → Push + correo
                    → Trámites → Formulario → Proceso CNFL → Notificar
                    → Productos y Servicios → Servicios / Marketplace / Subscripciones / Historial
                    → Carrito → Métodos de pago → Confirmación

Admin → Dashboard → Métricas + gráficos
      → Clientes → Búsqueda + detalle
      → Averías → Filtros + cambio de estado
      → Trámites → Filtros + cambio de estado
      → Reportes → Rango de fechas + export
      → Actividad → Log de uso por cliente
```

---

## 👨‍💻 Autor

- **Samuel Sandoval Ramírez**
- GitHub: [@Samuel-Sandoval21](https://github.com/Samuel-Sandoval21)

Proyecto desarrollado como **prototipo funcional** para la Compañía Nacional de Fuerza y Luz (CNFL) de Costa Rica.

---

## 📄 Licencia

Este proyecto es un **prototipo académico**. Todos los derechos de la marca CNFL pertenecen a la Compañía Nacional de Fuerza y Luz de Costa Rica.

---

## 🙏 Agradecimientos

- **CNFL** por permitir el desarrollo del prototipo.
- **Katherine Villalobos** y **Daniel Rodríguez** por la revisión del diagrama de proceso.
- **Comunidad de desarrolladores** por las librerías open source (Leaflet, Chart.js, etc.).

---

**Última actualización:** Septiembre 2026