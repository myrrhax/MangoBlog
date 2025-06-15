import api from "./api.js";

export const PublicationsService = {
    fetchMy: () => api.get('publications/my'),
    create: (publicationData) => api.post('publications', publicationData)
}