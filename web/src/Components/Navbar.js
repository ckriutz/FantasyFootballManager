import { FaGithub, FaHeartbeat, FaRocket } from 'react-icons/fa';
import LoginButton from '../Components/LoginButton'
import { useAuth0 } from "@auth0/auth0-react";
import UserMenu from './UserMenu';
import { Link } from 'react-router-dom';


export default function Navbar() {
    const { isLoading, user, isAuthenticated } = useAuth0();
    return (
        <nav className="bg-gray-800 text-white shadow-lg border-b border-gray-700">
            <div className="container mx-auto px-4 py-4">
                <div className="flex flex-wrap justify-between items-center gap-4">
                    {/* Logo and Title */}
                    <div className="flex items-center space-x-3 flex-shrink-0">
                        <Link to="/" className="group">
                            <h1 className="text-xl md:text-2xl font-bold group-hover:text-blue-400 transition-colors duration-200">
                                <span className="text-xl md:text-2xl mr-2">🏈 🔥</span> 
                                <span className="hidden sm:inline">Fantasy Football Manager</span>
                                <span className="sm:hidden">FF Manager</span>
                            </h1>
                        </Link>
                    </div>

                    {/* Action Buttons */}
                    <div className="flex flex-wrap items-center gap-2 sm:gap-4">
                        {/* Version Button */}
                        <div className="flex items-center space-x-2 bg-gray-600 text-white font-medium py-2 px-3 sm:px-4 rounded transition-colors duration-200 shadow-md hover:shadow-lg">
                            <FaRocket className="text-success" />
                            <span className="text-sm sm:text-base">v 16.0</span>
                        </div>
                        {/* Status Button */}
                        <Link to="/status" className="flex items-center space-x-2 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-3 sm:px-4 rounded transition-colors duration-200 shadow-md hover:shadow-lg">
                            <FaHeartbeat className="text-red-400" />
                            <span className="text-sm sm:text-base">Status</span>
                        </Link>
                        {/* Login/User Menu */}
                        <div className="flex-shrink-0">
                            { isAuthenticated && !isLoading ? <UserMenu user={user} /> : <LoginButton /> }
                        </div>
                    </div>
                </div>
            </div>
        </nav>
    );
}