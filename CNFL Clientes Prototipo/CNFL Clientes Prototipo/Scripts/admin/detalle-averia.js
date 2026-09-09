// ============================================================
// ADMIN DETALLE-AVERIA.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Detalle Avería cargado');

    // ===== Botón cambiar estado =====
    var btnCambiarEstado = document.getElementById('btnCambiarEstado');
    if (btnCambiarEstado) {
        btnCambiarEstado.addEventListener('click', function () {
            var averiaId = this.dataset.id || '0';
            var nuevoEstado = prompt('Ingrese el nuevo estado:', '');
            if (nuevoEstado && nuevoEstado.trim() !== '') {
                mostrarToast('⏳ Actualizando estado a: ' + nuevoEstado, 'info');
                setTimeout(function () {
                    mostrarToast('✅ Estado actualizado correctamente', 'success');
                    // Aquí iría la llamada AJAX real
                    // location.reload();
                }, 1500);
            }
        });
    }

    // ===== Botón imprimir =====
    var btnImprimir = document.getElementById('btnImprimir');
    if (btnImprimir) {
        btnImprimir.addEventListener('click', function () {
            window.print();
        });
    }

    // ===== Botón volver =====
    var btnVolver = document.querySelector('.btn-volver');
    if (btnVolver) {
        btnVolver.addEventListener('click', function (e) {
            mostrarToast('⬅️ Volviendo a lista de averías', 'info');
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