// ============================================================
// REPORTES.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Reportes cargado');

    // ===== Click en tarjetas de reporte =====
    document.querySelectorAll('.reporte-card').forEach(function (card) {
        card.addEventListener('click', function (e) {
            // Si el click es en un enlace, no hacer nada
            if (e.target.closest('a')) return;

            var titulo = this.querySelector('strong')?.textContent || 'Reporte';
            mostrarToast('📋 Abriendo: ' + titulo, 'info');
        });
    });

    // ===== Animación de entrada de tarjetas =====
    var cards = document.querySelectorAll('.reporte-card');
    cards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(function () {
            card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100 + (index * 100));
    });
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