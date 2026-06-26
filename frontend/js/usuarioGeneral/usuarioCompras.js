async function loadMisCompras() {

    const container = document.getElementById("misComprasContainer");
    container.innerHTML = "<p>Cargando...</p>";

    try {

        const res = await fetch(`${API_URL}/compra/mis-compras`, {
            headers: getAuthHeaders()
        });

        if (!res.ok)
            throw new Error(await res.text());

        const compras = await res.json();

        if (!compras.length) {
            container.innerHTML = "<p>No hay compras.</p>";
            return;
        }

        container.innerHTML = '';

        for (const compra of compras) {
            compra.fechaCompra = compra.fechaCompra.split(' ')[0];

            const card = document.createElement("div");
            card.className = "tarjetaContenedor";
            card.style.marginBottom = "15px";

            const entradasHtml = compra.entradas.map(e => `
                <li>
                    N° ${e.idEntrada} |
                    Sector: ${e.nombreSector} |
                    Estado: ${e.estadoEntrada}
                </li>
            `).join('');

            card.innerHTML = `
                <h4>Compra #${compra.idCompra}</h4>
                <p><strong>Fecha:</strong> ${compra.fechaCompra}</p>
                <p><strong>Estado:</strong> ${compra.estadoCompra}</p>
                <p><strong>Monto:</strong> $${compra.montoTotal}</p>
                <p><strong>Cantidad entradas:</strong> ${compra.entradas.length}</p>
                <hr>
                <strong>Entradas</strong>
                <ul style="list-style-type: none;">${entradasHtml}</ul>
            `;

            container.appendChild(card);
        }

    } catch (err) {
        container.innerHTML = `<p style="color:red;">${err.message}</p>`;
    }
}