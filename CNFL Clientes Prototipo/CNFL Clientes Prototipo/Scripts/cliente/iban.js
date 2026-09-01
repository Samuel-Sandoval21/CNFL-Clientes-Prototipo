// ==========================================
// IBAN - LÓGICA ESPECÍFICA
// ==========================================

$(document).ready(function () {
    console.log('🔵 IBAN - Inicializando...');

    validarCamposIban();
    configurarEventosIban();
});

function validarCamposIban() {
    // Validar formato IBAN
    var ibanInput = document.querySelector('#cuentaIban');
    if (ibanInput) {
        ibanInput.addEventListener('input', function (e) {
            this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
            if (this.value.length > 20) {
                this.value = this.value.slice(0, 20);
            }
        });
    }
}

function configurarEventosIban() {
    $('#btnPagarIban').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('🔘 Click en Confirmar transferencia IBAN');

        var facturaId = $('#facturaId').val() || '001';

        if (!validarIban()) {
            return;
        }

        iniciarPago('Iban', facturaId);
    });
}

function validarIban() {
    // Validar formato IBAN (CR + 20 dígitos)
    var iban = document.querySelector('#cuentaIban');
    if (iban) {
        var ibanValue = iban.value.trim();
        if (ibanValue.length < 20) {
            alert('❌ Por favor ingrese un IBAN válido (CR + 20 dígitos)');
            return false;
        }
    }

    return true;
}