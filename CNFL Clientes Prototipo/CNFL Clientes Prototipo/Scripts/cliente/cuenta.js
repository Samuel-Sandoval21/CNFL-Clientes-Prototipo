// ==========================================
// CUENTA / LOGIN - JAVASCRIPT COMPLETO
// ==========================================

// ==========================================================
// ===== TOGGLE PASSWORD - LOGIN =====
// ==========================================================
function togglePasswordLogin() {
    var passwordInput = document.getElementById('Contraseña');
    if (!passwordInput) return;

    var button = document.getElementById('togglePasswordBtn');
    if (!button) return;

    var isPassword = passwordInput.type === 'password';
    passwordInput.type = isPassword ? 'text' : 'password';

    button.innerHTML = isPassword
        ? '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
        : '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
}

// ==========================================================
// ===== TOGGLE PASSWORD - REGISTRO Y RECUPERAR =====
// ==========================================================
function togglePasswordVisibility() {
    var passwordInput = document.getElementById('contrasena') || document.getElementById('Contraseña');
    if (!passwordInput) return;

    var container = passwordInput.closest('.inp');
    if (!container) return;

    var button = container.querySelector('.toggle-password');
    if (!button) return;

    var isPassword = passwordInput.type === 'password';
    passwordInput.type = isPassword ? 'text' : 'password';

    button.innerHTML = isPassword
        ? '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
        : '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
}

// ==========================================================
// ===== FACE ID =====
// ==========================================================
function iniciarFaceId() {
    var rolElement = document.getElementById('sessionRol');
    var rol = rolElement ? rolElement.value : '';

    if (rol && rol !== '') {
        if (rol === 'Admin') {
            window.location.href = '/Admin/Dashboard';
        } else {
            window.location.href = '/Clientes/Inicio';
        }
        return;
    }

    var userName = document.getElementById('UserName');
    var password = document.getElementById('Contraseña');

    if (userName && password) {
        userName.value = 'cliente';
        password.value = '123456';

        var btnFaceId = document.getElementById('faceIdBtn');
        if (btnFaceId) {
            btnFaceId.classList.add('scanning');
            btnFaceId.style.background = 'linear-gradient(135deg, #2E7D32, #64B95A)';
        }

        setTimeout(function () {
            var form = document.getElementById('loginForm');
            if (form) {
                form.submit();
            }
        }, 600);
    } else {
        alert('❌ Error al iniciar sesión con Face ID. Usa tus credenciales.');
    }
}

// ==========================================================
// ===== RECUPERAR CONTRASEÑA =====
// ==========================================================
function recuperarContraseña() {
    window.location.href = '/Cuenta/RecuperarClave';
}

// ==========================================================
// ===== VALIDACIONES DE REGISTRO =====
// ==========================================================

function mostrarError(id, mensaje) {
    var el = document.getElementById(id);
    if (el) {
        el.textContent = mensaje;
        el.classList.add('show');
    }
}

function limpiarError(id) {
    var el = document.getElementById(id);
    if (el) {
        el.classList.remove('show');
    }
}

function marcarValido(groupId) {
    var group = document.getElementById(groupId);
    if (group) {
        group.classList.remove('invalid');
        group.classList.add('valid');
    }
}

function marcarInvalido(groupId) {
    var group = document.getElementById(groupId);
    if (group) {
        group.classList.remove('valid');
        group.classList.add('invalid');
    }
}

function limpiarEstado(groupId) {
    var group = document.getElementById(groupId);
    if (group) {
        group.classList.remove('valid', 'invalid');
    }
}

