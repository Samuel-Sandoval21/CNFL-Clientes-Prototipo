// ==========================================
// CHAT / ASISTENTE - IA SUPER INTELIGENTE
// ==========================================

var respuestasIA = {
    'factura': function () {
        return '💳 ¡Claro! Puedes pagar tu factura de varias formas:\n\n' +
            '1️⃣ **SINPE Móvil** - Desde la app de tu banco\n' +
            '2️⃣ **IBAN** - Transferencia bancaria\n' +
            '3️⃣ **Tarjeta** - Crédito o débito\n' +
            '4️⃣ **En línea** - Desde la sección Pagos\n\n' +
            '📱 También puedes pagar desde la sección Pagos de esta app.\n' +
            '📅 Recuerda que el vencimiento es el 28 de cada mes.';
    },
    'averia': function () {
        return '⚡ Para reportar una avería, sigue estos pasos:\n\n' +
            '1️⃣ Ve a la sección **Reportes** en el menú inferior\n' +
            '2️⃣ Selecciona el tipo de avería:\n' +
            '   • 💡 Alumbrado público (Foto + número de poste)\n' +
            '   • 🏠 Eléctrica propia (En tu NISE registrado)\n' +
            '   • 📍 Eléctrica ajena (Sube fotos y GPS)\n' +
            '3️⃣ Completa el formulario y envía\n\n' +
            '📩 Recibirás notificaciones del seguimiento de tu reporte.';
    },
    'consumo': function () {
        return '📊 Tu consumo mensual lo puedes ver en:\n\n' +
            '🔹 **Sección Inicio** - Tarjeta "Consulta al medidor"\n' +
            '🔹 Datos disponibles:\n' +
            '   • 📊 Última lectura: 742 kWh\n' +
            '   • 📈 Acumulado del mes: 198 kWh\n' +
            '   • 💰 Costo estimado: ₡21.300\n' +
            '   • 📉 Promedio mensual: 205 kWh\n\n' +
            '✅ ¡Buen ahorro! 12% menos que el mes anterior.';
    },
    'suspension': function () {
        return '🔔 Las suspensiones programadas se notifican con 48 horas de anticipación.\n\n' +
            '📌 **Suspensión activa:**\n' +
            '• NISE 7788 · mañana 8:00–10:00am\n' +
            '• Motivo: Mantenimiento programado\n\n' +
            '📱 Puedes consultar el estado en la sección **Alertas**.\n' +
            '📞 Para más información, llama al 800-ENERGÍA.';
    },
    'pago': function () {
        return '💳 Métodos de pago disponibles:\n\n' +
            '1️⃣ **SINPE Móvil**\n' +
            '   📱 Teléfono: 8888-8888\n\n' +
            '2️⃣ **IBAN**\n' +
            '   🏦 Cuenta: CR1234567890\n\n' +
            '3️⃣ **Tarjeta**\n' +
            '   💳 Crédito o débito (Visa/Mastercard)\n\n' +
            '4️⃣ **Factura eléctrica**\n' +
            '   📄 Se carga automáticamente en tu factura\n\n' +
            '📲 También puedes pagar desde la sección **Pagos** de la app.';
    },
    'contrato': function () {
        return '📋 Para contratar un nuevo servicio:\n\n' +
            '1️⃣ Ve a la sección **Trámites**\n' +
            '2️⃣ Selecciona "Solicitud de servicio nuevo"\n' +
            '3️⃣ Completa el formulario con tus datos\n' +
            '4️⃣ Adjunta los documentos requeridos:\n' +
            '   • 📄 Copia de la cédula\n' +
            '   • 📄 Comprobante de domicilio\n' +
            '   • 📄 NISE (si ya tienes uno)\n\n' +
            '📩 Recibirás la respuesta en 48 horas hábiles.';
    },
    'default': function () {
        return '🤖 Gracias por tu consulta. Estos son los temas que puedo resolver:\n\n' +
            '• 💳 **Factura** - Pago, vencimiento, métodos\n' +
            '• ⚡ **Avería** - Reporte, seguimiento, tipos\n' +
            '• 📊 **Consumo** - Lecturas, costos, ahorro\n' +
            '• 🔔 **Suspensión** - Programadas, estado\n' +
            '• 💰 **Pagos** - Métodos, transferencias\n' +
            '• 📋 **Contrato** - Nuevo servicio, requisitos\n\n' +
            '📞 También puedes llamarnos al **800-ENERGÍA** (800-363-7442)';
    }
};

var chatHistorial = [];

// ===== ENVIAR MENSAJE =====
function enviarMensaje() {
    var input = document.getElementById('chatInput');
    var mensaje = input.value.trim();

    if (mensaje === '') return;

    agregarMensajeUsuario(mensaje);
    input.value = '';

    chatHistorial.push({ rol: 'usuario', mensaje: mensaje });

    mostrarTyping();

    setTimeout(function () {
        var respuesta = obtenerRespuestaIA(mensaje);
        agregarMensajeBot(respuesta);
        chatHistorial.push({ rol: 'bot', mensaje: respuesta });

        var suggestions = document.getElementById('quickSuggestions');
        if (suggestions && chatHistorial.length > 2) {
            suggestions.style.display = 'none';
        }
    }, 600 + Math.random() * 500);
}

