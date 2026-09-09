// ===== Cliente - Servicios Contratados =====

document.addEventListener('DOMContentLoaded', function () {
    console.log('Servicios Contratados cargado');

    var botonesDetalle = document.querySelectorAll('.btn-detalle-servicio');

    botonesDetalle.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var servicio = this.dataset.servicio;
            alert('📋 Detalle del servicio: ' + servicio + '\n\n' +
                'Para más información, contacta a nuestro equipo de soporte.');
        });
    });
});