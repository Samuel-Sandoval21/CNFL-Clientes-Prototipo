// ==========================================
// ADMIN CLIENTES - JAVASCRIPT
// ==========================================

// ===== VARIABLES GLOBALES =====
var clienteOriginalEmail = {};

// ===== FILTRAR CLIENTES =====
function filtrarClientes() {
    var input = document.getElementById('searchInput');
    var filter = input.value.toLowerCase();
    var rows = document.querySelectorAll('.client-row');

    rows.forEach(function (row) {
        var searchData = row.dataset.search || '';
        if (searchData.indexOf(filter) > -1) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// ===== EDITAR CLIENTE =====
function editarCliente(id) {
    // Ocultar display, mostrar input
    document.getElementById('emailDisplay_' + id).style.display = 'none';
    document.getElementById('emailEdit_' + id).style.display = 'block';
    document.getElementById('editBtn_' + id).style.display = 'none';
    document.getElementById('saveBtn_' + id).style.display = 'inline-flex';
    document.getElementById('cancelBtn_' + id).style.display = 'inline-flex';

    // Guardar email original por si cancela
    clienteOriginalEmail[id] = document.getElementById('emailText_' + id).textContent;

    // Enfocar input
    var input = document.getElementById('emailInput_' + id);
    input.focus();
    input.select();
}

// ===== CANCELAR EDICIÓN =====
function cancelarEdicion(id) {
    // Restaurar email original
    document.getElementById('emailInput_' + id).value = clienteOriginalEmail[id];

    // Ocultar input, mostrar display
    document.getElementById('emailEdit_' + id).style.display = 'none';
    document.getElementById('emailDisplay_' + id).style.display = 'block';
    document.getElementById('editBtn_' + id).style.display = 'inline-flex';
    document.getElementById('saveBtn_' + id).style.display = 'none';
    document.getElementById('cancelBtn_' + id).style.display = 'none';
}

// ===== GUARDAR CORREO =====
function guardarCorreo(id) {
    var nuevoEmail = document.getElementById('emailInput_' + id).value.trim();
    var emailRegex = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;

    if (!nuevoEmail) {
        mostrarToast('❌ El correo no puede estar vacío', 'error');
        return;
    }

    if (!emailRegex.test(nuevoEmail)) {
        mostrarToast('❌ Ingrese un correo electrónico válido', 'error');
        return;
    }

    // Actualizar visualmente
    document.getElementById('emailText_' + id).textContent = nuevoEmail;

    // Ocultar input, mostrar display
    document.getElementById('emailEdit_' + id).style.display = 'none';
    document.getElementById('emailDisplay_' + id).style.display = 'block';
    document.getElementById('editBtn_' + id).style.display = 'inline-flex';
    document.getElementById('saveBtn_' + id).style.display = 'none';
    document.getElementById('cancelBtn_' + id).style.display = 'none';

    mostrarToast('✅ Correo actualizado correctamente para el cliente #' + id);

    // Actualizar data-search para búsqueda
    var row = document.querySelector('.client-row[data-id="' + id + '"]');
    if (row) {
        var searchData = row.dataset.search || '';
        row.dataset.search = searchData + ' ' + nuevoEmail.toLowerCase();
    }
}

// ===== MOSTRAR TOAST =====
function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.classList.add('show');
    setTimeout(function () {
        toast.classList.remove('show');
    }, 3000);
}

// ===== EVENTOS TECLADO =====
document.addEventListener('keydown', function (e) {
    // Enter para guardar
    if (e.key === 'Enter') {
        var activeInput = document.activeElement;
        if (activeInput && activeInput.id && activeInput.id.startsWith('emailInput_')) {
            var id = activeInput.id.replace('emailInput_', '');
            guardarCorreo(parseInt(id));
        }
    }
    // Escape para cancelar
    if (e.key === 'Escape') {
        var activeInput = document.activeElement;
        if (activeInput && activeInput.id && activeInput.id.startsWith('emailInput_')) {
            var id = activeInput.id.replace('emailInput_', '');
            cancelarEdicion(parseInt(id));
        }
    }
});