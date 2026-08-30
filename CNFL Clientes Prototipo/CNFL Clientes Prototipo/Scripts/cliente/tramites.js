// ==========================================
// CLIENTE TRÁMITES - JAVASCRIPT
// ==========================================

// ===== VARIABLES GLOBALES =====
var tramiteSeleccionado = '';

// ===== SELECCIONAR TRÁMITE Y ABRIR FORMULARIO =====
function seleccionarTramite(tipo) {
    var nombres = {
        'cambio_nombre': 'Cambio de nombre de abonado',
        'desconexion_reconexion': 'Desconexión y reconexión',
        'solicitud_servicio': 'Solicitud de servicio nuevo',
        'traslado_medidor': 'Traslado de medidor',
        'traspaso_servicio': 'Traspaso de servicio eléctrico',
        'reclamo_danos': 'Reclamo por daños'
    };

    var iconos = {
        'cambio_nombre': '📝',
        'desconexion_reconexion': '🔌',
        'solicitud_servicio': '⚡',
        'traslado_medidor': '📦',
        'traspaso_servicio': '🤝',
        'reclamo_danos': '⚠️'
    };

    var descripciones = {
        'cambio_nombre': 'Actualiza el titular del servicio',
        'desconexion_reconexion': 'Solicita cortar o restablecer el servicio',
        'solicitud_servicio': 'Contrata un nuevo servicio eléctrico',
        'traslado_medidor': 'Cambia la ubicación de tu medidor',
        'traspaso_servicio': 'Transfiere el servicio a otra persona',
        'reclamo_danos': 'Presenta un reclamo por daños ocasionados'
    };

    tramiteSeleccionado = tipo;

    // Crear modal si no existe
    if (!document.getElementById('modalTramite')) {
        crearModalTramite();
    }

    // Actualizar contenido del modal
    document.getElementById('modalTitulo').textContent = iconos[tipo] + ' ' + nombres[tipo];
    document.getElementById('modalDescripcionTramite').textContent = descripciones[tipo];
    document.getElementById('modalTipo').value = tipo;

    // Limpiar campos
    document.getElementById('modalNise').value = '';
    document.getElementById('modalNombre').value = '';
    document.getElementById('modalCedula').value = '';
    document.getElementById('modalTelefono').value = '';
    document.getElementById('modalCorreo').value = '';
    document.getElementById('modalDescripcion').value = '';

    // Mostrar campos adicionales según el tipo
    var camposAdicionales = document.getElementById('camposAdicionales');
    var htmlAdicional = '';

    switch (tipo) {
        case 'cambio_nombre':
            htmlAdicional = `
                <div class="form-group">
                    <label>Nuevo titular</label>
                    <input type="text" id="modalNuevoTitular" placeholder="Nombre completo del nuevo titular" class="form-input" />
                </div>
                <div class="form-group">
                    <label>Cédula nuevo titular</label>
                    <input type="text" id="modalCedulaNuevo" placeholder="1-2345-6789" class="form-input" />
                </div>
            `;
            break;
        case 'desconexion_reconexion':
            htmlAdicional = `
                <div class="form-group">
                    <label>Tipo de solicitud</label>
                    <select id="modalTipoSolicitud" class="form-input">
                        <option value="desconexion">Desconexión</option>
                        <option value="reconexion">Reconexión</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Fecha deseada</label>
                    <input type="date" id="modalFecha" class="form-input" />
                </div>
            `;
            break;
        case 'solicitud_servicio':
            htmlAdicional = `
                <div class="form-group">
                    <label>Tipo de servicio</label>
                    <select id="modalTipoServicio" class="form-input">
                        <option value="monofasico">Monofásico</option>
                        <option value="trifasico">Trifásico</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Dirección exacta</label>
                    <input type="text" id="modalDireccion" placeholder="Provincia, Cantón, Distrito, Dirección exacta" class="form-input" />
                </div>
            `;
            break;
        case 'traslado_medidor':
            htmlAdicional = `
                <div class="form-group">
                    <label>Nueva dirección</label>
                    <input type="text" id="modalNuevaDireccion" placeholder="Provincia, Cantón, Distrito, Dirección exacta" class="form-input" />
                </div>
                <div class="form-group">
                    <label>Motivo del traslado</label>
                    <select id="modalMotivoTraslado" class="form-input">
                        <option value="cambio_domicilio">Cambio de domicilio</option>
                        <option value="reubicacion">Reubicación del medidor</option>
                        <option value="otro">Otro</option>
                    </select>
                </div>
            `;
            break;
        case 'traspaso_servicio':
            htmlAdicional = `
                <div class="form-group">
                    <label>Nuevo titular</label>
                    <input type="text" id="modalNuevoTitularTraspaso" placeholder="Nombre completo del nuevo titular" class="form-input" />
                </div>
                <div class="form-group">
                    <label>Cédula nuevo titular</label>
                    <input type="text" id="modalCedulaTraspaso" placeholder="1-2345-6789" class="form-input" />
                </div>
                <div class="form-group">
                    <label>Parentesco (opcional)</label>
                    <input type="text" id="modalParentesco" placeholder="Ej: Hijo, Cónyuge, etc." class="form-input" />
                </div>
            `;
            break;
        case 'reclamo_danos':
            htmlAdicional = `
                <div class="form-group">
                    <label>Tipo de daño</label>
                    <select id="modalTipoDano" class="form-input">
                        <option value="electrodomestico">Electrodoméstico dañado</option>
                        <option value="instalacion">Instalación eléctrica</option>
                        <option value="infraestructura">Infraestructura</option>
                        <option value="otro">Otro</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Monto estimado del daño</label>
                    <input type="number" id="modalMontoDano" placeholder="0" class="form-input" />
                </div>
            `;
            break;
        default:
            htmlAdicional = '';
    }

    camposAdicionales.innerHTML = htmlAdicional;

    // Mostrar modal
    document.getElementById('modalTramite').style.display = 'block';
    document.getElementById('modalOverlay').style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// ===== CREAR MODAL DE TRÁMITE =====
function crearModalTramite() {
    var overlay = document.createElement('div');
    overlay.id = 'modalOverlay';
    overlay.className = 'modal-overlay';
    overlay.onclick = function (e) {
        if (e.target === this) cerrarModalTramite();
    };

    var modal = document.createElement('div');
    modal.id = 'modalTramite';
    modal.className = 'modal-tramite';
    modal.innerHTML = `
        <div class="modal-header">
            <h3 id="modalTitulo">Trámite</h3>
            <button class="modal-close" onclick="cerrarModalTramite()">✕</button>
        </div>
        <div class="modal-body">
            <input type="hidden" id="modalTipo" />
            <div style="background:#f7f8fc; border-radius:12px; padding:12px 16px; margin-bottom:16px; border:1px solid var(--line);">
                <span style="font-size:13px; color:var(--muted);">📋 Descripción:</span>
                <p id="modalDescripcionTramite" style="font-weight:600; font-size:14px; margin-top:4px; color:var(--ink);"></p>
            </div>
            <div class="form-group">
                <label>NISE del servicio</label>
                <input type="text" id="modalNise" placeholder="Ej: 402112345" class="form-input" required />
            </div>
            <div class="form-group">
                <label>Nombre completo</label>
                <input type="text" id="modalNombre" placeholder="Su nombre completo" class="form-input" required />
            </div>
            <div class="form-group">
                <label>Cédula</label>
                <input type="text" id="modalCedula" placeholder="1-2345-6789" class="form-input" required />
            </div>
            <div class="form-group">
                <label>Teléfono de contacto</label>
                <input type="text" id="modalTelefono" placeholder="8888-8888" class="form-input" required />
            </div>
            <div class="form-group">
                <label>Correo electrónico</label>
                <input type="email" id="modalCorreo" placeholder="usuario@correo.com" class="form-input" required />
            </div>
            <div id="camposAdicionales"></div>
            <div class="form-group">
                <label>Descripción detallada</label>
                <textarea id="modalDescripcion" rows="4" placeholder="Describa su solicitud en detalle..." class="form-input" required></textarea>
            </div>
        </div>
        <div class="modal-footer">
            <button class="btn-cancelar" onclick="cerrarModalTramite()">Cancelar</button>
            <button class="btn-enviar-tramite" onclick="enviarTramite()">Enviar Solicitud</button>
        </div>
    `;

    document.body.appendChild(overlay);
    document.body.appendChild(modal);
}

// ===== CERRAR MODAL =====
function cerrarModalTramite() {
    var modal = document.getElementById('modalTramite');
    var overlay = document.getElementById('modalOverlay');
    if (modal) modal.style.display = 'none';
    if (overlay) overlay.style.display = 'none';
    document.body.style.overflow = 'auto';
}

// ===== ENVIAR TRÁMITE =====
function enviarTramite() {
    var nise = document.getElementById('modalNise').value.trim();
    var nombre = document.getElementById('modalNombre').value.trim();
    var cedula = document.getElementById('modalCedula').value.trim();
    var telefono = document.getElementById('modalTelefono').value.trim();
    var correo = document.getElementById('modalCorreo').value.trim();
    var descripcion = document.getElementById('modalDescripcion').value.trim();

    if (!nise || !nombre || !cedula || !telefono || !correo || !descripcion) {
        alert('⚠️ Por favor, complete todos los campos obligatorios.');
        return;
    }

    if (!/^\d{1}-\d{4}-\d{4}$/.test(cedula) && !/^\d{9,10}$/.test(cedula)) {
        alert('⚠️ Formato de cédula inválido. Use 1-2345-6789 o 123456789');
        return;
    }

    if (!/^\d{4}-\d{4}$/.test(telefono) && !/^\d{8}$/.test(telefono)) {
        alert('⚠️ Formato de teléfono inválido. Use 8888-8888');
        return;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) {
        alert('⚠️ Ingrese un correo electrónico válido.');
        return;
    }

    var btn = document.querySelector('.btn-enviar-tramite');
    var textoOriginal = btn.textContent;
    btn.textContent = '⏳ Enviando...';
    btn.disabled = true;

    setTimeout(function () {
        var tipos = {
            'cambio_nombre': 'Cambio de nombre de abonado',
            'desconexion_reconexion': 'Desconexión y reconexión',
            'solicitud_servicio': 'Solicitud de servicio nuevo',
            'traslado_medidor': 'Traslado de medidor',
            'traspaso_servicio': 'Traspaso de servicio eléctrico',
            'reclamo_danos': 'Reclamo por daños'
        };

        var tipo = document.getElementById('modalTipo').value;
        var nombreTramite = tipos[tipo] || 'Trámite';

        alert('✅ ¡Solicitud enviada exitosamente!\n\n' +
            '📋 Trámite: ' + nombreTramite + '\n' +
            '📩 Número de seguimiento: #TR-' + Date.now().toString().slice(-6) + '\n' +
            '📧 Recibirás un correo con los detalles en 24 horas.\n\n' +
            '📞 Para consultas, llama al 800-ENERGÍA');

        btn.textContent = textoOriginal;
        btn.disabled = false;
        cerrarModalTramite();
    }, 1500);
}