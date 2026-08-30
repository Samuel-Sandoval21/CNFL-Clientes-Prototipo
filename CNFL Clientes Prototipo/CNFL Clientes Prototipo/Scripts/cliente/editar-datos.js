// ==========================================
// EDITAR DATOS - JAVASCRIPT
// ==========================================

// ===== GUARDAR CAMBIOS =====
function guardarCambios(event) {
    event.preventDefault();

    var nombre = document.getElementById('nombre');
    var apellidos = document.getElementById('apellidos');
    var telefono = document.getElementById('telefono');
    var correo = document.getElementById('correo');

    var errores = [];

    // Validaciones
    if (!nombre.value.trim()) {
        errores.push('⚠️ El nombre es obligatorio.');
        nombre.focus();
    }

    if (!apellidos.value.trim()) {
        errores.push('⚠️ Los apellidos son obligatorios.');
        apellidos.focus();
    }

    if (!telefono.value.trim()) {
        errores.push('⚠️ El teléfono es obligatorio.');
        telefono.focus();
    } else if (!/^\d{4}-\d{4}$/.test(telefono.value) && !/^\d{8}$/.test(telefono.value)) {
        errores.push('⚠️ Formato de teléfono inválido. Use 8888-8888');
        telefono.focus();
    }

    if (!correo.value.trim()) {
        errores.push('⚠️ El correo es obligatorio.');
        correo.focus();
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo.value)) {
        errores.push('⚠️ Ingrese un correo electrónico válido.');
        correo.focus();
    }

    if (errores.length > 0) {
        alert('❌ Por favor, corrige los siguientes errores:\n\n' + errores.join('\n'));
        return;
    }

    // Simular guardado
    var btn = document.querySelector('.btn-guardar');
    var textoOriginal = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Guardando...';
    btn.disabled = true;

    setTimeout(function () {
        alert('✅ ¡Datos actualizados correctamente!');
        btn.innerHTML = textoOriginal;
        btn.disabled = false;
    }, 1500);
}