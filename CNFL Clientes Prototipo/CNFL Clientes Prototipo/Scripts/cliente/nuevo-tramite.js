// ============================================================
// NUEVO-TRAMITE.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Nuevo Trámite cargado');

    // ===== Click en opción de trámite =====
    document.querySelectorAll('.tramite-opcion').forEach(function (opcion) {
        opcion.addEventListener('click', function (e) {
            var tipo = this.querySelector('h3')?.textContent || 'Trámite';
            if (confirm('¿Deseas iniciar el trámite: "' + tipo + '"?')) {
                mostrarToast('📋 Iniciando trámite de ' + tipo + '...', 'info');
                setTimeout(function () {
                    mostrarToast('✅ Trámite iniciado correctamente', 'success');
                }, 1500);
            } else {
                e.preventDefault();
            }
        });
    });

    // ===== Efecto hover mejorado =====
    document.querySelectorAll('.tramite-opcion').forEach(function (opcion) {
        opcion.addEventListener('mouseenter', function () {
            this.style.transition = 'transform 0.3s, box-shadow 0.3s';
        });
    });

    // ===== Botón "Volver" =====
    var btnVolver = document.querySelector('.btn-volver');
    if (btnVolver) {
        btnVolver.addEventListener('click', function (e) {
            mostrarToast('⬅️ Volviendo a trámites', 'info');
        });
    }
});

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