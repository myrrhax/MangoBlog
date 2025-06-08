import { makeAutoObservable } from 'mobx';
import api from '../services/api';

class ArticlesStore {
    articles = [];
    isLoading = false;
    error = null;
    totalPages = 0;
    currentPage = 1;
    filters = {
        tags: [],
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
            this.setArticles(response.data.items);
            this.setTotalPages(response.data.totalPages);
        } catch (error) {
            this.setError(error.response?.data?.message || 'Failed to fetch articles');
        } finally {
            this.setLoading(false);
        }
    }
}

export const articlesStore = new ArticlesStore(); 