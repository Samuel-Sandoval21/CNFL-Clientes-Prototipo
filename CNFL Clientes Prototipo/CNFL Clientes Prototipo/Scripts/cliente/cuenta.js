// ==========================================
// CUENTA / PERFIL - JAVASCRIPT
// ==========================================

// ===== FUNCIONES DE NAVEGACIÓN =====
function editarDatos() {
    window.location.href = '/Clientes/EditarDatos';
}

function verHistorial() {
    window.location.href = '/Clientes/HistorialCompras';
}

function verSuscripciones() {
    window.location.href = '/Clientes/Suscripciones';
}

function verServiciosContratados() {
    window.location.href = '/Cuenta/ServiciosContratados';
}

function abrirCalculadora() {
    window.location.href = '/Clientes/Calculadora';
}

function abrirChat() {
    window.location.href = '/Clientes/Chat';
}

// ===== CERRAR SESIÓN =====
function cerrarSesion() {
    if (confirm('¿Estás seguro de que deseas cerrar sesión?')) {
        window.location.href = '/Cuenta/Logout';
    }
}

// ===== MOSTRAR TOAST DE CONFIRMACIÓN =====
function mostrarToast(mensaje, tipo) {
    var toast = document.createElement('div');
    toast.className = 'toast-notificacion';
    toast.textContent = mensaje;
    toast.style.cssText = `
        position: fixed;
        bottom: 100px;
        left: 50%;
        transform: translateX(-50%);
        background: ${tipo === 'error' ? '#C62828' : '#2E7D32'};
        color: white;
        padding: 12px 24px;
        border-radius: 16px;
        font-weight: 600;
        font-size: 14px;
        box-shadow: 0 8px 30px rgba(0,0,0,0.2);
        z-index: 2000;
        animation: slideUp 0.3s ease;
        max-width: 90%;
    `;
    document.body.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(function () {
            document.body.removeChild(toast);
        }, 300);
    }, 3000);
}