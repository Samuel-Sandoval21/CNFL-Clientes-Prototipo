// ==========================================
// SERVICIOS CONTRATADOS - JAVASCRIPT
// ==========================================

// ===== VER DETALLE DEL SERVICIO =====
function verDetalleServicio(servicio) {
    var mensaje = '📋 Detalle del servicio\n';
    mensaje += '═'.repeat(30) + '\n\n';
    mensaje += '📌 Servicio: ' + servicio + '\n';
    mensaje += '📅 Contratado desde: 01/01/2026\n';
    mensaje += '💳 Próximo pago: 15/09/2026\n';
    mensaje += '📊 Estado: Activo\n\n';
    mensaje += '═'.repeat(30) + '\n';
    mensaje += '📞 ¿Necesitas ayuda con este servicio?\n';
    mensaje += '• Llama al 800-ENERGÍA (800-363-7442)\n';
    mensaje += '• Visita nuestra sección de Soporte\n\n';
    mensaje += '¿Deseas realizar alguna acción?';

    if (confirm(mensaje)) {
        var opcion = prompt('Selecciona una opción:\n\n1️⃣ Ver facturas\n2️⃣ Solicitar cambio de plan\n3️⃣ Cancelar servicio\n4️⃣ Contactar soporte\n\nIngresa el número de la opción:');

        if (opcion === '1') {
            alert('📄 Redirigiendo a tus facturas...');
            window.location.href = '/Clientes/Pagos';
        } else if (opcion === '2') {
            alert('📞 Comunícate al 800-ENERGÍA para cambiar tu plan.\n\nHorario: Lunes a Viernes 8:00am - 6:00pm');
        } else if (opcion === '3') {
            if (confirm('⚠️ ¿Estás seguro de que deseas cancelar este servicio?')) {
                alert('✅ Solicitud de cancelación recibida.\n📩 Recibirás un correo de confirmación en 24 horas.');
            }
        } else if (opcion === '4') {
            alert('📞 Contacta a nuestro equipo de soporte:\n\n• Teléfono: 800-ENERGÍA (800-363-7442)\n• Correo: soporte@cnfl.go.cr\n• Chat en línea: Disponible 24/7');
        } else {
            alert('❌ Opción no válida. Vuelve a intentarlo.');
        }
    }
}