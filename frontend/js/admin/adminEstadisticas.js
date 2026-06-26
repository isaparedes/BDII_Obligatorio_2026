async function loadEstadisticas() {
    const eventosContainer = document.getElementById('estadisticasEventosMasVendidos');
    const compradoresContainer = document.getElementById('estadisticasMayoresCompradores');
    eventosContainer.innerHTML = '<p>Cargando eventos...</p>';
    compradoresContainer.innerHTML = '<p>Cargando compradores...</p>';

    try {
        const [evRes, cRes] = await Promise.all([
            fetch(`${API_URL}/estadisticas/eventos-mas-vendidos`, { headers: getAuthHeaders() }),
            fetch(`${API_URL}/estadisticas/mayores-compradores`, { headers: getAuthHeaders() })
        ]);

        if (evRes.ok) {
            const eventos = await evRes.json();
            console.log(eventos)
            eventosContainer.innerHTML = eventos.length > 0 ? `
                <table class="tablaEstadisticas">
                    <thead><tr><th>Evento</th><th>Estadio</th><th>Fecha</th><th>Entradas</th></tr></thead>
                    <tbody>
                        ${eventos.map(ev => {
                            const fechaObj = new Date(ev.fechaEvento);

                            const fecha = fechaObj.toLocaleDateString("es-UY");
                            const hora = fechaObj.toLocaleTimeString("es-UY", {
                                hour: "2-digit",
                                minute: "2-digit"
                            });

                            return `
                                <tr>
                                    <td>${ev.equipoLocal} vs ${ev.equipoVisitante}</td>
                                    <td>${ev.nombreEstadio}</td>
                                    <td>${fecha} ${hora}</td>
                                    <td>${(ev.totalEntradasVendidas ?? ev.totalEntradas) || 0}</td>
                                </tr>
                            `;
                        }).join('')}
                    </tbody>
                </table>` : '<p>No hay datos de ventas disponibles.</p>';
        } else {
            eventosContainer.innerHTML = '<p>No autorizado o sin datos</p>';
        }

        if (cRes.ok) {
            const compradores = await cRes.json();
            compradoresContainer.innerHTML = compradores.length > 0 ? `
                <table class="tablaEstadisticas">
                    <thead><tr><th>Usuario general</th><th>Entradas</th><th>Compras</th><th>Gasto</th></tr></thead>
                    <tbody>${compradores.map(c => `<tr><td>${c.mailComprador || c.mail}</td><td>${c.totalEntradas}</td><td>${c.totalCompras}</td><td>$${c.gastoTotal ?? c.totalGastado ?? 0}</td></tr>`).join('')}</tbody>
                </table>` : '<p>No hay compradores registrados.</p>';
        } else {
            compradoresContainer.innerHTML = '<p>No autorizado o sin datos</p>';
        }
    } catch (error) {
        eventosContainer.innerHTML = '<p>Error cargando datos de estadísticas.</p>';
        compradoresContainer.innerHTML = '<p>Error cargando datos de estadísticas.</p>';
    }
}
