// ==========================================
// CUENTA / REGISTRO - JAVASCRIPT
// ==========================================

function autocompletarPorCedula() {
    var cedula = document.getElementById('cedula');
    if (!cedula) return;

    var valor = cedula.value.trim();
    if (valor.length < 9) {
        var nombre = document.getElementById('nombre');
        var apellidos = document.getElementById('apellidos');
        var nises = document.getElementById('nises');
        if (nombre) nombre.value = '';
        if (apellidos) apellidos.value = '';
        if (nises) nises.value = '';
        return;
    }

    var hint = document.querySelector('.registro-hint');
    if (hint) {
        hint.textContent = '⏳ Validando cédula con el TSE...';
        hint.style.color = '#1E23E6';
    }

    $.ajax({
        url: '/Cuenta/ValidarCedula',
        type: 'POST',
        data: { cedula: valor },
        success: function (response) {
            var nombre = document.getElementById('nombre');
            var apellidos = document.getElementById('apellidos');
            var nises = document.getElementById('nises');

            if (response.success) {
                if (nombre) nombre.value = response.nombre;
                if (apellidos) apellidos.value = response.apellidos;
                if (nises) nises.value = response.nises.join(', ');

                if (hint) {
                    hint.textContent = '✅ Datos cargados automáticamente desde el TSE';
                    hint.style.color = '#2E7D32';
                }
            } else {
                if (nombre) nombre.value = '';
                if (apellidos) apellidos.value = '';
                if (nises) nises.value = '';

                if (hint) {
                    hint.textContent = '⚠️ ' + response.message;
                    hint.style.color = '#C62828';
                }
            }
        },
        error: function () {
            if (hint) {
                hint.textContent = '❌ Error al validar la cédula. Intente nuevamente.';
                hint.style.color = '#C62828';
            }
        }
    });
}

function recuperarContraseña() {
    var correo = prompt('📧 Ingrese su correo electrónico para recuperar su contraseña:');
    if (!correo) return;

    if (!correo.includes('@') || !correo.includes('.')) {
        alert('❌ Ingrese un correo electrónico válido');
        return;
    }

    var btn = document.querySelector('.btn-login');
    var textoOriginal = btn ? btn.textContent : '';
    if (btn) {
        btn.textContent = '⏳ Enviando...';
        btn.disabled = true;
    }

    $.ajax({
        url: '/Cuenta/RecuperarContraseña',
        type: 'POST',
        data: { correo: correo },
        success: function (response) {
            alert(response.message);
        },
        error: function () {
            alert('❌ Error al procesar la solicitud. Intente nuevamente.');
        },
        complete: function () {
            if (btn) {
                btn.textContent = textoOriginal;
                btn.disabled = false;
            }
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    var cedulaInput = document.getElementById('cedula');
    if (cedulaInput) {
        cedulaInput.addEventListener('blur', function () {
            autocompletarPorCedula();
        });
    }
});