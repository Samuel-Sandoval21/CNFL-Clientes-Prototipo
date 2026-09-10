using System;
using System.Configuration;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CNFL_Clientes_Prototipo.Data;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Services
{
    /// <summary>
    /// Servicio centralizado para envío de notificaciones multicanal:
    ///   - Email (SMTP)
    ///   - SMS (proveedor HTTP configurable: Twilio, Vonage, etc.)
    ///   - Push (Firebase Cloud Messaging HTTP v1)
    ///   - Persistencia en la tabla Notificaciones
    ///
    /// Configuración en Web.config (appSettings):
    ///   - Smtp:Host, Smtp:Port, Smtp:User, Smtp:Password, Smtp:From, Smtp:EnableSsl
    ///   - Sms:Endpoint, Sms:ApiKey, Sms:From
    ///   - Fcm:ServerKey, Fcm:ProjectId
    /// </summary>
    public class NotificationService
    {
        private readonly CNFLDbContext _db;

        // ===== CONSTRUCTOR =====
        public NotificationService()
        {
            _db = new CNFLDbContext();
        }

        public NotificationService(CNFLDbContext db)
        {
            _db = db;
        }

        // ===== MÉTODO PRINCIPAL =====
        /// <summary>
        /// Envía una notificación por uno o varios canales y la persiste en BD.
        /// Los canales disponibles se definen en el parámetro 'canales'.
        /// </summary>
        /// <param name="usuarioId">ID del usuario destinatario</param>
        /// <param name="titulo">Título corto (máx 100 chars)</param>
        /// <param name="mensaje">Mensaje completo</param>
        /// <param name="tipo">Categoría: "Averia" | "Pago" | "Factura" | "Sistema"</param>
        /// <param name="canales">Array con: "push", "email", "sms"</param>
        /// <returns>Resultado con éxito/fallo por canal</returns>
        public async Task<NotificacionResultado> EnviarAsync(
            int usuarioId,
            string titulo,
            string mensaje,
            string tipo = "Sistema",
            string[] canales = null)
        {
            var resultado = new NotificacionResultado();
            canales = canales ?? new[] { "push" }; // default: solo push

            // 1) Persistir SIEMPRE en BD (para que aparezca en MisNotificaciones)
            try
            {
                var notif = new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = tipo
                };
                _db.Notificaciones.Add(notif);
                await _db.SaveChangesAsync();
                resultado.IdBd = notif.NotificacionId;
            }
            catch (Exception ex)
            {
                resultado.Errores.Add("BD: " + ex.Message);
            }

            // 2) Obtener datos del usuario (email, teléfono)
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
            if (usuario == null)
            {
                resultado.Errores.Add("Usuario no encontrado: " + usuarioId);
                return resultado;
            }

            // 3) Enviar por cada canal solicitado
            foreach (var canal in canales)
            {
                try
                {
                    switch (canal.ToLower())
                    {
                        case "email":
                            if (!string.IsNullOrWhiteSpace(usuario.Correo))
                                await EnviarEmailAsync(usuario.Correo, titulo, mensaje);
                            resultado.Enviados.Add("email");
                            break;

                        case "sms":
                            if (!string.IsNullOrWhiteSpace(usuario.Telefono))
                                await EnviarSmsAsync(usuario.Telefono, titulo + ": " + mensaje);
                            resultado.Enviados.Add("sms");
                            break;

                        case "push":
                            // Requiere que el dispositivo tenga un token FCM registrado.
                            // Por ahora solo lo dejamos registrado en logs.
                            await EnviarPushAsync(usuarioId, titulo, mensaje);
                            resultado.Enviados.Add("push");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add($"{canal}: {ex.Message}");
                }
            }

            return resultado;
        }

        // ===== EMAIL (SMTP) =====
        private async Task EnviarEmailAsync(string destinatario, string asunto, string cuerpo)
        {
            var host = ConfigurationManager.AppSettings["Smtp:Host"];
            var port = int.Parse(ConfigurationManager.AppSettings["Smtp:Port"] ?? "587");
            var user = ConfigurationManager.AppSettings["Smtp:User"];
            var pass = ConfigurationManager.AppSettings["Smtp:Password"];
            var from = ConfigurationManager.AppSettings["Smtp:From"] ?? "no-reply@cnfl.go.cr";
            var ssl = bool.Parse(ConfigurationManager.AppSettings["Smtp:EnableSsl"] ?? "true");

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = ssl;
                client.Credentials = new NetworkCredential(user, pass);

                var mail = new MailMessage
                {
                    From = new MailAddress(from, "CNFL"),
                    Subject = "[CNFL] " + asunto,
                    Body = ConstruirHtmlEmail(asunto, cuerpo),
                    IsBodyHtml = true
                };
                mail.To.Add(destinatario);

                await client.SendMailAsync(mail);
            }
        }

        private string ConstruirHtmlEmail(string titulo, string mensaje)
        {
            return $@"
            <!DOCTYPE html>
            <html><head><meta charset='utf-8'></head>
            <body style='font-family:Montserrat,Arial,sans-serif;background:#F5F6FA;padding:24px;margin:0;'>
              <div style='max-width:520px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 14px rgba(0,0,0,.06);'>
                <div style='background:linear-gradient(135deg,#001482,#1E23E6);color:#fff;padding:20px;text-align:center;'>
                  <div style='font-size:22px;font-weight:800;'>CNFL</div>
                  <div style='font-size:12px;opacity:.85;margin-top:4px;'>Compañía Nacional de Fuerza y Luz</div>
                </div>
                <div style='padding:24px;'>
                  <h2 style='color:#0E1116;font-size:18px;margin:0 0 12px;'>{WebUtility.HtmlEncode(titulo)}</h2>
                  <p style='color:#4A5568;font-size:14px;line-height:1.6;margin:0;'>{WebUtility.HtmlEncode(mensaje)}</p>
                </div>
                <div style='background:#F5F6FA;padding:16px;text-align:center;font-size:11px;color:#727A86;'>
                  Este es un mensaje automático de CNFL · www.cnfl.go.cr
                </div>
              </div>
            </body></html>";
        }

        // ===== SMS (HTTP genérico, compatible con Twilio/Vonage) =====
        private async Task EnviarSmsAsync(string destinatario, string mensaje)
        {
            var endpoint = ConfigurationManager.AppSettings["Sms:Endpoint"];
            var apiKey = ConfigurationManager.AppSettings["Sms:ApiKey"];
            var from = ConfigurationManager.AppSettings["Sms:From"] ?? "CNFL";

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                // ⚠️ Sin endpoint configurado: solo log, no falla.
                System.Diagnostics.Debug.WriteLine($"[SMS SIMULADO] Para {destinatario}: {mensaje}");
                return;
            }

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                // Payload genérico compatible con Twilio-like APIs
                var payload = new
                {
                    to = destinatario,
                    from = from,
                    message = mensaje
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await http.PostAsync(endpoint, content);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    throw new Exception($"SMS API {resp.StatusCode}: {body}");
                }
            }
        }

        // ===== PUSH (Firebase Cloud Messaging HTTP v1) =====
        private async Task EnviarPushAsync(int usuarioId, string titulo, string mensaje)
        {
            var serverKey = ConfigurationManager.AppSettings["Fcm:ServerKey"];
            if (string.IsNullOrWhiteSpace(serverKey))
            {
                // ⚠️ Sin FCM configurado: solo log.
                System.Diagnostics.Debug.WriteLine($"[PUSH SIMULADO] Usuario {usuarioId}: {titulo} - {mensaje}");
                return;
            }

            // ⚠️ Aquí en producción habría que:
            // 1) Buscar el token FCM del dispositivo del usuario en una tabla Dispositivos
            // 2) Enviar el push con el token
            // Por ahora, solo log.
            System.Diagnostics.Debug.WriteLine($"[PUSH] Usuario {usuarioId}: {titulo}");
            await Task.CompletedTask;
        }

        // ===== DISPOSE =====
        public void Dispose()
        {
            _db?.Dispose();
        }
    }

    // ===== RESULTADO =====
    public class NotificacionResultado
    {
        public int IdBd { get; set; }
        public System.Collections.Generic.List<string> Enviados { get; set; } = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> Errores { get; set; } = new System.Collections.Generic.List<string>();
        public bool Exito => Errores.Count == 0;
    }
}