// ==========================================
// ADMIN - REPORTES Y ESTADÍSTICAS
// ==========================================

function generarReporte() {
    var periodo = document.getElementById('periodoFilter');
    var fechaInicio = document.getElementById('fechaInicio');
    var fechaFin = document.getElementById('fechaFin');

    var data = {
        periodo: periodo ? periodo.value : '30d',
        fechaInicio: fechaInicio ? fechaInicio.value : '',
        fechaFin: fechaFin ? fechaFin.value : ''
    };

    // Mostrar loading en el botón
    var btn = document.querySelector('.btn-generar');
    if (btn) {
        var originalText = btn.innerHTML;
        btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generando...';
        btn.disabled = true;

        $.ajax({
            url: '/Admin/GenerarReporte',
            type: 'POST',
            data: data,
            success: function (response) {
                if (response.success) {
                    mostrarToast(response.message);
                    // Actualizar datos si es necesario
                    actualizarEstadisticas(response);
                } else {
                    mostrarToast(response.message || '❌ Error al generar reporte', 'error');
                }
            },
            error: function () {
                mostrarToast('❌ Error de conexión', 'error');
            },
            complete: function () {
                btn.innerHTML = originalText;
                btn.disabled = false;
            }
        });
    }
}

function actualizarEstadisticas(data) {
    // Actualizar tarjetas de resumen
    var totalCard = document.querySelector('.report-card .number');
    if (totalCard) {
        // Solo actualizar si hay datos
    }
}

function exportarReporte(tipo) {
    var url = '';

    switch (tipo) {
        case 'pdf':
            url = '/Admin/ExportarPDF';
            break;
        case 'excel':
            url = '/Admin/ExportarExcel';
            break;
        case 'csv':
            url = '/Admin/ExportarCSV';
            break;
        default:
            return;
    }

    // Abrir en nueva ventana para descarga
    window.open(url, '_blank');
}

function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    if (!toast) return;

    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.style.display = 'block';

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.display = 'none';
    }, 3000);
}

// Evento para mostrar/ocultar fechas personalizadas
document.addEventListener('DOMContentLoaded', function () {
    var periodoSelect = document.getElementById('periodoFilter');
    var fechaInicio = document.getElementById('fechaInicio');
    var fechaFin = document.getElementById('fechaFin');

    if (periodoSelect) {
        periodoSelect.addEventListener('change', function () {
            if (this.value === 'personalizado') {
                if (fechaInicio) fechaInicio.style.display = 'inline-block';
                if (fechaFin) fechaFin.style.display = 'inline-block';
            } else {
                if (fechaInicio) fechaInicio.style.display = 'none';
                if (fechaFin) fechaFin.style.display = 'none';
            }
        });
    }

    if (fechaInicio) fechaInicio.style.display = 'none';
    if (fechaFin) fechaFin.style.display = 'none';
});

// Exponer funciones globalmente
window.generarReporte = generarReporte;
window.exportarReporte = exportarReporte;
window.mostrarToast = mostrarToast;