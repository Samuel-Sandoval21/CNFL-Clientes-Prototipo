// ============================================================
// MIS-FACTURAS.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Mis Facturas cargado');

    // ===== Botones de pagar =====
    document.querySelectorAll('.btn-pagar').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            var monto = this.closest('.factura-card')?.querySelector('.monto')?.textContent || 'factura';
            if (confirm('¿Deseas pagar ' + monto + '?')) {
                mostrarToast('💳 Procesando pago de ' + monto + '...', 'success');
                // Simulación de pago
                setTimeout(function () {
                    mostrarToast('✅ Pago realizado exitosamente', 'success');
                }, 2000);
            }
        });
    });

    // ===== Click en tarjeta de factura =====
    document.querySelectorAll('.factura-card').forEach(function (card) {
        card.addEventListener('click', function () {
            var nise = this.querySelector('.nise')?.textContent || 'NISE';
            mostrarToast('📄 Ver detalle de ' + nise, 'info');
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