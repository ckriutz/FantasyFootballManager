import { FaGithub, FaHeartbeat } from 'react-icons/fa';
import LoginButton from '../Components/LoginButton'
import { useAuth0 } from "@auth0/auth0-react";
import UserMenu from './UserMenu';
import { Link } from 'react-router-dom';

export default function Navbar() {
    const { isLoading, user, isAuthenticated } = useAuth0();
    return (
        <nav className="bg-gray-800 text-white shadow-md">
            <div className="container mx-auto px-4 py-3 flex justify-between items-center">
                
                {/* Logo and Title */}
                <div className="flex items-center space-x-2">
                    <Link to="/">
                        <h1 className="text-xl font-bold"><span className="text-xl">🏈 🔥 </span> Fantasy Firewall</h1>
                    </Link>
                </div>

                {/* Status Button */}
                <div className="flex space-x-4">
                    {/* Status Button */}
                    <Link to="/status" className="flex items-center space-x-2 bg-stone-50 hover:bg-stone-200 text-black font-medium py-2 px-4 rounded">
                        <FaHeartbeat className="text-red-600" />
                        <span>Status</span>
                    </Link>

                    {/* GitHub Button */}
                    <Link to="github.com" className="flex items-center space-x-2 bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded">
                        <FaGithub />
                        <span>GitHub</span>
                    </Link>

                    { isAuthenticated && !isLoading ? <UserMenu user={user} /> : <LoginButton /> }
                </div>
            </div>
        </nav>
    );
}