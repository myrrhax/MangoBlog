import React from 'react';
import { useFormik } from 'formik';
import { observer } from 'mobx-react-lite';
import { useNavigate, Link } from 'react-router-dom';
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
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8 bg-white p-10 rounded-xl shadow-2xl">
                <div>
                    <h2 className="mt-2 text-center text-4xl font-extrabold text-gray-900">
                        Welcome Back
                    </h2>
                    <p className="mt-2 text-center text-sm text-gray-600">
                        Please sign in to your account
                    </p>
                </div>
                {authStore.error && (
                    <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded" role="alert">
                        <span className="block sm:inline">{authStore.error}</span>
                    </div>
                )}
                <form className="mt-8 space-y-8" onSubmit={formik.handleSubmit}>
                    <div className="space-y-6">
                        <div>
                            <label htmlFor="login" className="block text-sm font-medium text-gray-700 mb-2">
                                Login
                            </label>
                            <input
                                id="login"
                                name="login"
                                type="text"
                                required
                                className="appearance-none block w-full px-4 py-3 border border-gray-300 rounded-lg shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-green-500 focus:border-green-500 text-lg"
                                placeholder="Enter your login"
                                value={formik.values.login}
                                onChange={formik.handleChange}
                            />
                            {formik.touched.login && formik.errors.login && (
                                <p className="mt-2 text-sm text-red-600">{formik.errors.login}</p>
                            )}
                        </div>
                        <div>
                            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
                                Password
                            </label>
                            <input
                                id="password"
                                name="password"
                                type="password"
                                required
                                className="appearance-none block w-full px-4 py-3 border border-gray-300 rounded-lg shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-green-500 focus:border-green-500 text-lg"
                                placeholder="Enter your password"
                                value={formik.values.password}
                                onChange={formik.handleChange}
                            />
                            {formik.touched.password && formik.errors.password && (
                                <p className="mt-2 text-sm text-red-600">{formik.errors.password}</p>
                            )}
                        </div>
                    </div>

                    <div>
                        <button
                            type="submit"
                            disabled={authStore.isLoading}
                            className="w-full flex justify-center py-4 px-4 border border-transparent rounded-lg shadow-sm text-lg font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 transition-colors duration-200"
                        >
                            {authStore.isLoading ? (
                                <span className="flex items-center">
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Signing in...
                                </span>
                            ) : 'Sign In'}
                        </button>
                    </div>

                    <div className="text-center">
                        <Link to="/register" className="font-medium text-green-600 hover:text-green-500 text-lg transition-colors duration-200">
                            Don't have an account? Sign Up
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    );
});

export default Login; 