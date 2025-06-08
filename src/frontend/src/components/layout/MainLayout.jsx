import React from 'react';
import { Box, Container } from '@mui/material';
import Header from './Header';

const MainLayout = ({ children }) => {
    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <Header />
            <Container maxWidth="lg" sx={{ flexGrow: 1, py: 4 }}>
                {children}
            </Container>
        </Box>
    );
};

export default MainLayout; 