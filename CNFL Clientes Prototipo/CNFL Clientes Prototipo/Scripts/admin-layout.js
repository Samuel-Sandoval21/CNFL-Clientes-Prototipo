// ==========================================
// ADMIN LAYOUT - JAVASCRIPT COMPLETO
// ==========================================

// ===== RELOJ EN TIEMPO REAL =====
function updateTime() {
    var now = new Date();
    var h = String(now.getHours()).padStart(2, '0');
    var m = String(now.getMinutes()).padStart(2, '0');
    var timeElement = document.getElementById('statusTime');
    if (timeElement) {
        timeElement.textContent = h + ':' + m;
    }
}

updateTime();
setInterval(updateTime, 30000);

// ==========================================================
// ===== MARCAR ÍTEM ACTIVO DEL MENÚ ADMIN =====
// ==========================================================
function marcarItemActivo() {
    var currentPath = window.location.pathname.toLowerCase();

    console.log('=== MARCAR ITEM ACTIVO ADMIN ===');
    console.log('Path:', currentPath);

    // REMOVER active de TODOS los items
    document.querySelectorAll('.nav-item').forEach(function (item) {
        item.classList.remove('active');
    });

    // Determinar la acción actual
    var detectedAction = '';

    // 1. DASHBOARD - raíz o /Admin/Dashboard
    if (currentPath === '/' ||
        currentPath === '/home' ||
        currentPath === '/home/index' ||
        currentPath === '/admin/dashboard') {
        detectedAction = 'dashboard';
    }
    // 2. AVERÍAS - SOLO /Admin o /Admin/Index
    else if (currentPath === '/admin' ||
        currentPath === '/admin/' ||
        currentPath.includes('/admin/index')) {
        detectedAction = 'index';
    }
    // 3. CLIENTES
    else if (currentPath.includes('/admin/clientes')) {
        detectedAction = 'clientes';
    }
    // 4. REPORTES
    else if (currentPath.includes('/admin/reportes')) {
        detectedAction = 'reportes';
    }
    // 5. Si es otra ruta de admin
    else if (currentPath.includes('/admin/')) {
        var parts = currentPath.split('/');
        if (parts.length > 2) {
            var possibleAction = parts[2].toLowerCase();
            var validActions = ['dashboard', 'index', 'clientes', 'reportes'];
            if (validActions.indexOf(possibleAction) !== -1) {
                detectedAction = possibleAction;
            }
        }
    }

    // Si no se detectó nada, usar DASHBOARD
    if (!detectedAction) {
        detectedAction = 'dashboard';
    }

    console.log('Acción detectada:', detectedAction);

    // Activar SOLO el item que coincide
    document.querySelectorAll('.nav-item').forEach(function (item) {
        var dataAction = item.getAttribute('data-action');
        if (dataAction === 'logout') {
            return; // Salir nunca se activa
        }
        if (dataAction === detectedAction) {
            item.classList.add('active');
            console.log('✅ Activado:', dataAction);
        }
    });
}

// ==========================================================
// ===== MODAL DE CIERRE DE SESIÓN =====
// ==========================================================

function mostrarModalCierre() {
    var modal = document.getElementById('modalCierre');
    if (modal) {
        modal.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
}

function cerrarModalCierre() {
    var modal = document.getElementById('modalCierre');
    if (modal) {
        modal.classList.remove('show');
        document.body.style.overflow = '';
    }
}

function confirmarCierre() {
    cerrarModalCierre();
    if (window.logoutUrl) {
        window.location.href = window.logoutUrl;
    } else {
        window.location.href = '/Cuenta/Logout';
    }
}

// ==========================================================
// ===== EVENTOS =====
// ==========================================================

document.addEventListener('DOMContentLoaded', function () {
    marcarItemActivo();

    var modal = document.getElementById('modalCierre');
    if (modal) {
        modal.addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModalCierre();
            }
        });
    }

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            cerrarModalCierre();
        }
    });

    document.querySelectorAll('.nav-item[href="#"]').forEach(function (item) {
        item.addEventListener('click', function (e) {
            e.preventDefault();
        });
    });
});

