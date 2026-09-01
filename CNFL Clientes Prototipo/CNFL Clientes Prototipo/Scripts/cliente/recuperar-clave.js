// ==========================================
// RECUPERAR CONTRASEÑA - JAVASCRIPT
// ==========================================

// ==========================================================
// ===== TOGGLE PASSWORD - ESPECÍFICO PARA RECUPERAR =====
// ==========================================================
function togglePasswordVisibility() {
    var button = event ? event.target.closest('.toggle-password') : null;

    if (!button) {
        button = document.querySelector('.toggle-password');
    }

    if (!button) {
        console.warn('No se encontró el botón toggle password');
        return;
    }

    var container = button.closest('.inp');
    if (!container) {
        console.warn('No se encontró el contenedor .inp');
        return;
    }

    var passwordInput = container.querySelector('input[type="password"], input[type="text"]');
    if (!passwordInput) {
        console.warn('No se encontró el input de contraseña');
        return;
    }

    var isPassword = passwordInput.type === 'password';
    passwordInput.type = isPassword ? 'text' : 'password';

    button.innerHTML = isPassword
        ? '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
        : '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
}

// ==========================================================
// ===== VALIDAR CONTRASEÑA EN TIEMPO REAL =====
// ==========================================================
function validarContrasenaRecuperar() {
    var nuevaClave = document.getElementById('nuevaClave');
    var confirmarClave = document.getElementById('confirmarClave');
    var fuerzaBar = document.getElementById('passwordStrength');
    var hint = document.getElementById('passwordHint');

    if (!nuevaClave) return;

    var valor = nuevaClave.value;
    var fuerza = 0;

    if (valor.length >= 6) fuerza++;
    if (valor.length >= 10) fuerza++;
    if (/[A-Z]/.test(valor)) fuerza++;
    if (/[0-9]/.test(valor)) fuerza++;
    if (/[^A-Za-z0-9]/.test(valor)) fuerza++;

    if (fuerzaBar) {
        var barras = fuerzaBar.querySelectorAll('.bar');
        var niveles = ['weak', 'weak', 'medium', 'strong', 'strong'];
        var colores = ['#C62828', '#C62828', '#F5A623', '#2E7D32', '#2E7D32'];

        barras.forEach(function (bar, index) {
            if (index < fuerza) {
                bar.style.background = colores[index] || '#2E7D32';
                bar.style.opacity = '1';
            } else {
                bar.style.background = 'var(--line)';
                bar.style.opacity = '0.3';
            }
        });
    }

    if (hint) {
        if (valor.length === 0) {
            hint.textContent = 'Mínimo 6 caracteres.';
            hint.className = 'recuperar-hint';
        } else if (valor.length < 6) {
            hint.textContent = '⚠️ Mínimo 6 caracteres.';
            hint.className = 'recuperar-hint error';
        } else if (fuerza >= 4) {
            hint.textContent = '✅ Contraseña segura.';
            hint.className = 'recuperar-hint success';
        } else if (fuerza >= 2) {
            hint.textContent = '🔐 Contraseña aceptable.';
            hint.className = 'recuperar-hint';
        } else {
            hint.textContent = '⚠️ Agrega mayúsculas, números o símbolos.';
            hint.className = 'recuperar-hint error';
        }
    }

    if (confirmarClave && confirmarClave.value.length > 0) {
        validarConfirmacionRecuperar();
    }
}

