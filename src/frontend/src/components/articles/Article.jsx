import {useNavigate} from "react-router-dom";
import {
    Card,
    Box,
    CardContent,
    Typography,
    Avatar,
    Stack,
    Chip, IconButton
} from "@mui/material";
import ThumbUpIcon from "@mui/icons-material/ThumbUp";
import {mediaService} from "../../services/mediaService.js";
import {observer} from "mobx-react-lite";
import {authStore} from "../../stores/authStore.js";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import {articlesStore} from "../../stores/articlesStore.js";
import ThumbDownOutlinedIcon from "@mui/icons-material/ThumbDownOutlined";
import ThumbUpOutlinedIcon from "@mui/icons-material/ThumbUpOutlined";
import ThumbDownIcon from "@mui/icons-material/ThumbDown";
import React from "react";

const Article = observer(({article}) => {
    const navigate = useNavigate();

    const handleDelete = async (event, id) => {
        event.stopPropagation();
        await articlesStore.deleteArticle(id);
    }
    return (
        <Card
            key={article.id}
            onClick={() => navigate(`/article/${article.id}`)}
            sx={{
                cursor: 'pointer',
                mb: 3,
                '&:hover': {
                    boxShadow: 6
                }
            }}
        >
            <Box
                sx={{
                    width: '100%',
                    height: '300px'
                }}
            >
                <img
                    src={article.coverImageId
                        ? mediaService.makeImageUrl(article.coverImageId)
                        : '/default-article-cover.jpg'}
                    alt={article.title}
                    style={{
                        width: '100%',
                        height: '100%',
                        objectFit: 'cover'
                    }}
                />
            </Box>
            <CardContent>
                <Box
                    sx={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between'}}
                >
                    <Typography variant="h5" gutterBottom>
                        {article.title}
                    </Typography>
                    {authStore.user?.id === article.creator.id && (
                        <Box 
                            sx={{display: 'flex', flexDirection: 'row', gap: 1}}
                            onClick={(e) => e.stopPropagation()}
                        >
                            <IconButton 
                                onClick={(event) => {
                                    event.stopPropagation();
                                    navigate(`/edit-article/${article.id}`);
                                }}
                            >
                                <EditIcon />
                            </IconButton>
                            <IconButton onClick={(event) => handleDelete(event, article.id)}>
                                <DeleteIcon />
                            </IconButton>
                        </Box>
                    )}
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <Avatar
                        src={article.creator.avatarId
                            ? mediaService.makeImageUrl(article.creator.avatarId)
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
                <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
                    {article.tags.map((tag) => (
                        <Chip
                            key={tag}
                            label={tag}
                            size="small"
                        />
                    ))}
                </Stack>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <IconButton color={article.userRating === 'Like' ? 'primary' : 'default'}>
                            {article.userRating === 'like' ? <ThumbUpIcon /> : <ThumbUpOutlinedIcon />}
                        </IconButton>
                        <Typography variant="body2">
                            {article.likes}
                        </Typography>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <IconButton color={article.userRating === 'Dislike' ? 'primary' : 'default'}>
                            {article.userRating === 'dislike' ? <ThumbDownIcon /> : <ThumbDownOutlinedIcon />}
                        </IconButton>
                        <Typography variant="body2">
                            {article.dislikes}
                        </Typography>
                    </Box>
                </Box>
            </CardContent>
        </Card>
    )
})

export default Article;