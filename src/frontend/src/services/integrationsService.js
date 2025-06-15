import api from "./api.js";

export const integrationsService = {
    addIntegration: () => api.post(`/integrations/tg`),
}