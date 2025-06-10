import { makeAutoObservable } from 'mobx';
import { authService } from '../services/authService';
import { mediaService } from '../services/mediaService';

class AuthStore {
    user = null;
    isAuthenticated = false;
    isLoading = false;
    error = null;
    validationErrors = [];

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
            this.updateUserInfo(response.data);
            return true;
        } catch (error) {
            this.setError(error.response?.data?.message || 'Login failed');
            return false;
        } finally {
            this.setLoading(false);
        }
    }

    async register(login, password, email, firstName, lastName, birthDate, avatarId = null) {
        this.setLoading(true);
        this.setError(null);
        try {
            const response = await authService.register({login: login,
                password: password,
                email: email,
                firstName: firstName,
                lastName: lastName,
                birthDate: birthDate,
                avatarId: avatarId});
            this.updateUserInfo(response.data);
        } catch (error) {
            if (error.response.status === 400) {
                if (error.response.data.errors) {
                    this.parseValidationErrors(error.response.data.errors);
                }
            } else {
                this.setError(error.response.data.message ?? 'Registration failed');
            }
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

    updateUserInfo(response) {
        const { user, accessToken } = response.data;
        localStorage.setItem('accessToken', accessToken);
        this.isAuthenticated = true;
        this.user = user;
    }

    parseValidationErrors(errors) {
        this.validationErrors.login = errors.Login;
        this.validationErrors.password = errors.Password;
        this.validationErrors.email = errors.Email;
        this.validationErrors.FirstName = errors.FirstName;
        this.validationErrors.LastName = errors.LastName;
    }
}

export const authStore = new AuthStore(); 