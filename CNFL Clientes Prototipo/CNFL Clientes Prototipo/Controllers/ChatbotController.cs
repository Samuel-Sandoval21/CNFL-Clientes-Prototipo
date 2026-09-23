using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IAiService _aiService;

        public ChatbotController()
        {
            _aiService = new AiService();
        }

        [HttpPost]
        public async Task<JsonResult> EnviarMensaje(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return Json(new { success = false, respuesta = "Por favor, escribí tu pregunta." });
            }

            try
            {
                var respuesta = await _aiService.ProcessAsync(prompt);
                return Json(new { success = true, respuesta = respuesta });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    respuesta = "Lo siento, ocurrió un error al procesar tu mensaje. Intentá de nuevo."
                });
            }
        }
    }
}