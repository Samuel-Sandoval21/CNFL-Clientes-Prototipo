// ==========================================
// CLIENTE LAYOUT - JAVASCRIPT COMPLETO
// ==========================================

// ===== RELOJ =====
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
// ===== MARCAR ÍTEM ACTIVO DEL MENÚ CLIENTE =====
// ==========================================================
function marcarTabActivo() {
    var currentPath = window.location.pathname.toLowerCase();

    document.querySelectorAll('.tab').forEach(function (tab) {
        tab.classList.remove('active');
        tab.style.color = '';
        tab.style.fontWeight = '';
    });

    var tabActivo = '';

    if (currentPath === '/' ||
        currentPath === '/home' ||
        currentPath === '/home/index' ||
        currentPath === '/clientes' ||
        currentPath === '/clientes/' ||
        currentPath === '/clientes/inicio') {
        tabActivo = 'inicio';
    } else if (currentPath === '/clientes/tramites' || currentPath.indexOf('/clientes/tramites/') === 0) {
        tabActivo = 'tramites';
    } else if (currentPath === '/clientes/pagos' || currentPath.indexOf('/clientes/pagos/') === 0) {
        tabActivo = 'pagos';
    } else if (currentPath === '/clientes/tienda' || currentPath.indexOf('/clientes/tienda/') === 0) {
        tabActivo = 'tienda';
    } else if (currentPath === '/clientes/reportes' || currentPath.indexOf('/clientes/reportes/') === 0) {
        tabActivo = 'reportes';
    } else if (currentPath.indexOf('/clientes/') === 0) {
        tabActivo = 'perfil';
    } else if (currentPath === '/cuenta' ||
        currentPath === '/cuenta/' ||
        currentPath.indexOf('/cuenta/') === 0) {
        tabActivo = 'perfil';
    }

    if (tabActivo) {
        document.querySelectorAll('.tab').forEach(function (tab) {
            var seccion = tab.getAttribute('data-seccion');
            if (seccion === tabActivo) {
                tab.classList.add('active');
            }
        });
    }
}

// ==========================================================
// ===== CAMBIAR IDIOMA =====
// ==========================================================

var idiomas = {
    es: {
        'menu_inicio': 'Inicio',
        'menu_tramites': 'Trámites',
        'menu_pagos': 'Pagos',
        'menu_tienda': 'Tienda',
        'menu_reportes': 'Reportes',
        'menu_cuenta': 'Cuenta'
    },
    en: {
        'menu_inicio': 'Home',
        'menu_tramites': 'Requests',
        'menu_pagos': 'Payments',
        'menu_tienda': 'Store',
        'menu_reportes': 'Reports',
        'menu_cuenta': 'Account'
    }
};

var idiomaActual = 'es';

function cambiarIdioma(lang) {
    idiomaActual = lang;

    document.querySelectorAll('#langbar button').forEach(function (btn) {
        btn.classList.remove('active');
        if (btn.dataset.lang === lang) {
            btn.classList.add('active');
        }
    });

    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        var key = el.getAttribute('data-i18n');
        if (idiomas[lang] && idiomas[lang][key]) {
            el.textContent = idiomas[lang][key];
        }
    });

    document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
        var key = el.getAttribute('data-i18n-placeholder');
        if (idiomas[lang] && idiomas[lang][key]) {
            el.placeholder = idiomas[lang][key];
        }
    });

    localStorage.setItem('idioma', lang);
}

function cargarIdiomaGuardado() {
    var lang = localStorage.getItem('idioma') || 'es';
    cambiarIdioma(lang);
}

// ==========================================================
// ===== INICIALIZAR =====
// ==========================================================

document.addEventListener('DOMContentLoaded', function () {
    cargarIdiomaGuardado();

    setTimeout(function () {
        marcarTabActivo();
    }, 50);

    setTimeout(function () {
        marcarTabActivo();
    }, 200);

    setTimeout(function () {
        marcarTabActivo();
    }, 500);

    document.querySelectorAll('.tab').forEach(function (tab) {
        tab.addEventListener('click', function () {
            document.querySelectorAll('.tab').forEach(function (t) {
                t.classList.remove('active');
                t.style.color = '';
                t.style.fontWeight = '';
            });
            this.classList.add('active');
        });
    });
});

// ==========================================================
// ===== EXPONER FUNCIONES GLOBALMENTE =====
// ==========================================================

window.cambiarIdioma = cambiarIdioma;
window.marcarTabActivo = marcarTabActivo;