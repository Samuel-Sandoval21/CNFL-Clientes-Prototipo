// ==========================================
// REPORTES FORM - JAVASCRIPT CON ENVÍO REAL
// ==========================================

var ubicacionGPS = null;
var archivosSeleccionados = [];

function obtenerUbicacionReporte() {
    var texto = document.getElementById('ubicacionTexto');
    if (!texto) return;

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

function enviarReporte(tipo) {
    var titulo = document.getElementById('tituloAveria');
    var direccion = document.getElementById('direccionAveria');
    var descripcion = document.getElementById('descripcionAveria');
    var fotoInput = document.getElementById('fileInput');
    var tipoProblema = document.getElementById('tipoProblema');

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

    if (errores.length > 0) {
        alert('❌ Por favor, corrige los siguientes errores:\n\n' + errores.join('\n'));
        return;
    }

    var btn = document.querySelector('.btn-enviar-reporte');
    var textoOriginal = btn.innerHTML;

    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Enviando...';
    btn.disabled = true;

    // Obtener NISE del usuario desde la sesión (se pasa en la vista)
    var nise = document.querySelector('.nise-automatico') ?
        document.querySelector('.nise-automatico').textContent.trim().replace(/[^0-9]/g, '') :
        (document.getElementById('modalNise') ? document.getElementById('modalNise').value : '');

    var averia = {
        Titulo: titulo.value.trim(),
        Direccion: direccion.value.trim(),
        Descripcion: descripcion.value.trim(),
        Tipo: tipo,
        Estado: 'Reportado',
        NISE: nise, // <-- CORREGIDO: NISE en mayúsculas
        FotoUrl: tieneFoto ? fotoInput.files[0].name : null,
        Latitud: ubicacionGPS ? ubicacionGPS.split(',')[0] : null,
        Longitud: ubicacionGPS ? ubicacionGPS.split(',')[1] : null
    };

    if (tipo === 'alumbrado') {
        var numeroPoste = document.getElementById('numeroPoste');
        if (numeroPoste && numeroPoste.value) {
            averia.Descripcion += '\n📌 Número de poste: ' + numeroPoste.value;
        }
    }

    if (tipo === 'ajena') {
        var quienReporta = document.querySelector('input[name="quienReporta"]:checked');
        if (quienReporta) {
            var nombres = { 'yo': 'Yo mismo', 'vecino': 'Un vecino', 'otro': 'Otro' };
            averia.Descripcion += '\n👤 Quien reporta: ' + (nombres[quienReporta.value] || quienReporta.value);
        }
    }

    $.ajax({
        url: '/Clientes/RegistrarAveria',
        type: 'POST',
        data: JSON.stringify(averia),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                alert('✅ ' + response.message + '\n\n🔢 Número de seguimiento: #REP-' + response.id);
                setTimeout(function () {
                    window.location.href = '/Clientes/Reportes';
                }, 1500);
            } else {
                alert('❌ ' + response.message);
                btn.innerHTML = textoOriginal;
                btn.disabled = false;
            }
        },
        error: function () {
            alert('❌ Error al enviar el reporte. Intente nuevamente.');
            btn.innerHTML = textoOriginal;
            btn.disabled = false;
        }
    });
}