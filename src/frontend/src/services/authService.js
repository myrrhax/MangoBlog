import api from './api';

export const authService = {
    login: (credentials) => api.post('/users/login', credentials, {
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    }),
    register: (formData) => api.post('/users/register', formData, {
        withCredentials: true,
        headers: {
            'Content-Type': 'multipart/form-data',
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