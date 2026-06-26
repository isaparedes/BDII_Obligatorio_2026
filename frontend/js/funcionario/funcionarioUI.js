let selectedDeviceId = null;

function showValidationMessage(message, type) {
    const msg = document.getElementById('mensajeValidacion');
    msg.textContent = message;
    msg.style.color = type === 'success' ? 'green' : 'red';
}

function showError(message) {
    showValidationMessage(message, 'error');
}

function showSuccess(message) {
    showValidationMessage(message, 'success');
}

function renderDispositivos(dispositivos) {
    const container = document.getElementById('listaDispositivos');
    container.innerHTML = '';

    if (!Array.isArray(dispositivos) || dispositivos.length === 0) {
        container.innerHTML = '<p style="color: red;">No hay dispositivos asignados.</p>';
        return;
    }

    dispositivos.forEach((dispositivo, index) => {
        const id = dispositivo.IdDispositivo ?? dispositivo.idDispositivo ?? dispositivo.id ?? dispositivo.id_dispositivo;
        const card = document.createElement('div');
        card.className = 'dispositivoCard';
        card.style.cursor = 'pointer';
        card.style.border = '1px solid #ccc';
        card.style.borderRadius = '8px';
        card.style.padding = '12px';
        card.style.marginBottom = '10px';
        card.style.background = index === 0 ? '#f0f8ff' : '#ffffff';
        card.dataset.deviceId = id;
        card.innerHTML = `<p><strong>N° dispositivo:</strong> ${id}</p>`;
        card.addEventListener('click', () => selectDeviceCard(card, id));
        container.appendChild(card);

        if (index === 0) {
            selectedDeviceId = id;
        }
    });

    updateSelectedDeviceUI();
}

function selectDeviceCard(card, deviceId) {
    selectedDeviceId = deviceId;
    document.querySelectorAll('#listaDispositivos .dispositivoCard').forEach(c => {
        c.style.background = '#ffffff';
        c.style.borderColor = '#ccc';
    });
    card.style.background = '#f0f8ff';
    card.style.borderColor = '#007b33';
}

function updateSelectedDeviceUI() {
    const cards = document.querySelectorAll('#listaDispositivos .dispositivoCard');
    if (cards.length === 0) return;
    cards.forEach(card => {
        const id = card.dataset.deviceId;
        if (id && id.toString() === selectedDeviceId?.toString()) {
            card.style.background = '#f0f8ff';
            card.style.borderColor = '#007b33';
        } else {
            card.style.background = '#ffffff';
            card.style.borderColor = '#ccc';
        }
    });
}
