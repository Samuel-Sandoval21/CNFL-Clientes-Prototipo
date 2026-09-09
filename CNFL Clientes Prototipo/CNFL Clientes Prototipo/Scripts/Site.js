// ============================================================
// SITE.JS - FUNCIONES GLOBALES (Accesibilidad, etc.)
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Site JS cargado');

    // ===== Cargar preferencias guardadas =====
    if (localStorage.getItem('altoContraste') === 'true') {
        document.body.classList.add('alto-contraste');
    }

    var fuenteGuardada = localStorage.getItem('tamanoFuente');
    if (fuenteGuardada) {
        document.documentElement.style.fontSize = fuenteGuardada;
    }
});

// ===== Accesibilidad: Alto Contraste =====
window.toggleContraste = function () {
    document.body.classList.toggle('alto-contraste');
    var estado = document.body.classList.contains('alto-contraste');
    localStorage.setItem('altoContraste', estado);
};

// ===== Accesibilidad: Aumentar Fuente =====
window.aumentarFuente = function () {
    var html = document.documentElement;
    var size = parseFloat(getComputedStyle(html).fontSize);
    if (size < 28) {
        html.style.fontSize = (size + 2) + 'px';
    }
    localStorage.setItem('tamanoFuente', html.style.fontSize);
};

// ===== Accesibilidad: Disminuir Fuente =====
window.disminuirFuente = function () {
    var html = document.documentElement;
    var size = parseFloat(getComputedStyle(html).fontSize);
    if (size > 12) {
        html.style.fontSize = (size - 2) + 'px';
    }
    localStorage.setItem('tamanoFuente', html.style.fontSize);
};

// ===== Accesibilidad: Resetear Fuente =====
window.resetearFuente = function () {
    document.documentElement.style.fontSize = '16px';
    localStorage.removeItem('tamanoFuente');
};

// ===== Función para cerrar alertas =====
window.cerrarAlerta = function (id) {
    var elemento = document.getElementById(id);
    if (elemento) {
        elemento.style.display = 'none';
    }
};

// ===== Función para mostrar toast/notificación =====
window.mostrarToast = function (mensaje, tipo) {
    tipo = tipo || 'info';
    var toast = document.getElementById('toastGlobal');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastGlobal';
        toast.style.cssText = 'position:fixed; bottom:20px; left:50%; transform:translateX(-50%); padding:12px 24px; border-radius:12px; font-weight:700; z-index:9999; background:#0E1116; color:white; box-shadow:0 8px 24px rgba(0,0,0,0.2); opacity:0; transition:opacity 0.3s;';
        document.body.appendChild(toast);
    }

    toast.textContent = mensaje;
    toast.style.opacity = '1';

    if (tipo === 'success') {
        toast.style.background = '#2E7D32';
    } else if (tipo === 'error') {
        toast.style.background = '#D32F2F';
    } else if (tipo === 'warning') {
        toast.style.background = '#F5A623';
    } else {
        toast.style.background = '#0E1116';
    }

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.opacity = '0';
    }, 3000);
};