
async function loadEventosAdmin() {
    const select = document.getElementById('selectEventoAdmin');
    select.innerHTML = '<option value="">Cargando eventos futuros...</option>';
    try {
        const res = await fetch(`${API_URL}/evento/futuros`, { headers: getAuthHeaders() });
        if (!res.ok) {
            throw new Error('Error cargando eventos');
        }
        const eventos = await res.json();
        if (!eventos || eventos.length === 0) {
            select.innerHTML = '<option value="">No hay eventos próximos</option>';
            clearLogistica();
            return;
        }
        select.innerHTML = '<option value="">Seleccione un evento</option>';
        eventos.forEach(ev => {
            const date = ev.fechaEvento.split('T')[0];
            const hora = ev.horaEvento;
            const option = document.createElement('option');
            option.value = ev.idEvento ?? ev.id;
            option.innerText = `${ev.equipoLocal} vs ${ev.equipoVisitante} — ${ev.nombreEstadio} (${date} ${hora})`;
            select.appendChild(option);
        });
    } catch (error) {
        select.innerHTML = '<option value="">Error cargando eventos</option>';
    }
}

async function loadLogisticaForEvent(idEvento) {
    const sectoresContainer = document.getElementById('contenedorSectores');
    const funcionariosContainer = document.getElementById('contenedorFuncionarios');
    const dispositivosContainer = document.getElementById('contenedorDispositivos');
    const sectorSelect = document.getElementById('selectSectorAsignar');
    const habilitarMessage = document.getElementById('mensajeHabilitarSector');
    const asignarMessage = document.getElementById('mensajeAsignarFuncionario');

    habilitarMessage.innerText = '';
    asignarMessage.innerText = '';
    sectoresContainer.innerHTML = '<p>Cargando sectores habilitados...</p>';
    funcionariosContainer.innerHTML = '<p>Cargando funcionarios asignados...</p>';
    dispositivosContainer.innerHTML = '<p>Cargando dispositivos habilitados...</p>';
    sectorSelect.innerHTML = '<option value="">Cargando sectores...</option>';

    try {
        const [sectRes, funcRes, dispRes] = await Promise.all([
            fetch(`${API_URL}/evento/${idEvento}/sectores-habilitados`, { headers: getAuthHeaders() }),
            fetch(`${API_URL}/evento/${idEvento}/funcionarios-asignados`, { headers: getAuthHeaders() }),
            fetch(`${API_URL}/evento/${idEvento}/dispositivos-habilitados`, { headers: getAuthHeaders() })
        ]);

        if (sectRes.ok) {
            const sectores = await sectRes.json();

            if (sectores && sectores.length > 0) {
                sectoresContainer.innerHTML = sectores.map(s => `<span style="background:#fff; padding:3px 8px; border-radius:4px; border:1px solid #e2e8f0; display:inline-block; margin:2px;">${s.nombreSector || s.nombre}</span>`).join('');
            } else {
                sectoresContainer.innerHTML = '<p>No hay sectores habilitados para este evento.</p>';
            }

            try {
                const eventoRes = await fetch(`${API_URL}/evento/${idEvento}`, { headers: getAuthHeaders() });
                let idEstadio = null;
                if (eventoRes.ok) {
                    const eventoDetalle = await eventoRes.json();
                    idEstadio = eventoDetalle.idEstadio ?? eventoDetalle.IdEstadio ?? eventoDetalle.id_estadio ?? eventoDetalle.idEstadio;
                    console.log("EVENTO DETALLE:", eventoDetalle);
                    console.log("ID ESTADIO:", idEstadio);
                }

                if (idEstadio) {
                    const todosRes = await fetch(`${API_URL}/sector/${idEstadio}`, { headers: getAuthHeaders() });
                    if (todosRes.ok) {
                        const todosSectores = await todosRes.json();
                        console.log(todosSectores);

                        const habilitadosNombres = (sectores || []).map(s => ((s.nombreSector  || '') + '').toLowerCase().trim());
                        const todos = (todosSectores || []).map(s => ({
                            nombre: (s.nombreSector || '').trim()
                        }));

                        const habilitadosSet = new Set(
                            (sectores || []).map(s =>
                                (s.nombreSector || s.nombre || '').trim()
                            )
                        );

                        const habilitados = todos.filter(s => habilitadosSet.has(s.nombre));
                        const noHabilitados = todos.filter(s => !habilitadosSet.has(s.nombre));

                        const nuevoSectorSelect = document.getElementById('selectNuevoSector');

                        let optionsNoHab = '<option value="">Seleccione un sector</option>';

                        if (noHabilitados.length > 0) {
                            optionsNoHab += noHabilitados.map(s =>
                                `<option value="${s.nombre}">${s.nombre}</option>`
                            ).join('');
                        } else {
                            optionsNoHab += '<option disabled>No hay sectores disponibles</option>';
                        }

                        nuevoSectorSelect.innerHTML = optionsNoHab;

                        let optionsHab = '<option value="">Seleccione un sector habilitado</option>';

                        if (habilitados.length > 0) {
                            optionsHab += habilitados.map(s =>
                                `<option value="${s.nombre}">${s.nombre}</option>`
                            ).join('');
                        } else {
                            optionsHab += '<option disabled>No hay sectores habilitados</option>';
                        }

                        sectorSelect.innerHTML = optionsHab;
                    } else {
                        sectorSelect.innerHTML = (sectores && sectores.length > 0)
                            ? '<option value="">Seleccione un sector habilitado</option>' + sectores.map(s => `<option value="${s.nombreSector || s.nombre}">${s.nombreSector || s.nombre}</option>`).join('')
                            : '<option value="">No hay sectores habilitados</option>';
                    }
                } else {
                    sectorSelect.innerHTML = (sectores && sectores.length > 0)
                        ? '<option value="">Seleccione un sector habilitado</option>' + sectores.map(s => `<option value="${s.nombreSector || s.nombre}">${s.nombreSector || s.nombre}</option>`).join('')
                        : '<option value="">No hay sectores habilitados</option>';
                }
            } catch (err) {
                sectorSelect.innerHTML = (sectores && sectores.length > 0)
                    ? '<option value="">Seleccione un sector habilitado</option>' + sectores.map(s => `<option value="${s.nombreSector || s.nombre}">${s.nombreSector || s.nombre}</option>`).join('')
                    : '<option value="">No hay sectores habilitados</option>';
            }
        } else {
            sectoresContainer.innerHTML = '<p>No hay sectores habilitados para este evento.</p>';
            sectorSelect.innerHTML = '<option value="">Error cargando sectores</option>';
        }

        if (funcRes.ok) {
            const funcionarios = await funcRes.json();
            funcionariosContainer.innerHTML = funcionarios && funcionarios.length > 0 ? funcionarios.map(f => `<div style="margin-bottom:6px;"><strong>${f.nombreSector || ''}:</strong> ${f.mailFuncionario || f.mail}</div>`).join('') : '<p>No hay funcionarios asignados para este evento.</p>';
        } else {
            funcionariosContainer.innerHTML = '<p>No hay funcionarios asignados para este evento.</p>';
        }

        if (dispRes.ok) {
            const dispositivos = await dispRes.json();
            dispositivosContainer.innerHTML = dispositivos && dispositivos.length > 0 ? dispositivos.map(d => `<div style="margin-bottom:6px;"><strong>${d.nombreSector || ''}:</strong> ${d.mailFuncionario || d.mail} - Terminal ${d.idDispositivo || d.id}</div>`).join('') : '<p>No hay dispositivos habilitados para este evento.</p>';
        } else {
            dispositivosContainer.innerHTML = '<p>No hay dispositivos habilitados para este evento.</p>';
        }
    } catch (error) {
        sectoresContainer.innerHTML = '<p>Error al cargar información de logística.</p>';
        funcionariosContainer.innerHTML = '<p>Error al cargar información de logística.</p>';
        dispositivosContainer.innerHTML = '<p>Error al cargar información de logística.</p>';
        sectorSelect.innerHTML = '<option value="">Error cargando sectores</option>';
    }
}


