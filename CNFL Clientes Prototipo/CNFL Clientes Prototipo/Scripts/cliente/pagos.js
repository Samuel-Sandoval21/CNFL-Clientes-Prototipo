// ==========================================
// CLIENTE - PAGOS
// ==========================================

var pagoEnProceso = false;

// ==========================================================
// ===== INICIAR PAGO =====
// ==========================================================
function iniciarPago(metodo, facturaId) {
    if (pagoEnProceso) {
        alert('⏳ Ya hay un pago en proceso. Por favor espera.');
        return;
    }

    var montoEl = document.querySelector('.monto-pagar');
    var monto = '0';
    if (montoEl) {
        monto = montoEl.textContent.replace('₡', '').replace(/,/g, '').replace('€', '').trim();
    }

    if (parseFloat(monto) <= 0) {
        alert('❌ Error: No se pudo identificar el monto a pagar');
        return;
    }

    if (!facturaId || facturaId === '0' || facturaId === '') {
        var hidden = document.getElementById('facturaId');
        if (hidden) facturaId = hidden.value;
    }
    if (!facturaId || facturaId === '0' || facturaId === '') {
        facturaId = '001';
    }

    procesarPago(metodo, facturaId, monto);
}

// ==========================================================
// ===== PROCESAR PAGO =====
// ==========================================================
function procesarPago(metodo, facturaId, monto) {
    if (pagoEnProceso) return;
    pagoEnProceso = true;

    mostrarProcesamiento(metodo, monto, facturaId);

    var tiempoProceso = 3000 + Math.random() * 2000;
    var exito = Math.random() > 0.15;

    setTimeout(function () {
        if (exito) {
            pagoExitoso(metodo, facturaId, monto);
        } else {
            pagoFallido(metodo, facturaId);
        }
    }, tiempoProceso);
}

