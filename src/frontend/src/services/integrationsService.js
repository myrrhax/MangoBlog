import api from "./api.js";

export const integrationsService = {
    addIntegration: () => api.post(`/integrations/tg`),
    deleteIntegration: () => api.delete('/integrations', {
        data: {integrationType: 'tg'}
    })
}