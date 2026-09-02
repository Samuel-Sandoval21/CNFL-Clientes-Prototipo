// ==========================================
// CLIENTE - NOTIFICACIONES
// ==========================================

function marcarLeida(element, id) {
    // Si el elemento es un botón, obtener el contenedor padre
    var item = element.closest ? element.closest('.notificacion-item') : null;
    if (!item) {
        // Si no se pasó elemento o es el botón, buscar por data-id
        item = document.querySelector('.notificacion-item[data-id="' + id + '"]');
    }

    // Realizar petición AJAX
    $.ajax({
        url: '/Clientes/MarcarNotificacionLeida',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                // Marcar visualmente
                if (item) {
                    item.classList.add('leida');
                    var btnLeer = item.querySelector('.btn-leer');
                    if (btnLeer) {
                        btnLeer.textContent = '✓ Leída';
                        btnLeer.disabled = true;
                        btnLeer.style.opacity = '0.5';
                    }
                }

                // Actualizar badge
                actualizarBadge();

                mostrarToast('✅ Notificación marcada como leída');
            } else {
                mostrarToast('❌ ' + response.message, 'error');
            }
        },
        error: function () {
            mostrarToast('❌ Error al marcar notificación', 'error');
        }
    });
}

function marcarTodasLeidas() {
    $.ajax({
        url: '/Clientes/MarcarTodasLeidas',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                // Marcar todas visualmente
                document.querySelectorAll('.notificacion-item:not(.leida)').forEach(function (item) {
                    item.classList.add('leida');
                    var btnLeer = item.querySelector('.btn-leer');
                    if (btnLeer) {
                        btnLeer.textContent = '✓ Leída';
                        btnLeer.disabled = true;
                        btnLeer.style.opacity = '0.5';
                    }
                });

                actualizarBadge();
                mostrarToast('✅ Todas las notificaciones marcadas como leídas');
            } else {
                mostrarToast('❌ ' + response.message, 'error');
            }
        },
        error: function () {
            mostrarToast('❌ Error al marcar notificaciones', 'error');
        }
    });
}

function actualizarBadge() {
    var badge = document.getElementById('notificacionBadge');
    if (!badge) return;

    var noLeidas = document.querySelectorAll('.notificacion-item:not(.leida)');
    var count = noLeidas.length;

    if (count > 0) {
        badge.textContent = count;
        badge.style.display = 'inline-block';
    } else {
        badge.style.display = 'none';
    }
}

function mostrarToast(mensaje, tipo) {
    // Crear toast si no existe
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

// Exponer funciones globalmente
window.marcarLeida = marcarLeida;
window.marcarTodasLeidas = marcarTodasLeidas;
window.mostrarToast = mostrarToast;