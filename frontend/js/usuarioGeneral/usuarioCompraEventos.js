let compraSeleccionada = null;
let carrito = [];

async function loadEventosFuturos() {
    const list = document.getElementById('listaEventosFuturos');
    list.innerHTML = '<p>Cargando eventos...</p>';
    try {
        const res = await fetch(`${API_URL}/evento/futuros`, { headers: getAuthHeaders() });
        if (!res.ok) throw new Error(await res.text());
        const eventos = await res.json();
        list.innerHTML = '';
        if (!eventos || eventos.length === 0) {
            list.innerHTML = '<p>No hay eventos disponibles.</p>';
            return;
        }

        for (const ev of eventos) {
            const item = document.createElement('div');
            item.className = 'itemEvento';
            item.style.border = '1px solid var(--bordeSuave)';
            item.style.padding = '10px';
            item.style.borderRadius = '6px';
            item.style.marginBottom = '10px';

            const title = document.createElement('p');
            title.innerHTML = `<strong>${ev.equipoLocal} vs ${ev.equipoVisitante}</strong>`;
            item.appendChild(title);

            const subtitle = document.createElement('p');
            const fecha = new Date(ev.fechaEvento);
            subtitle.className = 'subtitulo';
            subtitle.style.marginBottom = '5px';
            subtitle.innerText = `Fecha: ${fecha.toLocaleDateString()} ${ev.horaEvento} - ${ev.nombreEstadio}`;
            item.appendChild(subtitle);

            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btnAccion btnBordeaux';
            btn.style.padding = '6px';
            btn.style.fontSize = '0.85rem';
            btn.innerText = 'Seleccionar';
            btn.onclick = () => abrirFormularioCompra(ev);
            item.appendChild(btn);

            list.appendChild(item);
        }

    } catch (err) {
        list.innerHTML = `<p style="color:red;">Error cargando eventos: ${err.message || err}</p>`;
    }
}

function setupCompraButtons() {
    document.getElementById('btnCrearCompra').addEventListener('click', crearCompra);
    document.getElementById('btnPagarCompra').addEventListener('click', pagarCompraActual);
}

async function abrirFormularioCompra(ev) {
    compraSeleccionada = ev;
    carrito = [];
    document.getElementById('formularioAdquisicion').style.display = 'block';
    document.getElementById('eventoSeleccionadoCompra').innerText = `Comprar: ${ev.equipoLocal} vs ${ev.equipoVisitante} — ${ev.nombreEstadio}`;

    const sectoresDiv = document.getElementById('sectoresDisponibles');
    sectoresDiv.innerHTML = '<p>Cargando sectores habilitados...</p>';
    try {
        const habilRes = await fetch(`${API_URL}/evento/${ev.idEvento}/sectores-habilitados`, { headers: getAuthHeaders() });
        if (!habilRes.ok) throw new Error(await habilRes.text());
        const habilitados = await habilRes.json();

        const habilNames = (habilitados || []).map(h => h.nombreSector || String(h).trim());

        // obtener datos completos de sectores del estadio para sacar costo/capacidad
        const res = await fetch(`${API_URL}/sector/${ev.idEstadio}`, { headers: getAuthHeaders() });
        if (!res.ok) throw new Error(await res.text());
        const sectoresAll = await res.json();

        // filtrar por habilitados
        const sectores = (sectoresAll || []).filter(s => habilNames.includes(s.nombreSector));

        sectoresDiv.innerHTML = '';
        if (!sectores.length) {
            sectoresDiv.innerHTML = '<p>No hay sectores habilitados para este evento.</p>';
        }

        for (const s of sectores) {
            const nombre = s.nombreSector || '';
            const costo = s.costoSector || 0;
            const capacidad = s.capacidad || '';

            const row = document.createElement('div');
            row.style.display = 'flex';
            row.style.justifyContent = 'space-between';
            row.style.alignItems = 'center';
            row.style.padding = '6px 0';

            const info = document.createElement('div');
            info.innerHTML = `<strong>${nombre}</strong><br/><small>$${costo} — Capacidad: ${capacidad}</small>`;

            const controls = document.createElement('div');
            controls.style.display = 'flex';
            controls.style.gap = '6px';

            const qty = document.createElement('input');
            qty.type = 'number';
            qty.min = 0;
            qty.max = 5;
            qty.value = 0;
            qty.style.width = '60px';

            const addBtn = document.createElement('button');
            addBtn.className = 'btnAccion';
            addBtn.innerText = 'Agregar';
            addBtn.onclick = () => {
                const cantidad = Math.max(0, Math.min(5, parseInt(qty.value || '0')));
                if (cantidad <= 0) {
                    document.getElementById('mensajeCompra').innerText = 'Ingrese una cantidad válida (1-5)';
                    return;
                }
                document.getElementById('mensajeCompra').innerText = '';
                agregarAlCarrito(ev.idEvento, ev.idEstadio, nombre, costo, cantidad);
            };

            controls.appendChild(qty);
            controls.appendChild(addBtn);

            row.appendChild(info);
            row.appendChild(controls);
            sectoresDiv.appendChild(row);
        }

    } catch (err) {
        sectoresDiv.innerHTML = `<p style="color:red;">Error cargando sectores habilitados: ${err.message || err}</p>`;
    }
    renderCarrito();
}

function agregarAlCarrito(idEvento, idEstadio, nombreSector, costoSector, cantidad) {
    // agregar o acumular
    const existing = carrito.find(i => i.nombreSector === nombreSector && i.idEvento === idEvento);
    if (existing) existing.cantidad = Math.min(5, existing.cantidad + cantidad);
    else carrito.push({ idEvento, idEstadio, nombreSector, costoSector, cantidad });
    renderCarrito();
}

