async function validarEntrada() {
    const input = document.getElementById('tokenEscaneado');
    const token = input.value.trim();
    const button = document.getElementById('btnValidarEntrada');

    if (!token) {
        showError('Ingrese un token válido antes de continuar.');
        return;
    }
    if (!selectedDeviceId) {
        showError('Seleccione un dispositivo antes de validar.');
        return;
    }

    button.disabled = true;
    showValidationMessage('Validando token...', '');

    try {
        console.log("Código QR:", token);
        const response = await fetch(`${API_URL}/validacion/escanear`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ CodigoQr: token, IdDispositivo: Number(selectedDeviceId) })
        });

        const text = await response.text();
        console.log("STATUS:", response.status);
        console.log("RESPUESTA:", text);
        if (response.ok) {
            showSuccess('Entrada validada correctamente.');
            input.value = '';
            setTimeout(() => {
                button.disabled = false;
                showValidationMessage('', '');
            }, 1500);
        } else {
            showError(text || 'Error en la validación del token.');
            button.disabled = false;
        }
    } catch (error) {
        showError('No se pudo conectar con el servidor.');
        button.disabled = false;
    }
}