import React from 'react';

const AuthLayout = ({ children }) => {
    return (
        <div className="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
            {children}
        </div>
    );
};

export default AuthLayout; 