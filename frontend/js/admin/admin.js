function cambiarAdminVista(idBloqueDestino) {
    const bloques = ['bloqueEstadisticas', 'bloqueCrear', 'bloqueLogistica'];
    const botones = ['btnAdminEstadisticas', 'btnAdminCrear', 'btnAdminLogistica'];

    bloques.forEach(b => document.getElementById(b).classList.remove('activa'));
    botones.forEach(btn => document.getElementById(btn).classList.remove('activa'));

    document.getElementById(idBloqueDestino).classList.add('activa');
    document.getElementById('btnAdmin' + idBloqueDestino.substring(6)).classList.add('activa');
}

window.addEventListener('DOMContentLoaded', async () => {
    ensureAdminLoggedIn();
    configurarSelectorDashboard('administrador.html');

    await Promise.all([
        loadEstadisticas(),
        loadEquipos(),
        loadEstadios(),
        loadEventosAdmin()
    ]);
});