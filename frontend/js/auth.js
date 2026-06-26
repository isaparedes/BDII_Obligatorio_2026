function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

function getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {})
    };
}

function getFuncionarioEmailFromToken() {
    const token = localStorage.getItem('token');
    if (!token) return null;

    const payload = parseJwt(token);
    if (!payload) return null;

    return (
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/emailaddress'] ||
        payload['email'] ||
        payload['sub'] ||
        null
    );
}


function getRolesFromToken() {
    const token = localStorage.getItem('token');
    if (!token) return [];

    const payload = parseJwt(token);
    if (!payload) return [];

    const claim =
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload['role'] ||
        payload['roles'];

    if (!claim) return [];

    return Array.isArray(claim) ? claim : [claim];
}

function redirectToLogin() {
    localStorage.removeItem('token');
    window.location.href = 'index.html';
}