// ==========================================================
// ===== MOSTRAR PROCESAMIENTO =====
// ==========================================================
function mostrarProcesamiento(metodo, monto, facturaId) {
    var nombresMetodo = {
        'Sinpe': 'SINPE Móvil',
        'Iban': 'Transferencia IBAN',
        'Tarjeta': 'Tarjeta'
    };

    var coloresMetodo = {
        'Sinpe': '#64B95A',
        'Iban': '#1E23E6',
        'Tarjeta': '#FF692D'
    };

    var iconosMetodo = {
        'Sinpe': '📱',
        'Iban': '🏦',
        'Tarjeta': '💳'
    };

    var overlay = document.createElement('div');
    overlay.id = 'overlayProcesamiento';
    overlay.style.cssText = 'position:fixed;top:0;left:0;right:0;bottom:0;background:rgba(0,0,0,0.8);backdrop-filter:blur(20px);z-index:10000;display:flex;justify-content:center;align-items:center;flex-direction:column;color:white;font-family:"Inter",sans-serif;animation:fadeInPago 0.4s ease;';

    overlay.innerHTML = '<div style="text-align:center;max-width:340px;width:100%;padding:20px;">' +
        '<div style="width:80px;height:80px;border-radius:50%;background:rgba(255,255,255,0.08);display:flex;justify-content:center;align-items:center;margin:0 auto 24px;border:2px solid rgba(255,255,255,0.1);position:relative;">' +
        '<div style="font-size:32px;">' + iconosMetodo[metodo] + '</div>' +
        '<div style="position:absolute;inset:-6px;border-radius:50%;border:2px solid rgba(255,255,255,0.05);animation:spinPago 3s linear infinite;"></div>' +
        '</div>' +
        '<div style="font-size:22px;font-weight:800;margin-bottom:6px;">Procesando pago</div>' +
        '<div style="font-size:14px;color:rgba(255,255,255,0.6);margin-bottom:4px;">' + nombresMetodo[metodo] + '</div>' +
        '<div style="font-size:28px;font-weight:800;color:' + coloresMetodo[metodo] + ';margin:12px 0 20px;">₡ ' + parseInt(monto).toLocaleString() + '</div>' +
        '<div style="width:100%;height:6px;background:rgba(255,255,255,0.1);border-radius:6px;overflow:hidden;margin-bottom:16px;">' +
        '<div id="barraProgresoPago" style="width:0%;height:100%;background:linear-gradient(90deg,' + coloresMetodo[metodo] + ',' + (metodo === 'Sinpe' ? '#43A047' : metodo === 'Iban' ? '#5a60f5' : '#ff8a5c') + ');border-radius:6px;transition:width 0.8s ease;"></div>' +
        '</div>' +
        '<div id="estadoProcesamiento" style="font-size:14px;color:rgba(255,255,255,0.7);font-weight:500;">Iniciando transacción...</div>' +
        '<div style="margin-top:20px;display:flex;flex-direction:column;gap:6px;text-align:left;padding:0 8px;">' +
        '<div id="paso1" style="display:flex;align-items:center;gap:10px;font-size:12px;color:rgba(255,255,255,0.3);"><span style="font-size:14px;">○</span><span>Conectando con el banco</span></div>' +
        '<div id="paso2" style="display:flex;align-items:center;gap:10px;font-size:12px;color:rgba(255,255,255,0.3);"><span style="font-size:14px;">○</span><span>Verificando datos</span></div>' +
        '<div id="paso3" style="display:flex;align-items:center;gap:10px;font-size:12px;color:rgba(255,255,255,0.3);"><span style="font-size:14px;">○</span><span>Autorizando transacción</span></div>' +
        '<div id="paso4" style="display:flex;align-items:center;gap:10px;font-size:12px;color:rgba(255,255,255,0.3);"><span style="font-size:14px;">○</span><span>Confirmando pago</span></div>' +
        '</div></div>';

    if (!document.getElementById('pagoStyles')) {
        var style = document.createElement('style');
        style.id = 'pagoStyles';
        style.textContent = '@keyframes fadeInPago{from{opacity:0}to{opacity:1}}@keyframes spinPago{to{transform:rotate(360deg)}}@keyframes checkBouncePago{0%{transform:scale(0)}50%{transform:scale(1.2)}70%{transform:scale(0.9)}100%{transform:scale(1)}}';
        document.head.appendChild(style);
    }

    document.body.appendChild(overlay);

    var progreso = 0;
    var pasoActual = 0;
    var pasos = ['paso1', 'paso2', 'paso3', 'paso4'];
    var mensajes = ['Conectando con el banco...', 'Verificando datos de pago...', 'Autorizando transacción...', 'Confirmando pago...'];

    var interval = setInterval(function () {
        progreso += 1 + Math.random() * 3;
        if (progreso > 100) {
            clearInterval(interval);
            progreso = 100;
        }

        var barra = document.getElementById('barraProgresoPago');
        if (barra) barra.style.width = Math.min(progreso, 100) + '%';

        var nuevoPaso = Math.floor(progreso / 25);
        if (nuevoPaso > pasoActual && nuevoPaso < 4) {
            pasoActual = nuevoPaso;
            var pasoEl = document.getElementById(pasos[pasoActual]);
            if (pasoEl) {
                pasoEl.style.color = 'rgba(255,255,255,0.9)';
                var icono = pasoEl.querySelector('span:first-child');
                if (icono) icono.textContent = '●';
                if (icono) icono.style.color = '#64B95A';
            }
            var estado = document.getElementById('estadoProcesamiento');
            if (estado) estado.textContent = mensajes[pasoActual] || 'Procesando...';

            if (pasoActual > 0) {
                var pasoAnt = document.getElementById(pasos[pasoActual - 1]);
                if (pasoAnt) {
                    pasoAnt.style.color = 'rgba(255,255,255,0.7)';
                    var iconoAnt = pasoAnt.querySelector('span:first-child');
                    if (iconoAnt) iconoAnt.textContent = '✅';
                }
            }
        }

        if (progreso >= 100) {
            pasos.forEach(function (p) {
                var el = document.getElementById(p);
                if (el) {
                    el.style.color = 'rgba(255,255,255,0.7)';
                    var icono = el.querySelector('span:first-child');
                    if (icono) icono.textContent = '✅';
                }
            });
            var estado = document.getElementById('estadoProcesamiento');
            if (estado) {
                estado.textContent = '¡Pago completado con éxito!';
                estado.style.color = '#64B95A';
            }
        }
    }, 200);
}

