using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CNFL_Clientes_Prototipo.Services
{
    public class AiService : IAiService
    {
        private static readonly HttpClient _http = new HttpClient();

        public async Task<string> ProcessAsync(string prompt)
        {
            var apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
            var model = ConfigurationManager.AppSettings["OpenAI_Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "El asistente no está configurado. Contactá al administrador.";
            }

            var systemPrompt = @"
Sos el asistente virtual de CNFL (Compañía Nacional de Fuerza y Luz) de Costa Rica.
Tu función es ayudar a los clientes con consultas sobre:
- Facturas, pagos, tarifas y consumos
- Reportes de averías y cortes de luz
- Trámites y servicios disponibles
- Horarios de atención, sucursales y canales de contacto
- Trámites en línea (cambios de titular, reconexiones, etc.)
- Información general sobre CNFL

Respondé siempre en español costarricense, de forma clara, amable y concisa.
Si no sabés algo, decí que no tenés esa información y sugerí contactar al 800-ENERGIA (800-363-7442) o visitar cnfl.go.cr.
NO inventes información. NO des consejos legales ni médicos.
Si el usuario pregunta algo fuera del ámbito de CNFL, redirigí amablemente la conversación a temas de la empresa.
";

            var requestBody = new
            {
                model = model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = prompt }
                },
                temperature = 0.5,
                max_tokens = 500
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _http.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                return "Lo siento, no pude procesar tu consulta en este momento.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseJson);

            try
            {
                return (string)data.choices[0].message.content;
            }
            catch
            {
                return "Recibí una respuesta inesperada del asistente.";
            }
        }
    }
}