// ==========================================
// CLIENTE - TRÁMITES
// ==========================================

// ==========================================================
// ===== SELECCIONAR TRÁMITE =====
// ==========================================================
function seleccionarTramite(tipo) {
    var nombres = {
        'cambio_nombre': 'Cambio de nombre de abonado',
        'desconexion_reconexion': 'Desconexión y reconexión',
        'solicitud_servicio': 'Solicitud de servicio nuevo',
        'traslado_medidor': 'Traslado de medidor',
        'traspaso_servicio': 'Traspaso de servicio eléctrico',
        'reclamo_danos': 'Reclamo por daños'
    };

    var nombre = nombres[tipo] || tipo;

    if (confirm('¿Desea iniciar el trámite "' + nombre + '"?')) {
        // Redirigir al formulario correspondiente
        window.location.href = '/Clientes/ReportarPropia?tramite=' + tipo;
    }
}

// ==========================================================
// ===== MOSTRAR TOAST =====
// ==========================================================
function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastNotif';
        toast.style.cssText = 'position:fixed;bottom:100px;left:50%;transform:translateX(-50%);background:#2E7D32;color:white;padding:12px 24px;border-radius:16px;font-weight:600;font-size:14px;box-shadow:0 8px 30px rgba(0,0,0,0.2);z-index:2000;display:none;max-width:90%;';
        document.body.appendChild(toast);
    }

    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.style.background = tipo === 'error' ? '#C62828' : '#2E7D32';
    toast.style.display = 'block';

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.display = 'none';
    }, 3000);
}

// ==========================================================
// ===== INICIALIZAR EVENTOS =====
// ==========================================================
document.addEventListener('DOMContentLoaded', function () {
    console.log('📋 Trámites cargados correctamente.');

    // ==========================================================
    // Efecto de entrada para las tarjetas
    // ==========================================================
    var cards = document.querySelectorAll('.tramite-card, .row-item');
    cards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(10px)';
        card.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100 + (index * 60));
    });
});

// Exponer funciones globalmente
window.seleccionarTramite = seleccionarTramite;
window.mostrarToast = mostrarToast;