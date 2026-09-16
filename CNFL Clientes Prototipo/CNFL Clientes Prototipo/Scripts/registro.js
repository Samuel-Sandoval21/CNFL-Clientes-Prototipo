// ============================================================
// Registro CNFL - JavaScript del formulario de registro
// ============================================================

// ═════════ Toggle mostrar/ocultar contraseña ═════════
function togglePass(id, btn) {
    var input = document.getElementById(id);
    if (!input) return;
    var esPassword = input.getAttribute('type') === 'password';
    input.setAttribute('type', esPassword ? 'text' : 'password');
    btn.innerHTML = esPassword
        ? '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
        : '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
}

// ═════════ Cascada Provincia → Cantón → Distrito ═════════
var urlCantones = '/Cuenta/GetCantones';
var urlDistritos = '/Cuenta/GetDistritos';

function onProvinciaChange(sel) {
    var selCanton = document.getElementById('Canton');
    var selDistrito = document.getElementById('Distrito');
    if (!selCanton || !selDistrito) return;

    selCanton.innerHTML = '<option value="">Cargando...</option>';
    selDistrito.innerHTML = '<option value="">— Primero elegí el cantón —</option>';
    if (!sel.value) {
        selCanton.innerHTML = '<option value="">— Primero elegí la provincia —</option>';
        return;
    }
    fetch(urlCantones + '?provincia=' + encodeURIComponent(sel.value))
        .then(function (r) { return r.json(); })
        .then(function (data) {
            var html = '<option value="">— Seleccioná cantón —</option>';
            data.forEach(function (c) {
                html += '<option value="' + c + '">' + c + '</option>';
            });
            selCanton.innerHTML = html;
        });
}

function onCantonChange(sel) {
    var selProv = document.getElementById('Provincia');
    var selDistrito = document.getElementById('Distrito');
    if (!selProv || !selDistrito) return;

    selDistrito.innerHTML = '<option value="">Cargando...</option>';
    if (!sel.value || !selProv.value) {
        selDistrito.innerHTML = '<option value="">— Primero elegí el cantón —</option>';
        return;
    }
    fetch(urlDistritos + '?provincia=' + encodeURIComponent(selProv.value) + '&canton=' + encodeURIComponent(sel.value))
        .then(function (r) { return r.json(); })
        .then(function (data) {
            var html = '<option value="">— Seleccioná distrito —</option>';
            data.forEach(function (d) {
                html += '<option value="' + d + '">' + d + '</option>';
            });
            selDistrito.innerHTML = html;
        });
}

// ═════════ Factura electrónica ═════════
function toggleFactura(chk) {
    var wrap = document.getElementById('actividadWrap');
    var hidden = document.getElementById('ActividadEconomicaId');
    var buscador = document.getElementById('buscadorActividad');
    var lista = document.getElementById('listaActividades');
    var seleccionada = document.getElementById('actividadSeleccionada');
    if (!wrap || !hidden || !buscador || !lista || !seleccionada) return;

    wrap.style.display = chk.checked ? 'block' : 'none';
    if (!chk.checked) {
        hidden.value = '';
        seleccionada.style.display = 'none';
        buscador.value = '';
        lista.style.display = 'none';
    }
}

function mostrarActividades() {
    var lista = document.getElementById('listaActividades');
    if (lista) lista.style.display = 'block';
    filtrarActividades();
}

function filtrarActividades() {
    var buscador = document.getElementById('buscadorActividad');
    var lista = document.getElementById('listaActividades');
    if (!buscador || !lista) return;

    var q = buscador.value.toLowerCase().trim();
    var visibles = 0;
    var items = document.querySelectorAll('.actividad-item');
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        var txt = it.getAttribute('data-text') || '';
        if (!q || txt.indexOf(q) >= 0) {
            it.style.display = 'block';
            visibles++;
        } else {
            it.style.display = 'none';
        }
    }
    lista.style.display = visibles > 0 ? 'block' : 'none';
}

function seleccionarActividad(id, texto) {
    var hidden = document.getElementById('ActividadEconomicaId');
    var textoEl = document.getElementById('actividadTexto');
    var seleccionada = document.getElementById('actividadSeleccionada');
    var lista = document.getElementById('listaActividades');
    var buscador = document.getElementById('buscadorActividad');
    if (!hidden || !textoEl || !seleccionada || !lista || !buscador) return;

    hidden.value = id;
    textoEl.textContent = texto;
    seleccionada.style.display = 'block';
    lista.style.display = 'none';
    buscador.value = texto;
}

// Cerrar lista al hacer click afuera
document.addEventListener('click', function (e) {
    var buscador = document.getElementById('buscadorActividad');
    var lista = document.getElementById('listaActividades');
    if (!buscador || !lista) return;
    var target = e.target;
    if (!target) return;
    if (!buscador.contains(target) && !lista.contains(target)) {
        lista.style.display = 'none';
    }
});

// ═════════ Validación al enviar ═════════
(function () {
    var formRegistro = document.getElementById('formRegistro');
    if (!formRegistro) return;
    formRegistro.addEventListener('submit', function (e) {
        var provEl = document.getElementById('Provincia');
        var cantEl = document.getElementById('Canton');
        var distEl = document.getElementById('Distrito');
        var politica = document.getElementById('chkPolitica');
        var consent = document.getElementById('chkConsentimiento');
        var chkFactura = document.getElementById('chkFactura');
        var actId = document.getElementById('ActividadEconomicaId');
        var buscador = document.getElementById('buscadorActividad');
        var passEl = document.getElementById('Contraseña');
        var confEl = document.getElementById('confirmarContraseña');

        if (!provEl || !cantEl || !distEl || !politica || !consent || !chkFactura || !actId || !buscador || !passEl || !confEl) return;

        if (!provEl.value || !cantEl.value || !distEl.value) {
            e.preventDefault();
            alert('Completá provincia, cantón y distrito.');
            return;
        }
        if (!politica.checked) {
            e.preventDefault();
            alert('Debés aceptar la Política de Privacidad.');
            return;
        }
        if (!consent.checked) {
            e.preventDefault();
            alert('Debés aceptar el Consentimiento Informado.');
            return;
        }
        if (chkFactura.checked && !actId.value) {
            e.preventDefault();
            alert('Elegí la actividad económica para activar la factura electrónica.');
            buscador.focus();
            return;
        }
        var pass = passEl.value;
        var conf = confEl.value;
        if (pass.length < 6) {
            e.preventDefault();
            alert('La contraseña debe tener al menos 6 caracteres.');
            return;
        }
        if (pass !== conf) {
            e.preventDefault();
            alert('Las contraseñas no coinciden.');
            return;
        }
    });
})();

// Si ya hay provincia seleccionada al cargar, cargar cantones
(function () {
    var prov = document.getElementById('Provincia');
    if (prov && prov.value) onProvinciaChange(prov);
})();