// ============================================================
// ADMIN.JS - FUNCIONES PARA EL PANEL ADMINISTRATIVO
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Admin JS cargado');

    // ===== Filtros de averías =====
    var btnFiltrar = document.getElementById('btnFiltrar');
    var btnLimpiar = document.getElementById('btnLimpiar');
    var filtroEstado = document.getElementById('filtroEstado');
    var filtroBusqueda = document.getElementById('filtroBusqueda');
    var tabla = document.getElementById('tablaAverias');

    if (btnFiltrar && tabla) {
        btnFiltrar.addEventListener('click', function () {
            var estado = filtroEstado ? filtroEstado.value.toLowerCase() : '';
            var busqueda = filtroBusqueda ? filtroBusqueda.value.toLowerCase() : '';
            var filas = tabla.querySelectorAll('tbody tr');

            filas.forEach(function (fila) {
                var mostrar = true;

                if (estado && fila.dataset && fila.dataset.estado) {
                    if (fila.dataset.estado.toLowerCase() !== estado) {
                        mostrar = false;
                    }
                }

                if (busqueda && mostrar) {
                    var texto = fila.textContent.toLowerCase();
                    if (!texto.includes(busqueda)) {
                        mostrar = false;
                    }
                }

                fila.style.display = mostrar ? '' : 'none';
            });
        });
    }

    if (btnLimpiar && filtroEstado && filtroBusqueda) {
        btnLimpiar.addEventListener('click', function () {
            filtroEstado.value = '';
            filtroBusqueda.value = '';
            var filas = tabla.querySelectorAll('tbody tr');
            filas.forEach(function (fila) {
                fila.style.display = '';
            });
        });
    }

    // ===== Cambiar Estado (Modal) =====
    var modal = document.getElementById('modalCambiarEstado');
    var btnGuardar = document.getElementById('modalGuardar');
    var btnCancelar = document.getElementById('modalCancelar');
    var selectEstado = document.getElementById('modalNuevoEstado');
    var spanAveriaId = document.getElementById('modalAveriaId');
    var averiaIdActual = null;

    document.querySelectorAll('.btn-cambiar-estado').forEach(function (btn) {
        btn.addEventListener('click', function () {
            averiaIdActual = this.dataset.id;
            var estadoActual = this.dataset.estado;
            if (spanAveriaId) spanAveriaId.textContent = averiaIdActual;
            if (selectEstado) selectEstado.value = estadoActual;
            if (modal) modal.style.display = 'flex';
        });
    });

    if (btnGuardar && modal) {
        btnGuardar.addEventListener('click', function () {
            if (!averiaIdActual) return;

            var nuevoEstado = selectEstado ? selectEstado.value : '';

            if (confirm('¿Estás seguro de cambiar el estado de esta avería?')) {
                // Simulación de cambio de estado
                alert('✅ Estado actualizado correctamente.');
                location.reload();
            }
        });
    }

    if (btnCancelar && modal) {
        btnCancelar.addEventListener('click', function () {
            modal.style.display = 'none';
            averiaIdActual = null;
        });
    }

    if (modal) {
        modal.addEventListener('click', function (e) {
            if (e.target === this) {
                modal.style.display = 'none';
                averiaIdActual = null;
            }
        });
    }

    // ===== Botones de generar reportes =====
    document.querySelectorAll('.btn-generar').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            var textoOriginal = this.textContent;
            this.textContent = '⏳ Generando...';
            this.disabled = true;

            setTimeout(function () {
                btn.textContent = textoOriginal;
                btn.disabled = false;
            }, 2000);
        });
    });
});