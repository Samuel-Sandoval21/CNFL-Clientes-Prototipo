// ============================================================
// CHAT.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Chat cargado');

    var input = document.getElementById('chatInput');
    var sendBtn = document.getElementById('chatSendBtn');
    var chatBox = document.getElementById('chatBox');
    var quickBtns = document.querySelectorAll('.quick-btn');

    if (!chatBox) return;

    function agregarMensaje(texto, tipo) {
        var div = document.createElement('div');
        div.className = 'chat-message ' + tipo;
        var avatar = document.createElement('span');
        avatar.className = 'chat-avatar';
        avatar.innerHTML = tipo === 'user' ? '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/></svg>' : '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20z"/><path d="M8 12h8"/></svg>';
        var textoMsg = document.createElement('span');
        textoMsg.className = 'chat-text';
        textoMsg.textContent = texto;
        div.appendChild(avatar);
        div.appendChild(textoMsg);
        chatBox.appendChild(div);
        chatBox.scrollTop = chatBox.scrollHeight;
    }

    function responderBot(mensaje) {
        var respuesta = '';
        var lower = mensaje.toLowerCase();

        if (lower.includes('factura') || lower.includes('pagar')) {
            respuesta = 'Tu factura actual es de ₡18,450 y vence el 28 de agosto. ¿Deseas pagarla ahora? 💳';
        } else if (lower.includes('avería') || lower.includes('reportar')) {
            respuesta = 'Para reportar una avería, necesito tu NISE y la dirección del problema. ¿Me los puedes indicar? 📍';
        } else if (lower.includes('trámite') || lower.includes('solicitud')) {
            respuesta = 'Puedes consultar el estado de tus trámites en la sección de Trámites. ¿Necesitas ayuda con algún trámite en específico? 📋';
        } else if (lower.includes('agente') || lower.includes('hablar')) {
            respuesta = 'Te conectaremos con un agente en breve. Por favor espera un momento... 👤';
        } else if (lower.includes('hola') || lower.includes('buenas') || lower.includes('saludos')) {
            respuesta = '¡Hola! ¿Cómo puedo ayudarte hoy? Puedo asistirte con facturas, averías, trámites o información general. 😊';
        } else if (lower.includes('consumo')) {
            respuesta = 'Tu consumo actual es de 742 kWh este mes, un 12% menos que el mes anterior. ¡Buen ahorro! 📊';
        } else {
            respuesta = 'Entendido. Estoy procesando tu solicitud. ¿Necesitas ayuda con facturas, averías, trámites o información general? 🔍';
        }

        setTimeout(function () {
            agregarMensaje(respuesta, 'bot');
        }, 600);
    }

    function enviarMensaje() {
        var texto = input.value.trim();
        if (!texto) return;
        agregarMensaje(texto, 'user');
        input.value = '';
        input.focus();
        responderBot(texto);
    }

    if (sendBtn) {
        sendBtn.addEventListener('click', enviarMensaje);
    }

    if (input) {
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                enviarMensaje();
            }
        });
    }

    quickBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var mensaje = this.dataset.mensaje;
            input.value = mensaje;
            enviarMensaje();
        });
    });
});