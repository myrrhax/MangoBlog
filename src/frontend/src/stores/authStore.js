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

        const isSuccess = await this.fetchUser();
        if (isSuccess) {
            this.setAuthenticated(true);
        } else {
            this.logout();
        }
        this.setLoading(false);
    }

    async fetchUser() {
        try {
            const response = await authService.getCurrentUser();
            this.setUser(response.data);
            return true;
        } catch {
            return false;
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

            return true;
        } catch (error) {
            console.log(error);
            if (error.response.status === 400) {
                if (error.response.data.errors) {
                    this.parseValidationErrors(error.response.data.errors);
                }
                this.setError(error.response.data.message ?? 'Registration failed');
            }
            return false;
        } finally {
            this.setLoading(false);
        }
    }

    async addIntegration() {

    }

    logout() {
        localStorage.removeItem('accessToken');
        this.setUser(null);
        this.setAuthenticated(false);
    }

    clearErrors() {
        this.error = null;
        this.validationErrors = [];
    }

    updateUserInfo(data) {
        console.log(data)
        const { user, accessToken } = data;
        localStorage.setItem('accessToken', accessToken);
        this.isAuthenticated = true;
        this.user = user;
        this.clearErrors();
    }

    parseValidationErrors(errors) {
        this.validationErrors.login = errors.Login ? errors.Login.join('\n') : null;
        this.validationErrors.password = errors.Password ? errors.Password.join('\n') : null;
        this.validationErrors.email = errors.Email ? errors.Email.join('\n') : null;
        this.validationErrors.FirstName = errors.FirstName ? errors.FirstName.join('\n') : null;
        this.validationErrors.LastName = errors.LastName ? errors.LastName.join('\n') : null;
    }
}

export const authStore = new AuthStore(); 