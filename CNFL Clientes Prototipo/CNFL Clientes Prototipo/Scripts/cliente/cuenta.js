// ============================================================
// CUENTA.JS - FUNCIONES PARA LOGIN Y REGISTRO
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Cuenta JS cargado');

    // ===== Face ID =====
    window.iniciarFaceId = function () {
        alert('🔐 Simulación de Face ID. Iniciando sesión...');
        window.location.href = '/Clientes/Dashboard';
    };

    // ===== Validación de formulario de login =====
    var loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            var usuario = document.getElementById('UserName');
            var contrasena = document.getElementById('Contraseña');

            if (usuario && contrasena && (!usuario.value.trim() || !contrasena.value.trim())) {
                e.preventDefault();
                alert('Por favor complete todos los campos.');
            }
        });
    }
});