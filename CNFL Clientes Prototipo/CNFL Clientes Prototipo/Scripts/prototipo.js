document.addEventListener('DOMContentLoaded', function () {
    console.log('Prototipo CNFL - RespuestaTCU cargado.');

    // Mostrar el Toast de notificación una sola vez
    var toastEl = document.getElementById('toast-notificacion');
    if (toastEl) {
        var toast = new bootstrap.Toast(toastEl);
        toast.show();
    }

    // Lógica para el botón de reportar avería (simulación de cámara)
    const btnReportar = document.getElementById('btn-reportar-averia');
    if (btnReportar) {
        btnReportar.addEventListener('click', function () {
            // En el prototipo real, esto abriría la cámara del celular
            alert("📷 Prototipo: Se abrirá la cámara para tomar una foto de la avería.");
        });
    }
});