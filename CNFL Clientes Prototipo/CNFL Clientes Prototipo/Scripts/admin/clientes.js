// ============================================================
// ADMIN CLIENTES.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Admin Clientes cargado');

    // ===== Buscador de clientes =====
    var buscador = document.getElementById('buscadorClientes');
    if (buscador) {
        buscador.addEventListener('keyup', function () {
            var filtro = this.value.toLowerCase();
            var filas = document.querySelectorAll('#tablaClientes tbody tr');
            filas.forEach(function (fila) {
                var texto = fila.textContent.toLowerCase();
                fila.style.display = texto.includes(filtro) ? '' : 'none';
            });
        });
    }

    // ===== Click en "Ver" =====
    document.querySelectorAll('.btn-ver-cliente').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            var id = this.dataset.id || '0';
            mostrarToast('👤 Cargando detalles del cliente #' + id, 'info');
        });
    });
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