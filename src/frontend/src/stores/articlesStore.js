import { makeAutoObservable } from 'mobx';
import api from '../services/api';

class ArticlesStore {
    articles = [];
    isLoading = false;
    error = null;
    totalPages = 0;
    currentPage = 1;
    filters = {
        query: '',
        sortByDate: 'none',
        sortByPopularity: 'none',
    };

    constructor() {
        makeAutoObservable(this);
    }

    setArticles(articles) {
        this.articles = articles;
    }

    setLoading(status) {
        this.isLoading = status;
    }

    setError(error) {
        this.error = error;
    }

    setTotalPages(pages) {
        this.totalPages = pages;
    }

    setCurrentPage(page) {
        this.currentPage = page;
    }

    setFilters(filters) {
        this.filters = { ...this.filters, ...filters };
    }

    async fetchArticles() {
        this.setLoading(true);
        this.setError(null);

        try {
            const params = {
                page: this.currentPage,
                ...this.filters,
            };

            const response = await api.get('/articles', { params });
            this.setArticles(response.data.articles);
            this.setTotalPages(response.data.totalPages);
        } catch (error) {
            this.setArticles([]);
            this.setTotalPages(0);
        } finally {
            this.setLoading(false);
        }
    }

    async fetchArticle(id) {
        try {
            const response = await api.get(`/articles/${id}`);
            return response.data.data;
        } catch (error) {
            throw new Error(error.response?.data?.message || 'Failed to fetch article');
        }
    }

    async rateArticle(id, rating) {
        try {
            await api.post(`/ratings`, { postId: id, ratingType: rating });
        } catch (error) {
            throw new Error(error.response?.data?.message || 'Failed to rate article');
        }
    }

    async createArticle(articleData) {
        try {
            const response = await api.post('/articles', articleData);
            return response.data;
        } catch (error) {
            console.log(error);
            throw new Error(error.response?.data?.message || 'Failed to create article');
        }
    }
}

export const articlesStore = new ArticlesStore(); 