// ==========================================
// SINPE - LÓGICA ESPECÍFICA
// ==========================================

$(document).ready(function () {
    console.log('🔵 SINPE - Inicializando...');

    // Validar campos específicos de SINPE
    validarCamposSinpe();

    // Configurar eventos específicos
    configurarEventosSinpe();
});

function validarCamposSinpe() {
    // Validaciones específicas de SINPE
    var telefono = document.querySelector('#telefonoSinpe');
    if (telefono) {
        telefono.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 8) {
                this.value = this.value.slice(0, 8);
            }
        });
    }
}

function configurarEventosSinpe() {
    // Eventos específicos de SINPE
    $('#btnPagarSinpe').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('🔘 Click en Confirmar pago SINPE');

        var facturaId = $('#facturaId').val() || '001';

        // Validaciones específicas de SINPE
        if (!validarSinpe()) {
            return;
        }

        iniciarPago('Sinpe', facturaId);
    });
}

function validarSinpe() {
    // Validar teléfono SINPE
    var telefono = document.querySelector('#telefonoSinpe');
    if (telefono && telefono.value.length < 8) {
        alert('❌ Por favor ingrese un número de teléfono válido (8 dígitos)');
        return false;
    }

    return true;
}