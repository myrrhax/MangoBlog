import api from './api';

export const mediaService = {
    getMedia: (id) => api.get(`/media/${id}`),
    loadMedia: (media, isAvatar = false) =>  {
        const formData = new FormData();
        formData.append('file', media);
        formData.append('isAvatar', isAvatar);
        return api.post('/media/', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            }
        });
    },
    makeImageUrl: (id) => `https://localhost:7117/api/media/${id}`,
};