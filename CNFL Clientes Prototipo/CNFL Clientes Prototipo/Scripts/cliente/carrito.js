// ============================================================
// CARRITO.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Carrito cargado');

    // ===== Eliminar producto =====
    document.querySelectorAll('.btn-eliminar').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            var item = this.closest('.carrito-item');
            if (!item) return;

            var producto = item.querySelector('span:first-child')?.textContent || 'Producto';
            if (confirm('¿Deseas eliminar "' + producto + '" del carrito?')) {
                item.style.transition = 'opacity 0.3s, transform 0.3s';
                item.style.opacity = '0';
                item.style.transform = 'translateX(50px)';
                setTimeout(function () {
                    item.remove();
                    actualizarTotal();
                    mostrarToast('🗑️ ' + producto + ' eliminado del carrito', 'warning');
                }, 300);
            }
        });
    });

    // ===== Pagar carrito =====
    var btnPagar = document.querySelector('.btn-pagar-carrito');
    if (btnPagar) {
        btnPagar.addEventListener('click', function () {
            var total = document.querySelector('.carrito-total h3')?.textContent || '₡0';
            if (confirm('¿Deseas proceder al pago de ' + total + '?')) {
                mostrarToast('💳 Procesando pago...', 'info');
                setTimeout(function () {
                    mostrarToast('✅ ¡Compra realizada exitosamente!', 'success');
                }, 2500);
            }
        });
    }

    function actualizarTotal() {
        var items = document.querySelectorAll('.carrito-item');
        var total = 0;
        items.forEach(function (item) {
            var precioTexto = item.querySelector('span:nth-child(2)')?.textContent || '0';
            var precio = parseFloat(precioTexto.replace(/[₡,]/g, '').trim());
            if (!isNaN(precio)) {
                total += precio;
            }
        });
        var totalElement = document.querySelector('.carrito-total h3');
        if (totalElement) {
            totalElement.textContent = 'Total: ₡' + total.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }
        if (total === 0) {
            document.querySelector('.carrito-lista').innerHTML = '<p style="text-align:center; color:var(--muted); padding:20px;">Tu carrito está vacío 🛒</p>';
            document.querySelector('.carrito-total').style.display = 'none';
        }
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