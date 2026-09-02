// ==========================================
// CLIENTE REPORTES - JAVASCRIPT PREMIUM
// ==========================================

// ===== INICIALIZAR =====
document.addEventListener('DOMContentLoaded', function () {
    console.log('📋 Reportes - Premium cargado');

    // Animar barras de progreso
    setTimeout(function () {
        var fills = document.querySelectorAll('.progress-fill');
        fills.forEach(function (fill) {
            var targetWidth = fill.style.width || '25%';
            fill.style.width = '0%';
            setTimeout(function () {
                fill.style.width = targetWidth;
            }, 300);
        });
    }, 600);

    // Efecto de entrada para el mapa
    var mapContainer = document.querySelector('.map-container');
    if (mapContainer) {
        mapContainer.style.opacity = '0';
        mapContainer.style.transform = 'translateY(20px)';
        setTimeout(function () {
            mapContainer.style.transition = 'all 0.8s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            mapContainer.style.opacity = '1';
            mapContainer.style.transform = 'translateY(0)';
        }, 100);
    }
});

// ===== REDIRIGIR A FORMULARIO DE REPORTE =====
function irAReporte(tipo) {
    var nombres = {
        'alumbrado': 'Alumbrado Público',
        'propia': 'Avería Eléctrica Propia',
        'ajena': 'Avería Eléctrica Ajena'
    };

    var cards = document.querySelectorAll('.nuevo-reporte-card');
    cards.forEach(function (card) {
        if (card.dataset.tipo === tipo) {
            card.style.transform = 'scale(0.92)';
            card.style.boxShadow = '0 0 0 4px #1E23E6, 0 12px 40px rgba(30,35,230,0.25)';
            setTimeout(function () {
                card.style.transform = '';
                card.style.boxShadow = '';
            }, 500);
        }
    });

    setTimeout(function () {
        window.location.href = '/Clientes/Reportar' +
            (tipo === 'alumbrado' ? 'Alumbrado' :
                tipo === 'propia' ? 'Propia' : 'Ajena');
    }, 500);
}

// ===== MOSTRAR DETALLE DE AVERÍA CON MODAL =====
function verDetalleAveria(id) {
    var modal = document.createElement('div');
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0,0,0,0.55);
        backdrop-filter: blur(12px);
        z-index: 2000;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 20px;
        animation: fadeIn 0.4s ease;
    `;

    modal.innerHTML = `
        <div style="background: white; border-radius: 28px; max-width: 400px; width: 100%; padding: 28px 24px 24px; box-shadow: 0 32px 80px rgba(0,0,0,0.35); animation: slideUp 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
                <div style="display: flex; align-items: center; gap: 10px;">
                    <span style="font-size: 28px;">⚡</span>
                    <span style="font-weight: 800; font-size: 18px; color: var(--ink);">Detalle de avería</span>
                </div>
                <button onclick="this.closest('div[style]').remove()" style="background: none; border: none; font-size: 22px; color: var(--muted); cursor: pointer; padding: 4px 8px; border-radius: 10px; transition: 0.3s;" onmouseover="this.style.background='#f7f8fc'" onmouseout="this.style.background='transparent'">✕</button>
            </div>

            <div id="detalleContenido" style="text-align: center; padding: 20px 0;">
                <div class="spinner" style="border: 4px solid #f3f3f3; border-top: 4px solid #1E23E6; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; margin: 0 auto;"></div>
                <p style="margin-top: 12px; color: var(--muted);">Cargando detalles...</p>
            </div>

            <button onclick="this.closest('div[style]').remove()" style="width: 100%; padding: 14px; border-radius: 16px; border: none; background: linear-gradient(135deg, #1E23E6, #5a60f5); color: white; font-weight: 700; font-size: 15px; cursor: pointer; transition: 0.3s; font-family: inherit; box-shadow: 0 8px 24px rgba(30,35,230,0.25);" onmouseover="this.style.transform='scale(0.98)'" onmouseout="this.style.transform='scale(1)'">
                Entendido
            </button>
        </div>
    `;

    // Agregar estilos de animación
    if (!document.querySelector('style[data-reportes-modal]')) {
        var style = document.createElement('style');
        style.setAttribute('data-reportes-modal', 'true');
        style.textContent = `
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            @keyframes slideUp {
                from { opacity: 0; transform: translateY(30px) scale(0.95); }
                to { opacity: 1; transform: translateY(0) scale(1); }
            }
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(modal);

    // Cargar detalles de la avería
    cargarDetalleAveria(id, modal);

    // Cerrar al hacer clic fuera
    modal.addEventListener('click', function (e) {
        if (e.target === this) {
            this.remove();
        }
    });
}

