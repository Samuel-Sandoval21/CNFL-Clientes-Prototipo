// ==========================================
// SUSCRIPCIONES - JAVASCRIPT
// ==========================================

// ===== GESTIONAR SUSCRIPCIÓN =====
function gestionarSuscripcion(servicio, estado) {
    var mensaje = '📋 Gestionando: ' + servicio + '\n\n';
    mensaje += '📌 Estado actual: ' + (estado ? 'Activo' : 'Inactivo') + '\n';
    mensaje += '\n¿Qué deseas hacer?\n';
    mensaje += '1️⃣ Cambiar plan\n';
    mensaje += '2️⃣ Cancelar suscripción\n';
    mensaje += '3️⃣ Modificar datos de pago\n';
    mensaje += '4️⃣ Ver detalles\n';

    var opcion = prompt(mensaje + '\nIngresa el número de la opción:');

    if (opcion === '1') {
        alert('📦 Planes disponibles:\n' +
            '• Plan Básico - ₡8.900/mes\n' +
            '• Plan Estándar - ₡15.900/mes\n' +
            '• Plan Premium - ₡29.900/mes\n\n' +
            '📞 Comunícate al 800-ENERGÍA para cambiar tu plan.');
    } else if (opcion === '2') {
        if (confirm('⚠️ ¿Estás seguro de que deseas cancelar esta suscripción?')) {
            alert('✅ Solicitud de cancelación recibida.\n' +
                '📩 Recibirás un correo de confirmación en 24 horas.');
        }
    } else if (opcion === '3') {
        alert('💳 Métodos de pago disponibles:\n' +
            '• Factura eléctrica\n' +
            '• Tarjeta de crédito/débito\n' +
            '• SINPE Móvil\n\n' +
            '📞 Para modificar, llama al 800-ENERGÍA.');
    } else if (opcion === '4') {
        alert('📋 Detalles de la suscripción:\n' +
            '• Servicio: ' + servicio + '\n' +
            '• Estado: ' + (estado ? 'Activo' : 'Inactivo') + '\n' +
            '• Próximo pago: 15 de cada mes\n' +
            '• Método de pago: Factura eléctrica');
    } else {
        alert('❌ Opción no válida. Vuelve a intentarlo.');
    }
}