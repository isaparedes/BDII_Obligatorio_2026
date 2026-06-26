async function loadEquipos() {
    const localSelect = document.getElementById('selectEquipoLocal');
    const visitanteSelect = document.getElementById('selectEquipoVisitante');
    localSelect.innerHTML = '<option value="">Cargando equipos...</option>';
    visitanteSelect.innerHTML = '<option value="">Cargando equipos...</option>';

    try {
        const res = await fetch(`${API_URL}/equipo`, { headers: getAuthHeaders() });
        if (!res.ok) {
            throw new Error('Error cargando equipos');
        }
        const equipos = await res.json();
        if (!equipos || equipos.length === 0) {
            localSelect.innerHTML = '<option value="">No hay equipos</option>';
            visitanteSelect.innerHTML = '<option value="">No hay equipos</option>';
            return;
        }
        localSelect.innerHTML = '<option value="">Seleccione un equipo local</option>';
        visitanteSelect.innerHTML = '<option value="">Seleccione un equipo visitante</option>';
        equipos.forEach(eq => {
            const name = eq.nombreEquipo || eq.nombre || '';
            const optionLocal = document.createElement('option');
            optionLocal.value = name;
            optionLocal.innerText = name;
            localSelect.appendChild(optionLocal);
            const optionVisitante = optionLocal.cloneNode(true);
            visitanteSelect.appendChild(optionVisitante);
        });
    } catch (error) {
        localSelect.innerHTML = '<option value="">Error cargando equipos</option>';
        visitanteSelect.innerHTML = '<option value="">Error cargando equipos</option>';
    }
}

async function loadEstadios() {
    const estadioSelect = document.getElementById('selectEstadio');
    estadioSelect.innerHTML = '<option value="">Cargando estadios...</option>';

    try {
        const res = await fetch(`${API_URL}/estadio`, { headers: getAuthHeaders() });
        if (!res.ok) {
            throw new Error('Error cargando estadios');
        }
        const estadios = await res.json();
        if (!estadios || estadios.length === 0) {
            estadioSelect.innerHTML = '<option value="">No hay estadios disponibles</option>';
            return;
        }
        estadioSelect.innerHTML = '<option value="">Seleccione un estadio</option>';
        estadios.forEach(est => {
            const option = document.createElement('option');
            option.value = est.idEstadio ?? est.id ?? '';
            option.innerText = `${est.nombreEstadio || est.nombre || ''} (${est.paisEstadio || est.pais || 'N/A'})`;
            estadioSelect.appendChild(option);
        });
    } catch (error) {
        estadioSelect.innerHTML = '<option value="">Error cargando estadios</option>';
    }
}

async function crearEvento() {
    const local = document.getElementById('selectEquipoLocal').value;
    const visitante = document.getElementById('selectEquipoVisitante').value;
    const idEstadio = parseInt(document.getElementById('selectEstadio').value || '0', 10);
    const fecha = document.getElementById('inputFechaEvento').value;
    const hora = document.getElementById('inputHoraEvento').value;
    const msgBox = document.getElementById('mensajeCrearEvento');
    msgBox.style.color = 'var(--colorBordeaux)';
    msgBox.innerText = '';

    if (!local || !visitante || !idEstadio || !fecha || !hora) {
        msgBox.innerText = 'Complete todos los campos';
        return;
    }
    if (local === visitante) {
        msgBox.innerText = 'El equipo local y visitante no pueden ser el mismo';
        return;
    }
    const dt = new Date(`${fecha}T${hora}`);
    if (isNaN(dt.getTime())) {
        msgBox.innerText = 'Fecha y hora inválidas';
        return;
    }
    if (dt < new Date()) {
        msgBox.innerText = 'No se pueden crear eventos en el pasado';
        return;
    }

    try {
        const res = await fetch(`${API_URL}/evento`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({
                fechaEvento: fecha,
                horaEvento: `${hora}:00`,
                idEstadio,
                equipoLocal: local,
                equipoVisitante: visitante
            })
        });
        const text = await res.text();
        if (res.ok) {
            msgBox.style.color = 'green';
            msgBox.innerText = 'Evento creado correctamente';
            document.getElementById('formCrearEvento').reset();
            await loadEventosAdmin();
        } else if (res.status === 500) {
            msgBox.innerText = 'No puede crear el evento porque el estadio está fuera de su jurisdicción.';
        } else if (res.status === 400) {
            msgBox.innerText = text || 'Error creando evento';
        } else {
            msgBox.innerText = text || 'Error del servidor al crear el evento';
        }
    } catch (error) {
        msgBox.innerText = 'Error comunicándose con el servidor';
    }
}