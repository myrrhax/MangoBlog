import React, { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useNavigate } from 'react-router-dom';
import {
    AppBar,
    Toolbar,
    IconButton,
    Typography,
    Menu,
    MenuItem,
    Avatar,
    Container,
} from '@mui/material';
import { authStore } from '../../stores/authStore';

const Header = observer(() => {
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = useState(null);

    const handleMenu = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleProfile = () => {
        handleClose();
        navigate('/profile');
    };

    const handleIntegrations = () => {
        handleClose();
        navigate('/integrations');
    };

    const handleLogout = () => {
        handleClose();
        authStore.logout();
        navigate('/login');
    };

    return (
        <AppBar position="static" color="default" elevation={1}>
            <Container maxWidth="lg">
                <Toolbar disableGutters>
                    <Typography
                        variant="h6"
                        component="div"
                        sx={{ flexGrow: 1, cursor: 'pointer' }}
                        onClick={() => navigate('/')}
                    >
                        Mango Blog
                    </Typography>
                    <IconButton
                        size="large"
                        onClick={handleMenu}
                        color="inherit"
                    >
                        <Avatar
                            alt={authStore.user?.email}
                            src="/static/images/avatar/2.jpg"
                        />
                    </IconButton>
                    <Menu
                        anchorEl={anchorEl}
                        open={Boolean(anchorEl)}
                        onClose={handleClose}
                    >
                        <MenuItem onClick={handleProfile}>Profile</MenuItem>
                        <MenuItem onClick={handleIntegrations}>Integrations</MenuItem>
                        <MenuItem onClick={handleLogout}>Logout</MenuItem>
                    </Menu>
                </Toolbar>
            </Container>
        </AppBar>
    );
});

export default Header; 