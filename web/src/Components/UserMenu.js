import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { FaUserCircle } from 'react-icons/fa';
import { useAuth0 } from "@auth0/auth0-react";

const UserMenu = ({ user }) => {
    const [isOpen, setIsOpen] = useState(false);
    const { logout } = useAuth0();
    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };

    return (
        <div className="relative">
            {/* Avatar Button */}
            <button onClick={toggleMenu} className="flex items-center space-x-2 bg-gray-800 text-white font-medium rounded p-2 hover:bg-gray-700">
                {user.picture ? (
                    <img src={user.picture} alt={user.name} className="w-8 h-8 rounded-full" />
                ) : (
                    <FaUserCircle className="w-8 h-8" />
                )}
            </button>

            {/* Dropdown Menu */}
            {isOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-white rounded shadow-lg">
                    <ul className="py-2">
                        <li>
                            <button className="block w-full text-left px-4 py-2 text-gray-800 hover:bg-gray-100 font-medium">
                                <Link to={`/profile/`}>Profile</Link>
                            </button>
                        </li>
                        <li>
                            <hr className="border-gray-200 my-2" />
                        </li>
                        <li>
                            <button className="block w-full text-left px-4 py-2 text-gray-800 hover:bg-gray-100 font-medium" onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}>
                                Log Out
                            </button>
                        </li>
                    </ul>
                </div>
            )}
        </div>
    );
};

export default UserMenu;