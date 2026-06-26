// Navegación entre sub-vistas
function cambiarSubVista(idBloqueDestino) {
    const bloques = ['bloqueMisEntradas', 'bloqueMisCompras', 'bloqueComprar', 'bloqueTransferir', 'bloqueHistorialTransferencias'];
    const botones = ['btnSubMisEntradas', 'btnSubMisCompras', 'btnSubComprar', 'btnSubTransferir', 'btnSubHistorialTransferencias'];
    bloques.forEach(b => document.getElementById(b).classList.remove('activa'));
    botones.forEach(btn => document.getElementById(btn).classList.remove('activa'));

    document.getElementById(idBloqueDestino).classList.add('activa');
    document.getElementById('btnSub' + idBloqueDestino.substring(6)).classList.add('activa');
    if (idBloqueDestino === 'bloqueTransferir') loadEntradasTransferibles();
    if (idBloqueDestino === 'bloqueHistorialTransferencias') loadMisTransferencias();
    if (idBloqueDestino === 'bloqueMisCompras') loadMisCompras();
}

// Cargar datos al inicio
document.addEventListener('DOMContentLoaded', () => {

    configurarSelectorDashboard("usuario.html");

    const mail = localStorage.getItem('mail') || '';

    document.getElementById('textoUsuarioLogueado').innerText = `${mail}`;

    loadMisEntradas();

    loadEventosFuturos();

    setupCompraButtons();

});