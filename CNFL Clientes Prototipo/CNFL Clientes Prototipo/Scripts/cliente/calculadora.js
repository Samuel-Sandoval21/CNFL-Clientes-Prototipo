// ============================================================
// CALCULADORA.JS - Cliente
// ============================================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('Calculadora cargado');

    var btn = document.getElementById('calcularBtn');
    var equipoSelect = document.getElementById('equipoSelect');
    var horasInput = document.getElementById('horasDia');

    if (!btn || !equipoSelect || !horasInput) return;

    btn.addEventListener('click', function () {
        var potencia = parseInt(equipoSelect.value);
        var horasDia = parseInt(horasInput.value) || 0;

        if (horasDia < 1 || horasDia > 24) {
            mostrarToast('⚠️ Ingresa un número de horas válido (1-24).', 'warning');
            horasInput.focus();
            return;
        }

        var consumoDiario = (potencia * horasDia) / 1000;
        var consumoMensual = consumoDiario * 30;
        var costoEstimado = consumoMensual * 60; // ₡60 por kWh

        document.getElementById('consumoMensual').textContent = consumoMensual.toFixed(2);
        document.getElementById('costoEstimado').textContent = costoEstimado.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ',');

        var resultado = document.getElementById('resultadoCard');
        if (resultado) {
            resultado.style.display = 'block';
            resultado.scrollIntoView({ behavior: 'smooth' });
        }

        mostrarToast('✅ Cálculo realizado correctamente', 'success');
    });

    // ===== Auto-calcular con Enter =====
    horasInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            btn.click();
        }
    });

    // ===== Validar entrada de horas =====
    horasInput.addEventListener('change', function () {
        var val = parseInt(this.value);
        if (val < 1) this.value = 1;
        if (val > 24) this.value = 24;
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