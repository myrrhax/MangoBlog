import React, { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { authStore } from '../../stores/authStore';
import {
    Box,
    Avatar,
    Button,
    Typography,
    Container,
    Paper,
    Alert,
    CircularProgress,
    Link as MuiLink,
} from '@mui/material';
import { styled } from '@mui/material/styles';
import useRegistrationForm from "../../hooks/useRegistrationForm.jsx";
import FormInputField from "../forms/FormInputField..jsx";

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

const StyledDateInput = styled('input')({
    width: '100%',
    padding: '16.5px 14px',
    marginTop: '10px',
    marginBottom: '10px',
    border: '1px solid rgba(0, 0, 0, 0.23)',
    borderRadius: '4px',
    fontSize: '1rem',
    fontFamily: 'inherit',
    '&:hover': {
        borderColor: 'rgba(0, 0, 0, 0.87)',
    },
    '&:focus': {
        outline: 'none',
        borderColor: '#1976d2',
        borderWidth: '2px',
    },
    '&:focus-visible': {
        outline: 'none',
    },
});

const InputLabel = styled('label')({
    display: 'block',
    marginBottom: '8px',
    color: 'rgba(0, 0, 0, 0.6)',
    fontSize: '0.875rem',
    fontWeight: 400,
    lineHeight: 1.43,
    letterSpacing: '0.01071em',
});

const ErrorText = styled('p')({
    color: '#d32f2f',
    margin: '3px 14px 0',
    fontSize: '0.75rem',
    textAlign: 'left',
    fontFamily: '"Roboto","Helvetica","Arial",sans-serif',
    fontWeight: 400,
    lineHeight: 1.66,
    letterSpacing: '0.03333em',
});

const Register = observer(() => {
    const [avatarPreview, setAvatarPreview] = useState('');
    const today = new Date();
    const minDate = new Date(today.getFullYear() - 100, today.getMonth(), today.getDate()); 
    const maxDate = new Date(today.getFullYear() - 1, today.getMonth(), today.getDate());
    const [formik] = useRegistrationForm(minDate, maxDate);

    const formatDateForInput = (date) => {
        return date.toISOString().split('T')[0];
    };
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
                            src={(avatarPreview === '' ? null : avatarPreview)}
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

                    <FormInputField
                        id={"firstName"}
                        name={"firstName"}
                        label={"Имя"}
                        value={formik.values.firstName}
                        onChange={formik.handleChange}
                        error={formik.touched.firstName && Boolean(formik.errors.firstName)}
                        helperText={formik.touched.firstName && formik.errors.firstName}
                    />

                    <FormInputField
                        id={"lastName"}
                        name={"lastName"}
                        label={"Фамилия"}
                        value={formik.values.lastName}
                        onChange={formik.handleChange}
                        error={formik.touched.lastName && Boolean(formik.errors.lastName)}
                        helperText={formik.touched.lastName && formik.errors.lastName}
                    />

                    <Box sx={{ mt: 2 }}>
                        <InputLabel htmlFor="birthDate">Birth Date *</InputLabel>
                        <StyledDateInput
                            id="birthDate"
                            name="birthDate"
                            type="date"
                            value={formik.values.birthDate}
                            onChange={formik.handleChange}
                            required
                            min={formatDateForInput(minDate)}
                            max={formatDateForInput(maxDate)}
                        />
                        {formik.touched.birthDate && formik.errors.birthDate && (
                            <ErrorText>{formik.errors.birthDate}</ErrorText>
                        )}
                    </Box>

                    <FormInputField
                        id={"login"}
                        name={"login"}
                        label={"Логин"}
                        value={formik.values.login}
                        onChange={formik.handleChange}
                        error={formik.touched.login && Boolean(formik.errors.login)}
                        helperText={formik.touched.login && formik.errors.login}
                    />

                    <FormInputField
                        id={"email"}
                        name={"email"}
                        label={"Почта"}
                        type={"email"}
                        value={formik.values.email}
                        onChange={formik.handleChange}
                        error={formik.touched.email && Boolean(formik.errors.email)}
                        helperText={formik.touched.email && formik.errors.email}
                    />

                    <FormInputField
                        id={"password"}
                        name={"password"}
                        label={"Пароль"}
                        type={"password"}
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        error={formik.touched.password && Boolean(formik.errors.password)}
                        helperText={formik.touched.password && formik.errors.password}
                    />

                    <FormInputField
                        id={"confirmPassword"}
                        name={"confirmPassword"}
                        label={"Подтвердите пароль"}
                        type={"password"}
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