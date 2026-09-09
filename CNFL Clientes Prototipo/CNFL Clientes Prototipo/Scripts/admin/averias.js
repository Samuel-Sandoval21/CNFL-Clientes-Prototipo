// ============================================================
// ADMIN AVERIAS.JS - Admin
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Admin Averías cargado');

    var btnFiltrar = document.getElementById('btnFiltrar');
    var btnLimpiar = document.getElementById('btnLimpiar');
    var filtroEstado = document.getElementById('filtroEstado');
    var filtroBusqueda = document.getElementById('filtroBusqueda');
    var tabla = document.getElementById('tablaAverias');

    // ===== Filtrar =====
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
            mostrarToast('🔍 Filtro aplicado', 'info');
        });
    }

    if (btnLimpiar) {
        btnLimpiar.addEventListener('click', function () {
            if (filtroEstado) filtroEstado.value = '';
            if (filtroBusqueda) filtroBusqueda.value = '';
            var filas = tabla.querySelectorAll('tbody tr');
            filas.forEach(function (fila) {
                fila.style.display = '';
            });
            mostrarToast('🧹 Filtros limpiados', 'info');
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
            if (confirm('¿Cambiar estado de la avería #' + averiaIdActual + ' a "' + nuevoEstado + '"?')) {
                mostrarToast('⏳ Actualizando estado...', 'info');
                setTimeout(function () {
                    mostrarToast('✅ Estado actualizado correctamente', 'success');
                    modal.style.display = 'none';
                    // Aquí iría la llamada AJAX real
                    // location.reload();
                }, 1500);
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

    // ===== Click en fila de avería =====
    document.querySelectorAll('.btn-ver').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            var id = this.getAttribute('href')?.split('/').pop() || '0';
            mostrarToast('📋 Cargando detalle de avería #' + id, 'info');
        });
    });
});

function mostrarToast(mensaje, tipo) {
    tipo = tipo || 'info';
    var toast = document.getElementById('toastGlobal');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastGlobal';
        toast.style.cssText = 'position:fixed; bottom:90px; left:50%; transform:translateX(-50%); padding:12px 24px; border-radius:12px; font-weight:700; z-index:9999; background:#0E1116; color:white; box-shadow:0 8px 24px rgba(0,0,0,0.2); opacity:0; transition:opacity 0.3s; max-width:90%; text-align:center;';
        document.body.appendChild(toast);
    }

    toast.textContent = mensaje;
    toast.style.opacity = '1';

    var colores = {
        success: '#2E7D32',
        error: '#D32F2F',
        warning: '#F5A623',
        info: '#0E1116'
    };
    toast.style.background = colores[tipo] || colores.info;

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.opacity = '0';
    }, 3000);
}