function renderCarrito() {
    const resumen = document.getElementById('resumenCarrito');
    resumen.innerHTML = '';
    if (!carrito.length) {
        resumen.innerHTML = '<p>No hay items seleccionados.</p>';
        document.getElementById('btnCrearCompra').style.display = 'none';
        document.getElementById('btnPagarCompra').style.display = 'none';
        return;
    }

    let total = 0;
    const ul = document.createElement('div');
    for (let idx = 0; idx < carrito.length; idx++) {
        const it = carrito[idx];
        const div = document.createElement('div');
        div.style.display = 'flex';
        div.style.justifyContent = 'space-between';
        div.style.alignItems = 'center';
        div.style.padding = '4px 0';

        const left = document.createElement('div');
        left.innerText = `${it.nombreSector} x${it.cantidad}`;

        const right = document.createElement('div');
        right.style.display = 'flex';
        right.style.gap = '6px';
        right.style.alignItems = 'center';

        const price = document.createElement('div');
        price.innerText = `$${it.costoSector * it.cantidad}`;

        const minus = document.createElement('button');
        minus.className = 'btnAccion';
        minus.style.padding = '4px';
        minus.innerText = '-';
        minus.onclick = () => {
            it.cantidad -= 1;
            if (it.cantidad <= 0) carrito.splice(idx, 1);
            renderCarrito();
        };

        const plus = document.createElement('button');
        plus.className = 'btnAccion';
        plus.style.padding = '4px';
        plus.innerText = '+';
        plus.onclick = () => {
            if (it.cantidad < 5) it.cantidad += 1;
            renderCarrito();
        };

        const del = document.createElement('button');
        del.className = 'btnAccion';
        del.style.padding = '4px';
        del.style.backgroundColor = '#e53e3e';
        del.innerText = 'Eliminar';
        del.onclick = () => { carrito.splice(idx, 1); renderCarrito(); };

        right.appendChild(minus);
        right.appendChild(plus);
        right.appendChild(price);
        right.appendChild(del);

        div.appendChild(left);
        div.appendChild(right);

        ul.appendChild(div);
        total += it.costoSector * it.cantidad;
    }

    const totalDiv = document.createElement('div');
    totalDiv.style.marginTop = '8px';
    totalDiv.innerHTML = `<strong>Total: $${total}</strong>`;

    resumen.appendChild(ul);
    resumen.appendChild(totalDiv);

    document.getElementById('btnCrearCompra').style.display = 'inline-block';

}

let compraActual = null;
async function crearCompra() {
    if (!carrito.length) return alert('Carrito vacío');
    const entradas = [];
    for (const it of carrito) {
        for (let i = 0; i < it.cantidad; i++) {
            entradas.push({ IdEvento: it.idEvento, NombreSector: it.nombreSector });
        }
    }

    const mensaje = document.getElementById('mensajeCompra');
    mensaje.innerText = '';
    try {
        document.getElementById('btnCrearCompra').disabled = true;
        const res = await fetch(`${API_URL}/compra`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ Entradas: entradas })
        });
        if (!res.ok) {
            const txt = await res.text();
            mensaje.innerText = txt;
            document.getElementById('btnCrearCompra').disabled = false;
            return;
        }
        const compra = await res.json();
        compraActual = compra;


        const subtotal = (compra.entradas || []).reduce((s, e) => s + (e.costoEntrada || e.costo_entrada || 0), 0);

        document.getElementById('btnPagarCompra').style.display = 'inline-block';
        document.getElementById('btnCrearCompra').style.display = 'none';

        mensaje.style.color = 'var(--colorVerdeOliva)';
        mensaje.innerText = `Compra creada. Subtotal: $${subtotal}. Presione Pagar para finalizar.`;
        document.getElementById('btnCrearCompra').disabled = false;

    } catch (err) {
        mensaje.style.color = 'var(--colorError)';
        mensaje.innerText = 'Error creando compra: ' + (err.message || err);
        document.getElementById('btnCrearCompra').disabled = false;
    }
}

async function pagarCompraActual() {
    if (!compraActual) return alert('No hay compra para pagar');
    const mensaje = document.getElementById('mensajeCompra');
    mensaje.innerText = '';
    try {
        const id = compraActual.idCompra;
        document.getElementById('btnPagarCompra').disabled = true;
        const res = await fetch(`${API_URL}/compra/${id}/pagar`, { method: 'PUT', headers: getAuthHeaders() });
        if (!res.ok) {
            const txt = await res.text();
            mensaje.style.color = 'var(--colorError)';
            mensaje.innerText = txt;
            document.getElementById('btnPagarCompra').disabled = false;
            return;
        }

        const updated = await (await fetch(`${API_URL}/compra/${id}`, { headers: getAuthHeaders() })).json();

        mensaje.style.color = 'var(--colorVerdeOliva)';
        mensaje.innerText = `Pago registrado. Monto final: $${updated.montoTotal || updated.monto_total || 0}`;

        await loadMisEntradas();
        cambiarSubVista('bloqueMisEntradas');
        compraSeleccionada = null;
        carrito = [];
        compraActual = null;
        document.getElementById('formularioAdquisicion').style.display = 'none';
        document.getElementById('btnPagarCompra').style.display = 'none';
        document.getElementById('btnPagarCompra').disabled = false;

    } catch (err) {
        mensaje.style.color = 'var(--colorError)';
        mensaje.innerText = 'Error al pagar: ' + (err.message || err);
        document.getElementById('btnPagarCompra').disabled = false;
    }
}