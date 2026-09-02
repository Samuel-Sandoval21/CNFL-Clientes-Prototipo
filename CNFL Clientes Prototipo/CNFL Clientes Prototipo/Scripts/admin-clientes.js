// ==========================================
// ADMIN - GESTIÓN DE CLIENTES
// ==========================================

function editarCliente(id) {
    document.getElementById('emailDisplay_' + id).style.display = 'none';
    document.getElementById('emailEdit_' + id).style.display = 'block';

    document.getElementById('editBtn_' + id).style.display = 'none';
    document.getElementById('saveBtn_' + id).style.display = 'inline-block';
    document.getElementById('cancelBtn_' + id).style.display = 'inline-block';

    var input = document.getElementById('emailInput_' + id);
    if (input) {
        input.focus();
        input.select();
    }
}

function cancelarEdicion(id) {
    var display = document.getElementById('emailText_' + id);
    var input = document.getElementById('emailInput_' + id);

    if (display && input) {
        input.value = display.textContent;
    }

    document.getElementById('emailDisplay_' + id).style.display = 'block';
    document.getElementById('emailEdit_' + id).style.display = 'none';

    document.getElementById('editBtn_' + id).style.display = 'inline-block';
    document.getElementById('saveBtn_' + id).style.display = 'none';
    document.getElementById('cancelBtn_' + id).style.display = 'none';
}

function guardarCorreo(id) {
    var input = document.getElementById('emailInput_' + id);
    var nuevoCorreo = input ? input.value.trim() : '';

    if (!nuevoCorreo || !nuevoCorreo.includes('@')) {
        mostrarToast('❌ Ingrese un correo electrónico válido', 'error');
        return;
    }

    var display = document.getElementById('emailText_' + id);
    if (display) {
        display.textContent = nuevoCorreo;
    }

    cancelarEdicion(id);
    mostrarToast('✅ Correo actualizado correctamente');
}

function filtrarClientes() {
    var input = document.getElementById('searchInput');
    var filter = input ? input.value.toLowerCase() : '';
    var rows = document.querySelectorAll('.client-row');

    rows.forEach(function (row) {
        var searchData = row.getAttribute('data-search') || '';
        if (searchData.includes(filter)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    if (!toast) return;

    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.style.display = 'block';

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.display = 'none';
    }, 3000);
}

window.editarCliente = editarCliente;
window.cancelarEdicion = cancelarEdicion;
window.guardarCorreo = guardarCorreo;
window.filtrarClientes = filtrarClientes;
window.mostrarToast = mostrarToast;