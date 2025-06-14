import {makeAutoObservable} from "mobx";
import { authService } from "../services/authService";
import {integrationsService} from "../services/integrationsService.js";
import {authStore} from "./authStore.js";

class ProfileStore {
    user = null;
    isLoading = false;
    loadingError = null;
    addIntegrationError = null;
    isCurrentUser = false;

    constructor() {
        makeAutoObservable(this);
    }

    async fetchUser(userId) {
        this.isLoading = true;
        this.isCurrentUser = false;
        try {
            const response = await authService.getUser(userId);
            this.setUser(response.data);
        } catch (error) {
            console.error(error);
            this.setLoadingError(error);
        } finally {
            this.isLoading = false;
        }
    }

    setLoadingError(error) {
        this.loadingError = error;
    }

    setUser(user, isCurrent = false) {
        this.isCurrentUser = isCurrent;
        this.user = user;
    }

    async addIntegration() {
        if (!this.isCurrentUser) {
            return false;
        }
        this.isLoading = true;
        try {
            await integrationsService.addIntegration();
            return true;
        } catch (error) {
            console.error(error);
            this.addIntegrationError = "Возникла ошибка при добавлении интеграции";
            return false;
        } finally {
            this.isLoading = false;
        }
    }
}

export const profileStore = new ProfileStore();