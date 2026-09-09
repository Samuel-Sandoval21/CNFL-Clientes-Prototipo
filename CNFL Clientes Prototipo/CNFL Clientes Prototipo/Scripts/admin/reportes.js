// ============================================================
// ADMIN REPORTES.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Admin Reportes cargado');

    // ===== Botones de generar reportes =====
    document.querySelectorAll('.btn-generar').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            var textoOriginal = this.textContent;
            var tipo = this.getAttribute('href')?.split('=').pop() || 'reporte';
            this.textContent = '⏳ Generando...';
            this.disabled = true;

            var nombres = {
                averias: 'Averías',
                clientes: 'Clientes',
                estadisticas: 'Estadísticas',
                consumo: 'Consumo'
            };

            mostrarToast('📄 Generando reporte de ' + (nombres[tipo] || tipo) + '...', 'info');

            setTimeout(function () {
                btn.textContent = '✅ Descargar PDF';
                btn.disabled = false;
                mostrarToast('✅ Reporte generado correctamente', 'success');
                // Simular descarga
                setTimeout(function () {
                    btn.textContent = textoOriginal;
                }, 3000);
            }, 2000);
        });
    });

    // ===== Click en tarjetas de reporte =====
    document.querySelectorAll('.reporte-card').forEach(function (card) {
        card.addEventListener('click', function (e) {
            if (e.target.closest('a')) return;
            var titulo = this.querySelector('h3')?.textContent || 'Reporte';
            mostrarToast('📊 Abriendo ' + titulo, 'info');
        });
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