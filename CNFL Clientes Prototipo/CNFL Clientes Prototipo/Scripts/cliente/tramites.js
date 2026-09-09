// ============================================================
// TRAMITES.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Trámites cargado');

    // ===== Filtro de trámites por estado =====
    var filtroEstado = document.getElementById('filtroEstado');
    if (filtroEstado) {
        filtroEstado.addEventListener('change', function () {
            var estado = this.value.toLowerCase();
            var tarjetas = document.querySelectorAll('.tramite-card');
            tarjetas.forEach(function (card) {
                var badge = card.querySelector('.estado-badge');
                if (badge) {
                    var badgeEstado = badge.textContent.toLowerCase();
                    if (estado === '' || badgeEstado === estado) {
                        card.style.display = 'block';
                    } else {
                        card.style.display = 'none';
                    }
                }
            });
        });
    }

    // ===== Click en "Ver detalle" =====
    document.querySelectorAll('.btn-ver-tramite').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            var id = this.dataset.id || '0';
            mostrarToast('📋 Cargando detalle del trámite #' + id, 'info');
        });
    });

    // ===== Animación de entrada =====
    var cards = document.querySelectorAll('.tramite-card');
    cards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(function () {
            card.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100 + (index * 80));
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