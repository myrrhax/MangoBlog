import React, { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useNavigate } from 'react-router-dom';
import { authStore } from '../../stores/authStore';
import { mediaService } from '../../services/mediaService';

const Header = observer(() => {
    const navigate = useNavigate();
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const { user } = authStore;

    const handleProfile = () => {
        setIsMenuOpen(false);
        navigate('/profile');
    };

    const handleIntegrations = () => {
        setIsMenuOpen(false);
        navigate('/integrations');
    };

    const handleLogout = () => {
        setIsMenuOpen(false);
        authStore.logout();
        navigate('/login');
    };

    return (
        <header className="bg-white shadow">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between h-16">
                    <div className="flex">
                        <div className="flex-shrink-0 flex items-center">
                            <h1 
                                className="text-xl font-bold text-gray-900 cursor-pointer"
                                onClick={() => navigate('/')}
                            >
                                Mango Blog
                            </h1>
                        </div>
                    </div>
                    <div className="flex items-center">
                        <div className="ml-3 relative">
                            <div>
                                <button
                                    type="button"
                                    className="bg-white rounded-full flex text-sm focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                                    onClick={() => setIsMenuOpen(!isMenuOpen)}
                                >
                                    <span className="sr-only">Open user menu</span>
                                    {user?.avatarId ? (
                                        <img 
                                            src={mediaService.makeImageUrl(user.avatarId)} 
                                            alt={user?.displayedName || 'User'} 
                                            className="h-8 w-8 rounded-full" 
                                        />
                                    ) : (
                                        <div className="h-8 w-8 rounded-full bg-green-600 flex items-center justify-center text-white">
                                            {user?.displayedName?.[0]?.toUpperCase() || 'U'}
                                        </div>
                                    )}
                                </button>
                            </div>
                            {isMenuOpen && (
                                <div className="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-white ring-1 ring-black ring-opacity-5 focus:outline-none">
                                    <button
                                        onClick={handleProfile}
                                        className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    >
                                        Profile
                                    </button>
                                    <button
                                        onClick={handleIntegrations}
                                        className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    >
                                        Integrations
                                    </button>
                                    <button
                                        onClick={handleLogout}
                                        className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    >
                                        Logout
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </header>
    );
});

export default Header; 