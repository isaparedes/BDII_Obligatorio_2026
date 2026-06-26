function configurarSelectorDashboard(paginaActual) {

    const token = localStorage.getItem("token");
    if (!token) return;

    const payload = parseJwt(token);

    let roles =
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
        || payload.role;

    if (!Array.isArray(roles))
        roles = roles ? [roles] : [];

    const selector = document.getElementById("selectorDashboard");
    selector.innerHTML = "";

    if (roles.includes("UsuarioGeneral")) {
        selector.innerHTML += `
            <option value="usuario.html">
                Usuario General
            </option>`;
    }

    if (roles.includes("Administrador")) {
        selector.innerHTML += `
            <option value="administrador.html">
                Administrador
            </option>`;
    }

    if (roles.includes("Funcionario")) {
        selector.innerHTML += `
            <option value="funcionario.html">
                Funcionario
            </option>`;
    }

    selector.style.display = selector.options.length > 1 ? "inline-block" : "none";

    selector.value = paginaActual;

    selector.onchange = function () {
        window.location.href = this.value;
    };
}