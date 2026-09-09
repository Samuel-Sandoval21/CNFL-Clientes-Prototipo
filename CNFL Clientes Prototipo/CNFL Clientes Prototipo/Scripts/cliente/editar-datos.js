// ============================================================
// EDITAR-DATOS.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Editar Datos cargado');

    var form = document.querySelector('form');
    var btnGuardar = document.querySelector('.btn-guardar');

    if (form && btnGuardar) {
        form.addEventListener('submit', function (e) {
            var campos = form.querySelectorAll('.form-control[required]');
            var valido = true;
            var primerError = null;

            campos.forEach(function (campo) {
                if (!campo.value.trim()) {
                    campo.style.borderColor = '#D32F2F';
                    valido = false;
                    if (!primerError) primerError = campo;
                } else {
                    campo.style.borderColor = '';
                }
            });

            // Validar email
            var email = document.getElementById('Correo');
            if (email && email.value.trim()) {
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(email.value.trim())) {
                    email.style.borderColor = '#D32F2F';
                    valido = false;
                    if (!primerError) primerError = email;
                }
            }

            if (!valido) {
                e.preventDefault();
                mostrarToast('⚠️ Por favor completa todos los campos obligatorios correctamente.', 'warning');
                if (primerError) primerError.focus();
            } else {
                mostrarToast('💾 Guardando cambios...', 'info');
            }
        });
    }

    // ===== Limpiar errores al escribir =====
    document.querySelectorAll('.form-control').forEach(function (campo) {
        campo.addEventListener('input', function () {
            this.style.borderColor = '';
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