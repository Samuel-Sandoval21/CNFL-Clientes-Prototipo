// ============================================================
// TIENDA.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Tienda cargado');

    // ===== Botones de agregar =====
    document.querySelectorAll('.btn-agregar').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            var producto = this.closest('.producto-card')?.querySelector('h3')?.textContent || 'Producto';
            var precio = this.closest('.producto-card')?.querySelector('.precio')?.textContent || '';
            mostrarToast('🛒 ' + producto + ' agregado al carrito por ' + precio, 'success');
            this.textContent = '✅ Agregado';
            this.style.background = '#2E7D32';
            setTimeout(function () {
                btn.textContent = 'Agregar';
                btn.style.background = '#FF692D';
            }, 2000);
        });
    });

    // ===== Click en tarjeta de producto =====
    document.querySelectorAll('.producto-card').forEach(function (card) {
        card.addEventListener('click', function () {
            var producto = this.querySelector('h3')?.textContent || 'Producto';
            mostrarToast('📦 Ver detalles de ' + producto, 'info');
        });
    });

    // ===== Animación de entrada =====
    var cards = document.querySelectorAll('.producto-card');
    cards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'scale(0.95)';
        setTimeout(function () {
            card.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            card.style.opacity = '1';
            card.style.transform = 'scale(1)';
        }, 100 + (index * 80));
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