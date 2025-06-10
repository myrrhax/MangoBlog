import React from 'react';
import { Box, Typography, List, ListItem, ListItemText, Checkbox } from '@mui/material';

const ArticleView = ({ data }) => {
    console.log(data);
    const renderBlock = (block) => {
        switch (block.type) {
            case 'header':
                return (
                    <Typography
                        variant={`h4`}
                        gutterBottom
                        sx={{ mt: 2, mb: 1 }}
                    >
                        {block.data.text}
                    </Typography>
                );
            case 'paragraph':
                return (
                    <Typography paragraph>
                        {block.data.text}
                    </Typography>
                );
            case 'image':
                return (
                    <Box sx={{ my: 2 }}>
                        <img src={block.data.file.url} alt={block.data.caption} />
                    </Box>
                );
            case 'list':
                const ListComponent = block.data.style === 'ordered' ? 'ol' : 'ul';
                return (
                    <List component={ListComponent}>
                        {block.data.items.map((item, index) => (
                            <ListItem key={index}>
                                {block.data.style === 'checklist' && (
                                    <Checkbox
                                        checked={item.meta.checked || false}
                                        disabled
                                        sx={{ mr: 1 }}
                                    />
                                )}
                                <ListItemText 
                                    primary={item.content}
                                    sx={{
                                        '& .MuiListItemText-primary': {
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: 1
                                        }
                                    }}
                                />
                                {block.data.style === 'marked' && (
                                    <Box
                                        component="span"
                                        sx={{
                                            width: 8,
                                            height: 8,
                                            borderRadius: '50%',
                                            backgroundColor: 'primary.main',
                                            display: 'inline-block',
                                            marginRight: 1
                                        }}
                                    />
                                )}
                            </ListItem>
                        ))}
                    </List>
                );
            case 'video':
                const getYouTubeId = (url) => {
                    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
                    const match = url.match(regExp);
                    return (match && match[2].length === 11) ? match[2] : null;
                };

                const videoId = getYouTubeId(block.data.url);
                if (!videoId) {
                    return (
                        <Box sx={{ my: 2 }}>
                            <Typography color="error">
                                Invalid video URL
                            </Typography>
                        </Box>
                    );
                }

                return (
                    <Box sx={{ my: 2 }}>
                        <Box
                            sx={{
                                position: 'relative',
                                paddingBottom: '56.25%', // 16:9 aspect ratio
                                height: 0,
                                overflow: 'hidden',
                                maxWidth: '100%',
                            }}
                        >
                            <iframe
                                src={`https://www.youtube.com/embed/${videoId}`}
                                title={block.data.caption || 'YouTube video'}
                                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                                allowFullScreen
                                style={{
                                    position: 'absolute',
                                    top: 0,
                                    left: 0,
                                    width: '100%',
                                    height: '100%',
                                    border: 0,
                                }}
                            />
                        </Box>
                        {block.data.caption && (
                            <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
                                {block.data.caption}
                            </Typography>
                        )}
                    </Box>
                );
            default:
                return null;
        }
    };

    return (
        <Box>
            {data.blocks.map((block, index) => (
                <React.Fragment key={index}>
                    {renderBlock(block)}
                </React.Fragment>
            ))}
        </Box>
    );
};

export default ArticleView;