import api from './api';

export const authService = {
    login: (credentials) => api.post('/users/login', credentials, {
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    }),
    register: (userData) => api.post('/users/register', userData, {
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    }),
    refreshToken: () => api.post('/users/refresh', {
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    }),
    getCurrentUser: () => api.get('/users/me', {
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    }),
    
};