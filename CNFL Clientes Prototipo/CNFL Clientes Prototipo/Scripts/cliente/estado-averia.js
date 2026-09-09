// ============================================================
// ESTADO-AVERIA.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Estado de Avería cargado');

    // ===== Auto-refresh de estado (cada 30 segundos) =====
    var autoRefresh = document.getElementById('autoRefresh');
    if (autoRefresh) {
        setInterval(function () {
            console.log('🔄 Actualizando estado en tiempo real...');
            // Aquí iría la lógica AJAX para actualizar el estado
            // Por ahora solo simulamos
            var badges = document.querySelectorAll('.estado-badge');
            var progressSteps = document.querySelectorAll('.track .step');
            // Simular cambio de estado después de 30s
        }, 30000);
    }

    // ===== Botón para imprimir =====
    var btnImprimir = document.getElementById('btnImprimir');
    if (btnImprimir) {
        btnImprimir.addEventListener('click', function () {
            window.print();
        });
    }

    // ===== Botón para compartir =====
    var btnCompartir = document.getElementById('btnCompartir');
    if (btnCompartir) {
        btnCompartir.addEventListener('click', function () {
            if (navigator.share) {
                navigator.share({
                    title: 'Estado de Avería CNFL',
                    text: 'Consulta el estado de mi avería',
                    url: window.location.href
                }).catch(function () { });
            } else {
                navigator.clipboard.writeText(window.location.href).then(function () {
                    mostrarToast('✅ Enlace copiado al portapapeles', 'success');
                }).catch(function () {
                    mostrarToast('❌ No se pudo copiar el enlace', 'error');
                });
            }
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