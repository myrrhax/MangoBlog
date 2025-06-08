import React from 'react';
import { useFormik } from 'formik';
import { observer } from 'mobx-react-lite';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import {
    Box,
    Typography,
    TextField,
    Button,
    Link,
    Alert,
    Paper,
} from '@mui/material';
import { authStore } from '../../stores/authStore';

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
        <Paper
            elevation={3}
            sx={{
                p: 4,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                width: '100%',
                maxWidth: 400,
            }}
        >
            <Typography component="h1" variant="h5" sx={{ mb: 3 }}>
                Sign in
            </Typography>
            {authStore.error && (
                <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
                    {authStore.error}
                </Alert>
            )}
            <Box component="form" onSubmit={formik.handleSubmit} sx={{ width: '100%' }}>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="login"
                    label="Login"
                    name="login"
                    autoComplete="username"
                    autoFocus
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
                    sx={{ mt: 3, mb: 2 }}
                    disabled={authStore.isLoading}
                >
                    {authStore.isLoading ? 'Signing in...' : 'Sign In'}
                </Button>
                <Box sx={{ textAlign: 'center' }}>
                    <Link component={RouterLink} to="/register" variant="body2">
                        {"Don't have an account? Sign Up"}
                    </Link>
                </Box>
            </Box>
        </Paper>
    );
});

export default Login; 