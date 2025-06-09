import axios from 'axios';

const API_URL = 'https://localhost:7117/api';

const api = axios.create({
    baseURL: API_URL,
    withCredentials: true,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response interceptor
api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        // If error is 401 and we haven't tried to refresh token yet
        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                // Try to refresh token
                const response = await api.post('/auth/refresh');
                const { accessToken } = response.data;
                
                // Store new access token
                localStorage.setItem('accessToken', accessToken);
                
                // Retry original request with new token
                originalRequest.headers.Authorization = `Bearer ${accessToken}`;
                return api(originalRequest);
            } catch (refreshError) {
                // If refresh token fails, redirect to login
                localStorage.removeItem('accessToken');
                window.location.href = '/login';
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);

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
    getMedia: (id) => api.get(`/media/${id}`),
    loadMedia: (formData) => api.post('/media/upload', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        }
    })
};

export default api; 