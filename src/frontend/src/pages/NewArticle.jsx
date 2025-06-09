import React, { useState, useRef, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { useNavigate } from 'react-router-dom';
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
} from '@mui/material';
import EditorJS from '@editorjs/editorjs';
import Header from '@editorjs/header';
import Paragraph from '@editorjs/paragraph';
import List from '@editorjs/list';
import Image from '@editorjs/image';
import { articlesStore } from '../stores/articlesStore';
import { mediaService } from '../services/mediaService';

const NewArticle = observer(() => {
    const navigate = useNavigate();
    const editorRef = useRef(null);
    const [title, setTitle] = useState('');
    const [tags, setTags] = useState([]);
    const [currentTag, setCurrentTag] = useState('');
    const [error, setError] = useState(null);
    const [media, setMedia] = useState([]);

    useEffect(() => {
        const initEditor = async () => {
            if (!editorRef.current) {
                editorRef.current = new EditorJS({
                    holder: 'editorjs',
                    tools: {
                        header: {
                            class: Header,
                            config: {
                                placeholder: 'Enter a header',
                                levels: [1, 2, 3, 4],
                                defaultLevel: 2
                            }
                        },
                        paragraph: {
                            class: Paragraph,
                            inlineToolbar: ['bold', 'italic', 'link'],
                        },
                        list: {
                            class: List,
                            inlineToolbar: ['bold', 'italic', 'link'],
                        },
                        image: {
                            class: Image,
                            config: {
                                uploader: {
                                    uploadByFile(file) {
                                        return new Promise((resolve, reject) => {
                                            const formData = new FormData();
                                            formData.append('file', file);
                                            formData.append('isAvatar', false);
                                            
                                            mediaService.loadMedia(formData)
                                                .then(response => {
                                                    resolve({
                                                        success: 1,
                                                        file: {
                                                            url: mediaService.makeImageUrl(response.data.id)
                                                        }
                                                    });
                                                })
                                                .catch(error => {
                                                    reject(error);
                                                });
                                        });
                                    }
                                }
                            }
                        }
                    },
                    inlineToolbar: ['bold', 'italic', 'link'],
                    placeholder: 'Start writing your article...',
                    data: {
                        blocks: [
                            {
                                type: "paragraph",
                                data: {
                                    text: "Start writing your article..."
                                }
                            }
                        ]
                    }
                });
            }
        };

        initEditor();

        return () => {
            if (editorRef.current && editorRef.current.destroy) {
                editorRef.current.destroy();
            }
        };
    }, []);

    const uploadMedia = async (file) => {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('isAvatar', false);
        const response = await mediaService.loadMedia(formData);
        setMedia([...media, response.data]);
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

    const handleRemoveTag = (tagToRemove) => {
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
            await articlesStore.createArticle({
                title: title.trim(),
                content: editorData,
                tags,
            });
            navigate('/');
        } catch (err) {
            setError(err.message || 'Failed to create article');
        }
    };

    return (
        <Box sx={{ maxWidth: 800, mx: 'auto', py: 4 }}>
            <Card>
                <CardContent>
                    <Typography variant="h4" gutterBottom>
                        Create New Article
                    </Typography>

                    {error && (
                        <Alert severity="error" sx={{ mb: 2 }}>
                            {error}
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
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
                                        onDelete={() => handleRemoveTag(tag)}
                                    />
                                ))}
                            </Stack>
                        </Box>

                        <Box sx={{ mt: 2, mb: 2 }}>
                            <Typography variant="subtitle1" gutterBottom>
                                Content
                            </Typography>
                            <div
                                id="editorjs"
                                className="prose lg:prose-xl max-w-none [&_h1]:text-4xl [&_h2]:text-3xl [&_h3]:text-2xl [&_h4]:text-xl"
                            />
                        </Box>

                        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
                            <Button
                                variant="contained"
                                color="success"
                                type="submit"
                            >
                                Publish
                            </Button>
                            <Button
                                variant="outlined"
                                onClick={() => navigate('/')}
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

export default NewArticle; 