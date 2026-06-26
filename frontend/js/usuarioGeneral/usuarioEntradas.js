const displayedEntries = [];
let tokenRefreshRunning = false;

async function loadMisEntradas() {

    const container = document.getElementById('misEntradasContainer');
    container.innerHTML = '<p>Cargando...</p>';
    displayedEntries.length = 0;

    try {

        const res = await fetch(`${API_URL}/entrada/mis-entradas`, {
            headers: getAuthHeaders()
        });

        if (!res.ok)
            throw new Error(await res.text());

        const entradas = await res.json();

        if (!entradas.length) {
            container.innerHTML = '<p>No tienes entradas.</p>';
            return;
        }

        container.innerHTML = '';

        for (const entrada of entradas) {

            const div = document.createElement('div');
            div.className = 'tarjetaContenedor';
            div.style.marginBottom = '12px';

            const eventInfo = await fetchEvento(entrada.idEvento);
            console.log(eventInfo)
            console.log(entrada)

            const local = eventInfo?.equipoLocal;
            const visitante = eventInfo?.equipoVisitante;
            const estadio = eventInfo?.nombreEstadio;

            const fecha = new Date(eventInfo?.fechaEvento || '');
            const hora = eventInfo?.horaEvento || '';

            const titulo = local && visitante
                ? `${local} vs ${visitante}`
                : `Evento #${entrada.idEvento}`;

            const estadoEntrada = entrada.estadoEntrada;

            div.innerHTML = `
                <h4>N°${entrada.idEntrada}:  ${titulo}</h4>
                <p><strong>Estado:</strong> ${estadoEntrada}</p>
                <p><strong>Sector:</strong> ${entrada.nombreSector}</p>
                <p><strong>Estadio:</strong> ${estadio || ''}</p>
                <p><strong>Fecha:</strong> ${fecha ? fecha.toLocaleDateString() : ''} ${hora}</p>
            `;

            const qrBox = document.createElement('div');
            qrBox.style.width = '120px';
            qrBox.style.height = '120px';

            if (estadoEntrada === 'Consumida') {
                qrBox.innerHTML = `
                    <div style="
                        width:120px;
                        height:120px;
                        display:flex;
                        align-items:center;
                        justify-content:center;
                        border:1px solid #ccc;
                        color:#666;
                        font-weight:bold;">
                        Consumida
                    </div>`;
            } else {
                displayedEntries.push({
                    idEntrada: entrada.idEntrada,
                    qrBox
                });
            }

            div.appendChild(qrBox);
            container.appendChild(div);
        }

        refreshAllTokens();

    } catch (err) {
        container.innerHTML = `<p style="color:red;">${err.message}</p>`;
    }
}

async function refreshAllTokens() {
    if (tokenRefreshRunning) return;
    tokenRefreshRunning = true;
    try {
        for (const entry of displayedEntries) {
            try {
                const res = await fetch(`${API_URL}/token/activo/${entry.idEntrada}`, { headers: getAuthHeaders() });
                if (!res.ok) {
                    entry.qrBox.innerHTML = '<div style="font-size:0.85rem; color:gray;">Sin token disponible</div>';
                    continue;
                }
                const tokenDto = await res.json();
                const code = tokenDto.codigoQR || tokenDto.CodigoQR || tokenDto.codigoQr || tokenDto.codigo_qr || '';
                if (!code) {
                    entry.qrBox.innerHTML = '<div style="font-size:0.85rem; color:gray;">Sin token disponible</div>';
                    continue;
                }
                entry.qrBox.innerHTML = '';
                new QRCode(entry.qrBox, { text: code, width: 120, height: 120 });
            } catch {
                entry.qrBox.innerHTML = '<div style="font-size:0.85rem; color:gray;">Error</div>';
            }
        }
    } finally {
        tokenRefreshRunning = false;
    }
}

setInterval(() => {
    if (displayedEntries.length) refreshAllTokens();
}, 30000);

async function fetchEvento(idEvento) {
    try {
        const res = await fetch(`${API_URL}/evento/${idEvento}`, { headers: getAuthHeaders() });
        if (!res.ok) return null;
        return await res.json();
    } catch {
        return null;
    }
}