function validarConfirmacionRecuperar() {
    var nuevaClave = document.getElementById('nuevaClave');
    var confirmarClave = document.getElementById('confirmarClave');
    var hint = document.getElementById('confirmHint');
    var grupo = document.getElementById('confirmarClaveGroup');

    if (!nuevaClave || !confirmarClave) return;

    if (confirmarClave.value.length === 0) {
        if (hint) {
            hint.textContent = 'Confirma tu nueva contraseña.';
            hint.className = 'recuperar-hint';
        }
        if (grupo) {
            grupo.classList.remove('valid', 'invalid');
        }
        return;
    }

    if (nuevaClave.value === confirmarClave.value) {
        if (hint) {
            hint.textContent = '✅ Las contraseñas coinciden.';
            hint.className = 'recuperar-hint success';
        }
        if (grupo) {
            grupo.classList.remove('invalid');
            grupo.classList.add('valid');
        }
    } else {
        if (hint) {
            hint.textContent = '❌ Las contraseñas no coinciden.';
            hint.className = 'recuperar-hint error';
        }
        if (grupo) {
            grupo.classList.remove('valid');
            grupo.classList.add('invalid');
        }
    }
}

function validarCorreoRecuperar() {
    var correo = document.getElementById('correoRecuperar');
    var hint = document.getElementById('correoHint');
    var grupo = document.getElementById('correoRecuperarGroup');

    if (!correo) return;

    var valor = correo.value.trim();

    if (valor.length === 0) {
        if (hint) {
            hint.textContent = 'Recibirás un enlace de verificación.';
            hint.className = 'recuperar-hint';
        }
        if (grupo) {
            grupo.classList.remove('valid', 'invalid');
        }
        return;
    }

    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (regex.test(valor)) {
        if (hint) {
            hint.textContent = '✅ Correo válido.';
            hint.className = 'recuperar-hint success';
        }
        if (grupo) {
            grupo.classList.remove('invalid');
            grupo.classList.add('valid');
        }
    } else {
        if (hint) {
            hint.textContent = '❌ Ingresa un correo electrónico válido.';
            hint.className = 'recuperar-hint error';
        }
        if (grupo) {
            grupo.classList.remove('valid');
            grupo.classList.add('invalid');
        }
    }
}

function validarFormularioRecuperar() {
    var correo = document.getElementById('correoRecuperar');
    var nuevaClave = document.getElementById('nuevaClave');
    var confirmarClave = document.getElementById('confirmarClave');
    var errores = [];

    if (!correo || !correo.value.trim()) {
        errores.push('Ingresa tu correo electrónico.');
    } else {
        var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!regex.test(correo.value.trim())) {
            errores.push('Ingresa un correo electrónico válido.');
        }
    }

    if (!nuevaClave || !nuevaClave.value.trim()) {
        errores.push('Ingresa una nueva contraseña.');
    } else if (nuevaClave.value.length < 6) {
        errores.push('La contraseña debe tener al menos 6 caracteres.');
    }

    if (!confirmarClave || !confirmarClave.value.trim()) {
        errores.push('Confirma tu nueva contraseña.');
    } else if (nuevaClave && nuevaClave.value !== confirmarClave.value) {
        errores.push('Las contraseñas no coinciden.');
    }

    if (errores.length > 0) {
        alert('❌ Por favor, corrige los siguientes errores:\n\n- ' + errores.join('\n- '));
        return false;
    }

    return true;
}

// ==========================================================
// ===== INICIALIZAR EVENTOS =====
// ==========================================================
document.addEventListener('DOMContentLoaded', function () {

    var correo = document.getElementById('correoRecuperar');
    if (correo) {
        correo.addEventListener('blur', validarCorreoRecuperar);
        correo.addEventListener('input', validarCorreoRecuperar);
    }

    var nuevaClave = document.getElementById('nuevaClave');
    if (nuevaClave) {
        nuevaClave.addEventListener('blur', validarContrasenaRecuperar);
        nuevaClave.addEventListener('input', validarContrasenaRecuperar);
    }

    var confirmarClave = document.getElementById('confirmarClave');
    if (confirmarClave) {
        confirmarClave.addEventListener('blur', validarConfirmacionRecuperar);
        confirmarClave.addEventListener('input', validarConfirmacionRecuperar);
    }

    var form = document.getElementById('recuperarForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validarFormularioRecuperar()) {
                e.preventDefault();
            }
        });
    }

    console.log('🔐 Recuperar contraseña cargado correctamente.');
});