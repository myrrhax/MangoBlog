import {useNavigate} from "react-router-dom";
import {
    Card,
    Box,
    CardContent,
    Typography,
    Avatar,
    Stack,
    Chip
} from "@mui/material";
import ThumbUpIcon from "@mui/icons-material/ThumbUp";
import ThumbDownIcon from "@mui/icons-material/ThumbDown";
import {mediaService} from "../../services/mediaService.js";

const Article = ({article}) => {
    const navigate = useNavigate();
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
                <Typography variant="h5" gutterBottom>
                    {article.title}
                </Typography>
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
    )
}

export default Article;