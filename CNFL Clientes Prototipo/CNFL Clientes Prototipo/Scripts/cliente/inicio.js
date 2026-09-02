// ==========================================
// CLIENTE - INICIO (Dashboard)
// ==========================================

// ===== CAMBIAR PERÍODO DEL GRÁFICO =====
function cambiarPeriodo(periodo) {
    // Actualizar chips
    document.querySelectorAll('.chip').forEach(function (chip) {
        chip.classList.remove('active');
    });
    var chips = document.querySelectorAll('.chip');
    for (var i = 0; i < chips.length; i++) {
        if (chips[i].getAttribute('onclick') && chips[i].getAttribute('onclick').indexOf(periodo) !== -1) {
            chips[i].classList.add('active');
        }
    }

    // Simular cambio de datos en el gráfico
    var barras = document.querySelectorAll('.barra');
    var randomHeights = [];
    for (var j = 0; j < barras.length; j++) {
        randomHeights.push(Math.floor(Math.random() * 70) + 30);
    }

    barras.forEach(function (barra, index) {
        var height = randomHeights[index] || 50;
        barra.style.height = height + '%';
        barra.style.background = 'linear-gradient(var(--sky), #a9dce8)';
    });

    // Resaltar la última barra
    if (barras.length > 0) {
        var lastBar = barras[barras.length - 1];
        lastBar.style.background = 'linear-gradient(var(--blue), #5a60f5)';
    }

    mostrarToast('✅ Período actualizado: ' + periodo);
}

// ===== MOSTRAR TOAST =====
function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toastNotif';
        toast.style.cssText = 'position:fixed;bottom:100px;left:50%;transform:translateX(-50%);background:#2E7D32;color:white;padding:12px 24px;border-radius:16px;font-weight:600;font-size:14px;box-shadow:0 8px 30px rgba(0,0,0,0.2);z-index:2000;display:none;max-width:90%;';
        document.body.appendChild(toast);
    }

    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.style.background = tipo === 'error' ? '#C62828' : '#2E7D32';
    toast.style.display = 'block';

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.display = 'none';
    }, 3000);
}

// Exponer funciones
window.cambiarPeriodo = cambiarPeriodo;
window.mostrarToast = mostrarToast;