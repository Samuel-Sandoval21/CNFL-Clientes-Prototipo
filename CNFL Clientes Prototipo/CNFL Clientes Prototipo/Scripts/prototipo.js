/* ==========================================
   PROTOTIPO CNFL - LÓGICA JAVASCRIPT GLOBAL
   ========================================== */

/* ==========================================
   1. GRÁFICO DE CONSUMO CON ETIQUETAS (Canvas)
   ========================================== */
function dibujarGraficoConsumo(periodo) {
    var canvas = document.getElementById('consumoChart');
    if (!canvas) return;

    var ctx = canvas.getContext('2d');
    var width = canvas.width;  // Usa width del atributo HTML
    var height = canvas.height; // Usa height del atributo HTML

    ctx.clearRect(0, 0, width, height);

    var etiquetas = [];
    var data = [];
    var colores = [];

    if (periodo === '6m') {
        etiquetas = ["Mar", "Abr", "May", "Jun", "Jul", "Ago"]; // Iniciales de meses
        data = [45, 62, 53, 71, 58, 84];
        colores = ["#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD", "#1E23E6"];
    } else if (periodo === '31d') {
        etiquetas = ["S1", "S2", "S3", "S4", "S5"]; // Semanas
        data = [70, 45, 60, 80, 55];
        colores = ["#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD"];
    } else if (periodo === 'lecturas') {
        etiquetas = ["L1", "L2", "L3", "L4", "L5", "L6"]; // Lecturas manuales
        data = [100, 80, 90, 60, 50, 70];
        colores = ["#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD", "#64B9CD", "#1E23E6"];
    }

    // Configuración de dibujo
    var barWidth = 30;
    var gap = 30;
    var startX = 30;
    var baseY = height - 35; // Deja espacio para etiquetas
    var maxData = Math.max.apply(null, data);

    // Dibujar barras y etiquetas
    for (var i = 0; i < data.length; i++) {
        var barHeight = (data[i] / maxData) * (baseY - 20);
        var x = startX + i * (barWidth + gap);
        var y = baseY - barHeight;

        ctx.fillStyle = colores[i];
        ctx.beginPath();
        ctx.roundRect(x, y, barWidth, barHeight, 4);
        ctx.fill();

        // Etiqueta de la barra (inicial del mes o semana)
        ctx.fillStyle = "#727A86";
        ctx.font = "10px Montserrat";
        ctx.textAlign = "center";
        ctx.fillText(etiquetas[i], x + barWidth / 2, baseY + 15);
    }
}

/* ==========================================
   2. FUNCIÓN PARA CAMBIAR PERIODO DEL GRÁFICO
   ========================================== */
function cambiarPeriodo(periodo) {
    // Actualizar clases activas en los chips
    document.querySelectorAll('#chartTabs .chip').forEach(function (chip) {
        chip.classList.remove('active');
        if (chip.dataset.periodo === periodo) {
            chip.classList.add('active');
        }
    });

    // Redibujar gráfico
    dibujarGraficoConsumo(periodo);
}

/* ==========================================
   3. LLAMADA INICIAL (Se ejecuta al cargar la página)
   ========================================== */
document.addEventListener('DOMContentLoaded', function () {
    console.log('Prototipo CNFL - RespuestaTCU cargado.');

    // Si existe el canvas, dibujar el gráfico inicial
    if (document.getElementById('consumoChart')) {
        dibujarGraficoConsumo('6m');
    }
});

/* ==========================================
   4. FUNCIÓN PARA OBTENER UBICACIÓN (GPS)
   ========================================== */
function obtenerUbicacion() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {
            var texto = document.getElementById('ubicacionTexto');
            if (texto) {
                texto.innerHTML = "📍 Ubicación obtenida: " + pos.coords.latitude.toFixed(4) + ", " + pos.coords.longitude.toFixed(4);
            }
        }, function () {
            var texto = document.getElementById('ubicacionTexto');
            if (texto) {
                texto.innerHTML = "⚠️ No se pudo acceder a la ubicación. Verifique los permisos.";
            }
        });
    }
}

/* ==========================================
   5. FUNCIÓN PARA CALCULAR CONSUMO
   ========================================== */
function calcularConsumo() {
    var kwh = parseFloat(document.getElementById("kwh").value) || 0;
    var costo = kwh * 95;

    var resultado = document.getElementById("resultado");
    if (resultado) {
        resultado.style.display = "block";
        resultado.innerHTML = "El costo aproximado es: <strong>₡" + costo.toLocaleString() + "</strong>";
    }
}

/* ==========================================
   6. FUNCIÓN PARA EL CHAT
   ========================================== */
function enviarMensajeChat() {
    var input = document.getElementById("chatInput").value;
    if (input.trim() === "") return;

    var chatBox = document.getElementById("chatBox");
    if (chatBox) {
        chatBox.innerHTML += '<div style="background:#1E23E6; color:#fff; border-radius:10px; padding:8px; margin-bottom:8px; text-align:right;">' + input + '</div>';
        document.getElementById("chatInput").value = "";

        setTimeout(function () {
            chatBox.innerHTML += '<div style="background:#fff; border-radius:10px; padding:8px; margin-bottom:8px;">Gracias por tu consulta. Un agente te atenderá en breve.</div>';
            chatBox.scrollTop = chatBox.scrollHeight;
        }, 800);
    }
}

/* ==========================================
   7. LÓGICA PARA LA VISTA DE FACTURAS (REDIRECCIÓN)
   ========================================== */
function pagarFactura(metodo, monto, periodo) {
    var url = '/Pagos/' + metodo + '?monto=' + monto + '&producto=' + encodeURIComponent(periodo);
    window.location.href = url;
}