import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { theme } from './theme';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import Home from './pages/Home';
import Article from './pages/Article';
import NewArticle from './pages/NewArticle';
import MainLayout from './components/layout/MainLayout';
import AuthLayout from './components/layout/AuthLayout';
import { authStore } from './stores/authStore';

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
                                <div>Profile Page (Coming Soon)</div>
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
                                <Article />
                            </PrivateRoute>
                        }
                    />
                </Routes>
            </Router>
        </ThemeProvider>
    );
});

export default App;
