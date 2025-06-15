import React, { useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import {
    Box,
    Pagination,
    CircularProgress,
    Alert,
} from '@mui/material';
import { articlesStore } from '../stores/articlesStore';
import ArticlesFilters from "../components/articles/ArticlesFilters";
import ArticlesList from "../components/articles/ArticlesList";

const Home = observer(() => {
    useEffect(() => {
        articlesStore.clearFilters();
    }, []);

    useEffect(() => {
        articlesStore.fetchArticles();
    }, [articlesStore.currentPage, articlesStore.filters]);

    return (
        <Box>
            <ArticlesFilters />

            {articlesStore.error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {articlesStore.error}
                </Alert>
            )}

            {articlesStore.isLoading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                    <CircularProgress />
                </Box>
            ) : (
                <ArticlesList articles={articlesStore.articles} />
            )}
        </Box>
    );
});

export default Home; 