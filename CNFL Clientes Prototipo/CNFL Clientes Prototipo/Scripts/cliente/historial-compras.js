// ==========================================
// HISTORIAL DE COMPRAS - JAVASCRIPT
// ==========================================

// ===== VARIABLES =====
var filtroActual = 'todos';

// ===== FILTRAR POR ESTADO =====
function filtrarHistorial(estado) {
    filtroActual = estado;

    // Actualizar botones
    document.querySelectorAll('.filtro-btn').forEach(function (btn) {
        btn.classList.remove('active');
        if (btn.dataset.estado === estado) {
            btn.classList.add('active');
        }
    });

    // Filtrar items
    var items = document.querySelectorAll('.historial-item');
    var visibleCount = 0;

    items.forEach(function (item) {
        var itemEstado = item.dataset.estado || 'pagada';
        if (estado === 'todos' || itemEstado === estado) {
            item.style.display = 'flex';
            visibleCount++;
        } else {
            item.style.display = 'none';
        }
    });

    // Actualizar contador
    var totalEl = document.querySelector('.total-items strong');
    if (totalEl) {
        totalEl.textContent = visibleCount;
    }
}

// ===== VER DETALLE DE COMPRA =====
function verDetalleCompra(id, nombre, precio, fecha, estado) {
    var estados = {
        'pagada': '✅ Pagada',
        'pendiente': '⏳ Pendiente',
        'cancelada': '❌ Cancelada',
        'procesando': '🔄 Procesando'
    };

    var mensaje = '📦 DETALLE DE COMPRA\n';
    mensaje += '═'.repeat(30) + '\n';
    mensaje += '🆔 #' + id + '\n';
    mensaje += '📦 ' + nombre + '\n';
    mensaje += '💰 ' + precio + '\n';
    mensaje += '📅 ' + fecha + '\n';
    mensaje += '📌 ' + (estados[estado] || estado) + '\n';
    mensaje += '\n' + '═'.repeat(30) + '\n';
    mensaje += '📋 Factura disponible en tu correo.\n';
    mensaje += '🔄 ¿Necesitas ayuda con esta compra?';

    if (confirm(mensaje)) {
        alert('📧 Se ha enviado un correo con los detalles de tu compra.');
    }
}

// ===== DESCARGAR FACTURA =====
function descargarFactura(id) {
    alert('📄 Descargando factura #' + id + '...\n\nEl archivo se descargará en breve.');
}

// ===== INICIALIZAR =====
document.addEventListener('DOMContentLoaded', function () {
    // Contar items visibles
    var items = document.querySelectorAll('.historial-item');
    var totalEl = document.querySelector('.total-items strong');
    if (totalEl) {
        totalEl.textContent = items.length;
    }
});