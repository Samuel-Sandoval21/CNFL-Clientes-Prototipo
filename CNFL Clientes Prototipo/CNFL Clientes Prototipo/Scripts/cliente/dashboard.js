// ============================================================
// DASHBOARD.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Dashboard cargado');

    // ===== Animación de números (estadísticas) =====
    var numeros = document.querySelectorAll('.stat-number');
    numeros.forEach(function (num) {
        var valor = parseInt(num.textContent.replace(/[₡,.]/g, ''));
        if (!isNaN(valor) && valor > 0) {
            animarNumero(num, valor);
        }
    });

    function animarNumero(elemento, objetivo) {
        var duracion = 800;
        var paso = 20;
        var incremento = objetivo / (duracion / paso);
        var actual = 0;
        var intervalo = setInterval(function () {
            actual += incremento;
            if (actual >= objetivo) {
                actual = objetivo;
                clearInterval(intervalo);
            }
            elemento.textContent = Math.round(actual).toLocaleString();
        }, paso);
    }

    // ===== Click en tarjetas de NISE =====
    document.querySelectorAll('.nise').forEach(function (card) {
        card.addEventListener('click', function () {
            var nise = this.querySelector('.t')?.textContent || 'NISE';
            mostrarToast('Abriendo servicio: ' + nise, 'info');
        });
    });

    // ===== Click en botones de factura =====
    document.querySelectorAll('.btn-ghost, .btn-cta').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            var texto = this.textContent.trim();
            if (texto.includes('Ver detalle')) {
                mostrarToast('📄 Abriendo detalle de factura...', 'info');
            } else if (texto.includes('Pagar ahora')) {
                mostrarToast('💳 Redirigiendo a pago seguro...', 'success');
            }
        });
    });

    // ===== Tabs de consumo =====
    document.querySelectorAll('.chip').forEach(function (chip) {
        chip.addEventListener('click', function () {
            document.querySelectorAll('.chip').forEach(function (c) {
                c.classList.remove('active');
            });
            this.classList.add('active');
            mostrarToast('📊 Cambiando a: ' + this.textContent.trim(), 'info');
        });
    });

    // ===== Actualizar badge de notificaciones =====
    actualizarBadge(3);
});

// ===== Toast =====
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

function actualizarBadge(cantidad) {
    document.querySelectorAll('.badge').forEach(function (badge) {
        if (cantidad > 0) {
            badge.textContent = cantidad;
            badge.style.display = 'grid';
        } else {
            badge.style.display = 'none';
        }
    });
}