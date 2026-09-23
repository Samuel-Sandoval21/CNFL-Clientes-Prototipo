using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CNFL_Clientes_Prototipo.Services
{
    /// <summary>
    /// Servicio centralizado de envío de correos.
    /// Usado desde ClientesController y AdminNotificacionesController.
    /// Tipografía oficial: AvenirNextLPro (con fallback para clientes de correo).
    /// </summary>
    public static class EmailService
    {
        /// <summary>
        /// Envía un correo de notificación CNFL con diseño HTML profesional.
        /// </summary>
        public static bool Enviar(string destinatario, string asunto, string titulo, string mensajeHtml, string nombreDestinatario = "Cliente")
        {
            System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
            System.Diagnostics.Debug.WriteLine("[EmailService] INICIO");
            System.Diagnostics.Debug.WriteLine("  → Para: " + destinatario);
            System.Diagnostics.Debug.WriteLine("  → Asunto: " + asunto);
            System.Diagnostics.Debug.WriteLine("  → Nombre: " + nombreDestinatario);

            if (string.IsNullOrWhiteSpace(destinatario))
            {
                System.Diagnostics.Debug.WriteLine("  ✗ Destinatario vacío - CANCELADO");
                return false;
            }

            try
            {
                var smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                var smtpPortStr = ConfigurationManager.AppSettings["SmtpPort"] ?? "587";
                var smtpUser = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
                var smtpPass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";
                var smtpFrom = ConfigurationManager.AppSettings["SmtpFrom"] ?? smtpUser;
                var smtpFromNombre = ConfigurationManager.AppSettings["SmtpFromNombre"] ?? "CNFL Clientes";
                var enableSsl = (ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true") == "true";

                System.Diagnostics.Debug.WriteLine("  ─── CONFIGURACIÓN SMTP ───");
                System.Diagnostics.Debug.WriteLine("  → Host: " + smtpHost);
                System.Diagnostics.Debug.WriteLine("  → Port: " + smtpPortStr);
                System.Diagnostics.Debug.WriteLine("  → User: " + smtpUser);
                System.Diagnostics.Debug.WriteLine("  → From: " + smtpFrom);
                System.Diagnostics.Debug.WriteLine("  → FromNombre: " + smtpFromNombre);
                System.Diagnostics.Debug.WriteLine("  → SSL: " + enableSsl);
                System.Diagnostics.Debug.WriteLine("  → Pass length: " + (smtpPass != null ? smtpPass.Length.ToString() : "0"));
                if (!string.IsNullOrEmpty(smtpPass) && smtpPass.Length > 4)
                {
                    System.Diagnostics.Debug.WriteLine("  → Pass preview: " + smtpPass.Substring(0, 4) + "****");
                }

                // Validación de credenciales placeholder
                if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass)
                    || smtpUser.StartsWith("CAMBIAR_POR_")
                    || smtpPass.StartsWith("CAMBIAR_POR_"))
                {
                    System.Diagnostics.Debug.WriteLine("  ✗ SMTP no configurado (placeholders sin cambiar)");
                    System.Diagnostics.Debug.WriteLine("  ✗ Revisá Web.config: SmtpUser, SmtpPass, SmtpFrom");
                    System.Diagnostics.Debug.WriteLine("[EmailService] FIN CANCELADO");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    return false;
                }

                var smtpPort = int.Parse(smtpPortStr);

                using (var smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.EnableSsl = enableSsl;
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.Timeout = 20000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;

                    var mail = new MailMessage
                    {
                        From = new MailAddress(smtpFrom, smtpFromNombre),
                        Subject = asunto,
                        IsBodyHtml = true,
                        Body = ConstruirHtml(titulo, mensajeHtml, nombreDestinatario),
                        BodyEncoding = System.Text.Encoding.UTF8,
                        SubjectEncoding = System.Text.Encoding.UTF8
                    };
                    mail.To.Add(destinatario);

                    System.Diagnostics.Debug.WriteLine("  ─── ENVIANDO ───");
                    System.Diagnostics.Debug.WriteLine("  → Llamando smtp.Send()...");

                    smtp.Send(mail);

                    System.Diagnostics.Debug.WriteLine("  ✓ smtp.Send() completado SIN ERROR");
                    System.Diagnostics.Debug.WriteLine("  ✓ Correo aceptado por el servidor SMTP");
                    System.Diagnostics.Debug.WriteLine("  ⚠ IMPORTANTE: Si no llega, revisá SPAM del destinatario");
                    System.Diagnostics.Debug.WriteLine("[EmailService] FIN EXITOSO");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                System.Diagnostics.Debug.WriteLine("  ✗✗✗ ERROR SMTP ✗✗✗");
                System.Diagnostics.Debug.WriteLine("  → StatusCode: " + smtpEx.StatusCode);
                System.Diagnostics.Debug.WriteLine("  → Message: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                    System.Diagnostics.Debug.WriteLine("  → InnerException: " + smtpEx.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("  → StackTrace: " + smtpEx.StackTrace);
                System.Diagnostics.Debug.WriteLine("[EmailService] FIN CON ERROR SMTP");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("  ✗✗✗ ERROR GENERAL ✗✗✗");
                System.Diagnostics.Debug.WriteLine("  → Tipo: " + ex.GetType().Name);
                System.Diagnostics.Debug.WriteLine("  → Message: " + ex.Message);
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine("  → InnerException: " + ex.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("  → StackTrace: " + ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("[EmailService] FIN CON ERROR");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                return false;
            }
        }

        /// <summary>
        /// HTML bonito y responsive del correo con tipografía AvenirNextLPro.
        /// NOTA: Los clientes de correo (Outlook, Gmail, etc.) bloquean @font-face
        /// por seguridad. Por eso se incluye la fuente declarada + fallbacks robustos.
        /// </summary>
        private static string ConstruirHtml(string titulo, string mensajeHtml, string nombre)
        {
            var tituloEncoded = HttpUtility.HtmlEncode(titulo);
            var nombreEncoded = HttpUtility.HtmlEncode(nombre);

            // Definir la familia tipográfica como variable para reutilizar
            var fontFamily = "'AvenirNextLPro', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif";

            return @"
<!DOCTYPE html>
<html lang='es'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>CNFL</title>
<style>
    /* Fuente oficial CNFL con fallbacks robustos para clientes de correo */
    body, table, td, div, p, h1, h2, h3, a, span, strong, em {
        font-family: 'AvenirNextLPro', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif !important;
    }
</style>
</head>
<body style='margin:0; padding:0; background:#eef0f5; font-family: " + fontFamily + @";'>

<table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background:#eef0f5; padding:32px 12px; font-family: " + fontFamily + @";'>
<tr>
<td align='center'>

<!-- CARD PRINCIPAL -->
<table role='presentation' width='600' cellpadding='0' cellspacing='0' border='0' style='max-width:600px; width:100%; background:#ffffff; border-radius:20px; overflow:hidden; box-shadow:0 8px 32px rgba(16,20,40,0.10); font-family: " + fontFamily + @";'>

<!-- HEADER -->
<tr>
<td style='background: linear-gradient(135deg, #001482 0%, #1E23E6 60%, #3b6ce0 100%); padding:40px 32px; text-align:center; position:relative; font-family: " + fontFamily + @";'>
<div style='color:#ffffff; font-size:32px; font-weight:900; letter-spacing:-1px; margin:0; line-height:1; font-family: " + fontFamily + @";'>CNFL</div>
<div style='color:rgba(255,255,255,0.75); font-size:11px; font-weight:700; margin-top:8px; letter-spacing:3px; font-family: " + fontFamily + @";'>AGENCIA VIRTUAL</div>
</td>
</tr>

<!-- BODY -->
<tr>
<td style='padding:40px 32px; font-family: " + fontFamily + @";'>

<!-- BADGE -->
<div style='margin-bottom:20px; font-family: " + fontFamily + @";'>
<span style='display:inline-block; background:#eef0ff; color:#1E23E6; font-size:11px; font-weight:800; padding:7px 14px; border-radius:999px; letter-spacing:0.5px; font-family: " + fontFamily + @";'>
NUEVA NOTIFICACIÓN
</span>
</div>

<!-- TÍTULO -->
<h1 style='color:#0E1116; font-size:22px; font-weight:800; margin:0 0 20px; line-height:1.35; letter-spacing:-0.3px; font-family: " + fontFamily + @";'>" + tituloEncoded + @"</h1>

<!-- MENSAJE -->
<div style='color:#4a5361; font-size:15px; line-height:1.7; margin:0 0 28px; font-family: " + fontFamily + @";'>
" + mensajeHtml + @"
</div>

<!-- DIVIDER -->
<div style='height:1px; background:#EDEFF3; margin:0 0 24px;'></div>

<!-- SALUDO -->
<p style='color:#727A86; font-size:13px; line-height:1.6; margin:0 0 24px; font-family: " + fontFamily + @";'>
Hola <strong style='color:#0E1116; font-weight:800; font-family: " + fontFamily + @";'>" + nombreEncoded + @"</strong>, este aviso te llega porque tenés activadas las notificaciones en CNFL Clientes.
</p>

<!-- BOTÓN CTA -->
<table role='presentation' cellpadding='0' cellspacing='0' border='0' style='margin:0 auto; font-family: " + fontFamily + @";'>
<tr>
<td align='center' style='background: linear-gradient(135deg, #FF692D 0%, #ff8a4c 100%); border-radius:14px; font-family: " + fontFamily + @";'>
<a href='http://localhost:44387/Clientes/Alertas' target='_blank' style='display:inline-block; color:#ffffff; text-decoration:none; padding:16px 36px; font-size:15px; font-weight:800; letter-spacing:0.2px; font-family: " + fontFamily + @";'>
Ver todas las alertas
</a>
</td>
</tr>
</table>

</td>
</tr>

<!-- FOOTER -->
<tr>
<td style='background:#f8f9fc; padding:28px 32px; text-align:center; border-top:1px solid #EDEFF3; font-family: " + fontFamily + @";'>
<p style='color:#0E1116; font-size:12px; font-weight:800; margin:0 0 6px; letter-spacing:0.3px; font-family: " + fontFamily + @";'>Compañía Nacional de Fuerza y Luz S.A.</p>
<p style='color:#9aa3b2; font-size:11px; margin:0 0 14px; line-height:1.6; font-family: " + fontFamily + @";'>
<a href='https://www.cnfl.go.cr' style='color:#1E23E6; text-decoration:none; font-weight:700; font-family: " + fontFamily + @";'>www.cnfl.go.cr</a>
&nbsp;·&nbsp; 800-ENERGIA (800-3637442)
</p>
<p style='color:#b5bcc9; font-size:10.5px; margin:0; line-height:1.6; font-family: " + fontFamily + @";'>
Si no querés recibir estas notificaciones,<br>desactivalas desde la app CNFL Clientes.
</p>
</td>
</tr>

</table>

<!-- COPYRIGHT -->
<p style='color:#9aa3b2; font-size:11px; text-align:center; margin:20px 0 0; line-height:1.6; font-family: " + fontFamily + @";'>
© " + DateTime.Now.Year + @" Compañía Nacional de Fuerza y Luz S.A.<br>
Todos los derechos reservados.
</p>

</td>
</tr>
</table>

</body>
</html>";
        }
    }
}