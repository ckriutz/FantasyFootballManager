import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Navbar from '../Components/Navbar';
import { useAuth0 } from "@auth0/auth0-react";

export default function Home() {
    const { isLoading, user, isAuthenticated } = useAuth0();
    const [players, setPlayers] = useState([]);
    const [playersLoading, setPlayersLoading] = useState(false);

    useEffect(() => {
        if (isAuthenticated && user) {
            console.log("Loading user data...");
            setPlayersLoading(true);
            console.log("Fetching players for user:", user.name);
            
            // Use environment variable or relative URL for API endpoint
            const apiUrl = process.env.REACT_APP_API_URL || 'https://ffootball-api.caseyk.dev';
            
            fetch(`${apiUrl}/players/drafted/${user.sub}`)
                .then(res => {
                    if (!res.ok) {
                        throw new Error(`HTTP error! status: ${res.status}`);
                    }
                    return res.json();
                })
                .then(data => {
                    console.log("Players fetched successfully:", data);
                    setPlayers(Array.isArray(data) ? data : []);
                })
                .catch(error => {
                    console.error("Error fetching players:", error);
                    setPlayers([]);
                })
                .finally(() => setPlayersLoading(false));
        }
    }, [isAuthenticated, user]);

    // This is only when things are loading, I guess.
    if (isLoading) {
        return (
            <div className="page">
                <Navbar />
                <div className="flex items-center justify-center h-screen bg-gray-800">
                    <div className="text-center space-y-4">
                        <div className="flex justify-center">
                            <div className="w-16 h-16 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                        </div>
                        <p className="text-white font-medium">Loading, please wait...</p>
                        {/* Show loading players if authenticated */}
                        {isAuthenticated && playersLoading && (
                            <p className="text-blue-400 font-medium">Fetching your players...</p>
                        )}
                    </div>
                </div>
            </div>
        );
    }

    // This is when the user is not authenticated, but the page is loaded.
    if (!isAuthenticated) {
        return (
            <div className="page">
                <Navbar />
                <div className="flex items-center justify-center h-screen bg-gray-800">
                    <div className="text-center text-white space-y-4">
                        <h1 className="text-4xl font-bold">Welcome to Fantasy Firewall</h1>
                        <p className="text-lg text-gray-300">
                            Manage your fantasy football team with ease and stay ahead of the competition byt looking at things a bit differently, and some AI in there to help.
                        </p>
                        <div className="space-x-4">
                            <button className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded">
                                About
                            </button>
                            <button className="bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded">
                                Login
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    // This is when the user is authenticated, and the page is loaded.
    if (isAuthenticated && !isLoading) {
        console.log("players", players);
        return (
            <div className="page">
                <Navbar />
                <div className="min-h-screen bg-gray-800 p-6">
                    <div className="text-center text-white space-y-4 mb-8">
                        <h1 className="text-4xl font-bold">Welcome, {user.name}!</h1>
                        <p className="text-lg text-gray-300">
                            Manage your fantasy football team with ease and stay ahead of the competition.
                        </p>
                        <div className="space-x-4">
                            <Link to="/players" className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded">
                                View Players
                            </Link>
                            <Link to="/profile" className="bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded">
                                View Profile
                            </Link>
                        </div>
                    </div>

                    {/* Two-column layout for cards */}
                    <div className="flex space-x-4 max-w-7xl mx-auto">
                        {/* Players Card */}
                        <div className="w-1/2 bg-gray-700 rounded-lg p-6">
                            <h2 className="text-2xl font-bold text-white mb-4">Your Players</h2>
                            {players.length > 0 ? (
                                <div className="space-y-2 max-h-96 overflow-y-auto">
                                    {players.map((player, idx) => (
                                        <Link key={idx} to={`/player/${player.sleeperId}`} className="block bg-gray-600 rounded p-2.5 hover:bg-gray-500 transition-colors cursor-pointer">
                                            <div className="flex justify-between items-center">
                                                <div className="flex-1 min-w-0">
                                                    <h3 className="text-white font-medium text-sm truncate">{player.position || 'N/A'} • {player.name || 'Unknown Player'}</h3>
                                                    <p className="text-gray-300 text-xs truncate">{player.team.name || 'N/A'}</p>
                                                </div>
                                                <div className="text-right flex-shrink-0 ml-3">
                                                    <p className="text-gray-300 font-medium text-xs">Rank {player.rankEcr || 'N/A'} | Proj. {player.projPoints || 'N/A'}</p>
                                                    <p className="text-gray-300 text-xs">Bye {player.byeWeek || 'N/A'}</p>
                                                </div>
                                            </div>
                                        </Link>
                                    ))}
                                </div>
                            ) : (
                                <div className="text-center py-8">
                                    <p className="text-gray-400 text-lg">No players found</p>
                                    <p className="text-gray-500 text-sm">Add some players to get started!</p>
                                </div>
                            )}
                        </div>

                        {/* TBD Card */}
                        <div className="w-1/2 bg-gray-700 rounded-lg p-6">
                            <h2 className="text-2xl font-bold text-white mb-4">Coming Soon</h2>
                            <div className="flex items-center justify-center h-64">
                                <div className="text-center">
                                    <p className="text-gray-400 text-3xl font-bold mb-2">TBD</p>
                                    <p className="text-gray-500">More features coming soon!</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}