function validarCedula(cedula) {
    var errorId = 'cedulaError';
    var groupId = 'cedulaGroup';
    var hint = document.querySelector('#cedulaGroup .registro-hint');

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!cedula || cedula.trim() === '') {
        mostrarError(errorId, '⚠️ Ingrese su número de cédula');
        marcarInvalido(groupId);
        return false;
    }

    var formatoValido = /^\d{1}-\d{4}-\d{4}$/.test(cedula) || /^\d{9,10}$/.test(cedula);
    if (!formatoValido) {
        mostrarError(errorId, '❌ Formato inválido. Use 1-2345-6789');
        marcarInvalido(groupId);
        return false;
    }

    $.ajax({
        url: '/Cuenta/ValidarCedula',
        type: 'POST',
        data: { cedula: cedula },
        async: false,
        success: function (response) {
            if (response.success) {
                marcarValido(groupId);
                if (hint) {
                    hint.textContent = '✅ Cédula verificada en el TSE';
                    hint.className = 'registro-hint success';
                }
            } else {
                mostrarError(errorId, '❌ ' + response.message);
                marcarInvalido(groupId);
                if (hint) {
                    hint.textContent = '⚠️ ' + response.message;
                    hint.className = 'registro-hint error';
                }
            }
        },
        error: function () {
            mostrarError(errorId, '❌ Error al validar la cédula');
            marcarInvalido(groupId);
        }
    });

    return true;
}

