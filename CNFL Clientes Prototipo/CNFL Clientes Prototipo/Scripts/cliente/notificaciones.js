// ==========================================
// NOTIFICACIONES - JAVASCRIPT COMPLETO
// ==========================================

// ==========================================================
// ===== VARIABLES GLOBALES =====
// ==========================================================
var notificacionesLeidas = [];

// ==========================================================
// ===== CARGAR NOTIFICACIONES LEÍDAS DE SESSION =====
// ==========================================================
function cargarNotificacionesLeidas() {
    // Intentar obtener de SessionStorage (persistente)
    var stored = sessionStorage.getItem('notificacionesLeidas');
    if (stored) {
        try {
            notificacionesLeidas = JSON.parse(stored);
            return notificacionesLeidas;
        } catch (e) {
            notificacionesLeidas = [];
        }
    }

    // Si no hay en sessionStorage, intentar obtener de la sesión del servidor
    var leidasElement = document.getElementById('notificacionesLeidas');
    if (leidasElement) {
        try {
            var leidas = JSON.parse(leidasElement.value);
            notificacionesLeidas = leidas;
            sessionStorage.setItem('notificacionesLeidas', JSON.stringify(leidas));
            return notificacionesLeidas;
        } catch (e) {
            notificacionesLeidas = [];
        }
    }

    notificacionesLeidas = [];
    return notificacionesLeidas;
}

// ==========================================================
// ===== GUARDAR NOTIFICACIONES LEÍDAS =====
// ==========================================================
function guardarNotificacionesLeidas() {
    sessionStorage.setItem('notificacionesLeidas', JSON.stringify(notificacionesLeidas));

    // Actualizar badge
    actualizarBadge();

    // Actualizar contador de notificaciones en el header
    actualizarContadorHeader();
}

// ==========================================================
// ===== ACTUALIZAR BADGE =====
// ==========================================================
function actualizarBadge() {
    var items = document.querySelectorAll('.notificacion-item');
    var badge = document.getElementById('notificacionBadge');
    if (!badge) return;

    var pendientes = items.length;
    badge.textContent = pendientes;
    badge.style.display = pendientes > 0 ? 'inline-flex' : 'none';
}

// ==========================================================
// ===== ACTUALIZAR CONTADOR EN HEADER =====
// ==========================================================
function actualizarContadorHeader() {
    var headerBadge = document.querySelector('.icon-btn .badge');
    if (!headerBadge) return;

    var pendientes = document.querySelectorAll('.notificacion-item').length;
    headerBadge.textContent = pendientes;
    headerBadge.style.display = pendientes > 0 ? 'grid' : 'none';
}

// ==========================================================
// ===== MARCAR NOTIFICACIÓN COMO LEÍDA =====
// ==========================================================
function marcarLeida(elemento, id) {
    var card = elemento.closest ? elemento.closest('.notificacion-item') : elemento;
    if (!card) return;

    var idNotificacion = card.dataset.id || id;

    // Verificar si ya está leída
    if (notificacionesLeidas.includes(parseInt(idNotificacion))) {
        // Si ya está en la lista, simplemente ocultar
        ocultarNotificacion(card);
        return;
    }

    // Agregar a la lista de leídas
    notificacionesLeidas.push(parseInt(idNotificacion));
    guardarNotificacionesLeidas();

    // Ocultar la notificación con animación
    ocultarNotificacion(card);

    // Mostrar mensaje
    mostrarToast('✅ Notificación marcada como leída');
}

// ==========================================================
// ===== OCULTAR NOTIFICACIÓN CON ANIMACIÓN =====
// ==========================================================
function ocultarNotificacion(card) {
    card.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
    card.style.opacity = '0';
    card.style.transform = 'translateX(30px)';
    card.style.height = card.offsetHeight + 'px';

    setTimeout(function () {
        card.style.height = '0';
        card.style.margin = '0';
        card.style.padding = '0';
        card.style.overflow = 'hidden';
        card.style.border = 'none';
    }, 400);

    setTimeout(function () {
        card.remove();
        actualizarBadge();
        actualizarContadorHeader();

        // Verificar si no hay más notificaciones
        var items = document.querySelectorAll('.notificacion-item');
        if (items.length === 0) {
            mostrarMensajeVacio();
        }
    }, 800);
}

// ==========================================================
// ===== MARCAR TODAS COMO LEÍDAS =====
// ==========================================================
function marcarTodasLeidas() {
    var items = document.querySelectorAll('.notificacion-item');
    if (items.length === 0) {
        mostrarToast('ℹ️ No hay notificaciones pendientes');
        return;
    }

    if (!confirm('¿Marcar todas las notificaciones como leídas?')) return;

    var ids = [];
    items.forEach(function (item) {
        var id = parseInt(item.dataset.id);
        if (id && !notificacionesLeidas.includes(id)) {
            ids.push(id);
            notificacionesLeidas.push(id);
        }
        ocultarNotificacion(item);
    });

    guardarNotificacionesLeidas();

    if (ids.length > 0) {
        mostrarToast('✅ Todas las notificaciones marcadas como leídas');
    }
}

// ==========================================================
// ===== MOSTRAR MENSAJE VACÍO =====
// ==========================================================
function mostrarMensajeVacio() {
    var container = document.querySelector('.section-pad:last-child');
    if (!container) return;

    // Eliminar botón de marcar todas si existe
    var btnMarcar = container.querySelector('.btn-marcar-todas');
    if (btnMarcar) btnMarcar.remove();

    var empty = document.createElement('div');
    empty.className = 'empty-notificaciones';
    empty.innerHTML = `
        <div class="empty-icon">🎉</div>
        <h3>¡No tienes notificaciones pendientes!</h3>
        <p>Todas tus notificaciones han sido leídas.</p>
        <button class="btn-volver" onclick="location.reload()" style="margin-top:16px;">
            <i class="fas fa-sync"></i> Recargar
        </button>
    `;
    container.appendChild(empty);
}

// ==========================================================
// ===== MOSTRAR TOAST =====
// ==========================================================
function mostrarToast(mensaje) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    toast.textContent = mensaje;
    document.body.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(function () {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 3000);
}

// ==========================================================
// ===== INICIALIZAR =====
// ==========================================================
document.addEventListener('DOMContentLoaded', function () {
    // Cargar notificaciones leídas
    cargarNotificacionesLeidas();
    actualizarBadge();
    actualizarContadorHeader();

    // Ocultar notificaciones que ya están leídas
    var items = document.querySelectorAll('.notificacion-item');
    items.forEach(function (item) {
        var id = parseInt(item.dataset.id);
        if (notificacionesLeidas.includes(id)) {
            item.style.display = 'none';
        }
    });

    // Verificar si no hay notificaciones
    var visibleItems = document.querySelectorAll('.notificacion-item:not([style*="display: none"])');
    if (visibleItems.length === 0 && items.length > 0) {
        mostrarMensajeVacio();
    }

    // Si no hay notificaciones desde el inicio
    var itemsTotal = document.querySelectorAll('.notificacion-item');
    if (itemsTotal.length === 0) {
        mostrarMensajeVacio();
    }

    console.log('🔔 Notificaciones cargadas correctamente');
});