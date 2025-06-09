import React from 'react';
import { Box, Typography } from '@mui/material';

const ArticleView = ({ data }) => {
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
            case 'video':
                // Extract video ID from YouTube URL
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