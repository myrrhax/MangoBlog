import React, { useEffect, useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useParams, useNavigate } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    Typography,
    Avatar,
    Alert,
    IconButton,
    Chip,
    Skeleton,
} from '@mui/material';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';  
import ThumbDownIcon from '@mui/icons-material/ThumbDown';
import ThumbUpOutlinedIcon from '@mui/icons-material/ThumbUpOutlined';
import ThumbDownOutlinedIcon from '@mui/icons-material/ThumbDownOutlined';
import { articlesStore } from '../stores/articlesStore';
import { authStore } from '../stores/authStore';
import ArticleView from '../components/ArticleView';
import SnackbarNotification from '../components/SnackbarNotification';
import useNotificationSnackbar from '../hooks/useNotificationSnackbar';
import { mediaService } from '../services/mediaService';

const Article = observer(() => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [article, setArticle] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const { isAuthenticated, user } = authStore;
    const { isOpen, setIsOpen, message, setMessage } = useNotificationSnackbar();

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
            return;
        }

        const fetchArticle = async () => {
            try {
                setIsLoading(true);
                const response = await articlesStore.fetchArticle(id);
                setArticle(response);
                console.log(response);  
            } catch (err) {
                setError(err.message);
            } finally {
                setIsLoading(false);
            }
        };

        fetchArticle();
    }, [id, isAuthenticated, navigate]);

    const handleChangeRating = async (rating) => {
        if (article.creator.id === user.id) {
            setMessage('You cannot rate your own article');
            setIsOpen(true);
            return;
        }

        try {
            await articlesStore.rateArticle(id, rating);
            const updatedArticle = await articlesStore.fetchArticle(id);
            setArticle(updatedArticle);
        } catch (err) {
            setError(err.message);
        }
    }

    if (error) {
        return (
            <Alert severity="error" sx={{ my: 2 }}>
                {error}
            </Alert>
        );
    }

    return (
        <Box sx={{ maxWidth: 800, mx: 'auto', py: 4 }}>
            <SnackbarNotification open={isOpen} setOpen={setIsOpen} message={message} />
            <Card>
                <CardContent>
                    {isLoading ? (
                        <>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
                                <Skeleton variant="circular" width={48} height={48} sx={{ mr: 2 }} />
                                <Box>
                                    <Skeleton variant="text" width={120} height={24} />
                                    <Skeleton variant="text" width={180} height={20} />
                                </Box>
                            </Box>
                            <Skeleton variant="text" width="80%" height={40} sx={{ mb: 2 }} />
                            <Box sx={{ mb: 3 }}>
                                <Skeleton variant="text" width={60} height={32} sx={{ mr: 1, display: 'inline-block' }} />
                                <Skeleton variant="text" width={60} height={32} sx={{ mr: 1, display: 'inline-block' }} />
                                <Skeleton variant="text" width={60} height={32} sx={{ display: 'inline-block' }} />
                            </Box>
                            <Box sx={{ mb: 3 }}>
                                <Skeleton variant="rectangular" height={200} />
                            </Box>
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                <Skeleton variant="circular" width={40} height={40} />
                                <Skeleton variant="text" width={30} />
                                <Skeleton variant="circular" width={40} height={40} />
                                <Skeleton variant="text" width={30} />
                            </Box>
                        </>
                    ) : article ? (
                        <>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
                                <Avatar
                                    src={article.creator?.avatarId 
                                        ? mediaService.makeImageUrl(article.creator.avatarId)
                                        : '/default-avatar.png'}
                                    alt={article.creator?.displayedName || 'Anonymous'}
                                    sx={{ width: 48, height: 48, mr: 2 }}
                                />
                                <Box>
                                    <Typography variant="h6">
                                        {article.creator?.displayedName || 'Anonymous'}
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

                            <Typography variant="h4" gutterBottom>
                                {article.title}
                            </Typography>

                            <Box sx={{ mb: 3 }}>
                                {article.tags?.map((tag) => (
                                    <Chip
                                        key={tag}
                                        label={tag}
                                        size="small"
                                        sx={{ mr: 1, mb: 1 }}
                                    />
                                ))}
                            </Box>

                            <Box sx={{ mb: 3 }}>
                                <ArticleView data={article.content} />
                            </Box>

                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                    <IconButton onClick={() => handleChangeRating('like')} color={article.userRating === 'Like' ? 'primary' : 'default'}>
                                        {article.userRating === 'like' ? <ThumbUpIcon /> : <ThumbUpOutlinedIcon />}
                                    </IconButton>
                                    <Typography variant="body1">
                                        {article.likes}
                                    </Typography>
                                </Box>
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                    <IconButton onClick={() => handleChangeRating('dislike')} color={article.userRating === 'Dislike' ? 'primary' : 'default'}>
                                        {article.userRating === 'dislike' ? <ThumbDownIcon /> : <ThumbDownOutlinedIcon />}
                                    </IconButton>
                                    <Typography variant="body1">
                                        {article.dislikes}
                                    </Typography>
                                </Box>
                            </Box>
                        </>
                    ) : null}
                </CardContent>
            </Card>
        </Box>
    );
});

export default Article; 