// ===== Cliente - Pagos =====

document.addEventListener('DOMContentLoaded', function () {
    console.log('Pagos cargado');

    var botonesPagar = document.querySelectorAll('.btn-pagar-factura');

    botonesPagar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var facturaId = this.dataset.id;
            var monto = this.dataset.monto;

            if (confirm('¿Deseas pagar la factura #' + facturaId + ' por ₡' + monto + '?')) {
                $.ajax({
                    url: '/Clientes/PagarFactura',
                    type: 'POST',
                    data: { facturaId: facturaId },
                    success: function (data) {
                        if (data.success) {
                            alert('✅ Pago realizado exitosamente.');
                            location.reload();
                        } else {
                            alert(data.message || 'Error al procesar el pago.');
                        }
                    },
                    error: function () {
                        alert('❌ Error de conexión.');
                    }
                });
            }
        });
    });
});