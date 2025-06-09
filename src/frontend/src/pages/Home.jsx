import React, { useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import {
    Box,
    Grid,
    Card,
    CardContent,
    Typography,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    Stack,
    Pagination,
    CircularProgress,
    Alert,
    Avatar,
    Button,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import ThumbDownIcon from '@mui/icons-material/ThumbDown';
import { articlesStore } from '../stores/articlesStore';
import { useNavigate } from 'react-router-dom';
import { getMedia } from '../services/media';

const Home = observer(() => {
    const navigate = useNavigate();
    useEffect(() => {
        articlesStore.fetchArticles();
    }, [articlesStore.currentPage, articlesStore.filters]);

    const handlePageChange = (event, value) => {
        articlesStore.setCurrentPage(value);
    };

    const handleQueryChange = (event) => {
        articlesStore.setFilters({ query: event.target.value });
    };

    const handleSortByDateChange = (event) => {
        articlesStore.setFilters({ sortByDate: event.target.value });
    };

    const handleSortByPopularityChange = (event) => {
        articlesStore.setFilters({ sortByPopularity: event.target.value });
    };

    return (
        <Box>
            <Box sx={{ mb: 4 }}>
                <Grid container spacing={2} alignItems="center">
                    <Grid item xs={12} md={3}>
                        <TextField
                            fullWidth
                            label="Search"
                            value={articlesStore.filters.query}
                            onChange={handleQueryChange}
                        />
                    </Grid>
                    <Grid item xs={12} md={3}>
                        <FormControl fullWidth>
                            <InputLabel>Sort by Date</InputLabel>
                            <Select
                                value={articlesStore.filters.sortByDate}
                                label="Sort by Date"
                                onChange={handleSortByDateChange}
                            >
                                <MenuItem value="none">None</MenuItem>
                                <MenuItem value="asc">Oldest First</MenuItem>
                                <MenuItem value="desc">Newest First</MenuItem>
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} md={3}>
                        <FormControl fullWidth>
                            <InputLabel>Sort by Popularity</InputLabel>
                            <Select
                                value={articlesStore.filters.sortByPopularity}
                                label="Sort by Popularity"
                                onChange={handleSortByPopularityChange}
                            >
                                <MenuItem value="none">None</MenuItem>
                                <MenuItem value="asc">Least Popular</MenuItem>
                                <MenuItem value="desc">Most Popular</MenuItem>
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} md={3} sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                        <Button
                            variant="contained"
                            color="success"
                            startIcon={<AddIcon />}
                            onClick={() => navigate('/article/new')}
                        >
                            Add Post
                        </Button>
                    </Grid>
                </Grid>
            </Box>

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
                <>
                    <Grid container spacing={3}>
                        {articlesStore.articles.map((article) => (
                            <Grid item xs={12} md={6} lg={4} key={article.id}>
                                <Card onClick={() => navigate(`/article/${article.id}`)}>
                                    <CardContent>
                                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                            <Avatar
                                                src={article.creator.avatarId 
                                                    ? getMedia(article.creator.avatarId)
                                                    : '/default-avatar.png'}
                                                alt={article.creator.displayedName}
                                                sx={{ width: 40, height: 40, mr: 2 }}
                                            />
                                            <Box>
                                                <Typography variant="subtitle1">
                                                    {article.creator.displayedName}
                                                </Typography>
                                                <Typography variant="caption" color="text.secondary">
                                                    {new Date(article.creatioDate).toLocaleString('ru-RU', {
                                                        year: 'numeric',
                                                        month: 'long',
                                                        day: 'numeric',
                                                        hour: '2-digit',
                                                        minute: '2-digit'
                                                    })}
                                                </Typography>
                                            </Box>
                                        </Box>
                                        <Typography variant="h6" gutterBottom>
                                            {article.title}
                                        </Typography>
                                        <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
                                            {article.tags.map((tag) => (
                                                <Chip
                                                    key={tag}
                                                    label={tag}
                                                    size="small"
                                                    onClick={() =>
                                                        articlesStore.setFilters({
                                                            tags: [...articlesStore.filters.tags, tag],
                                                        })
                                                    }
                                                />
                                            ))}
                                        </Stack>
                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                                <ThumbUpIcon fontSize="small" color="action" />
                                                <Typography variant="body2">
                                                    {article.likes}
                                                </Typography>
                                            </Box>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                                <ThumbDownIcon fontSize="small" color="action" />
                                                <Typography variant="body2">
                                                    {article.dislikes}
                                                </Typography>
                                            </Box>
                                        </Box>
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))}
                    </Grid>

                    <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                        <Pagination
                            count={articlesStore.totalPages}
                            page={articlesStore.currentPage}
                            onChange={handlePageChange}
                            color="primary"
                        />
                    </Box>
                </>
            )}
        </Box>
    );
});

export default Home; 