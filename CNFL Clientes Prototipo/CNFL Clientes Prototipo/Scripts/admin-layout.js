// ==========================================
// ADMIN LAYOUT - JAVASCRIPT
// ==========================================

// ===== RELOJ =====
function updateTime() {
    var now = new Date();
    var h = String(now.getHours()).padStart(2, '0');
    var m = String(now.getMinutes()).padStart(2, '0');
    document.getElementById('statusTime').textContent = h + ':' + m;
}
updateTime();
setInterval(updateTime, 30000);

// ===== MODAL DE CIERRE =====
function mostrarModalCierre() {
    document.getElementById('modalCierre').classList.add('show');
}

function cerrarModalCierre() {
    document.getElementById('modalCierre').classList.remove('show');
}

function confirmarCierre() {
    cerrarModalCierre();
    // Usar la URL pasada desde el layout
    if (window.logoutUrl) {
        window.location.href = window.logoutUrl;
    } else {
        // Fallback por si no se pasó la URL
        window.location.href = '/Cuenta/Logout';
    }
}

// ===== CERRAR MODAL AL HACER CLIC FUERA =====
document.getElementById('modalCierre').addEventListener('click', function (e) {
    if (e.target === this) {
        cerrarModalCierre();
    }
});

// ===== PREVENIR COMPORTAMIENTO POR DEFECTO EN ENLACES "#" =====
document.querySelectorAll('.nav-item').forEach(function (item) {
    item.addEventListener('click', function (e) {
        if (this.getAttribute('href') === '#') {
            e.preventDefault();
        }
    });
});