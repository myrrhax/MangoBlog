import React from 'react';
import { useFormik } from 'formik';
import { observer } from 'mobx-react-lite';
import { useNavigate, Link } from 'react-router-dom';
import { authStore } from '../../stores/authStore';
import {
    Box,
    TextField,
    Button,
    Typography,
    Container,
    Paper,
    Alert,
    CircularProgress,
    Link as MuiLink
} from '@mui/material';

const Login = observer(() => {
    const navigate = useNavigate();

    const formik = useFormik({
        initialValues: {
            login: '',
            password: '',
        },
        validate: (values) => {
            const errors = {};
            if (!values.login) {
                errors.login = 'Required';
            }
            if (!values.password) {
                errors.password = 'Required';
            }
            return errors;
        },
        onSubmit: async (values) => {
            const success = await authStore.login(values.login, values.password);
            if (success) {
                navigate('/');
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
                    Welcome Back
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                    Please sign in to your account
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
                        autoComplete="current-password"
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        error={formik.touched.password && Boolean(formik.errors.password)}
                        helperText={formik.touched.password && formik.errors.password}
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
                            'Sign In'
                        )}
                    </Button>

                    <Box sx={{ textAlign: 'center' }}>
                        <MuiLink
                            component={Link}
                            to="/register"
                            variant="body2"
                            sx={{
                                textDecoration: 'none',
                                '&:hover': {
                                    textDecoration: 'underline',
                                },
                            }}
                        >
                            Don't have an account? Sign Up
                        </MuiLink>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
});

export default Login; 