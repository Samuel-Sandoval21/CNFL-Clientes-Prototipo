// ============================================================
// RECUPERAR-CLAVE.JS - FUNCIONES PARA RECUPERACIÓN DE CONTRASEÑA
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Recuperar Clave JS cargado');

    // ===== Mostrar/Ocultar contraseña =====
    var toggleBtns = document.querySelectorAll('.toggle-password');
    toggleBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = this.closest('.inp').querySelector('input[type="password"]');
            if (input) {
                if (input.type === 'password') {
                    input.type = 'text';
                    this.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><path d="M1 1l22 22"/></svg>';
                } else {
                    input.type = 'password';
                    this.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
                }
            }
        });
    });

    // ===== Validación de formulario =====
    var form = document.getElementById('recuperarForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            var correo = document.getElementById('correoRecuperar');
            var nuevaClave = document.getElementById('nuevaClave');
            var confirmarClave = document.getElementById('confirmarClave');

            if (!correo.value.trim()) {
                e.preventDefault();
                alert('Por favor ingrese su correo electrónico.');
                correo.focus();
                return;
            }

            if (nuevaClave.value.length < 6) {
                e.preventDefault();
                alert('La contraseña debe tener al menos 6 caracteres.');
                nuevaClave.focus();
                return;
            }

            if (nuevaClave.value !== confirmarClave.value) {
                e.preventDefault();
                alert('Las contraseñas no coinciden.');
                confirmarClave.focus();
                return;
            }
        });
    }

    // ===== Validación de fortaleza de contraseña =====
    var nuevaClave = document.getElementById('nuevaClave');
    if (nuevaClave) {
        nuevaClave.addEventListener('input', function () {
            var bars = document.querySelectorAll('.password-strength .bar');
            var strength = this.value.length;

            bars.forEach(function (bar) {
                bar.className = 'bar';
            });

            if (strength === 0) return;

            if (strength < 6) {
                bars[0].classList.add('weak');
                bars[1].classList.add('weak');
            } else if (strength < 10) {
                bars[0].classList.add('medium');
                bars[1].classList.add('medium');
                bars[2].classList.add('medium');
            } else {
                bars[0].classList.add('strong');
                bars[1].classList.add('strong');
                bars[2].classList.add('strong');
                bars[3].classList.add('strong');
                bars[4].classList.add('strong');
            }
        });
    }
});