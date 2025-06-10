import { makeAutoObservable } from 'mobx';
import { authService } from '../services/authService';

class AuthStore {
    user = null;
    isAuthenticated = false;
    isLoading = false;
    error = null;

    constructor() {
        makeAutoObservable(this);
    }

    async checkAuth() {
        const token = localStorage.getItem('accessToken');

        if (!token) {
            return;
        }

        try {
            const response = await authService.getCurrentUser();
            this.setUser(response.data);
            this.setAuthenticated(true);
        } catch (error) {
            this.logout();
        }
    }

    setUser(user) {
        this.user = user;
    }

    setAuthenticated(status) {
        this.isAuthenticated = status;
    }

    setLoading(status) {
        this.isLoading = status;
    }

    setError(error) {
        this.error = error;
    }

    async login(login, password) {
        this.setLoading(true);
        this.setError(null);
        try {
            const response = await authService.login({ login, password });
            const { accessToken } = response.data;
            localStorage.setItem('accessToken', accessToken);
            await this.checkAuth();
            return true;
        } catch (error) {
            this.setError(error.response?.data?.message || 'Login failed');
            return false;
        } finally {
            this.setLoading(false);
        }
    }

    async register(email, password, confirmPassword, avatar) {
        this.setLoading(true);
        this.setError(null);
        try {
            const formData = new FormData();
            formData.append('email', email);
            formData.append('password', password);
            formData.append('confirmPassword', confirmPassword);
            if (avatar) {
                formData.append('avatar', avatar);
            }

            await authService.register(formData);
            return true;
        } catch (error) {
            this.setError(error.response?.data?.message || 'Registration failed');
            return false;
        } finally {
            this.setLoading(false);
        }
    }

    logout() {
        localStorage.removeItem('accessToken');
        this.setUser(null);
        this.setAuthenticated(false);
    }
}

export const authStore = new AuthStore(); 