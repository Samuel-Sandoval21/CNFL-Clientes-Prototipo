// ==========================================
// CALCULADORA - JAVASCRIPT
// ==========================================

// ===== CALCULAR CONSUMO =====
function calcularConsumo() {
    var input = document.getElementById('kwhInput');
    var kwh = parseFloat(input.value) || 0;

    if (kwh <= 0) {
        alert('⚠️ Ingresa un valor de consumo válido (mayor a 0).');
        input.focus();
        return;
    }

    var costoPorKwh = 95; // Precio por kWh en colones
    var costo = kwh * costoPorKwh;

    var resultado = document.getElementById('resultadoConsumo');
    document.getElementById('costoCalculado').textContent = '₡ ' + costo.toLocaleString();
    resultado.style.display = 'block';

    // Animar el resultado
    resultado.style.opacity = '0';
    resultado.style.transform = 'translateY(10px)';
    setTimeout(function () {
        resultado.style.transition = 'all 0.4s ease';
        resultado.style.opacity = '1';
        resultado.style.transform = 'translateY(0)';
    }, 50);

    // Scroll al resultado
    setTimeout(function () {
        resultado.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }, 200);
}

// ===== SETEAR KWH DESDE EQUIPO COMÚN =====
function setKwh(valor) {
    document.getElementById('kwhInput').value = valor;
    calcularConsumo();
}

// ===== ENTER PARA CALCULAR =====
document.addEventListener('DOMContentLoaded', function () {
    var input = document.getElementById('kwhInput');
    if (input) {
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                calcularConsumo();
            }
        });
    }
});