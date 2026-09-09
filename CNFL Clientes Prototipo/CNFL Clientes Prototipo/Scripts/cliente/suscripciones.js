// ===== Cliente - Suscripciones =====

document.addEventListener('DOMContentLoaded', function () {
    console.log('Suscripciones cargado');

    var botonesCancelar = document.querySelectorAll('.btn-cancelar-suscripcion');

    botonesCancelar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = this.dataset.id;
            var servicio = this.dataset.servicio;

            if (confirm('¿Estás seguro de que deseas cancelar la suscripción a ' + servicio + '?')) {
                $.ajax({
                    url: '/Clientes/CancelarSuscripcion',
                    type: 'POST',
                    data: { suscripcionId: id },
                    success: function (data) {
                        if (data.success) {
                            alert('✅ Suscripción cancelada.');
                            location.reload();
                        } else {
                            alert(data.message || 'Error al cancelar.');
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