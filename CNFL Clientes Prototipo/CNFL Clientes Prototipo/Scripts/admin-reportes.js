// ==========================================
// ADMIN REPORTES - JAVASCRIPT
// ==========================================

// ===== GENERAR REPORTE =====
function generarReporte() {
    var periodo = document.getElementById('periodoFilter').value;
    var fechaInicio = document.getElementById('fechaInicio').value;
    var fechaFin = document.getElementById('fechaFin').value;

    var btn = document.querySelector('.btn-generar');
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generando...';
    btn.disabled = true;

    $.ajax({
        url: '/Admin/GenerarReporte',
        type: 'POST',
        data: {
            periodo: periodo,
            fechaInicio: fechaInicio,
            fechaFin: fechaFin
        },
        success: function (response) {
            mostrarToast(response.message || '✅ Reporte generado correctamente');
            setTimeout(function () {
                location.reload();
            }, 1000);
        },
        error: function () {
            mostrarToast('❌ Error al generar el reporte', 'error');
        },
        complete: function () {
            btn.innerHTML = '<i class="fas fa-sync-alt"></i> Generar Reporte';
            btn.disabled = false;
        }
    });
}

// ===== EXPORTAR REPORTE (PDF, EXCEL, CSV) =====
function exportarReporte(formato) {
    var btn = event.target.closest('button');
    var originalText = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Exportando...';
    btn.disabled = true;

    var url = '';
    switch (formato) {
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
            url = '/Admin/ExportarCSV';
    }

    // Abrir en nueva ventana/pestaña para descarga
    window.open(url, '_blank');

    var mensaje = '';
    switch (formato) {
        case 'pdf': mensaje = '📄 Exportando archivo PDF'; break;
        case 'excel': mensaje = '📊 Exportando archivo Excel'; break;
        case 'csv': mensaje = '📋 Exportando archivo CSV'; break;
        default: mensaje = '📥 Exportando archivo';
    }
    mostrarToast(mensaje);

    setTimeout(function () {
        btn.innerHTML = originalText;
        btn.disabled = false;
    }, 2000);
}

// ===== MOSTRAR TOAST =====
function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.classList.add('show');
    setTimeout(function () {
        toast.classList.remove('show');
    }, 3000);
}