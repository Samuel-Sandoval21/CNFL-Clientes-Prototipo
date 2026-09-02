// ==========================================
// CARRITO DE COMPRAS - JAVASCRIPT
// ==========================================

var carrito = [];
var cuponAplicado = null;
var descuento = 0;

// ==========================================================
// CARGAR PRODUCTO A FACTURA (DESDE TIENDA)
// ==========================================================

function cargarAFactura(producto, precio, elemento) {
    var existe = carrito.find(function (item) {
        return item.producto === producto;
    });

    if (existe) {
        if (!confirm('⚠️ "' + producto + '" ya está en tu carrito.\n\n¿Deseas agregar otra unidad?')) {
            return;
        }
        existe.cantidad = (existe.cantidad || 1) + 1;
    } else {
        carrito.push({
            producto: producto,
            precio: precio,
            cantidad: 1,
            fecha: new Date().toLocaleDateString('es-CR')
        });
    }

    if (elemento) {
        var textoOriginal = elemento.textContent;
        elemento.textContent = '✅ Agregado';
        elemento.classList.add('agregado');
        setTimeout(function () {
            elemento.textContent = textoOriginal;
            elemento.classList.remove('agregado');
        }, 2000);
    }

    actualizarCarrito();
    mostrarNotificacion('🛒 "' + producto + '" agregado a tu carrito');
}

// ==========================================================
// ACTUALIZAR CARRITO (CONTADOR Y RESUMEN)
// ==========================================================

function actualizarCarrito() {
    // Actualizar contador en el ícono flotante
    var contador = document.getElementById('carritoContador');
    if (contador) {
        var totalItems = carrito.reduce(function (sum, item) { return sum + (item.cantidad || 1); }, 0);
        contador.textContent = totalItems;
        var flotante = document.getElementById('carritoFlotante');
        if (flotante) {
            flotante.style.display = totalItems > 0 ? 'grid' : 'none';
        }
    }

    // Actualizar resumen en el perfil
    var resumen = document.getElementById('carritoResumen');
    if (resumen) {
        var totalProductos = carrito.length;
        var totalPrecio = carrito.reduce(function (sum, item) { return sum + (item.precio * (item.cantidad || 1)); }, 0);
        resumen.textContent = totalProductos + ' productos · ₡' + totalPrecio.toLocaleString();
    }

    // Guardar en localStorage
    try {
        localStorage.setItem('carritoCNFL', JSON.stringify(carrito));
    } catch (e) {
        console.log('No se pudo guardar en localStorage');
    }

    // Actualizar vista del carrito si está abierta
    if (document.getElementById('carritoLleno')) {
        renderizarCarrito();
    }
}

// ==========================================================
// RENDERIZAR CARRITO (EN LA VISTA DEL CARRITO)
// ==========================================================

function renderizarCarrito() {
    var vacio = document.getElementById('carritoVacio');
    var lleno = document.getElementById('carritoLleno');
    var lista = document.getElementById('listaProductosCarrito');

    if (!lista) return;

    if (carrito.length === 0) {
        if (vacio) vacio.style.display = 'block';
        if (lleno) lleno.style.display = 'none';
        return;
    }

    if (vacio) vacio.style.display = 'none';
    if (lleno) lleno.style.display = 'block';

    lista.innerHTML = '';

    var subtotal = 0;

    carrito.forEach(function (item, index) {
        var totalItem = item.precio * (item.cantidad || 1);
        subtotal += totalItem;

        var div = document.createElement('div');
        div.className = 'item-carrito';
        div.innerHTML = `
            <div class="item-info">
                <span class="item-nombre">${item.producto}</span>
                <span class="item-precio">₡${item.precio.toLocaleString()}</span>
            </div>
            <div class="item-cantidad">
                <button onclick="cambiarCantidad(${index}, -1)">−</button>
                <span>${item.cantidad || 1}</span>
                <button onclick="cambiarCantidad(${index}, 1)">+</button>
                <button class="btn-eliminar" onclick="eliminarProducto(${index})"><i class="fas fa-trash"></i></button>
            </div>
        `;
        lista.appendChild(div);
    });

    // Actualizar subtotal
    document.getElementById('subtotalCarrito').textContent = '₡' + subtotal.toLocaleString();

    // Aplicar descuento si existe
    aplicarDescuento(subtotal);
}

// ==========================================================
// FUNCIONES DEL CARRITO
// ==========================================================

function cambiarCantidad(index, cambio) {
    if (carrito[index]) {
        carrito[index].cantidad = Math.max(1, (carrito[index].cantidad || 1) + cambio);
        actualizarCarrito();
    }
}

function eliminarProducto(index) {
    if (confirm('¿Eliminar "' + carrito[index].producto + '" del carrito?')) {
        carrito.splice(index, 1);
        actualizarCarrito();
        mostrarNotificacion('🗑️ Producto eliminado del carrito');
    }
}

function vaciarCarrito() {
    if (carrito.length === 0) return;
    if (confirm('¿Vaciar todo el carrito?')) {
        carrito = [];
        actualizarCarrito();
        mostrarNotificacion('🗑️ Carrito vaciado');
    }
}

// ==========================================================
// CUPÓN DE DESCUENTO
// ==========================================================

var cuponesValidos = {
    'CNFL2026': 10,
    'AHORRO20': 20,
    'BIENVENIDA': 15
};

