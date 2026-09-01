// ==========================================
// TARJETA - LÓGICA ESPECÍFICA
// ==========================================

$(document).ready(function () {
    console.log('🔵 Tarjeta - Inicializando...');

    validarCamposTarjeta();
    configurarEventosTarjeta();
});

function validarCamposTarjeta() {
    // Formatear número de tarjeta
    var tarjetaInput = document.querySelector('#numeroTarjeta');
    if (tarjetaInput) {
        tarjetaInput.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 16) {
                this.value = this.value.slice(0, 16);
            }
        });
    }

    // Formatear fecha de expiración
    var fechaInput = document.querySelector('#fechaExpiracion');
    if (fechaInput) {
        fechaInput.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 2 && this.value.length <= 4) {
                this.value = this.value.slice(0, 2) + '/' + this.value.slice(2);
            }
            if (this.value.length > 5) {
                this.value = this.value.slice(0, 5);
            }
        });
    }

    // Limitar CVV
    var cvvInput = document.querySelector('#cvvTarjeta');
    if (cvvInput) {
        cvvInput.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 3) {
                this.value = this.value.slice(0, 3);
            }
        });
    }
}

function configurarEventosTarjeta() {
    $('#btnPagarTarjeta').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('🔘 Click en Pagar con Tarjeta');

        var facturaId = $('#facturaId').val() || '001';

        if (!validarTarjeta()) {
            return;
        }

        iniciarPago('Tarjeta', facturaId);
    });
}

function validarTarjeta() {
    var tarjeta = document.querySelector('#numeroTarjeta');
    var fecha = document.querySelector('#fechaExpiracion');
    var cvv = document.querySelector('#cvvTarjeta');

    if (tarjeta && tarjeta.value.length < 16) {
        alert('❌ Por favor ingrese un número de tarjeta válido (16 dígitos)');
        return false;
    }

    if (fecha && fecha.value.length < 5) {
        alert('❌ Por favor ingrese una fecha de expiración válida (MM/AA)');
        return false;
    }

    if (cvv && cvv.value.length < 3) {
        alert('❌ Por favor ingrese un CVV válido (3 dígitos)');
        return false;
    }

    return true;
}