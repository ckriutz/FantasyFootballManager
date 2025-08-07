import { FaGithub, FaHeartbeat, FaRocket } from 'react-icons/fa';
import LoginButton from '../Components/LoginButton'
import { useAuth0 } from "@auth0/auth0-react";
import UserMenu from './UserMenu';
import { Link } from 'react-router-dom';


export default function Navbar() {
    const { isLoading, user, isAuthenticated } = useAuth0();
    return (
        <nav className="bg-gray-800 text-white shadow-lg border-b border-gray-700">
            <div className="container mx-auto px-6 py-4 flex justify-between items-center">
                
                {/* Logo and Title */}
                <div className="flex items-center space-x-3">
                    <Link to="/" className="group">
                        <h1 className="text-2xl font-bold group-hover:text-blue-400 transition-colors duration-200">
                            <span className="text-2xl mr-2">🏈 🔥</span> 
                            Fantasy Football Manager
                        </h1>
                    </Link>
                </div>

                {/* Action Buttons */}
                <div className="flex items-center space-x-4">
                    {/* Status Button */}
                    <div className="flex items-center space-x-2 bg-gray-600 text-white font-medium py-2 px-4 rounded transition-colors duration-200 shadow-md hover:shadow-lg">
                        <FaRocket className="text-success" />
                        <span>v 15</span>
                    </div>
                    <Link to="/status" className="flex items-center space-x-2 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded transition-colors duration-200 shadow-md hover:shadow-lg">
                        <FaHeartbeat className="text-red-400" />
                        <span>Status</span>
                    </Link>

                    {/* GitHub Button */}
                    <a href="https://github.com/ckriutz/FantasyFootballManager" target="_blank" rel="noopener noreferrer" className="flex items-center space-x-2 bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded transition-colors duration-200 shadow-md hover:shadow-lg">
                        <FaGithub />
                        <span>GitHub</span>
                    </a>

                    { isAuthenticated && !isLoading ? <UserMenu user={user} /> : <LoginButton /> }
                </div>
            </div>
        </nav>
    );
}