// ==========================================
// PERFIL DEL CLIENTE - JAVASCRIPT
// ==========================================

// ==========================================================
// ===== NAVEGACIÓN =====
// ==========================================================

function irAHistorial() {
    window.location.href = '/Clientes/HistorialCompras';
}

function irASuscripciones() {
    window.location.href = '/Clientes/Suscripciones';
}

function irAServiciosContratados() {
    window.location.href = '/Clientes/ServiciosContratados';
}

function irAEditarDatos() {
    window.location.href = '/Clientes/EditarDatos';
}

function irACalculadora() {
    window.location.href = '/Clientes/Calculadora';
}

function irAChat() {
    window.location.href = '/Clientes/Chat';
}

function irANotificaciones() {
    window.location.href = '/Clientes/Notificaciones';
}

// ==========================================================
// ===== CERRAR SESIÓN =====
// ==========================================================

function cerrarSesion() {
    if (confirm('¿Estás seguro de que deseas cerrar sesión?')) {
        window.location.href = '/Cuenta/Logout';
    }
}

// ==========================================================
// ===== MOSTRAR TOAST =====
// ==========================================================

function mostrarToast(mensaje, tipo) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    if (tipo === 'error') {
        toast.classList.add('error');
    }
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
// ===== INICIALIZAR =====
// ==========================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('📱 Perfil del cliente cargado correctamente.');

    // ==========================================================
    // Efecto de entrada para los items
    // ==========================================================
    var items = document.querySelectorAll('.perfil-item');
    items.forEach(function (item, index) {
        item.style.opacity = '0';
        item.style.transform = 'translateY(10px)';
        item.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            item.style.opacity = '1';
            item.style.transform = 'translateY(0)';
        }, 100 + (index * 60));
    });

    // ==========================================================
    // Efecto de entrada para el header
    // ==========================================================
    var header = document.querySelector('.perfil-header');
    if (header) {
        header.style.opacity = '0';
        header.style.transform = 'translateY(-10px)';
        header.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            header.style.opacity = '1';
            header.style.transform = 'translateY(0)';
        }, 50);
    }

    // ==========================================================
    // Efecto de entrada para datos protegidos
    // ==========================================================
    var protegido = document.querySelector('.perfil-protegido');
    if (protegido) {
        protegido.style.opacity = '0';
        protegido.style.transform = 'translateX(-10px)';
        protegido.style.transition = 'all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            protegido.style.opacity = '1';
            protegido.style.transform = 'translateX(0)';
        }, 200);
    }

    // ==========================================================
    // Prevenir comportamiento por defecto en enlaces
    // ==========================================================
    document.querySelectorAll('.perfil-item').forEach(function (item) {
        item.addEventListener('click', function (e) {
            // Si tiene href, dejar que navegue normalmente
            if (this.getAttribute('href') && this.getAttribute('href') !== '#') {
                return;
            }
        });
    });

});