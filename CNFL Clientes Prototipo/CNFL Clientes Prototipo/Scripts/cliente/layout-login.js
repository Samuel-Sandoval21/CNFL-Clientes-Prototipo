// ==========================================
// LAYOUT LOGIN - JAVASCRIPT
// ==========================================

var idiomaActual = 'es';

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
// ===== MARCAR TAB ACTIVO DEL LOGIN =====
// ==========================================================
function marcarTabActivoLogin() {
    var currentPath = window.location.pathname.toLowerCase();

    document.querySelectorAll('.tab').forEach(function (tab) {
        tab.classList.remove('active');
    });

    var tabActivo = '';

    if (currentPath === '/' ||
        currentPath === '/home' ||
        currentPath === '/home/index') {
        tabActivo = 'inicio';
    } else if (currentPath.indexOf('/cuenta/login') === 0 ||
        currentPath.indexOf('/cuenta/registro') === 0 ||
        currentPath.indexOf('/cuenta/recuperarclave') === 0 ||
        currentPath === '/cuenta' ||
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

var idiomasLogin = {
    es: {
        'menu_inicio': 'Inicio',
        'menu_cuenta': 'Cuenta'
    },
    en: {
        'menu_inicio': 'Home',
        'menu_cuenta': 'Account'
    }
};

function cambiarIdiomaLogin(lang) {
    idiomaActual = lang;

    document.querySelectorAll('#langbar button').forEach(function (btn) {
        btn.classList.remove('active');
        if (btn.dataset.lang === lang) {
            btn.classList.add('active');
        }
    });

    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        var key = el.dataset.i18n;
        if (idiomasLogin[lang] && idiomasLogin[lang][key]) {
            el.textContent = idiomasLogin[lang][key];
        }
    });

    document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
        var key = el.dataset.i18nPlaceholder;
        if (idiomasLogin[lang] && idiomasLogin[lang][key]) {
            el.placeholder = idiomasLogin[lang][key];
        }
    });

    localStorage.setItem('idioma', lang);
}

function cargarIdiomaGuardado() {
    var lang = localStorage.getItem('idioma') || 'es';
    cambiarIdiomaLogin(lang);
}

// ==========================================================
// ===== INICIALIZAR =====
// ==========================================================

document.addEventListener('DOMContentLoaded', function () {
    cargarIdiomaGuardado();

    setTimeout(function () {
        marcarTabActivoLogin();
    }, 100);

    document.querySelectorAll('.tab').forEach(function (tab) {
        tab.addEventListener('click', function () {
            document.querySelectorAll('.tab').forEach(function (t) {
                t.classList.remove('active');
            });
            this.classList.add('active');
        });
    });
});

// ==========================================================
// ===== EXPONER FUNCIONES GLOBALMENTE =====
// ==========================================================

window.cambiarIdioma = cambiarIdiomaLogin;
window.marcarTabActivoLogin = marcarTabActivoLogin;