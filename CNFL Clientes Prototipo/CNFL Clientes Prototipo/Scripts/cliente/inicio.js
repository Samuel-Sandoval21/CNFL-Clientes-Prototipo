// ==========================================
// CLIENTE INICIO - JAVASCRIPT
// ==========================================

// ===== CAMBIAR PERIODO DEL GRÁFICO =====
function cambiarPeriodo(periodo) {
    // Actualizar chips
    document.querySelectorAll('.chip').forEach(function (chip) {
        chip.classList.remove('active');
    });

    var chips = document.querySelectorAll('.chip');
    var mapa = {
        '6m': 0,
        '31d': 1,
        'lecturas': 2
    };

    if (mapa[periodo] !== undefined && chips[mapa[periodo]]) {
        chips[mapa[periodo]].classList.add('active');
    }

    // Obtener las barras del gráfico
    var barras = document.querySelectorAll('.barra');

    if (barras.length === 0) {
        barras = document.querySelectorAll('#graficoConsumo > div');
    }

    if (barras.length === 0) {
        barras = document.querySelectorAll('.chart-container > div');
    }

    if (barras.length === 0) {
        var card = document.querySelector('.card');
        if (card) {
            barras = card.querySelectorAll('div[style*="height:"]');
        }
    }

    var alturas = {
        '6m': ['45%', '62%', '53%', '71%', '58%', '84%'],
        '31d': ['60%', '40%', '75%', '50%', '65%'],
        'lecturas': ['80%', '60%', '70%', '45%', '55%', '75%']
    };

    var nuevasAlturas = alturas[periodo] || alturas['6m'];

    barras.forEach(function (barra, index) {
        if (index < nuevasAlturas.length) {
            barra.style.height = nuevasAlturas[index];
            barra.style.transition = 'height 0.6s ease';
        }
    });
}

// ===== INICIALIZAR =====
document.addEventListener('DOMContentLoaded', function () {
    var barras = document.querySelectorAll('.barra');

    if (barras.length === 0) {
        barras = document.querySelectorAll('#graficoConsumo > div');
    }

    if (barras.length === 0) {
        barras = document.querySelectorAll('.chart-container > div');
    }

    if (barras.length === 0) {
        var card = document.querySelector('.card');
        if (card) {
            barras = card.querySelectorAll('div[style*="height:"]');
        }
    }

    if (barras.length > 0) {
        var alturasDefecto = ['45%', '62%', '53%', '71%', '58%', '84%'];
        barras.forEach(function (barra, index) {
            if (index < alturasDefecto.length && !barra.style.height) {
                barra.style.height = alturasDefecto[index];
            }
        });
    }
});