// ==========================================
// ADMIN - INDEX (PANEL ADMINISTRATIVO)
// ==========================================

function mostrarToast(mensaje, tipo) {
    var toast = document.getElementById('toastNotif');
    if (!toast) return;

    toast.textContent = mensaje;
    toast.className = 'toast-notification' + (tipo === 'error' ? ' error' : '');
    toast.style.display = 'block';

    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(function () {
        toast.style.display = 'none';
    }, 3000);
}

document.addEventListener('DOMContentLoaded', function () {
    console.log('📋 Panel Administrativo cargado correctamente');

    // ==========================================================
    // Efecto de entrada para las tarjetas
    // ==========================================================
    var cards = document.querySelectorAll('.card-cliente');
    cards.forEach(function (card, index) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(15px)';
        card.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 150 + (index * 80));
    });

    // ==========================================================
    // Efecto de entrada para las estadísticas
    // ==========================================================
    var stats = document.querySelectorAll('.stat-box');
    stats.forEach(function (stat, index) {
        stat.style.opacity = '0';
        stat.style.transform = 'scale(0.95)';
        stat.style.transition = 'all 0.4s ease';

        setTimeout(function () {
            stat.style.opacity = '1';
            stat.style.transform = 'scale(1)';
        }, 100 + (index * 60));
    });

    // ==========================================================
    // Manejo de envío de formularios con AJAX
    // ==========================================================
    var forms = document.querySelectorAll('.admin-actions form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            var formData = new FormData(this);
            var url = this.getAttribute('action');

            fetch(url, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(function (response) {
                    return response.json();
                })
                .then(function (data) {
                    if (data.success) {
                        mostrarToast(data.message || '✅ Estado actualizado correctamente');
                        // Recargar la página después de 1 segundo
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
                    } else {
                        mostrarToast(data.message || '❌ Error al actualizar el estado', 'error');
                    }
                })
                .catch(function (error) {
                    mostrarToast('❌ Error de conexión', 'error');
                });
        });
    });
});

// Exponer funciones globalmente
window.mostrarToast = mostrarToast;