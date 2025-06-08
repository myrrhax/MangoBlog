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

const Register = observer(() => {
    const navigate = useNavigate();

    const formik = useFormik({
        initialValues: {
            login: '',
            password: '',
            confirmPassword: '',
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
                values.confirmPassword
            );
            if (success) {
                navigate('/login');
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
                Sign up
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
                    value={formik.values.confirmPassword}
                    onChange={formik.handleChange}
                    error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
                    helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
                />
                <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                    disabled={authStore.isLoading}
                >
                    {authStore.isLoading ? 'Signing up...' : 'Sign Up'}
                </Button>
                <Box sx={{ textAlign: 'center' }}>
                    <Link component={RouterLink} to="/login" variant="body2">
                        {"Already have an account? Sign In"}
                    </Link>
                </Box>
            </Box>
        </Paper>
    );
});

export default Register; 