// ===== PREGUNTA RÁPIDA =====
function preguntaRapida(tipo) {
    var preguntas = {
        'factura': '💳 ¿Cómo puedo pagar mi factura?',
        'averia': '⚡ ¿Cómo reporto una avería?',
        'consumo': '📊 ¿Cómo veo mi consumo?',
        'suspension': '🔔 ¿Cuándo hay suspensiones?',
        'pago': '💳 ¿Qué métodos de pago tengo disponibles?',
        'contrato': '📋 ¿Cómo contrato un nuevo servicio?'
    };

    var mensaje = preguntas[tipo] || 'Hola';
    document.getElementById('chatInput').value = mensaje;
    enviarMensaje();
}

// ===== AGREGAR MENSAJE DE USUARIO =====
function agregarMensajeUsuario(mensaje) {
    var chatBox = document.getElementById('chatBox');
    var div = document.createElement('div');
    div.className = 'chat-message user';
    div.innerHTML = `
        <div class="avatar-user"><i class="fas fa-user"></i></div>
        <div class="message-bubble user-bubble">${escapeHtml(mensaje)}</div>
    `;
    chatBox.appendChild(div);
    chatBox.scrollTop = chatBox.scrollHeight;
}

// ===== AGREGAR MENSAJE DE BOT =====
function agregarMensajeBot(mensaje) {
    var chatBox = document.getElementById('chatBox');
    var div = document.createElement('div');
    div.className = 'chat-message bot';
    div.innerHTML = `
        <div class="avatar-bot"><i class="fas fa-robot"></i></div>
        <div class="message-bubble bot-bubble">${formatRespuesta(mensaje)}</div>
    `;
    chatBox.appendChild(div);
    chatBox.scrollTop = chatBox.scrollHeight;
}

// ===== MOSTRAR TYPING =====
function mostrarTyping() {
    var chatBox = document.getElementById('chatBox');
    var div = document.createElement('div');
    div.className = 'chat-message bot';
    div.id = 'typingIndicator';
    div.innerHTML = `
        <div class="avatar-bot"><i class="fas fa-robot"></i></div>
        <div class="message-bubble bot-bubble">
            <div class="typing-indicator">
                <span></span><span></span><span></span>
            </div>
        </div>
    `;
    chatBox.appendChild(div);
    chatBox.scrollTop = chatBox.scrollHeight;
}

// ===== OCULTAR TYPING =====
function ocultarTyping() {
    var indicator = document.getElementById('typingIndicator');
    if (indicator) {
        indicator.remove();
    }
}

// ===== OBTENER RESPUESTA IA =====
function obtenerRespuestaIA(mensaje) {
    var msg = mensaje.toLowerCase();

    // Detectar palabras clave con prioridad
    var palabrasClave = {
        'factura': ['factura', 'pagar', 'pago', 'sinpe', 'iban', 'tarjeta', 'vencimiento', 'monto', 'cobro'],
        'averia': ['avería', 'averia', 'reporte', 'reparar', 'falla', 'problema', 'daño', 'daño', 'luz', 'poste'],
        'consumo': ['consumo', 'kwh', 'lectura', 'medidor', 'ami', 'costo', 'ahorro', 'kilovatio'],
        'suspension': ['suspensión', 'suspension', 'corte', 'apagón', 'sin luz', 'mantenimiento'],
        'pago': ['pago', 'transferencia', 'método', 'banco', 'cuenta', 'depósito', 'sinpe', 'iban'],
        'contrato': ['contrato', 'contratar', 'servicio nuevo', 'alta', 'solicitud', 'trámite']
    };

    var mejorCoincidencia = null;
    var maxPuntaje = 0;

    for (var tema in palabrasClave) {
        var puntaje = 0;
        var palabras = palabrasClave[tema];
        for (var i = 0; i < palabras.length; i++) {
            if (msg.includes(palabras[i])) {
                puntaje++;
            }
        }
        if (puntaje > maxPuntaje) {
            maxPuntaje = puntaje;
            mejorCoincidencia = tema;
        }
    }

    if (mejorCoincidencia && maxPuntaje > 0) {
        return respuestasIA[mejorCoincidencia]();
    }

    return respuestasIA['default']();
}

// ===== FORMATO DE RESPUESTA =====
function formatRespuesta(texto) {
    return texto
        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
        .replace(/\n/g, '<br>');
}

// ===== ESCAPE HTML =====
function escapeHtml(text) {
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// ===== ENTER PARA ENVIAR =====
document.addEventListener('DOMContentLoaded', function () {
    var input = document.getElementById('chatInput');
    if (input) {
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                enviarMensaje();
            }
        });
    }
});