function aplicarCupon() {
    var input = document.getElementById('codigoCupon');
    var mensaje = document.getElementById('mensajeCupon');
    var codigo = input.value.trim().toUpperCase();

    if (!codigo) {
        mensaje.textContent = '⚠️ Ingresa un código de cupón';
        mensaje.style.color = '#E5484D';
        return;
    }

    if (cuponAplicado) {
        mensaje.textContent = '⚠️ Ya tienes un cupón aplicado';
        mensaje.style.color = '#E5484D';
        return;
    }

    if (cuponesValidos[codigo]) {
        cuponAplicado = codigo;
        descuento = cuponesValidos[codigo];
        mensaje.textContent = '✅ Cupón "' + codigo + '" aplicado! ' + descuento + '% de descuento';
        mensaje.style.color = '#2E7D32';
        input.disabled = true;
        aplicarDescuento(calcularSubtotal());
        mostrarNotificacion('🎉 Cupón "' + codigo + '" aplicado correctamente');
    } else {
        mensaje.textContent = '❌ Código de cupón inválido';
        mensaje.style.color = '#E5484D';
    }
}

function calcularSubtotal() {
    return carrito.reduce(function (sum, item) { return sum + (item.precio * (item.cantidad || 1)); }, 0);
}

function aplicarDescuento(subtotal) {
    var total = subtotal;
    if (descuento > 0) {
        total = subtotal * (1 - descuento / 100);
    }
    document.getElementById('totalCarrito').textContent = '₡' + Math.round(total).toLocaleString();
    return total;
}

// ==========================================================
// PAGO
// ==========================================================

function verCarrito() {
    window.location.href = '/Clientes/Carrito';
}

function procederPago() {
    if (carrito.length === 0) {
        alert('🛒 Tu carrito está vacío');
        return;
    }
    document.getElementById('modalPago').style.display = 'flex';
}

function cerrarModalPago() {
    document.getElementById('modalPago').style.display = 'none';
}

function formatearNumeroTarjeta(input) {
    var value = input.value.replace(/\D/g, '');
    var formatted = '';
    for (var i = 0; i < value.length && i < 16; i++) {
        if (i > 0 && i % 4 === 0) {
            formatted += ' ';
        }
        formatted += value[i];
    }
    input.value = formatted;
}

function confirmarPago() {
    var nombre = document.getElementById('nombreTarjeta').value.trim();
    var numero = document.getElementById('numeroTarjeta').value.replace(/\s/g, '');
    var mes = document.getElementById('mesTarjeta').value;
    var anio = document.getElementById('anioTarjeta').value;
    var cvv = document.getElementById('cvvTarjeta').value;

    var errores = [];

    if (!nombre) errores.push('Ingresa el nombre del propietario');
    if (numero.length < 16) errores.push('Número de tarjeta inválido');
    if (cvv.length < 3) errores.push('CVV inválido');

    if (errores.length > 0) {
        alert('❌ Por favor, corrige los siguientes errores:\n\n- ' + errores.join('\n- '));
        return;
    }

    var total = document.getElementById('totalCarrito').textContent.replace('₡', '').replace(/,/g, '');

    alert('✅ ¡Pago procesado con éxito!\n\n' +
        '💰 Total: ₡' + parseInt(total).toLocaleString() + '\n' +
        '📦 Productos: ' + carrito.length + '\n' +
        '📩 Recibirás el comprobante en tu correo.\n\n' +
        '🔄 Redirigiendo al historial de compras...');

    // Guardar en historial y vaciar carrito
    var productos = carrito.map(function (item) {
        return item.producto + ' x' + (item.cantidad || 1);
    }).join(', ');

    console.log('Compra realizada:', {
        productos: productos,
        total: total,
        fecha: new Date().toISOString()
    });

    carrito = [];
    cuponAplicado = null;
    descuento = 0;
    actualizarCarrito();
    cerrarModalPago();

    setTimeout(function () {
        window.location.href = '/Clientes/HistorialCompras';
    }, 2000);
}

// ==========================================================
// NOTIFICACIONES
// ==========================================================

function mostrarNotificacion(mensaje) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    toast.textContent = mensaje;
    document.body.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(function () {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 3000);
}

// ==========================================================
// INICIALIZAR
// ==========================================================

document.addEventListener('DOMContentLoaded', function () {
    // Cargar carrito desde localStorage
    try {
        var saved = localStorage.getItem('carritoCNFL');
        if (saved) {
            carrito = JSON.parse(saved);
        }
    } catch (e) {
        console.log('No se pudo cargar carrito');
    }

    actualizarCarrito();

    // Renderizar carrito si estamos en la página del carrito
    if (document.getElementById('carritoLleno')) {
        renderizarCarrito();
    }

    // Enter para aplicar cupón
    var cuponInput = document.getElementById('codigoCupon');
    if (cuponInput) {
        cuponInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                aplicarCupon();
            }
        });
    }
});

// ==========================================================
// EXPONER FUNCIONES GLOBALMENTE
// ==========================================================

window.cargarAFactura = cargarAFactura;
window.verCarrito = verCarrito;
window.cambiarCantidad = cambiarCantidad;
window.eliminarProducto = eliminarProducto;
window.vaciarCarrito = vaciarCarrito;
window.aplicarCupon = aplicarCupon;
window.procederPago = procederPago;
window.cerrarModalPago = cerrarModalPago;
window.confirmarPago = confirmarPago;
window.formatearNumeroTarjeta = formatearNumeroTarjeta;
window.mostrarNotificacion = mostrarNotificacion;