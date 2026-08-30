// ==========================================
// TIENDA - JAVASCRIPT CON CARRITO
// ==========================================

// ===== CARRITO =====
var carrito = [];

// ===== CARGAR PRODUCTO A FACTURA =====
function cargarAFactura(producto, precio, elemento) {
    // Verificar si ya está en el carrito
    var existe = carrito.find(function (item) {
        return item.producto === producto;
    });

    if (existe) {
        if (!confirm('⚠️ "' + producto + '" ya está en tu carrito.\n\n¿Deseas agregar otra unidad?')) {
            return;
        }
    }

    // Agregar al carrito
    carrito.push({
        producto: producto,
        precio: precio,
        fecha: new Date().toLocaleDateString('es-CR'),
        cantidad: 1
    });

    // Feedback visual en el botón
    var btn = elemento || event.target;
    var textoOriginal = btn.textContent;
    btn.textContent = '✅ Agregado';
    btn.classList.add('agregado');

    setTimeout(function () {
        btn.textContent = textoOriginal;
        btn.classList.remove('agregado');
    }, 2000);

    // Actualizar contador del carrito
    actualizarContadorCarrito();

    // Mostrar notificación
    mostrarNotificacion('🛒 "' + producto + '" agregado a tu carrito');
}

// ===== ACTUALIZAR CONTADOR DEL CARRITO =====
function actualizarContadorCarrito() {
    var badge = document.querySelector('.carrito-flotante .contador');
    if (badge) {
        badge.textContent = carrito.length;
        if (carrito.length === 0) {
            badge.style.display = 'none';
        } else {
            badge.style.display = 'grid';
        }
    }
}

// ===== MOSTRAR NOTIFICACIÓN =====
function mostrarNotificacion(mensaje) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    toast.textContent = mensaje;
    document.body.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(function () {
            document.body.removeChild(toast);
        }, 300);
    }, 3000);
}

// ===== VER CARRITO =====
function verCarrito() {
    if (carrito.length === 0) {
        alert('🛒 Tu carrito está vacío.\n\nExplora nuestros productos y agrega lo que necesites.');
        return;
    }

    var mensaje = '🛒 MI CARRITO\n' + '═'.repeat(30) + '\n\n';
    var total = 0;

    carrito.forEach(function (item, index) {
        mensaje += (index + 1) + '. ' + item.producto + '\n';
        mensaje += '   💰 ₡ ' + item.precio.toLocaleString() + '\n';
        mensaje += '   📅 ' + item.fecha + '\n\n';
        total += item.precio;
    });

    mensaje += '═'.repeat(30) + '\n';
    mensaje += '💰 TOTAL: ₡ ' + total.toLocaleString() + '\n\n';
    mensaje += '¿Deseas proceder con el pago?';

    if (confirm(mensaje)) {
        var opcion = prompt('Selecciona método de pago:\n\n1️⃣ Factura eléctrica\n2️⃣ SINPE Móvil\n3️⃣ IBAN\n4️⃣ Tarjeta\n\nIngresa el número de la opción:');

        if (opcion === '1') {
            alert('✅ ¡Compra realizada con éxito!\n\n📩 Recibirás el detalle en tu correo.\n📦 Los productos serán entregados en 3-5 días hábiles.\n💳 Se cargará en tu próxima factura eléctrica.');
        } else if (opcion === '2' || opcion === '3' || opcion === '4') {
            alert('✅ ¡Compra realizada con éxito!\n\n📩 Recibirás el detalle en tu correo.\n📦 Los productos serán entregados en 3-5 días hábiles.\n💳 Se enviarán las instrucciones de pago a tu correo.');
        } else {
            alert('❌ Opción no válida. Intenta nuevamente.');
            return;
        }

        carrito = [];
        actualizarContadorCarrito();
    }
}

// ===== CREAR CARRITO FLOTANTE =====
function crearCarritoFlotante() {
    var carritoBtn = document.createElement('div');
    carritoBtn.className = 'carrito-flotante';
    carritoBtn.innerHTML = `
        <i class="fas fa-shopping-cart"></i>
        <span class="contador" style="display:none;">0</span>
    `;
    carritoBtn.onclick = verCarrito;
    document.body.appendChild(carritoBtn);
}

// ===== INICIALIZAR =====
document.addEventListener('DOMContentLoaded', function () {
    crearCarritoFlotante();
    actualizarContadorCarrito();

    // Animar productos al cargar
    var productos = document.querySelectorAll('.prod');
    productos.forEach(function (prod, index) {
        prod.style.opacity = '0';
        prod.style.transform = 'translateY(20px)';
        setTimeout(function () {
            prod.style.transition = 'all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            prod.style.opacity = '1';
            prod.style.transform = 'translateY(0)';
        }, 100 + (index * 80));
    });
});