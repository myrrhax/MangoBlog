import React from 'react';
import { Box, Container } from '@mui/material';

const AuthLayout = ({ children }) => {
    return (
        <Box
            sx={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                bgcolor: 'background.default',
            }}
        >
            <Container maxWidth="sm">
                {children}
            </Container>
        </Box>
    );
};

export default AuthLayout; 