// ==========================================================
// ===== CAMBIAR IDIOMA - SIN TOAST =====
// ==========================================================

var idiomasAdmin = {
    es: {
        'dashboard': 'Dashboard',
        'averias': 'Averías',
        'clientes': 'Clientes',
        'reportes': 'Reportes',
        'salir': 'Salir',
        'volver': 'Volver'
    },
    en: {
        'dashboard': 'Dashboard',
        'averias': 'Faults',
        'clientes': 'Clients',
        'reportes': 'Reports',
        'salir': 'Logout',
        'volver': 'Back'
    }
};

var idiomaActualAdmin = 'es';

function cambiarIdiomaAdmin(lang) {
    idiomaActualAdmin = lang;

    var btn = document.querySelector('.lang-toggle-top span');
    if (btn) {
        btn.textContent = lang === 'es' ? 'ES ▼' : 'EN ▼';
    }

    localStorage.setItem('idioma_admin', lang);

    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        var key = el.getAttribute('data-i18n');
        if (idiomasAdmin[lang] && idiomasAdmin[lang][key]) {
            el.textContent = idiomasAdmin[lang][key];
        }
    });
}

(function cargarIdiomaGuardado() {
    var lang = localStorage.getItem('idioma_admin') || 'es';
    idiomaActualAdmin = lang;
    var btn = document.querySelector('.lang-toggle-top span');
    if (btn) {
        btn.textContent = lang === 'es' ? 'ES ▼' : 'EN ▼';
    }
    setTimeout(function () {
        cambiarIdiomaAdmin(lang);
    }, 50);
})();

// ==========================================================
// ===== FUNCIONES ADICIONALES =====
// ==========================================================

function generarReporteAdmin() {
    console.log('Generando reporte...');
    setTimeout(function () {
        console.log('Reporte generado');
    }, 1500);
}

function exportarReporteAdmin(formato) {
    console.log('Exportando ' + formato + '...');
    setTimeout(function () {
        console.log('Exportado');
    }, 1000);
}

function filtrarClientesAdmin() {
    var input = document.getElementById('searchInput');
    if (!input) return;

    var filter = input.value.toLowerCase();
    var rows = document.querySelectorAll('.client-row');
    var visibleCount = 0;

    rows.forEach(function (row) {
        var searchData = row.dataset.search || '';
        if (searchData.indexOf(filter) > -1) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });

    var totalEl = document.querySelector('.client-total strong');
    if (totalEl) {
        totalEl.textContent = visibleCount;
    }
}

function actualizarEstadoAveria(id, nuevoEstado) {
    var select = document.querySelector('.estado-select[data-id="' + id + '"]');
    var badge = document.getElementById('status-badge-' + id);
    if (!select || !badge) return;

    var estado = nuevoEstado || select.value;

    var btn = document.querySelector('.btn-actualizar[data-id="' + id + '"]');
    if (btn) {
        var textoOriginal = btn.innerHTML;
        btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
        btn.disabled = true;

        setTimeout(function () {
            badge.textContent = estado;
            badge.className = 'status-badge estado-' + estado.toLowerCase().replace(' ', '-');
            select.value = estado;
            btn.innerHTML = textoOriginal;
            btn.disabled = false;
        }, 800);
    }
}

// ==========================================================
// ===== EXPONER FUNCIONES GLOBALMENTE =====
// ==========================================================

window.mostrarModalCierre = mostrarModalCierre;
window.cerrarModalCierre = cerrarModalCierre;
window.confirmarCierre = confirmarCierre;
window.cambiarIdiomaAdmin = cambiarIdiomaAdmin;
window.generarReporteAdmin = generarReporteAdmin;
window.exportarReporteAdmin = exportarReporteAdmin;
window.filtrarClientesAdmin = filtrarClientesAdmin;
window.actualizarEstadoAveria = actualizarEstadoAveria;
window.marcarItemActivo = marcarItemActivo;