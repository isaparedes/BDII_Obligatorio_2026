let entradaParaTransferir = null;

async function loadEntradasTransferibles() {
    const container = document.getElementById('listaEntradasTransferibles');
    container.innerHTML = 'Cargando...';

    try {
        const res = await fetch(`${API_URL}/entrada/mis-entradas`, { headers: getAuthHeaders() });
        if (!res.ok) throw new Error(await res.text());
        const entradas = await res.json();
        const items = [];
        for (const e of entradas) {
            if ((e.estadoEntrada) === 'Consumida') continue;
            const idEv = e.idEvento || e.id_evento;
            if (!idEv) continue;
            const ev = await fetchEvento(idEv);
            if (!ev) continue;
            const fechaHora = new Date(ev.fechaEvento );
            // If event has horaEvento, combine
            if (ev.horaEvento || ev.HoraEvento) {
                const hora = ev.horaEvento || ev.HoraEvento;
                // try to parse time if string
                const timeStr = typeof hora === 'string' ? hora : hora.toString();
                const [hh, mm] = timeStr.split(':').map(x => parseInt(x || '0'));
                fechaHora.setHours(hh, mm);
            }
            if (fechaHora <= new Date()) continue;

            items.push({ entrada: e, evento: ev });
        }

        if (!items.length) {
            container.innerHTML = '<p>No hay entradas transferibles.</p>';
            return;
        }

        container.innerHTML = '';
        items.forEach(it => {
            const e = it.entrada;
            const ev = it.evento;
            const div = document.createElement('div');
            div.style.padding = '8px';
            div.style.borderBottom = '1px solid var(--bordeSuave)';
            div.style.display = 'flex';
            div.style.justifyContent = 'space-between';
            div.style.alignItems = 'center';

            const left = document.createElement('div');
            left.innerHTML = `<strong>N°${e.idEntrada}: ${(ev.equipoLocal)} vs ${(ev.equipoVisitante)}</strong><br/><small>Sector: ${e.nombreSector}</small>`;

            const btn = document.createElement('button');
            btn.className = 'btnAccion btnBordeaux';
            btn.innerText = 'Transferir';
            btn.onclick = () => selectEntradaForTransfer(e.idEntrada, left.innerText);

            div.appendChild(left);
            div.appendChild(btn);
            container.appendChild(div);
        });

    } catch (err) {
        container.innerHTML = `<p style="color:red;">Error cargando entradas transferibles: ${err.message || err}</p>`;
    }
}

function selectEntradaForTransfer(idEntrada, descripcion) {
    entradaParaTransferir = idEntrada;
    document.getElementById('entradaSeleccionadaTexto').innerText = descripcion;
    document.getElementById('bloqueFormularioTransferencia').style.display = 'block';
    document.getElementById('mensajeTransferencia').innerText = '';
}

