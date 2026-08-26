// Scripts/prototipo.js
document.addEventListener('DOMContentLoaded', function () {
    console.log('Prototipo CNFL Cargado correctamente.');

    // Capturamos el formulario si existe (para futuras implementaciones)
    const formCliente = document.getElementById('form-cliente');

    if (formCliente) {
        formCliente.addEventListener('submit', function (e) {
            // Aquí solo simulamos el envío en el prototipo
            e.preventDefault();
            alert('Prototipo: Datos del cliente recibidos para su procesamiento.');
            // En un futuro aquí se haría el fetch o ajax hacia el controlador
        });
    }
});