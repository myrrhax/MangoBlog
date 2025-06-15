import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { CssBaseline, ThemeProvider, CircularProgress, Box } from '@mui/material';
import { theme } from './theme';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import Home from './pages/Home';
import ArticlePage from './pages/ArticlePage.jsx';
import NewArticle from './pages/NewArticle';
import Profile from './pages/Profile';
import MainLayout from './components/layout/MainLayout';
import AuthLayout from './components/layout/AuthLayout';
import { authStore } from './stores/authStore';
import {useEffect, useRef, useState} from 'react';

const PrivateRoute = observer(({ children }) => {
    if (!authStore.isAuthenticated) {
        return <Navigate to="/login" />;
    }
    return <MainLayout>{children}</MainLayout>;
});

const AuthRoute = observer(({ children }) => {
    if (authStore.isAuthenticated) {
        return <Navigate to="/" />;
    }
    return <AuthLayout>{children}</AuthLayout>;
});

const App = observer(() => {
    const [isCheckingAuth, setIsCheckingAuth] = useState(true);
    const hasCheckedRef = useRef(false);

    useEffect(() => {
        if (hasCheckedRef.current) return;
        hasCheckedRef.current = true;

        const checkAuth = async () => {
            await authStore.checkAuth();
            setIsCheckingAuth(false);
        };
        checkAuth();
    }, []);

    if (isCheckingAuth) {
        return (
            <ThemeProvider theme={theme}>
                <CssBaseline />
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        minHeight: '100vh'
                    }}
                >
                    <CircularProgress />
                </Box>
            </ThemeProvider>
        );
    }

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Router>
                <Routes>
                    <Route
                        path="/login"
                        element={
                            <AuthRoute>
                                <Login />
                            </AuthRoute>
                        }
                    />
                    <Route
                        path="/register"
                        element={
                            <AuthRoute>
                                <Register />
                            </AuthRoute>
                        }
                    />
                    <Route
                        path="/"
                        element={
                            <PrivateRoute>
                                <Home />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path="/profile"
                        element={
                            <PrivateRoute>
                                <Profile />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path="/profile/:userId"
                        element={
                            <PrivateRoute>
                                <Profile />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path="/integrations"
                        element={
                            <PrivateRoute>
                                <div>Integrations Page (Coming Soon)</div>
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path="/article/new"
                        element={
                            <PrivateRoute>
                                <NewArticle />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path="/article/:id"
                        element={
                            <PrivateRoute>
                                <ArticlePage />
                            </PrivateRoute>
                        }
                    />
                </Routes>
            </Router>
        </ThemeProvider>
    );
});

export default App;
