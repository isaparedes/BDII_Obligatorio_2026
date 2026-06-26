async function fetchFuncionario(mail) {
    try {
        const response = await fetch(`${API_URL}/usuario/${encodeURIComponent(mail)}/funcionario`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) {
            throw new Error('No se pudo obtener datos del funcionario');
        }
        return await response.json();
    } catch {
        return null;
    }
}

async function fetchDispositivos(mail) {
    try {
        const response = await fetch(`${API_URL}/dispositivo/${encodeURIComponent(mail)}`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) {
            throw new Error('No se pudo obtener dispositivos');
        }
        return await response.json();
    } catch {
        return [];
    }
}