async function cambiarEventoLogistica(idEvento) {
    const eventoId = parseInt(idEvento, 10);
    if (!eventoId) {
        clearLogistica();
        return;
    }
    await loadLogisticaForEvent(eventoId);
}

async function habilitarSector() {
    const idEvento = parseInt(document.getElementById('selectEventoAdmin').value || '0', 10);
    const nombreSector = document.getElementById('selectNuevoSector').value;
    const msgBox = document.getElementById('mensajeHabilitarSector');
    msgBox.style.color = 'var(--colorBordeaux)';
    msgBox.innerText = '';

    if (!idEvento) {
        msgBox.innerText = 'Seleccione primero un evento';
        return;
    }
    if (!nombreSector) {
        msgBox.innerText = 'Ingrese el nombre del sector';
        return;
    }

    try {
        const res = await fetch(`${API_URL}/evento/habilitar-sector`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ idEvento, nombreSector })
        });
        const text = await res.text();
        if (res.ok) {
            msgBox.style.color = 'green';
            msgBox.innerText = 'Sector habilitado correctamente';
            document.getElementById('selectNuevoSector').value = '';;
            await loadLogisticaForEvent(idEvento);
        } else if (res.status === 403) {
            msgBox.innerText = 'No puede habilitar sector en este evento por jurisdicción.';
        } else if (res.status === 400) {
            msgBox.innerText = text || 'Error al habilitar sector';
        } else {
            msgBox.innerText = text || 'Error del servidor al habilitar sector';
        }
    } catch (error) {
        msgBox.innerText = 'Error comunicándose con el servidor';
    }
}

