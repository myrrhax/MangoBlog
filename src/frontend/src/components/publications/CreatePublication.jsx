import { useState } from 'react';
import {
    Box,
    Button,
    TextField,
    Typography,
    IconButton,
    Alert,
    CircularProgress,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { mediaService } from '../../services/mediaService';
import { publicationsStore } from '../../stores/publicationsStore';

const CreatePublication = ({ open, onClose, onSuccess }) => {
    const [content, setContent] = useState('');
    const [files, setFiles] = useState([]);
    const [previews, setPreviews] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleFileChange = (event) => {
        const newFiles = Array.from(event.target.files);
        setFiles(prev => [...prev, ...newFiles]);
        
        // Create previews for new files
        newFiles.forEach(file => {
            const reader = new FileReader();
            reader.onloadend = () => {
                setPreviews(prev => [...prev, reader.result]);
            };
            reader.readAsDataURL(file);
        });
    };

    const removeFile = (index) => {
        setFiles(prev => prev.filter((_, i) => i !== index));
        setPreviews(prev => prev.filter((_, i) => i !== index));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            // Upload all files first
            const mediaResponses = await Promise.all(
                files.map(file => mediaService.loadMedia(file))
            );
            const mediaIds = mediaResponses.map(response => response.data.id);

            // Create publication
            await publicationsStore.createPublication({
                content,
                mediaIds
            });

            // Reset form
            setContent('');
            setFiles([]);
            setPreviews([]);
            
            if (onSuccess) {
                onSuccess();
            }
            onClose();
        } catch (error) {
            console.error(error);
            setError(error.response?.data?.message || 'Failed to create publication');
        } finally {
            setLoading(false);
        }
    };

    const handleClose = () => {
        setContent('');
        setFiles([]);
        setPreviews([]);
        setError('');
        onClose();
    };

    return (
        <Dialog 
            open={open} 
            onClose={handleClose}
            maxWidth="md"
            fullWidth
        >
            <DialogTitle>
                Create New Publication
            </DialogTitle>
            
            <DialogContent>
                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <form onSubmit={handleSubmit}>
                    <TextField
                        fullWidth
                        multiline
                        rows={4}
                        label="Content"
                        value={content}
                        onChange={(e) => setContent(e.target.value)}
                        sx={{ mb: 2, mt: 1 }}
                        required
                    />

                    <Box sx={{ mb: 2 }}>
                        <input
                            accept="image/*"
                            style={{ display: 'none' }}
                            id="image-upload"
                            type="file"
                            multiple
                            onChange={handleFileChange}
                        />
                        <label htmlFor="image-upload">
                            <Button
                                variant="outlined"
                                component="span"
                                disabled={loading}
                            >
                                Add Images
                            </Button>
                        </label>
                    </Box>

                    {previews.length > 0 && (
                        <Box sx={{ 
                            display: 'flex', 
                            flexWrap: 'wrap', 
                            gap: 2,
                            mb: 2 
                        }}>
                            {previews.map((preview, index) => (
                                <Box
                                    key={index}
                                    sx={{
                                        position: 'relative',
                                        width: 150,
                                        height: 150,
                                    }}
                                >
                                    <img
                                        src={preview}
                                        alt={`Preview ${index + 1}`}
                                        style={{
                                            width: '100%',
                                            height: '100%',
                                            objectFit: 'cover',
                                            borderRadius: 4
                                        }}
                                    />
                                    <IconButton
                                        size="small"
                                        onClick={() => removeFile(index)}
                                        sx={{
                                            position: 'absolute',
                                            top: 4,
                                            right: 4,
                                            bgcolor: 'rgba(255, 255, 255, 0.8)',
                                            '&:hover': {
                                                bgcolor: 'rgba(255, 255, 255, 0.9)',
                                            },
                                        }}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
                                </Box>
                            ))}
                        </Box>
                    )}
                </form>
            </DialogContent>

            <DialogActions>
                <Button onClick={handleClose} disabled={loading}>
                    Cancel
                </Button>
                <Button
                    onClick={handleSubmit}
                    variant="contained"
                    disabled={loading || !content.trim()}
                >
                    {loading ? <CircularProgress size={24} /> : 'Create Publication'}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default CreatePublication; 