function validarNombre(nombre) {
    var errorId = 'nombreError';
    var groupId = 'nombreGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!nombre || nombre.trim() === '') {
        mostrarError(errorId, '⚠️ El nombre es obligatorio');
        marcarInvalido(groupId);
        return false;
    }

    if (nombre.trim().length < 2) {
        mostrarError(errorId, '⚠️ El nombre debe tener al menos 2 caracteres');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarApellidos(apellidos) {
    var errorId = 'apellidosError';
    var groupId = 'apellidosGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!apellidos || apellidos.trim() === '') {
        mostrarError(errorId, '⚠️ Los apellidos son obligatorios');
        marcarInvalido(groupId);
        return false;
    }

    if (apellidos.trim().length < 2) {
        mostrarError(errorId, '⚠️ Los apellidos deben tener al menos 2 caracteres');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarFechaNacimiento(fecha) {
    var errorId = 'fechaError';
    var groupId = 'fechaGroup';
    var hint = document.getElementById('fechaHint');

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!fecha) {
        mostrarError(errorId, '⚠️ La fecha de nacimiento es obligatoria');
        marcarInvalido(groupId);
        return false;
    }

    var fechaNac = new Date(fecha);
    if (isNaN(fechaNac.getTime())) {
        mostrarError(errorId, '❌ Fecha inválida');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    if (hint) {
        hint.textContent = '✅ Fecha de nacimiento registrada';
        hint.className = 'registro-hint success';
    }
    return true;
}

function validarCorreo(correo) {
    var errorId = 'correoError';
    var groupId = 'correoGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!correo || correo.trim() === '') {
        mostrarError(errorId, '⚠️ El correo es obligatorio');
        marcarInvalido(groupId);
        return false;
    }

    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(correo.trim())) {
        mostrarError(errorId, '❌ Ingrese un correo electrónico válido');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarTelefono(telefono) {
    var errorId = 'telefonoError';
    var groupId = 'telefonoGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!telefono || telefono.trim() === '') {
        mostrarError(errorId, '⚠️ El teléfono es obligatorio');
        marcarInvalido(groupId);
        return false;
    }

    var formatoValido = /^\d{4}-\d{4}$/.test(telefono.trim()) || /^\d{8}$/.test(telefono.trim());
    if (!formatoValido) {
        mostrarError(errorId, '❌ Formato inválido. Use 8888-8888');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarUsuario(usuario) {
    var errorId = 'usuarioError';
    var groupId = 'usuarioGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!usuario || usuario.trim() === '') {
        mostrarError(errorId, '⚠️ El usuario es obligatorio');
        marcarInvalido(groupId);
        return false;
    }

    if (usuario.trim().length < 3) {
        mostrarError(errorId, '⚠️ El usuario debe tener al menos 3 caracteres');
        marcarInvalido(groupId);
        return false;
    }

    var disponible = false;
    $.ajax({
        url: '/Cuenta/ValidarUsuario',
        type: 'POST',
        data: { userName: usuario.trim() },
        async: false,
        success: function (response) {
            if (response.success) {
                marcarValido(groupId);
                disponible = true;
            } else {
                mostrarError(errorId, '❌ ' + response.message);
                marcarInvalido(groupId);
                disponible = false;
            }
        },
        error: function () {
            mostrarError(errorId, '❌ Error al validar el usuario');
            marcarInvalido(groupId);
            disponible = false;
        }
    });

    return disponible;
}

function validarContrasena(contrasena) {
    var errorId = 'contrasenaError';
    var groupId = 'contrasenaGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!contrasena || contrasena.trim() === '') {
        mostrarError(errorId, '⚠️ La contraseña es obligatoria');
        marcarInvalido(groupId);
        return false;
    }

    if (contrasena.trim().length < 6) {
        mostrarError(errorId, '⚠️ La contraseña debe tener al menos 6 caracteres');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarConfirmacion(confirm) {
    var errorId = 'confirmError';
    var groupId = 'confirmGroup';
    var contrasena = document.getElementById('contrasena');

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!confirm || confirm.trim() === '') {
        mostrarError(errorId, '⚠️ Confirma tu contraseña');
        marcarInvalido(groupId);
        return false;
    }

    if (!contrasena || confirm.trim() !== contrasena.value.trim()) {
        mostrarError(errorId, '❌ Las contraseñas no coinciden');
        marcarInvalido(groupId);
        return false;
    }

    marcarValido(groupId);
    return true;
}

function validarNISE(nise) {
    var errorId = 'nisesError';
    var groupId = 'nisesGroup';

    limpiarError(errorId);
    limpiarEstado(groupId);

    if (!nise || nise.trim() === '' || nise === '-- Selecciona un NISE --' || nise === '-- No hay NISEs asociados --') {
        mostrarError(errorId, '⚠️ Selecciona un NISE válido');
        marcarInvalido(groupId);
        return false;
    }

    if (!/^\d{9}$/.test(nise.trim())) {
        mostrarError(errorId, '❌ El NISE debe tener 9 dígitos');
        marcarInvalido(groupId);
        return false;
    }

    var valido = false;
    $.ajax({
        url: '/Cuenta/ValidarFormatoNISE',
        type: 'POST',
        data: { nise: nise.trim() },
        async: false,
        success: function (response) {
            if (response.success) {
                marcarValido(groupId);
                valido = true;
            } else {
                mostrarError(errorId, '❌ ' + response.message);
                marcarInvalido(groupId);
                valido = false;
            }
        },
        error: function () {
            mostrarError(errorId, '❌ Error al validar el NISE');
            marcarInvalido(groupId);
            valido = false;
        }
    });

    return valido;
}

function mostrarNisesComoTags(nises, niseSeleccionado) {
    var container = document.getElementById('nisesTags');
    if (!container) return;

    container.innerHTML = '';
    if (!nises || nises.length === 0) {
        container.innerHTML = '<span style="font-size:12px; color:var(--muted);">No hay NISEs asociados</span>';
        return;
    }

    nises.forEach(function (nise) {
        var tag = document.createElement('span');
        tag.className = 'nise-tag';
        if (nise === niseSeleccionado) {
            tag.classList.add('active');
            tag.innerHTML = '✅ ' + nise;
        } else {
            tag.textContent = '⚡ ' + nise;
        }
        container.appendChild(tag);
    });
}

function autocompletarPorCedula() {
    var cedula = document.getElementById('cedula');
    if (!cedula) return;

    var valor = cedula.value.trim();
    var hint = document.querySelector('#cedulaGroup .registro-hint');

    if (valor.length < 9) {
        document.getElementById('nombre').value = '';
        document.getElementById('apellidos').value = '';
        document.getElementById('fechaNacimiento').value = '';
        var niseSelect = document.getElementById('niseSelect');
        if (niseSelect) {
            niseSelect.innerHTML = '<option value="">-- Selecciona un NISE --</option>';
            niseSelect.disabled = true;
        }
        document.getElementById('nisesTags').innerHTML = '';
        document.getElementById('niseSelectContainer').style.display = 'none';
        if (hint) {
            hint.textContent = 'Al escribir tu cédula, se cargan tus datos y tus NISEs automáticamente.';
            hint.className = 'registro-hint';
            hint.style.color = '';
        }
        limpiarEstado('cedulaGroup');
        limpiarError('cedulaError');
        return;
    }

    if (hint) {
        hint.textContent = '⏳ Validando cédula con el TSE...';
        hint.className = 'registro-hint';
        hint.style.color = '#1E23E6';
    }

    $.ajax({
        url: '/Cuenta/ValidarCedula',
        type: 'POST',
        data: { cedula: valor },
        success: function (response) {
            var nombre = document.getElementById('nombre');
            var apellidos = document.getElementById('apellidos');
            var niseSelect = document.getElementById('niseSelect');
            var fechaInput = document.getElementById('fechaNacimiento');
            var container = document.getElementById('niseSelectContainer');

            if (response.success) {
                if (nombre) nombre.value = response.nombre;
                if (apellidos) apellidos.value = response.apellidos;

                if (fechaInput && response.fechaNacimiento) {
                    fechaInput.value = response.fechaNacimiento;
                    validarFechaNacimiento(response.fechaNacimiento);
                }

                if (niseSelect && response.nises && response.nises.length > 0) {
                    if (container) container.style.display = 'block';

                    niseSelect.innerHTML = '';
                    response.nises.forEach(function (nise) {
                        var option = document.createElement('option');
                        option.value = nise;
                        option.textContent = nise;
                        niseSelect.appendChild(option);
                    });

                    var primerNise = response.nises[0];
                    niseSelect.value = primerNise;
                    niseSelect.disabled = false;

                    validarNISE(primerNise);
                    mostrarNisesComoTags(response.nises, primerNise);

                } else {
                    if (container) container.style.display = 'none';
                    if (niseSelect) {
                        niseSelect.innerHTML = '<option value="">-- No hay NISEs asociados --</option>';
                        niseSelect.disabled = true;
                    }
                    document.getElementById('nisesTags').innerHTML = '';
                }

                if (hint) {
                    hint.textContent = '✅ Datos cargados automáticamente desde el TSE';
                    hint.className = 'registro-hint success';
                }

                validarNombre(response.nombre);
                validarApellidos(response.apellidos);
                marcarValido('cedulaGroup');

            } else {
                if (nombre) nombre.value = '';
                if (apellidos) apellidos.value = '';
                if (fechaInput) fechaInput.value = '';
                if (niseSelect) {
                    niseSelect.innerHTML = '<option value="">-- Selecciona un NISE --</option>';
                    niseSelect.disabled = true;
                }
                if (container) container.style.display = 'none';
                document.getElementById('nisesTags').innerHTML = '';

                if (hint) {
                    hint.textContent = '⚠️ ' + response.message;
                    hint.className = 'registro-hint error';
                }
                marcarInvalido('cedulaGroup');
            }
        },
        error: function () {
            if (hint) {
                hint.textContent = '❌ Error al validar la cédula. Intente nuevamente.';
                hint.className = 'registro-hint error';
            }
        }
    });
}

function validarFormularioRegistro() {
    var errores = [];

    var cedula = document.getElementById('cedula');
    var nombre = document.getElementById('nombre');
    var apellidos = document.getElementById('apellidos');
    var correo = document.getElementById('correo');
    var telefono = document.getElementById('telefono');
    var usuario = document.getElementById('userName');
    var contrasena = document.getElementById('contrasena');
    var confirm = document.getElementById('confirm');
    var niseSelect = document.getElementById('niseSelect');
    var fecha = document.getElementById('fechaNacimiento');

    if (!validarCedula(cedula ? cedula.value : '')) errores.push('Cédula inválida');
    if (!validarNombre(nombre ? nombre.value : '')) errores.push('Nombre obligatorio');
    if (!validarApellidos(apellidos ? apellidos.value : '')) errores.push('Apellidos obligatorios');
    if (!validarCorreo(correo ? correo.value : '')) errores.push('Correo inválido');
    if (!validarTelefono(telefono ? telefono.value : '')) errores.push('Teléfono inválido');
    if (!validarUsuario(usuario ? usuario.value : '')) errores.push('Usuario inválido');
    if (!validarContrasena(contrasena ? contrasena.value : '')) errores.push('Contraseña inválida');
    if (!validarConfirmacion(confirm ? confirm.value : '')) errores.push('Las contraseñas no coinciden');
    if (!validarFechaNacimiento(fecha ? fecha.value : '')) errores.push('Fecha de nacimiento inválida');

    var niseValue = niseSelect ? niseSelect.value : '';
    if (niseValue && niseValue !== '' && niseValue !== '-- Selecciona un NISE --' && niseValue !== '-- No hay NISEs asociados --') {
        if (!validarNISE(niseValue)) errores.push('NISE inválido');
    } else {
        errores.push('Selecciona un NISE');
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

    // ==========================================================
    // EVENTOS DE LOGIN
    // ==========================================================

    var userName = document.getElementById('UserName');
    if (userName) {
        userName.addEventListener('blur', function () {
            if (this.value.trim() === '') {
                document.getElementById('usuarioError').textContent = '⚠️ Ingrese su usuario';
                document.getElementById('usuarioError').classList.add('show');
                document.getElementById('usuarioGroup').classList.add('invalid');
            } else {
                document.getElementById('usuarioError').classList.remove('show');
                document.getElementById('usuarioGroup').classList.remove('invalid');
                document.getElementById('usuarioGroup').classList.add('valid');
            }
        });
        userName.addEventListener('input', function () {
            document.getElementById('usuarioError').classList.remove('show');
            document.getElementById('usuarioGroup').classList.remove('invalid');
            if (this.value.trim() !== '') {
                document.getElementById('usuarioGroup').classList.add('valid');
            } else {
                document.getElementById('usuarioGroup').classList.remove('valid');
            }
        });
    }

    var contrasena = document.getElementById('Contraseña');
    if (contrasena) {
        contrasena.addEventListener('blur', function () {
            if (this.value.trim() === '') {
                document.getElementById('contrasenaError').textContent = '⚠️ Ingrese su contraseña';
                document.getElementById('contrasenaError').classList.add('show');
                document.getElementById('contrasenaGroup').classList.add('invalid');
            } else {
                document.getElementById('contrasenaError').classList.remove('show');
                document.getElementById('contrasenaGroup').classList.remove('invalid');
                document.getElementById('contrasenaGroup').classList.add('valid');
            }
        });
        contrasena.addEventListener('input', function () {
            document.getElementById('contrasenaError').classList.remove('show');
            document.getElementById('contrasenaGroup').classList.remove('invalid');
            if (this.value.trim() !== '') {
                document.getElementById('contrasenaGroup').classList.add('valid');
            } else {
                document.getElementById('contrasenaGroup').classList.remove('valid');
            }
        });
    }

    var loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            var userName = document.getElementById('UserName');
            var contrasena = document.getElementById('Contraseña');
            var errores = [];

            if (!userName || userName.value.trim() === '') {
                errores.push('Usuario obligatorio');
                if (document.getElementById('usuarioError')) {
                    document.getElementById('usuarioError').textContent = '⚠️ Ingrese su usuario';
                    document.getElementById('usuarioError').classList.add('show');
                }
                document.getElementById('usuarioGroup').classList.add('invalid');
            }

            if (!contrasena || contrasena.value.trim() === '') {
                errores.push('Contraseña obligatoria');
                if (document.getElementById('contrasenaError')) {
                    document.getElementById('contrasenaError').textContent = '⚠️ Ingrese su contraseña';
                    document.getElementById('contrasenaError').classList.add('show');
                }
                document.getElementById('contrasenaGroup').classList.add('invalid');
            }

            if (errores.length > 0) {
                e.preventDefault();
                alert('❌ Por favor, complete todos los campos.');
            }
        });
    }

    // ==========================================================
    // EVENTOS DE REGISTRO
    // ==========================================================

    var cedula = document.getElementById('cedula');
    if (cedula) {
        cedula.addEventListener('blur', function () {
            if (this.value.length >= 9) {
                validarCedula(this.value);
                autocompletarPorCedula();
            }
        });
        cedula.addEventListener('input', function () {
            limpiarError('cedulaError');
            limpiarEstado('cedulaGroup');
            if (this.value.length < 9) {
                document.getElementById('nombre').value = '';
                document.getElementById('apellidos').value = '';
                document.getElementById('fechaNacimiento').value = '';
                var niseSelect = document.getElementById('niseSelect');
                if (niseSelect) {
                    niseSelect.innerHTML = '<option value="">-- Selecciona un NISE --</option>';
                    niseSelect.disabled = true;
                }
                document.getElementById('nisesTags').innerHTML = '';
                document.getElementById('niseSelectContainer').style.display = 'none';
                var hint = document.querySelector('#cedulaGroup .registro-hint');
                if (hint) {
                    hint.textContent = 'Al escribir tu cédula, se cargan tus datos y tus NISEs automáticamente.';
                    hint.className = 'registro-hint';
                    hint.style.color = '';
                }
                limpiarEstado('cedulaGroup');
            }
        });
    }

    var nombre = document.getElementById('nombre');
    if (nombre) {
        nombre.addEventListener('blur', function () {
            validarNombre(this.value);
        });
        nombre.addEventListener('input', function () {
            limpiarError('nombreError');
            limpiarEstado('nombreGroup');
        });
    }

    var apellidos = document.getElementById('apellidos');
    if (apellidos) {
        apellidos.addEventListener('blur', function () {
            validarApellidos(this.value);
        });
        apellidos.addEventListener('input', function () {
            limpiarError('apellidosError');
            limpiarEstado('apellidosGroup');
        });
    }

    var fecha = document.getElementById('fechaNacimiento');
    if (fecha) {
        fecha.addEventListener('blur', function () {
            validarFechaNacimiento(this.value);
        });
        fecha.addEventListener('change', function () {
            validarFechaNacimiento(this.value);
        });
    }

    var correo = document.getElementById('correo');
    if (correo) {
        correo.addEventListener('blur', function () {
            validarCorreo(this.value);
        });
        correo.addEventListener('input', function () {
            limpiarError('correoError');
            limpiarEstado('correoGroup');
        });
    }

    var telefono = document.getElementById('telefono');
    if (telefono) {
        telefono.addEventListener('blur', function () {
            validarTelefono(this.value);
        });
        telefono.addEventListener('input', function () {
            limpiarError('telefonoError');
            limpiarEstado('telefonoGroup');
        });
    }

    var usuario = document.getElementById('userName');
    if (usuario) {
        usuario.addEventListener('blur', function () {
            validarUsuario(this.value);
        });
        usuario.addEventListener('input', function () {
            limpiarError('usuarioError');
            limpiarEstado('usuarioGroup');
        });
    }

    var contrasenaReg = document.getElementById('contrasena');
    if (contrasenaReg) {
        contrasenaReg.addEventListener('blur', function () {
            validarContrasena(this.value);
        });
        contrasenaReg.addEventListener('input', function () {
            limpiarError('contrasenaError');
            limpiarEstado('contrasenaGroup');
            var confirm = document.getElementById('confirm');
            if (confirm && confirm.value) {
                validarConfirmacion(confirm.value);
            }
        });
    }

    var confirm = document.getElementById('confirm');
    if (confirm) {
        confirm.addEventListener('blur', function () {
            validarConfirmacion(this.value);
        });
        confirm.addEventListener('input', function () {
            validarConfirmacion(this.value);
        });
    }

    var niseSelect = document.getElementById('niseSelect');
    if (niseSelect) {
        niseSelect.addEventListener('change', function () {
            var selectedValue = this.value;
            if (selectedValue && selectedValue !== '' && selectedValue !== '-- Selecciona un NISE --') {
                validarNISE(selectedValue);
                var nises = [];
                for (var i = 0; i < this.options.length; i++) {
                    if (this.options[i].value && this.options[i].value !== '') {
                        nises.push(this.options[i].value);
                    }
                }
                mostrarNisesComoTags(nises, selectedValue);
            }
        });
    }

    var registroForm = document.getElementById('registroForm');
    if (registroForm) {
        registroForm.addEventListener('submit', function (e) {
            if (!validarFormularioRegistro()) {
                e.preventDefault();
            }
        });
    }

});