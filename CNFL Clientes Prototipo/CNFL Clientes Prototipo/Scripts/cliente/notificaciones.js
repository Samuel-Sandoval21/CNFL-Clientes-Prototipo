// ============================================================
// NOTIFICACIONES.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Notificaciones cargado');

    // ===== Click en notificación =====
    document.querySelectorAll('.notificacion-item').forEach(function (item) {
        item.addEventListener('click', function () {
            var titulo = this.querySelector('h4')?.textContent || 'Notificación';
            var mensaje = this.querySelector('p')?.textContent || '';
            mostrarToast('📬 ' + titulo + ': ' + mensaje, 'info');

            // Marcar como leída
            this.classList.remove('no-leida');
            var badge = this.querySelector('.badge-nueva');
            if (badge) {
                badge.style.display = 'none';
            }
            // Actualizar contador
            actualizarBadge();
        });
    });

    // ===== Marcar todas como leídas =====
    var btnMarcarTodas = document.getElementById('marcarTodas');
    if (btnMarcarTodas) {
        btnMarcarTodas.addEventListener('click', function () {
            document.querySelectorAll('.notificacion-item.no-leida').forEach(function (item) {
                item.classList.remove('no-leida');
                var badge = item.querySelector('.badge-nueva');
                if (badge) {
                    badge.style.display = 'none';
                }
            });
            actualizarBadge();
            mostrarToast('✅ Todas las notificaciones marcadas como leídas', 'success');
        });
    }

    // ===== Contar y actualizar badge =====
    actualizarBadge();

    function actualizarBadge() {
        var noLeidas = document.querySelectorAll('.notificacion-item.no-leida').length;
        var badges = document.querySelectorAll('.badge');
        badges.forEach(function (badge) {
            if (noLeidas > 0) {
                badge.textContent = noLeidas;
                badge.style.display = 'grid';
            } else {
                badge.style.display = 'none';
            }
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