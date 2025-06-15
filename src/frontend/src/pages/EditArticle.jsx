import React, { useState, useRef, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { useNavigate, useParams } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Chip,
    Stack,
    Alert,
    CircularProgress
} from '@mui/material';
import { articlesStore } from '../stores/articlesStore';
import { mediaService } from '../services/mediaService';
import Editor from '../components/EditorJS/Editor';
import VisuallyHiddenInput from "../components/forms/VisuallyHiddenInput.jsx";
import { authStore } from '../stores/authStore';

const EditArticle = observer(() => {
    const navigate = useNavigate();
    const { id } = useParams();
    const editorRef = useRef(null);
    const [title, setTitle] = useState('');
    const [tags, setTags] = useState([]);
    const [currentTag, setCurrentTag] = useState('');
    const [error, setError] = useState(null);
    const [cover, setCover] = useState('');
    const [coverFile, setCoverFile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [editorData, setEditorData] = useState(null);

    useEffect(() => {
        const loadArticle = async () => {
            try {
                const article = await articlesStore.fetchArticle(id);
                
                // Check if current user is the creator
                if (article.creator.id !== authStore.user?.id) {
                    setError('You do not have permission to edit this article');
                    setTimeout(() => navigate(`/article/${id}`), 2000);
                    return;
                }

                setTitle(article.title);
                setTags(article.tags || []);
                setCover(article.coverImageId ? mediaService.makeImageUrl(article.coverImageId) : '');
                setEditorData(article.content);
            } catch (error) {
                setError(error.message || 'Failed to load article');
                setTimeout(() => navigate('/'), 2000);
            } finally {
                setLoading(false);
            }
        };

        loadArticle();
    }, [id, navigate]);

    const uploadMedia = async (file) => {
        try {
            const response = await mediaService.loadMedia(file, false);
            return response.data.id;
        } catch (error) {
            console.log(error);
            setError(error.message);
            return null;
        }
    };

    const handleAddTag = (event) => {
        if (event.key === 'Enter' && currentTag.trim()) {
            event.preventDefault();
            if (!tags.includes(currentTag.trim())) {
                setTags([...tags, currentTag.trim()]);
            }
            setCurrentTag('');
        }
    };

    const handleRemoveTag = (event, tagToRemove) => {
        event.preventDefault();
        setTags(tags.filter(tag => tag !== tagToRemove));
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        setError(null);

        if (!title.trim()) {
            setError('Title is required');
            return;
        }

        if (!editorRef.current) {
            setError('Editor is not initialized');
            return;
        }

        try {
            const editorData = await editorRef.current.save();
            let coverId = null;
            
            if (coverFile) {
                coverId = await uploadMedia(coverFile);
                if (coverId === null) {
                    return;
                }
            }

            await articlesStore.updateArticle(id, {
                title: title.trim(),
                content: editorData,
                tags,
                coverImageId: coverId,
            });
            
            navigate(`/article/${id}`);
        } catch (err) {
            setError(err.message || 'Failed to update article');
        }
    };

    const handleEditorChange = async (data) => {
        setEditorData(data);
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Box sx={{ maxWidth: 800, mx: 'auto', py: 4 }}>
            <Card>
                <CardContent>
                    <Typography variant="h4" gutterBottom>
                        Edit Article
                    </Typography>

                    {error && (
                        <Alert severity="error" sx={{ mb: 2 }}>
                            {error}
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
                        <Box component="label" sx={{ cursor: 'pointer', display: 'block', position: 'relative' }}>
                            <Box
                                component="img"
                                src={cover || 'https://via.placeholder.com/600x300?text=Выберите+изображение'}
                                alt="Cover"
                                sx={{
                                    width: '100%',
                                    height: '300px',
                                    objectFit: 'cover',
                                    borderRadius: 2,
                                }}
                            />
                            <VisuallyHiddenInput
                                type="file"
                                accept="image/*"
                                onChange={(e) => {
                                    const file = e.target.files[0];
                                    if (file) {
                                        const reader = new FileReader();
                                        reader.onloadend = () => {
                                            setCover(reader.result);
                                        };
                                        reader.readAsDataURL(file);
                                        setCoverFile(file);
                                    }
                                }}
                            />
                        </Box>

                        <TextField
                            fullWidth
                            label="Title"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            margin="normal"
                            required
                        />

                        <Box sx={{ mt: 2, mb: 2 }}>
                            <Typography variant="subtitle1" gutterBottom>
                                Content
                            </Typography>
                            {editorData && (
                                <Editor 
                                    editorRef={editorRef} 
                                    initialData={editorData}
                                    onChange={handleEditorChange}
                                />
                            )}
                        </Box>

                        <Box sx={{ mt: 2, mb: 2 }}>
                            <Typography variant="subtitle1" gutterBottom>
                                Tags
                            </Typography>
                            <TextField
                                fullWidth
                                label="Add tags (press Enter)"
                                value={currentTag}
                                onChange={(e) => setCurrentTag(e.target.value)}
                                onKeyPress={handleAddTag}
                                margin="normal"
                            />
                            <Stack direction="row" spacing={1} sx={{ mt: 1, flexWrap: 'wrap', gap: 1 }}>
                                {tags.map((tag) => (
                                    <Chip
                                        key={tag}
                                        label={tag}
                                        onDelete={(event) => handleRemoveTag(event, tag)}
                                    />
                                ))}
                            </Stack>
                        </Box>

                        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
                            <Button
                                variant="contained"
                                color="primary"
                                type="submit"
                            >
                                Save Changes
                            </Button>
                            <Button
                                variant="outlined"
                                onClick={() => navigate(`/article/${id}`)}
                            >
                                Cancel
                            </Button>
                        </Box>
                    </form>
                </CardContent>
            </Card>
        </Box>
    );
});

export default EditArticle; 