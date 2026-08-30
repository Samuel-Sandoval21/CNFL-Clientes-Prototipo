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

// ===== CAMBIAR IDIOMA =====
function cambiarIdioma(lang) {
    idiomaActual = lang;

    document.querySelectorAll('#langbar button').forEach(function (btn) {
        btn.classList.remove('active');
        if (btn.dataset.lang === lang) {
            btn.classList.add('active');
        }
    });

    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        var key = el.dataset.i18n;
        if (idiomas[lang] && idiomas[lang][key]) {
            el.textContent = idiomas[lang][key];
        }
    });

    document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
        var key = el.dataset.i18nPlaceholder;
        if (idiomas[lang] && idiomas[lang][key]) {
            el.placeholder = idiomas[lang][key];
        }
    });

    localStorage.setItem('idioma', lang);
}

// ===== CARGAR IDIOMA GUARDADO =====
function cargarIdiomaGuardado() {
    var lang = localStorage.getItem('idioma') || 'es';
    cambiarIdioma(lang);
}

// ===== NAVEGACIÓN ACTIVA =====
function actualizarTabActivo() {
    var currentPath = window.location.pathname;
    document.querySelectorAll('.tab').forEach(function (tab) {
        tab.classList.remove('active');
        var href = tab.getAttribute('href');
        if (href && currentPath.includes(href)) {
            tab.classList.add('active');
        }
    });
}

// ===== INICIALIZAR =====
document.addEventListener('DOMContentLoaded', function () {
    cargarIdiomaGuardado();
    actualizarTabActivo();
});