function cargarDetalleAveria(id, modal) {
    $.ajax({
        url: '/Clientes/ObtenerAveriaDetalle',
        type: 'GET',
        data: { id: id },
        success: function (data) {
            var container = modal.querySelector('#detalleContenido');
            if (data.success && data.averia) {
                var a = data.averia;
                var colores = {
                    'Reportado': { bg: '#FFEBEE', color: '#C62828' },
                    'En revisión': { bg: '#FFF8E1', color: '#E65100' },
                    'En camino': { bg: '#E3F2FD', color: '#0D47A1' },
                    'Resuelto': { bg: '#E8F5E9', color: '#1B5E20' }
                };
                var estadoInfo = colores[a.Estado] || { bg: '#F5F6FA', color: '#727A86' };

                var progreso = 0;
                if (a.Estado === 'Reportado') progreso = 25;
                else if (a.Estado === 'En revisión') progreso = 50;
                else if (a.Estado === 'En camino') progreso = 75;
                else if (a.Estado === 'Resuelto') progreso = 100;

                container.innerHTML = `
                    <div style="background: linear-gradient(135deg, #f7f8fc, #eef0f5); border-radius: 16px; padding: 16px; margin-bottom: 16px; text-align: left;">
                        <h3 style="font-weight: 800; font-size: 17px; margin: 0;">${a.TipoAveria}</h3>
                        <div style="color: var(--muted); font-size: 13px; margin-top: 4px;">
                            <span>📍 ${a.Direccion}</span><br>
                            <span>📅 ${a.FechaReporte}</span>
                        </div>
                    </div>

                    <div style="background: #f7f8fc; border-radius: 12px; padding: 14px 18px; margin-bottom: 16px; border: 1px solid var(--line); position: relative; padding-left: 40px; text-align: left;">
                        <span style="font-size: 24px; color: var(--blue); opacity: 0.15; position: absolute; top: 2px; left: 12px; font-family: Georgia, serif;">"</span>
                        <span style="font-size: 14px; color: var(--ink); line-height: 1.5;">${a.Descripcion || 'Sin descripción adicional'}</span>
                    </div>

                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding: 8px 4px;">
                        <span style="font-weight: 700; font-size: 13px;">Estado</span>
                        <span style="background: ${estadoInfo.bg}; color: ${estadoInfo.color}; padding: 4px 20px; border-radius: 999px; font-size: 12px; font-weight: 700;">${a.Estado}</span>
                    </div>

                    <div style="margin-bottom: 4px;">
                        <div style="display: flex; justify-content: space-between; font-size: 12px; font-weight: 600; color: var(--muted); margin-bottom: 4px;">
                            <span>Progreso</span>
                            <span style="color: var(--blue); font-weight: 800;">${progreso}%</span>
                        </div>
                        <div style="height: 8px; background: #eef0f5; border-radius: 10px; overflow: hidden;">
                            <div style="height: 100%; width: ${progreso}%; background: linear-gradient(90deg, #1E23E6, #5a60f5); border-radius: 10px; transition: width 0.8s ease;"></div>
                        </div>
                    </div>
                `;
            } else {
                container.innerHTML = `
                    <div style="text-align: center; padding: 20px;">
                        <span style="font-size: 48px;">❌</span>
                        <p style="color: var(--muted);">No se pudo cargar el detalle de la avería.</p>
                    </div>
                `;
            }
        },
        error: function () {
            var container = modal.querySelector('#detalleContenido');
            container.innerHTML = `
                <div style="text-align: center; padding: 20px;">
                    <span style="font-size: 48px;">⚠️</span>
                    <p style="color: var(--muted);">Error al cargar los detalles.</p>
                </div>
            `;
        }
    });
}

