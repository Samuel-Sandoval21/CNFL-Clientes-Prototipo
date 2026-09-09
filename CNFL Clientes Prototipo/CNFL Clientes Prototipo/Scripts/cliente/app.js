// ============================================================
// APP.JS - FUNCIONES GLOBALES PARA APP MÓVIL
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('App Móvil CNFL cargada');

    // ===== Reloj =====
    actualizarReloj();
    setInterval(actualizarReloj, 10000);

    function actualizarReloj() {
        var ahora = new Date();
        var horas = String(ahora.getHours()).padStart(2, '0');
        var minutos = String(ahora.getMinutes()).padStart(2, '0');
        var reloj = document.getElementById('clock');
        if (reloj) reloj.textContent = horas + ':' + minutos;
    }

    // ===== Cerrar alertas =====
    document.querySelectorAll('.cerrar-alerta').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var alerta = this.closest('.alerta');
            if (alerta) alerta.style.display = 'none';
        });
    });

    // ===== Toast =====
    window.mostrarToast = function (mensaje, tipo) {
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
    };

    // ===== Badge notificaciones =====
    window.actualizarBadge = function (cantidad) {
        document.querySelectorAll('.badge').forEach(function (badge) {
            if (cantidad > 0) {
                badge.textContent = cantidad;
                badge.style.display = 'grid';
            } else {
                badge.style.display = 'none';
            }
        });
    };
});