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

    // Cerrar modal al hacer clic fuera
    var modal = document.getElementById('modalCierre');
    if (modal) {
        modal.addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModalCierre();
            }
        });
    }

    // Cerrar modal con tecla ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            cerrarModalCierre();
        }
    });

    // Prevenir comportamiento en enlaces "#"
    document.querySelectorAll('.nav-item[href="#"]').forEach(function (item) {
        item.addEventListener('click', function (e) {
            e.preventDefault();
        });
    });

    // Marcar ítem activo del menú
    var currentPath = window.location.pathname;
    document.querySelectorAll('.nav-item').forEach(function (item) {
        var href = item.getAttribute('href');
        if (href && href !== '#' && currentPath.includes(href)) {
            item.classList.add('active');
        }
    });

});

// ==========================================================
// ===== CAMBIAR IDIOMA =====
// ==========================================================

function cambiarIdiomaAdmin(lang) {
    var btn = document.querySelector('.lang-toggle-top');
    if (btn) {
        btn.textContent = lang === 'es' ? 'ES ▼' : 'EN ▼';
    }
    localStorage.setItem('idioma_admin', lang);
    mostrarToastAdmin('✅ Idioma cambiado a ' + (lang === 'es' ? 'Español' : 'English'));
}

// Cargar idioma guardado
(function cargarIdiomaGuardado() {
    var lang = localStorage.getItem('idioma_admin') || 'es';
    var btn = document.querySelector('.lang-toggle-top');
    if (btn) {
        btn.textContent = lang === 'es' ? 'ES ▼' : 'EN ▼';
    }
})();

// ==========================================================
// ===== TOAST DE NOTIFICACIÓN =====
// ==========================================================

function mostrarToastAdmin(mensaje, tipo) {
    var toast = document.getElementById('toastNotifAdmin');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastNotifAdmin';
        toast.style.cssText = `
            position: fixed;
            bottom: 100px;
            left: 50%;
            transform: translateX(-50%);
            background: #2E7D32;
            color: white;
            padding: 12px 24px;
            border-radius: 16px;
            font-weight: 600;
            font-size: 14px;
            box-shadow: 0 8px 30px rgba(0,0,0,0.2);
            z-index: 2000;
            display: none;
            max-width: 90%;
            text-align: center;
            font-family: 'Inter', sans-serif;
        `;
        document.body.appendChild(toast);

        if (!document.getElementById('toastAdminStyles')) {
            var style = document.createElement('style');
            style.id = 'toastAdminStyles';
            style.textContent = `
                @keyframes slideUpToast {
                    from { opacity: 0; transform: translateX(-50%) translateY(20px); }
                    to { opacity: 1; transform: translateX(-50%) translateY(0); }
                }
                #toastNotifAdmin.show {
                    display: block;
                    animation: slideUpToast 0.3s ease;
                }
            `;
            document.head.appendChild(style);
        }
    }

    toast.textContent = mensaje;
    toast.className = 'show';

    if (tipo === 'error') {
        toast.style.background = '#C62828';
    } else if (tipo === 'warning') {
        toast.style.background = '#F57F17';
    } else {
        toast.style.background = '#2E7D32';
    }

    toast.classList.add('show');

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.classList.remove('show');
        setTimeout(function () {
            toast.style.display = 'none';
        }, 300);
    }, 3000);
}

// ==========================================================
// ===== FUNCIONES ADICIONALES =====
// ==========================================================

function generarReporteAdmin() {
    mostrarToastAdmin('📊 Generando reporte...');
    setTimeout(function () {
        mostrarToastAdmin('✅ Reporte generado correctamente');
    }, 1500);
}

function exportarReporteAdmin(formato) {
    var nombres = { 'pdf': 'PDF', 'excel': 'Excel', 'csv': 'CSV' };
    mostrarToastAdmin('📥 Exportando ' + (nombres[formato] || formato) + '...');
    setTimeout(function () {
        mostrarToastAdmin('✅ Archivo exportado correctamente');
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
            mostrarToastAdmin('✅ Estado actualizado a: ' + estado);
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
window.mostrarToastAdmin = mostrarToastAdmin;
window.generarReporteAdmin = generarReporteAdmin;
window.exportarReporteAdmin = exportarReporteAdmin;
window.filtrarClientesAdmin = filtrarClientesAdmin;
window.actualizarEstadoAveria = actualizarEstadoAveria;