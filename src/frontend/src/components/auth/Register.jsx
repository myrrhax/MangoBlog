import React, { useState } from 'react';
import { useFormik } from 'formik';
import { observer } from 'mobx-react-lite';
import { useNavigate, Link } from 'react-router-dom';
import { authStore } from '../../stores/authStore';
import {
    Box,
    Avatar,
    TextField,
    Button,
    Typography,
    Container,
    Paper,
    Alert,
    CircularProgress,
    Link as MuiLink
} from '@mui/material';
import { styled } from '@mui/material/styles';

const VisuallyHiddenInput = styled('input')({
    clip: 'rect(0 0 0 0)',
    clipPath: 'inset(50%)',
    height: 1,
    overflow: 'hidden',
    position: 'absolute',
    bottom: 0,
    left: 0,
    whiteSpace: 'nowrap',
    width: 1,
});

const Register = observer(() => {
    const navigate = useNavigate();
    const [avatarPreview, setAvatarPreview] = useState(null);

    const handleAvatarChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setAvatarPreview(reader.result);
            };
            reader.readAsDataURL(file);
        }
    };

    const formik = useFormik({
        initialValues: {
            login: '',
            password: '',
            confirmPassword: '',
            avatar: null,
        },
        validate: (values) => {
            const errors = {};
            if (!values.login) {
                errors.login = 'Required';
            }
            if (!values.password) {
                errors.password = 'Required';
            } else if (values.password.length < 6) {
                errors.password = 'Password must be at least 6 characters';
            }
            if (!values.confirmPassword) {
                errors.confirmPassword = 'Required';
            } else if (values.password !== values.confirmPassword) {
                errors.confirmPassword = 'Passwords do not match';
            }
            return errors;
        },
        onSubmit: async (values) => {
            const success = await authStore.register(
                values.login,
                values.password,
                values.confirmPassword,
                values.avatar
            );
            if (success) {
                navigate('/login');
            }
        },
    });

    return (
        <Container component="main" maxWidth="xs">
            <Paper
                elevation={3}
                sx={{
                    marginTop: 8,
                    padding: 4,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h4" gutterBottom>
                    Create Account
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                    Join our community today
                </Typography>

                {authStore.error && (
                    <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
                        {authStore.error}
                    </Alert>
                )}

                <Box
                    component="form"
                    onSubmit={formik.handleSubmit}
                    sx={{ mt: 3, width: '100%' }}
                >
                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                            mb: 3,
                        }}
                    >
                        <Avatar
                            src={avatarPreview}
                            sx={{
                                width: 120,
                                height: 120,
                                cursor: 'pointer',
                                '&:hover': {
                                    opacity: 0.8,
                                },
                                mb: 1,
                            }}
                            component="label"
                        >
                            {!avatarPreview && 'Add Photo'}
                            <VisuallyHiddenInput
                                type="file"
                                accept="image/*"
                                onChange={(e) => {
                                    handleAvatarChange(e);
                                    formik.setFieldValue('avatar', e.target.files[0]);
                                }}
                            />
                        </Avatar>
                        <Typography variant="body2" color="text.secondary">
                            Click to upload your avatar
                        </Typography>
                    </Box>

                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        id="login"
                        label="Login"
                        name="login"
                        autoComplete="username"
                        value={formik.values.login}
                        onChange={formik.handleChange}
                        error={formik.touched.login && Boolean(formik.errors.login)}
                        helperText={formik.touched.login && formik.errors.login}
                    />

                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        name="password"
                        label="Password"
                        type="password"
                        id="password"
                        autoComplete="new-password"
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        error={formik.touched.password && Boolean(formik.errors.password)}
                        helperText={formik.touched.password && formik.errors.password}
                    />

                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        name="confirmPassword"
                        label="Confirm Password"
                        type="password"
                        id="confirmPassword"
                        autoComplete="new-password"
                        value={formik.values.confirmPassword}
                        onChange={formik.handleChange}
                        error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
                        helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
                    />

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        size="large"
                        disabled={authStore.isLoading}
                        sx={{ mt: 3, mb: 2 }}
                    >
                        {authStore.isLoading ? (
                            <CircularProgress size={24} color="inherit" />
                        ) : (
                            'Create Account'
                        )}
                    </Button>

                    <Box sx={{ textAlign: 'center' }}>
                        <MuiLink
                            component={Link}
                            to="/login"
                            variant="body2"
                            sx={{
                                textDecoration: 'none',
                                '&:hover': {
                                    textDecoration: 'underline',
                                },
                            }}
                        >
                            Already have an account? Sign In
                        </MuiLink>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
});

export default Register; 