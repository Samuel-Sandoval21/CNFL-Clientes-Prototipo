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

    var emojis = {
        'alumbrado': '💡',
        'propia': '🏠',
        'ajena': '📍'
    };

    // Feedback visual con efecto de click
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

    // Redirigir después del feedback visual
    setTimeout(function () {
        window.location.href = '/Clientes/Reportar' +
            (tipo === 'alumbrado' ? 'Alumbrado' :
                tipo === 'propia' ? 'Propia' : 'Ajena');
    }, 500);
}

// ===== MOSTRAR DETALLE DE AVERÍA CON MODAL PREMIUM =====
function verDetalleAveria(id) {
    var modal = document.createElement('div');
    modal.className = 'modal-reporte-premium';
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
                    <span style="font-weight: 800; font-size: 18px; color: var(--ink);">Detalle</span>
                </div>
                <button onclick="this.closest('div[style]').remove()" style="background: none; border: none; font-size: 22px; color: var(--muted); cursor: pointer; padding: 4px 8px; border-radius: 10px; transition: 0.3s;" onmouseover="this.style.background='#f7f8fc'" onmouseout="this.style.background='transparent'">✕</button>
            </div>
            <div style="background: linear-gradient(135deg, #f7f8fc, #eef0f5); border-radius: 16px; padding: 4px 0 0 0; margin-bottom: 16px;">
                <div style="padding: 12px 16px 8px;">
                    <h3 style="font-weight: 800; font-size: 17px; margin: 0;">Transformador dañado</h3>
                    <div style="color: var(--muted); font-size: 13px; margin-top: 4px;">
                        <span>📍 Barrio Los Ángeles, Cartago</span><br>
                        <span>📅 29/08/2026 10:49</span>
                    </div>
                </div>
            </div>
            <div style="background: #f7f8fc; border-radius: 12px; padding: 14px 18px; margin-bottom: 16px; border: 1px solid var(--line); position: relative; padding-left: 40px;">
                <span style="font-size: 24px; color: var(--blue); opacity: 0.15; position: absolute; top: 2px; left: 12px; font-family: Georgia, serif;">"</span>
                <span style="font-size: 14px; color: var(--ink); line-height: 1.5;">Se escuchan explosiones y no hay luz en el sector</span>
            </div>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding: 8px 4px;">
                <span style="font-weight: 700; font-size: 13px;">Estado</span>
                <span style="background: #FFEBEE; color: #C62828; padding: 4px 20px; border-radius: 999px; font-size: 12px; font-weight: 700; border: 1px solid #FFCDD2;">Reportado</span>
            </div>
            <div style="margin-bottom: 20px;">
                <div style="display: flex; justify-content: space-between; font-size: 12px; font-weight: 600; color: var(--muted); margin-bottom: 4px;">
                    <span>Progreso</span>
                    <span style="color: var(--blue); font-weight: 800;">25%</span>
                </div>
                <div style="height: 8px; background: #eef0f5; border-radius: 10px; overflow: hidden;">
                    <div style="height: 100%; width: 25%; background: linear-gradient(90deg, #1E23E6, #5a60f5); border-radius: 10px; transition: width 0.8s ease;"></div>
                </div>
            </div>
            <button onclick="this.closest('div[style]').remove()" style="width: 100%; padding: 14px; border-radius: 16px; border: none; background: linear-gradient(135deg, #1E23E6, #5a60f5); color: white; font-weight: 700; font-size: 15px; cursor: pointer; transition: 0.3s; font-family: inherit; box-shadow: 0 8px 24px rgba(30,35,230,0.25);" onmouseover="this.style.transform='scale(0.98)'" onmouseout="this.style.transform='scale(1)'">
                Entendido
            </button>
        </div>
    `;

    // Agregar estilos de animación
    var style = document.createElement('style');
    style.textContent = `
        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }
        @keyframes slideUp {
            from { opacity: 0; transform: translateY(30px) scale(0.95); }
            to { opacity: 1; transform: translateY(0) scale(1); }
        }
    `;
    document.head.appendChild(style);

    document.body.appendChild(modal);

    // Cerrar al hacer clic fuera
    modal.addEventListener('click', function (e) {
        if (e.target === this) {
            this.remove();
        }
    });
}

// ===== VER MAPA COMPLETO CON MODAL PREMIUM =====
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
            <div style="height: 220px; background: linear-gradient(135deg, #0a1628, #1a2a4a); border-radius: 20px; position: relative; overflow: hidden; margin-bottom: 16px; border: 1px solid rgba(255,255,255,0.05);">
                <div style="position: absolute; inset: 0; background-image: linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px); background-size: 30px 30px;"></div>
                <div style="position: absolute; top: 20%; left: 12%; width: 34px; height: 34px; background: linear-gradient(135deg, #E5484D, #b71c1c); border-radius: 50% 50% 50% 2px; transform: rotate(45deg); display: grid; place-items: center; box-shadow: 0 8px 30px rgba(229,72,77,0.4); border: 2px solid rgba(255,255,255,0.15);">
                    <span style="transform: rotate(-45deg); color: white; font-size: 13px; font-weight: 700;">!</span>
                </div>
                <div style="position: absolute; top: 45%; left: 68%; width: 34px; height: 34px; background: linear-gradient(135deg, #F5A623, #e65100); border-radius: 50% 50% 50% 2px; transform: rotate(45deg); display: grid; place-items: center; box-shadow: 0 8px 30px rgba(245,166,35,0.4); border: 2px solid rgba(255,255,255,0.15);">
                    <span style="transform: rotate(-45deg); color: white; font-size: 13px; font-weight: 700;">⏱</span>
                </div>
                <div style="position: absolute; top: 72%; left: 25%; width: 34px; height: 34px; background: linear-gradient(135deg, #43A047, #1b5e20); border-radius: 50% 50% 50% 2px; transform: rotate(45deg); display: grid; place-items: center; box-shadow: 0 8px 30px rgba(67,160,71,0.4); border: 2px solid rgba(255,255,255,0.15);">
                    <span style="transform: rotate(-45deg); color: white; font-size: 13px; font-weight: 700;">✓</span>
                </div>
                <div style="position: absolute; bottom: 16px; left: 50%; transform: translateX(-50%); background: rgba(0,0,0,0.5); backdrop-filter: blur(8px); color: white; padding: 4px 18px; border-radius: 20px; font-size: 11px; font-weight: 600; border: 1px solid rgba(255,255,255,0.08);">
                    <i class="fas fa-search-plus"></i> Interactivo próximamente
                </div>
                <div style="position: absolute; top: 14px; right: 14px; display: flex; align-items: center; gap: 6px; background: rgba(0,0,0,0.4); backdrop-filter: blur(6px); padding: 4px 12px; border-radius: 16px;">
                    <span style="width: 6px; height: 6px; border-radius: 50%; background: #43A047; animation: pulse-dot 2s infinite;"></span>
                    <span style="color: rgba(255,255,255,0.7); font-size: 10px; font-weight: 600;">En vivo</span>
                </div>
            </div>
            <div style="display: flex; gap: 14px; flex-wrap: wrap; margin-bottom: 16px; justify-content: center;">
                <div style="display: flex; align-items: center; gap: 8px; font-size: 12px; font-weight: 600; color: var(--muted);">
                    <span style="width: 12px; height: 12px; border-radius: 50%; background: linear-gradient(135deg, #E5484D, #b71c1c);"></span>
                    Activas
                </div>
                <div style="display: flex; align-items: center; gap: 8px; font-size: 12px; font-weight: 600; color: var(--muted);">
                    <span style="width: 12px; height: 12px; border-radius: 50%; background: linear-gradient(135deg, #F5A623, #e65100);"></span>
                    Revisión
                </div>
                <div style="display: flex; align-items: center; gap: 8px; font-size: 12px; font-weight: 600; color: var(--muted);">
                    <span style="width: 12px; height: 12px; border-radius: 50%; background: linear-gradient(135deg, #43A047, #1b5e20);"></span>
                    Resueltas
                </div>
            </div>
            <button onclick="this.closest('div[style]').remove()" style="width: 100%; padding: 14px; border-radius: 16px; border: 1.5px solid var(--line); background: white; color: var(--ink); font-weight: 700; font-size: 14px; cursor: pointer; transition: 0.3s; font-family: inherit;" onmouseover="this.style.background='#f7f8fc'" onmouseout="this.style.background='white'">
                Cerrar
            </button>
        </div>
    `;

    // Agregar estilos de animación si no existen
    if (!document.querySelector('style[data-reportes]')) {
        var style = document.createElement('style');
        style.setAttribute('data-reportes', 'true');
        style.textContent = `
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            @keyframes slideUp {
                from { opacity: 0; transform: translateY(30px) scale(0.95); }
                to { opacity: 1; transform: translateY(0) scale(1); }
            }
            @keyframes pulse-dot {
                0%, 100% { opacity: 1; }
                50% { opacity: 0.4; }
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(modal);

    modal.addEventListener('click', function (e) {
        if (e.target === this) {
            this.remove();
        }
    });
}