import React from 'react';
import { FaGithub, FaSignInAlt, FaHeartbeat } from 'react-icons/fa';

export default function Navbar() {
    return (
        <nav className="bg-gray-800 text-white shadow-md">
            <div className="container mx-auto px-4 py-3 flex justify-between items-center">
                {/* Logo and Title */}
                <div className="flex items-center space-x-2">
                    <span className="text-2xl">🏈</span>
                    <h1 className="text-xl font-bold">Fantasy Football Manager</h1>
                </div>

                {/* Buttons */}
                <div className="flex space-x-4">
                    {/* Status Button */}
                    <button className="flex items-center space-x-2 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded">
                        <FaHeartbeat />
                        <span>Status</span>
                    </button>

                    {/* GitHub Button */}
                    <button
                        className="flex items-center space-x-2 bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded"
                        onClick={() => window.open('https://github.com', '_blank')}
                    >
                        <FaGithub />
                        <span>GitHub</span>
                    </button>

                    {/* Login Button */}
                    <button className="flex items-center space-x-2 bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded">
                        <FaSignInAlt />
                        <span>Login</span>
                    </button>
                </div>
            </div>
        </nav>
    );
}