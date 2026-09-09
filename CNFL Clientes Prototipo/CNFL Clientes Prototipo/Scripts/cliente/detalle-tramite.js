// ============================================================
// DETALLE-TRAMITE.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Detalle Trámite cargado');

    // ===== Botón para cancelar trámite =====
    var btnCancelar = document.getElementById('btnCancelarTramite');
    if (btnCancelar) {
        btnCancelar.addEventListener('click', function () {
            var tramiteId = this.dataset.id || '0';
            if (confirm('¿Estás seguro de que deseas cancelar este trámite?\nEsta acción no se puede deshacer.')) {
                mostrarToast('⏳ Cancelando trámite...', 'warning');
                setTimeout(function () {
                    mostrarToast('✅ Trámite cancelado correctamente', 'success');
                }, 2000);
            }
        });
    }

    // ===== Botón para imprimir =====
    var btnImprimir = document.getElementById('btnImprimir');
    if (btnImprimir) {
        btnImprimir.addEventListener('click', function () {
            window.print();
        });
    }

    // ===== Botón para descargar PDF =====
    var btnDescargar = document.getElementById('btnDescargar');
    if (btnDescargar) {
        btnDescargar.addEventListener('click', function () {
            var textoOriginal = this.textContent;
            this.textContent = '⏳ Generando...';
            this.disabled = true;
            setTimeout(function () {
                mostrarToast('✅ PDF generado correctamente', 'success');
                btnDescargar.textContent = textoOriginal;
                btnDescargar.disabled = false;
            }, 2000);
        });
    }

    // ===== Mostrar más información =====
    var btnMasInfo = document.getElementById('btnMasInfo');
    if (btnMasInfo) {
        btnMasInfo.addEventListener('click', function () {
            mostrarToast('📞 Contacta al 800-ENERGIA (800-3637442)', 'info');
        });
    }
});

function mostrarToast(mensaje, tipo) {
    tipo = tipo || 'info';
    var toast = document.getElementById('toastGlobal');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastGlobal';
        toast.style.cssText = 'position:fixed; bottom:90px; left:50%; transform:translateX(-50%); padding:12px 24px; border-radius:12px; font-weight:700; z-index:9999; background:#0E1116; color:white; box-shadow:0 8px 24px rgba(0,0,0,0.2); opacity:0; transition:opacity 0.3s; max-width:90%; text-align:center;';
        document.body.appendChild(toast);
    }

    toast.textContent = mensaje;
    toast.style.opacity = '1';

    var colores = {
        success: '#2E7D32',
        error: '#D32F2F',
        warning: '#F5A623',
        info: '#0E1116'
    };
    toast.style.background = colores[tipo] || colores.info;

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.opacity = '0';
    }, 3000);
}