// ==========================================================
// ===== PAGO EXITOSO =====
// ==========================================================
function pagoExitoso(metodo, facturaId, monto) {
    var transaccion = 'CNFL-' + Date.now().toString().slice(-6) + '-' + String(Math.floor(Math.random() * 9000) + 1000);
    var autorizacion = String(Math.floor(Math.random() * 900000) + 100000);

    var overlay = document.getElementById('overlayProcesamiento');
    if (overlay) {
        overlay.innerHTML = '<div style="text-align:center;max-width:380px;width:100%;padding:20px;">' +
            '<div style="width:100px;height:100px;border-radius:50%;background:rgba(100,185,90,0.15);display:flex;justify-content:center;align-items:center;margin:0 auto 20px;border:3px solid #64B95A;animation:checkBouncePago 0.6s ease;">' +
            '<svg viewBox="0 0 24 24" width="56" height="56" fill="none" stroke="#64B95A" stroke-width="3.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12" /></svg>' +
            '</div>' +
            '<div style="font-size:26px;font-weight:800;color:white;margin-bottom:4px;">¡Pago exitoso!</div>' +
            '<div style="font-size:14px;color:rgba(255,255,255,0.6);margin-bottom:8px;">' + (metodo === 'Sinpe' ? 'SINPE Móvil' : metodo === 'Iban' ? 'Transferencia IBAN' : 'Tarjeta') + '</div>' +
            '<div style="font-size:30px;font-weight:900;color:#64B95A;margin-bottom:16px;">₡ ' + parseInt(monto).toLocaleString() + '</div>' +
            '<div style="background:rgba(255,255,255,0.06);border-radius:16px;padding:16px 20px;border:1px solid rgba(255,255,255,0.08);margin-bottom:16px;text-align:left;">' +
            '<div style="display:grid;grid-template-columns:1fr 1fr;gap:8px;">' +
            '<div><div style="font-size:10px;color:rgba(255,255,255,0.4);">Transacción</div><div style="font-size:13px;font-weight:700;color:white;">' + transaccion + '</div></div>' +
            '<div><div style="font-size:10px;color:rgba(255,255,255,0.4);">Autorización</div><div style="font-size:13px;font-weight:700;color:white;">' + autorizacion + '</div></div>' +
            '<div><div style="font-size:10px;color:rgba(255,255,255,0.4);">Fecha</div><div style="font-size:13px;font-weight:700;color:white;">' + new Date().toLocaleString('es-CR') + '</div></div>' +
            '<div><div style="font-size:10px;color:rgba(255,255,255,0.4);">Factura</div><div style="font-size:13px;font-weight:700;color:white;">#' + facturaId + '</div></div>' +
            '</div></div>' +
            '<button onclick="cerrarPago()" style="padding:14px 32px;border-radius:14px;border:none;background:linear-gradient(135deg,#1E23E6,#5a60f5);color:white;font-weight:800;font-size:14px;cursor:pointer;transition:0.2s;font-family:inherit;box-shadow:0 6px 20px rgba(30,35,230,0.3);width:100%;"><i class="fas fa-file-invoice"></i> Ver comprobante</button>' +
            '<div style="margin-top:12px;font-size:12px;color:rgba(255,255,255,0.3);">Se envió un comprobante a tu correo electrónico</div>' +
            '</div>';
    }

    setTimeout(function () {
        cerrarPago();
        window.location.href = '/Clientes/Pagos';
    }, 5000);
}

// ==========================================================
// ===== PAGO FALLIDO =====
// ==========================================================
function pagoFallido(metodo, facturaId) {
    var motivos = ['Saldo insuficiente en la cuenta', 'Error de conexión con el banco', 'Tarjeta rechazada por el emisor', 'Tiempo de espera agotado', 'Datos de pago incorrectos'];
    var motivo = motivos[Math.floor(Math.random() * motivos.length)];

    var overlay = document.getElementById('overlayProcesamiento');
    if (overlay) {
        overlay.innerHTML = '<div style="text-align:center;max-width:340px;width:100%;padding:20px;">' +
            '<div style="width:100px;height:100px;border-radius:50%;background:rgba(229,72,77,0.15);display:flex;justify-content:center;align-items:center;margin:0 auto 20px;border:3px solid #E5484D;">' +
            '<svg viewBox="0 0 24 24" width="48" height="48" fill="none" stroke="#E5484D" stroke-width="3"><line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" /></svg>' +
            '</div>' +
            '<div style="font-size:24px;font-weight:800;color:white;margin-bottom:4px;">Error en el pago</div>' +
            '<div style="font-size:14px;color:rgba(255,255,255,0.6);margin-bottom:16px;">No se pudo completar la transacción</div>' +
            '<div style="background:rgba(255,255,255,0.06);border-radius:14px;padding:14px 18px;border:1px solid rgba(255,255,255,0.08);margin-bottom:16px;text-align:left;">' +
            '<div style="font-size:11px;color:rgba(255,255,255,0.4);">Motivo</div>' +
            '<div style="font-size:14px;font-weight:600;color:#E5484D;margin-top:2px;">' + motivo + '</div>' +
            '</div>' +
            '<div style="display:flex;gap:12px;">' +
            '<button onclick="cerrarPago()" style="flex:1;padding:14px;border-radius:14px;border:1.5px solid rgba(255,255,255,0.15);background:transparent;color:rgba(255,255,255,0.7);font-weight:700;font-size:14px;cursor:pointer;font-family:inherit;">Cancelar</button>' +
            '<button onclick="cerrarPago();setTimeout(function(){ iniciarPago(\'' + metodo + '\', \'' + facturaId + '\'); },300);" style="flex:2;padding:14px;border-radius:14px;border:none;background:linear-gradient(135deg,#1E23E6,#5a60f5);color:white;font-weight:800;font-size:14px;cursor:pointer;font-family:inherit;box-shadow:0 6px 20px rgba(30,35,230,0.3);"><i class="fas fa-redo"></i> Reintentar</button>' +
            '</div></div>';
    }
}

// ==========================================================
// ===== CERRAR PAGO =====
// ==========================================================
function cerrarPago() {
    var overlay = document.getElementById('overlayProcesamiento');
    if (overlay) overlay.remove();
    document.body.style.overflow = '';
    pagoEnProceso = false;
}

// Exponer funciones
window.iniciarPago = iniciarPago;
window.procesarPago = procesarPago;
window.cerrarPago = cerrarPago;