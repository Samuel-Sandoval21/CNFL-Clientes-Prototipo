// ==========================================
// NOTIFICACIONES - JAVASCRIPT
// ==========================================

// ===== MARCAR NOTIFICACIÓN COMO LEÍDA =====
function marcarLeida(elemento, id) {
    var card = elemento.closest('.notificacion-item');
    if (card) {
        card.style.opacity = '0.5';
        card.style.transition = 'opacity 0.3s ease';

        // Actualizar badge
        var badge = document.querySelector('.icon-btn .badge');
        if (badge) {
            var count = parseInt(badge.textContent) || 0;
            if (count > 0) {
                badge.textContent = count - 1;
                if (badge.textContent === '0') {
                    badge.style.display = 'none';
                }
            }
        }

        // Mostrar mensaje
        mostrarNotificacion('✅ Notificación marcada como leída');
    }
}

// ===== MARCAR TODAS COMO LEÍDAS =====
function marcarTodasLeidas() {
    var items = document.querySelectorAll('.notificacion-item');
    if (items.length === 0) return;

    if (!confirm('¿Marcar todas las notificaciones como leídas?')) return;

    items.forEach(function (item) {
        item.style.opacity = '0.5';
        item.style.transition = 'opacity 0.3s ease';
    });

    var badge = document.querySelector('.icon-btn .badge');
    if (badge) {
        badge.textContent = '0';
        badge.style.display = 'none';
    }

    mostrarNotificacion('✅ Todas las notificaciones marcadas como leídas');
}

// ===== MOSTRAR NOTIFICACIÓN =====
function mostrarNotificacion(mensaje) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    toast.textContent = mensaje;
    toast.style.cssText = `
        position: fixed;
        bottom: 100px;
        left: 50%;
        transform: translateX(-50%);
        background: #2E7D32;
        color: white;
        padding: 12px 24px;
        border-radius: 16px;
        font-weight: 600;
        font-size: 14px;
        box-shadow: 0 8px 30px rgba(0,0,0,0.2);
        z-index: 2000;
        animation: slideUp 0.3s ease;
        max-width: 90%;
    `;
    document.body.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(function () {
            document.body.removeChild(toast);
        }, 300);
    }, 3000);
}