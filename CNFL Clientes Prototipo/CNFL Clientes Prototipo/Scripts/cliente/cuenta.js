// ============================================================
// CUENTA.JS - FUNCIONES PARA LOGIN Y REGISTRO
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Cuenta JS cargado');

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

    // ===== Face ID =====
    window.iniciarFaceId = function () {
        alert('🔐 Simulación de Face ID. Iniciando sesión...');
        // Aquí iría la lógica real de Face ID
        // Por ahora redirige al dashboard
        window.location.href = '/Clientes/Dashboard';
    };

    // ===== Abrir documentos (Política y Consentimiento) =====
    window.abrirDocumento = function (tipo) {
        var nombre = document.getElementById('nombre')?.value || 'Cliente';
        var cedula = document.getElementById('cedula')?.value || '0-0000-0000';
        var correo = document.getElementById('correo')?.value || 'cliente@correo.cr';
        var telefono = document.getElementById('telefono')?.value || '0000-0000';

        var fecha = new Date();
        var dia = fecha.getDate();
        var mes = fecha.toLocaleString('es-CR', { month: 'long' });
        var anio = fecha.getFullYear();

        var contenido = '';

        if (tipo === 'politica') {
            contenido = `
                <div style="font-family:'Inter',sans-serif; max-width:500px; margin:0 auto; padding:24px; background:white; border-radius:16px; box-shadow:0 8px 40px rgba(0,0,0,0.15);">
                    <h2 style="color:#0033A0; margin:0 0 12px;">Política de Privacidad</h2>
                    <hr style="border:1px solid #EDEFF3; margin:12px 0;" />
                    <div style="background:#f8f9fe; padding:12px; border-radius:8px; margin-bottom:12px;">
                        <p style="margin:4px 0;"><strong>Cliente:</strong> ${nombre}</p>
                        <p style="margin:4px 0;"><strong>Cédula:</strong> ${cedula}</p>
                        <p style="margin:4px 0;"><strong>Correo:</strong> ${correo}</p>
                        <p style="margin:4px 0;"><strong>Fecha:</strong> ${dia} de ${mes} de ${anio}</p>
                    </div>
                    <div style="font-size:14px; color:#0E1116; line-height:1.8;">
                        <p><strong>1. Datos Personales</strong></p>
                        <p style="color:#727A86;">Los datos recopilados son: nombre, cédula, dirección, teléfonos, correo electrónico, NISE(s) asociados.</p>
                        <p><strong>2. Finalidad</strong></p>
                        <p style="color:#727A86;">Serán utilizados para la prestación del servicio eléctrico, facturación, atención de averías, trámites y comunicaciones.</p>
                        <p><strong>3. Derechos del Titular</strong></p>
                        <p style="color:#727A86;">El titular tiene derecho a acceder, rectificar, actualizar y cancelar sus datos personales.</p>
                    </div>
                    <button onclick="this.closest('div[style]').remove()" style="width:100%; padding:12px; background:#0033A0; color:white; border:none; border-radius:8px; font-weight:700; cursor:pointer; margin-top:12px;">Aceptar y Cerrar</button>
                </div>
            `;
        } else if (tipo === 'consentimiento') {
            contenido = `
                <div style="font-family:'Inter',sans-serif; max-width:500px; margin:0 auto; padding:24px; background:white; border-radius:16px; box-shadow:0 8px 40px rgba(0,0,0,0.15);">
                    <h2 style="color:#0033A0; margin:0 0 12px;">Consentimiento Informado F-085</h2>
                    <hr style="border:1px solid #EDEFF3; margin:12px 0;" />
                    <p><strong>COMPAÑÍA NACIONAL DE FUERZA Y LUZ S.A.</strong></p>
                    <p style="color:#727A86;">Yo, <strong>${nombre}</strong>, cédula <strong>${cedula}</strong>, declaro haber sido informado sobre el uso de mis datos personales como cliente de la CNFL.</p>
                    <div style="background:#f8f9fe; padding:12px; border-radius:8px; margin:12px 0;">
                        <p style="margin:4px 0;"><strong>Cliente:</strong> ${nombre}</p>
                        <p style="margin:4px 0;"><strong>Cédula:</strong> ${cedula}</p>
                        <p style="margin:4px 0;"><strong>Fecha:</strong> ${dia} de ${mes} de ${anio}</p>
                    </div>
                    <div style="font-size:14px; color:#0E1116; line-height:1.8;">
                        <p><strong>a) Custodia de Datos</strong></p>
                        <p style="color:#727A86;">La CNFL custodia bases de datos electrónicas con información personal de los clientes.</p>
                        <p><strong>b) Uso de Datos Personales</strong></p>
                        <p style="color:#727A86;">Se utilizan para la prestación del servicio eléctrico y comercialización de productos y servicios.</p>
                        <p><strong>c) Transferencia de Datos</strong></p>
                        <p style="color:#727A86;">La CNFL puede transferir datos a socios comerciales autorizados.</p>
                    </div>
                    <button onclick="this.closest('div[style]').remove()" style="width:100%; padding:12px; background:#0033A0; color:white; border:none; border-radius:8px; font-weight:700; cursor:pointer; margin-top:12px;">Aceptar y Cerrar</button>
                </div>
            `;
        }

        var modal = document.createElement('div');
        modal.style.cssText = 'position:fixed; top:0; left:0; right:0; bottom:0; background:rgba(0,0,0,0.5); z-index:9999; display:flex; justify-content:center; align-items:center; padding:20px;';
        modal.innerHTML = contenido;

        modal.addEventListener('click', function (e) {
            if (e.target === this) { this.remove(); }
        });

        document.body.appendChild(modal);

        // Marcar checkbox automáticamente
        setTimeout(function () {
            var checkbox = document.getElementById(tipo === 'politica' ? 'aceptaPolitica' : 'aceptaConsentimiento');
            if (checkbox) { checkbox.checked = true; }
        }, 500);
    };

    // ===== Validación de formulario de login =====
    var loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            var usuario = document.getElementById('UserName');
            var contrasena = document.getElementById('Contraseña');

            if (!usuario.value.trim() || !contrasena.value.trim()) {
                e.preventDefault();
                alert('Por favor complete todos los campos.');
            }
        });
    }

    // ===== Validación de formulario de registro =====
    var registroForm = document.getElementById('registroForm');
    if (registroForm) {
        registroForm.addEventListener('submit', function (e) {
            var contrasena = document.getElementById('contrasena');
            var confirmar = document.getElementById('confirm');

            if (contrasena.value !== confirmar.value) {
                e.preventDefault();
                alert('Las contraseñas no coinciden.');
                confirmar.style.borderColor = '#D32F2F';
            }

            var aceptaPolitica = document.getElementById('aceptaPolitica');
            var aceptaConsentimiento = document.getElementById('aceptaConsentimiento');

            if (!aceptaPolitica.checked || !aceptaConsentimiento.checked) {
                e.preventDefault();
                alert('Debe aceptar la Política de Privacidad y el Consentimiento Informado.');
            }
        });
    }

    // ===== Alternar visibilidad de contraseña por ID =====
    window.togglePasswordById = function (id) {
        var input = document.getElementById(id);
        if (input) {
            if (input.type === 'password') {
                input.type = 'text';
            } else {
                input.type = 'password';
            }
        }
    };
});