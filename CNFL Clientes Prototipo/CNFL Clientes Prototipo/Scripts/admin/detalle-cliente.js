// ============================================================
// ADMIN DETALLE-CLIENTE.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Detalle Cliente cargado');

    // ===== Botón ver facturas =====
    var btnVerFacturas = document.getElementById('btnVerFacturas');
    if (btnVerFacturas) {
        btnVerFacturas.addEventListener('click', function () {
            var clienteId = this.dataset.id || '0';
            mostrarToast('📄 Cargando facturas del cliente #' + clienteId, 'info');
        });
    }

    // ===== Botón ver averías =====
    var btnVerAverias = document.getElementById('btnVerAverias');
    if (btnVerAverias) {
        btnVerAverias.addEventListener('click', function () {
            var clienteId = this.dataset.id || '0';
            mostrarToast('⚡ Cargando averías del cliente #' + clienteId, 'info');
        });
    }

    // ===== Botón editar =====
    var btnEditar = document.getElementById('btnEditarCliente');
    if (btnEditar) {
        btnEditar.addEventListener('click', function () {
            var clienteId = this.dataset.id || '0';
            mostrarToast('✏️ Abriendo edición del cliente #' + clienteId, 'info');
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
            mostrarToast('⬅️ Volviendo a lista de clientes', 'info');
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