async function asignarFuncionario() {
    const idEvento = parseInt(document.getElementById('selectEventoAdmin').value || '0', 10);
    const nombreSector = document.getElementById('selectSectorAsignar').value;
    const mailFuncionario = document.getElementById('inputMailFuncionario').value.trim();
    const msgBox = document.getElementById('mensajeAsignarFuncionario');
    msgBox.style.color = 'var(--colorBordeaux)';
    msgBox.innerText = '';

    if (!idEvento) {
        msgBox.innerText = 'Seleccione primero un evento';
        return;
    }
    if (!nombreSector) {
        msgBox.innerText = 'Seleccione un sector habilitado';
        return;
    }
    if (!mailFuncionario) {
        msgBox.innerText = 'Ingrese el email del funcionario';
        return;
    }

    try {
        const res = await fetch(`${API_URL}/evento/asignar-funcionario`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ idEvento, nombreSector, mailFuncionario })
        });
        const text = await res.text();
        if (res.ok) {
            msgBox.style.color = 'green';
            msgBox.innerText = 'Funcionario asignado correctamente';
            document.getElementById('inputMailFuncionario').value = '';
            await loadLogisticaForEvent(idEvento);
        } else if (res.status === 403) {
            msgBox.innerText = 'No puede asignar funcionario en este evento por jurisdicción.';
        } else if (res.status === 400) {
            msgBox.innerText = text || 'Error al asignar funcionario';
        } else {
            msgBox.innerText = text || 'Error del servidor al asignar funcionario';
        }
    } catch (error) {
        msgBox.innerText = 'Error comunicándose con el servidor';
    }
}

function clearLogistica() {
    document.getElementById('contenedorSectores').innerHTML = 'Seleccione un evento para ver sectores habilitados.';
    document.getElementById('contenedorFuncionarios').innerHTML = 'Seleccione un evento para ver funcionarios asignados.';
    document.getElementById('contenedorDispositivos').innerHTML = 'Seleccione un evento para ver dispositivos habilitados.';
    document.getElementById('selectSectorAsignar').innerHTML = '<option value="">Seleccione un evento primero</option>';
}