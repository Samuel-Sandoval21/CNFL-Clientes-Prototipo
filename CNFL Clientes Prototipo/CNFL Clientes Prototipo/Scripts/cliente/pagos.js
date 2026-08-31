// ==========================================
// PAGOS - JAVASCRIPT CON PAGO REAL
// ==========================================

function irAPagar(metodo, monto, facturaId) {
    var url = '/Clientes/' + metodo + '?monto=' + monto + '&facturaId=' + facturaId;
    window.location.href = url;
}

function confirmarPago(metodo, facturaId) {
    if (!facturaId) {
        var hidden = document.getElementById('facturaId');
        if (hidden) facturaId = hidden.value;
    }

    if (!facturaId) {
        alert('❌ No se pudo identificar la factura a pagar');
        return;
    }

    if (!confirm('¿Confirmas el pago de esta factura?')) {
        return;
    }

    var btn = document.querySelector('.btn-cta, .btn-pagar');
    var textoOriginal = btn ? btn.textContent : '';

    if (btn) {
        btn.textContent = '⏳ Procesando...';
        btn.disabled = true;
    }

    $.ajax({
        url: '/Clientes/PagarFactura',
        type: 'POST',
        data: { id: facturaId, metodo: metodo },
        success: function (response) {
            if (response.success) {
                alert('✅ ' + response.message);
                window.location.href = '/Clientes/Pagos';
            } else {
                alert('❌ ' + response.message);
            }
        },
        error: function () {
            alert('❌ Error al procesar el pago. Intente nuevamente.');
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
    var urlParams = new URLSearchParams(window.location.search);
    var facturaId = urlParams.get('facturaId');
    if (facturaId) {
        var hidden = document.getElementById('facturaId');
        if (hidden) hidden.value = facturaId;
    }

    var btnPagar = document.querySelector('.btn-pagar, .btn-cta');
    if (btnPagar && btnPagar.textContent.includes('Pagar')) {
        var metodo = '';
        if (window.location.pathname.includes('Sinpe')) metodo = 'Sinpe';
        else if (window.location.pathname.includes('Iban')) metodo = 'Iban';
        else if (window.location.pathname.includes('Tarjeta')) metodo = 'Tarjeta';

        if (metodo) {
            btnPagar.onclick = function () {
                confirmarPago(metodo, facturaId);
            };
        }
    }
});