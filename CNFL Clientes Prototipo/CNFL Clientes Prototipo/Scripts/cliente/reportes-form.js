// ==========================================
// REPORTES FORM - JAVASCRIPT PREMIUM
// ==========================================

// ===== VARIABLES GLOBALES =====
var ubicacionGPS = null;
var archivosSeleccionados = [];

// ===== OBTENER UBICACIÓN =====
function obtenerUbicacionReporte() {
    var texto = document.getElementById('ubicacionTexto');

    if (navigator.geolocation) {
        texto.textContent = '⏳ Obteniendo ubicación...';
        texto.classList.add('show');
        texto.style.background = '#f7f8fc';
        texto.style.color = 'var(--muted)';
        texto.style.border = '1px solid var(--line)';

        navigator.geolocation.getCurrentPosition(
            function (pos) {
                var lat = pos.coords.latitude.toFixed(6);
                var lng = pos.coords.longitude.toFixed(6);
                ubicacionGPS = lat + ',' + lng;

                texto.textContent = '📍 Ubicación obtenida: ' + lat + ', ' + lng;
                texto.style.background = '#E8F5E9';
                texto.style.color = '#2E7D32';
                texto.style.border = '1px solid #A5D6A7';
            },
            function (err) {
                var mensajes = {
                    1: 'Permiso denegado',
                    2: 'Posición no disponible',
                    3: 'Tiempo de espera agotado'
                };
                ubicacionGPS = null;
                texto.textContent = '⚠️ ' + (mensajes[err.code] || 'Error al obtener ubicación') + '. Verifique los permisos.';
                texto.style.background = '#FFEBEE';
                texto.style.color = '#C62828';
                texto.style.border = '1px solid #FFCDD2';
            }
        );
    } else {
        texto.textContent = '⚠️ Tu navegador no soporta geolocalización.';
        texto.classList.add('show');
        texto.style.background = '#FFEBEE';
        texto.style.color = '#C62828';
        texto.style.border = '1px solid #FFCDD2';
    }
}

// ===== DROPZONE =====
document.addEventListener('DOMContentLoaded', function () {
    var dropzone = document.getElementById('dropzone');
    if (!dropzone) return;

    var fileInput = document.getElementById('fileInput');

    dropzone.addEventListener('click', function () {
        fileInput.click();
    });

    dropzone.addEventListener('dragover', function (e) {
        e.preventDefault();
        this.classList.add('dragover');
    });

    dropzone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        this.classList.remove('dragover');
    });

    dropzone.addEventListener('drop', function (e) {
        e.preventDefault();
        this.classList.remove('dragover');

        if (e.dataTransfer.files.length > 0) {
            fileInput.files = e.dataTransfer.files;
            actualizarNombreArchivo(fileInput.files[0].name);
            archivosSeleccionados = Array.from(e.dataTransfer.files);
        }
    });

    fileInput.addEventListener('change', function () {
        if (this.files.length > 0) {
            actualizarNombreArchivo(this.files[0].name);
            archivosSeleccionados = Array.from(this.files);
        }
    });
});

function actualizarNombreArchivo(nombre) {
    var texto = document.getElementById('dropText');
    if (texto) {
        texto.textContent = '📎 ' + nombre;
        texto.style.color = 'var(--blue)';
        texto.style.fontWeight = '600';
    }
    var sub = document.getElementById('dropSub');
    if (sub) {
        sub.textContent = 'Archivo seleccionado correctamente';
        sub.style.color = 'var(--green)';
    }
}

// ===== ENVIAR REPORTE =====
function enviarReporte(tipo) {
    var titulo = document.getElementById('tituloAveria');
    var direccion = document.getElementById('direccionAveria');
    var descripcion = document.getElementById('descripcionAveria');
    var fotoInput = document.getElementById('fileInput');
    var tipoProblema = document.getElementById('tipoProblema');

    // Validaciones
    var errores = [];

    if (!titulo || !titulo.value.trim()) {
        errores.push('⚠️ Ingresa un título para la avería.');
        if (titulo) titulo.focus();
    }

    if (!direccion || !direccion.value.trim()) {
        errores.push('⚠️ Ingresa la dirección.');
        if (direccion) direccion.focus();
    }

    if (!descripcion || !descripcion.value.trim()) {
        errores.push('⚠️ Describe el problema.');
        if (descripcion) descripcion.focus();
    }

    if (tipoProblema && (!tipoProblema.value || tipoProblema.value === '')) {
        errores.push('⚠️ Selecciona un tipo de problema.');
        if (tipoProblema) tipoProblema.focus();
    }

    var tieneFoto = fotoInput && fotoInput.files && fotoInput.files.length > 0;
    if (!tieneFoto) {
        if (!confirm('⚠️ No has adjuntado ninguna foto.\n\n¿Deseas continuar sin foto?')) {
            return;
        }
    }

    if (errores.length > 0) {
        alert('❌ Por favor, corrige los siguientes errores:\n\n' + errores.join('\n'));
        return;
    }

    var nombresTipo = {
        'alumbrado': 'Alumbrado Público',
        'propia': 'Eléctrica Propia',
        'ajena': 'Eléctrica Ajena'
    };

    var btn = document.querySelector('.btn-enviar-reporte');
    var textoOriginal = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Enviando...';
    btn.disabled = true;

    // Obtener datos adicionales según tipo
    var datosAdicionales = '';

    if (tipo === 'alumbrado') {
        var numeroPoste = document.getElementById('numeroPoste');
        if (numeroPoste && numeroPoste.value) {
            datosAdicionales += '\n📌 Número de poste: ' + numeroPoste.value;
        }
    }

    if (tipo === 'propia') {
        datosAdicionales += '\n📋 NISE: 4021 · Casa';
    }

    if (tipo === 'ajena') {
        var quienReporta = document.querySelector('input[name="quienReporta"]:checked');
        if (quienReporta) {
            var nombres = { 'yo': 'Yo mismo', 'vecino': 'Un vecino', 'otro': 'Otro' };
            datosAdicionales += '\n👤 Quien reporta: ' + (nombres[quienReporta.value] || quienReporta.value);
        }
    }

    // Simular envío
    setTimeout(function () {
        var mensaje = '✅ ¡Reporte enviado exitosamente!\n\n';
        mensaje += '📋 Tipo: ' + (nombresTipo[tipo] || tipo) + '\n';
        mensaje += '📍 Título: ' + titulo.value + '\n';
        mensaje += '📌 Dirección: ' + direccion.value + '\n';
        mensaje += '📎 Foto: ' + (tieneFoto ? '✅ Adjuntada (' + fotoInput.files[0].name + ')' : '❌ No adjuntada') + '\n';
        mensaje += '📱 Ubicación: ' + (ubicacionGPS || 'No disponible') + datosAdicionales + '\n\n';
        mensaje += '🔢 Número de seguimiento: #REP-' + Date.now().toString().slice(-6) + '\n';
        mensaje += '📩 Recibirás notificaciones del estado de tu reporte.';

        alert(mensaje);

        btn.innerHTML = textoOriginal;
        btn.disabled = false;

        // Redirigir a reportes
        setTimeout(function () {
            window.location.href = '/Clientes/Reportes';
        }, 2000);
    }, 2000);
}