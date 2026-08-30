// ==========================================
// CLIENTE PAGOS - JAVASCRIPT
// ==========================================

// ===== REDIRIGIR A MÉTODO DE PAGO =====
function irAPagar(metodo, monto) {
    var url = '/Cliente/' + metodo + '?monto=' + monto;
    window.location.href = url;
}