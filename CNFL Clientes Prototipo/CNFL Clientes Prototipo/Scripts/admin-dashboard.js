// ==========================================
// ADMIN - DASHBOARD
// ==========================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Dashboard Admin cargado correctamente');

    // ==========================================================
    // Efecto de entrada para las tarjetas
    // ==========================================================
    var stats = document.querySelectorAll('.dash-stat');
    stats.forEach(function (stat, index) {
        stat.style.opacity = '0';
        stat.style.transform = 'translateY(15px)';
        stat.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';

        setTimeout(function () {
            stat.style.opacity = '1';
            stat.style.transform = 'translateY(0)';
        }, 100 + (index * 80));
    });

    // ==========================================================
    // Efecto de entrada para los items recientes
    // ==========================================================
    var items = document.querySelectorAll('.recent-item');
    items.forEach(function (item, index) {
        item.style.opacity = '0';
        item.style.transform = 'translateX(-10px)';
        item.style.transition = 'all 0.4s ease';

        setTimeout(function () {
            item.style.opacity = '1';
            item.style.transform = 'translateX(0)';
        }, 300 + (index * 100));
    });
});