// ===== VER MAPA COMPLETO CON MODAL =====
function verMapaCompleto() {
    var modal = document.createElement('div');
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0,0,0,0.6);
        backdrop-filter: blur(12px);
        z-index: 2000;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 20px;
        animation: fadeIn 0.4s ease;
    `;

    modal.innerHTML = `
        <div style="background: white; border-radius: 28px; max-width: 420px; width: 100%; padding: 24px; box-shadow: 0 32px 80px rgba(0,0,0,0.4); animation: slideUp 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
                <span style="font-weight: 800; font-size: 18px;">🗺️ Mapa de averías</span>
                <button onclick="this.closest('div[style]').remove()" style="background: none; border: none; font-size: 22px; color: var(--muted); cursor: pointer; padding: 4px 8px; border-radius: 10px; transition: 0.3s;" onmouseover="this.style.background='#f7f8fc'" onmouseout="this.style.background='transparent'">✕</button>
            </div>

            <div id="modalMapContainer" style="height: 300px; border-radius: 16px; overflow: hidden; margin-bottom: 16px; border: 1px solid var(--line);"></div>

            <button onclick="this.closest('div[style]').remove()" style="width: 100%; padding: 14px; border-radius: 16px; border: 1.5px solid var(--line); background: white; color: var(--ink); font-weight: 700; font-size: 14px; cursor: pointer; transition: 0.3s; font-family: inherit;" onmouseover="this.style.background='#f7f8fc'" onmouseout="this.style.background='white'">
                Cerrar
            </button>
        </div>
    `;

    document.body.appendChild(modal);

    // Inicializar mapa en el modal
    setTimeout(function () {
        var container = document.getElementById('modalMapContainer');
        if (container) {
            var map = new google.maps.Map(container, {
                zoom: 8,
                center: { lat: 9.7489, lng: -83.7534 },
                mapTypeId: 'roadmap'
            });

            // Cargar marcadores en el mapa del modal
            $.ajax({
                url: '/Clientes/ObtenerAverias',
                type: 'GET',
                success: function (data) {
                    var colores = {
                        'Reportado': '#E5484D',
                        'En revisión': '#F5A623',
                        'En camino': '#1E23E6',
                        'Resuelto': '#64B95A'
                    };

                    data.forEach(function (averia) {
                        if (averia.Latitud && averia.Longitud) {
                            var marcador = new google.maps.Marker({
                                position: {
                                    lat: parseFloat(averia.Latitud),
                                    lng: parseFloat(averia.Longitud)
                                },
                                map: map,
                                title: averia.TipoAveria,
                                icon: {
                                    path: google.maps.SymbolPath.CIRCLE,
                                    fillColor: colores[averia.Estado] || '#1E23E6',
                                    fillOpacity: 0.8,
                                    strokeColor: '#FFFFFF',
                                    strokeWeight: 1,
                                    scale: 8
                                }
                            });
                        }
                    });

                    // Ajustar zoom
                    if (data.length > 0) {
                        var bounds = new google.maps.LatLngBounds();
                        data.forEach(function (averia) {
                            if (averia.Latitud && averia.Longitud) {
                                bounds.extend({
                                    lat: parseFloat(averia.Latitud),
                                    lng: parseFloat(averia.Longitud)
                                });
                            }
                        });
                        if (!bounds.isEmpty()) {
                            map.fitBounds(bounds);
                        }
                    }
                }
            });
        }
    }, 100);

    modal.addEventListener('click', function (e) {
        if (e.target === this) {
            this.remove();
        }
    });
}

// ===== EXPONER FUNCIONES GLOBALMENTE =====
window.irAReporte = irAReporte;
window.verDetalleAveria = verDetalleAveria;
window.verMapaCompleto = verMapaCompleto;