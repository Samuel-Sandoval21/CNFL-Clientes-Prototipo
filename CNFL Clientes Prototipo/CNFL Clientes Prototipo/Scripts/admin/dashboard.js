// ============================================================
// ADMIN DASHBOARD.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Admin Dashboard cargado');

    // ===== Animación de números =====
    var numeros = document.querySelectorAll('.stat-number');
    numeros.forEach(function (num) {
        var valor = parseInt(num.textContent.replace(/[%,.]/g, ''));
        if (!isNaN(valor) && valor > 0) {
            animarNumero(num, valor);
        }
    });

    function animarNumero(elemento, objetivo) {
        var duracion = 800;
        var paso = 20;
        var incremento = objetivo / (duracion / paso);
        var actual = 0;
        var intervalo = setInterval(function () {
            actual += incremento;
            if (actual >= objetivo) {
                actual = objetivo;
                clearInterval(intervalo);
            }
            elemento.textContent = Math.round(actual).toLocaleString();
        }, paso);
    }

    // ===== Click en averías =====
    document.querySelectorAll('.averia-item').forEach(function (item) {
        item.addEventListener('click', function () {
            var id = this.querySelector('strong')?.textContent || '';
            mostrarToast('📋 Ver detalle de avería ' + id, 'info');
        });
    });

    // ===== Auto-refresh cada 60 segundos =====
    setInterval(function () {
        console.log('🔄 Actualizando dashboard...');
        // Aquí iría la lógica AJAX para actualizar estadísticas
    }, 60000);
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