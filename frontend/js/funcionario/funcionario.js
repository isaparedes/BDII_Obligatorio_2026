let funcionarioMail = null;

async function initializePage() {

    const roles = getRolesFromToken();
    console.log(roles);

    if (!roles.includes("Funcionario")) {
        redirectToLogin();
        return;
    }

    configurarSelectorDashboard("funcionario.html");

    funcionarioMail = getFuncionarioEmailFromToken();

    if (!funcionarioMail) {
        redirectToLogin();
        return;
    }

    const funcionario = await fetchFuncionario(funcionarioMail);

    if (funcionario && funcionario.numeroLegajo !== undefined) {
        document.getElementById("legajoFuncionario").textContent =
            funcionario.numeroLegajo;
    }

    const dispositivos = await fetchDispositivos(funcionarioMail);

    renderDispositivos(dispositivos);
}

window.addEventListener("DOMContentLoaded", initializePage);