async function loadMisTransferencias() {
    const recibidasContainer = document.getElementById('recibidasTransferencias');
    const enviadasContainer = document.getElementById('enviadasTransferencias');
    recibidasContainer.innerHTML = '<p>Cargando recibidas...</p>';
    enviadasContainer.innerHTML = '<p>Cargando enviadas...</p>';

    try {
        const res = await fetch(`${API_URL}/transferencia/mis-transferencias`, { headers: getAuthHeaders() });
        if (!res.ok) {
            const text = await res.text();
            recibidasContainer.innerHTML = `<p style="color:red;">Error cargando transferencias: ${text || res.statusText}</p>`;
            enviadasContainer.innerHTML = `<p style="color:red;">Error cargando transferencias: ${text || res.statusText}</p>`;
            return;
        }

        const data = await res.json();
        const recibidas = data.recibidas || data.Recibidas || [];
        const enviadas = data.enviadas || data.Enviadas || [];

        if (!recibidas.length) {
            recibidasContainer.innerHTML = '<p>No hay transferencias recibidas.</p>';
        } else {
            recibidasContainer.innerHTML = '';
            recibidas.forEach(t => {
                const estado = t.estadoTransferencia  || '';
                const correoRemitente = t.mailRemitente  || '';
                const idEntrada = t.idEntrada || 0;
                const fecha = formatDateTime(t.fechaTransferencia);

                const item = document.createElement('div');
                item.className = 'itemTransferencia';
                item.style.background = '#edf2f7';
                item.style.padding = '10px';
                item.style.borderRadius = '6px';
                item.style.borderLeft = '4px solid gold';
                item.style.marginBottom = '8px';

                item.innerHTML = `
                    <p style="font-size:0.85rem;"><strong>De:</strong> ${correoRemitente}</p>
                    <p style="font-size:0.85rem;"><strong>Entrada:</strong> #${idEntrada}</p>
                    <p style="font-size:0.85rem;"><strong>Fecha:</strong> ${fecha}</p>
                    <p style="font-size:0.85rem;"><strong>Estado:</strong> ${estado}</p>
                `;

                if (estado === 'En proceso') {
                    const acciones = document.createElement('div');
                    acciones.style.display = 'flex';
                    acciones.style.gap = '5px';
                    acciones.style.marginTop = '8px';

                    const aceptarBtn = document.createElement('button');
                    aceptarBtn.className = 'btnAccion btnVerde';
                    aceptarBtn.style.padding = '4px';
                    aceptarBtn.style.fontSize = '0.8rem';
                    aceptarBtn.innerText = 'Aceptar';
                    aceptarBtn.onclick = () => responderTransferencia(t.idTransferencia, 'Aceptada');

                    const rechazarBtn = document.createElement('button');
                    rechazarBtn.className = 'btnAccion';
                    rechazarBtn.style.padding = '4px';
                    rechazarBtn.style.fontSize = '0.8rem';
                    rechazarBtn.style.backgroundColor = '#e53e3e';
                    rechazarBtn.innerText = 'Rechazar';
                    rechazarBtn.onclick = () => responderTransferencia(t.idTransferencia, 'Rechazada');

                    acciones.appendChild(aceptarBtn);
                    acciones.appendChild(rechazarBtn);
                    item.appendChild(acciones);
                }

                recibidasContainer.appendChild(item);
            });
        }

        if (!enviadas.length) {
            enviadasContainer.innerHTML = '<p>No hay transferencias enviadas.</p>';
        } else {
            enviadasContainer.innerHTML = '';
            enviadas.forEach(t => {
                const estado = t.estadoTransferencia || '';
                const correoDestinatario = t.mailDestinatario || '';
                const idEntrada = t.idEntrada || t.IdEntrada || 0;
                const fecha = formatDateTime(t.fechaTransferencia);

                const item = document.createElement('div');
                item.style.fontSize = '0.85rem';
                item.style.padding = '8px';
                item.style.borderBottom = '1px solid var(--bordeSuave)';
                item.innerHTML = `
                    <p><strong>Para:</strong> ${correoDestinatario}</p>
                    <p><strong>Entrada:</strong> #${idEntrada}</p>
                    <p><strong>Fecha:</strong> ${fecha}</p>
                    <p><strong>Estado:</strong> <b style="color: ${estado === 'Aceptada' ? 'green' : estado === 'Rechazada' ? '#e53e3e' : '#b7791f'};">${estado}</b></p>
                `;
                enviadasContainer.appendChild(item);
            });
        }
    } catch (err) {
        recibidasContainer.innerHTML = `<p style="color:red;">Error cargando transferencias: ${err.message || err}</p>`;
        enviadasContainer.innerHTML = `<p style="color:red;">Error cargando transferencias: ${err.message || err}</p>`;
    }
}

async function responderTransferencia(idTransferencia, estadoTransferencia) {
    try {
        const res = await fetch(`${API_URL}/transferencia/${idTransferencia}/responder`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify({ EstadoTransferencia: estadoTransferencia })
        });
        if (!res.ok) {
            const text = await res.text();
            alert(text || 'Error al responder transferencia');
            return;
        }
        await loadMisTransferencias();
    } catch (err) {
        alert('Error al responder transferencia: ' + (err.message || err));
    }
}

function formatDateTime(dateValue) {
    if (!dateValue) return 'Sin fecha';
    const dt = new Date(dateValue);
    if (isNaN(dt.getTime())) return String(dateValue);
    return `${dt.toLocaleDateString()} ${dt.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
}

document.addEventListener('click', (e) => {
            if (e.target && e.target.id === 'btnEnviarTransferencia') {
                e.preventDefault();
                realizarTransferencia();
            }
        });

async function realizarTransferencia() {
    const mensaje = document.getElementById('mensajeTransferencia');
    mensaje.innerText = '';
    const mail = document.getElementById('correoDestino').value;
    if (!entradaParaTransferir) { mensaje.innerText = 'Seleccione una entrada primero.'; return; }
    if (!mail) { mensaje.innerText = 'Ingrese un correo válido.'; return; }

    try {
        document.getElementById('btnEnviarTransferencia').disabled = true;
        const res = await fetch(`${API_URL}/transferencia`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ IdEntrada: entradaParaTransferir, MailDestinatario: mail })
        });
        const text = await res.text();
        if (!res.ok) {
            mensaje.style.color = 'var(--colorError)';
            mensaje.innerText = text;
            document.getElementById('btnEnviarTransferencia').disabled = false;
            return;
        }

        mensaje.style.color = 'var(--colorVerdeOliva)';
        mensaje.innerText = text || 'Transferencia creada correctamente.';
        await loadEntradasTransferibles();
        await loadMisEntradas();
        document.getElementById('bloqueFormularioTransferencia').style.display = 'none';
        document.getElementById('correoDestino').value = '';
        entradaParaTransferir = null;
        document.getElementById('btnEnviarTransferencia').disabled = false;

    } catch (err) {
        mensaje.style.color = 'var(--colorError)';
        mensaje.innerText = 'Error enviando transferencia: ' + (err.message || err);
        document.getElementById('btnEnviarTransferencia').disabled = false;
    }
}
