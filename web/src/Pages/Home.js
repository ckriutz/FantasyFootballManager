import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Navbar from '../Components/Navbar';
import { useAuth0 } from "@auth0/auth0-react";

export default function Home() {
    const { isLoading, user, isAuthenticated, loginWithRedirect } = useAuth0();
    const [players, setPlayers] = useState([]);
    const [playersLoading, setPlayersLoading] = useState(false);
    const [recommendations, setRecommendations] = useState([]);
    const [recommendationsLoading, setRecommendationsLoading] = useState(false);
    const [recommendationsError, setRecommendationsError] = useState('');

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

        // Load cached AI recommendations
        const cachedRecommendations = sessionStorage.getItem('aiRecommendations');
        if (cachedRecommendations) {
            try {
                const parsed = JSON.parse(cachedRecommendations);
                // Check if it's for the same user and not too old (1 hour)
                const oneHour = 60 * 60 * 1000;
                if (parsed.userId === user.sub && (Date.now() - parsed.timestamp) < oneHour) {
                    setRecommendations(parsed.data);
                } else {
                    // Clear old/invalid cache
                    sessionStorage.removeItem('aiRecommendations');
                }
            } catch (error) {
                console.error("Error loading cached recommendations:", error);
                sessionStorage.removeItem('aiRecommendations');
            }
        }
    }, [isAuthenticated, user]);

    const fetchAiRecommendations = async () => {
        if (!isAuthenticated || !user) return;
        
        setRecommendationsLoading(true);
        setRecommendationsError('');
        
        try {
            const apiUrl = process.env.REACT_APP_API_URL || 'https://ffootball-api.caseyk.dev';
            const response = await fetch(`${apiUrl}/ai/draft-reccomendations/${user.sub}`);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            
            if (data.success) {
                const recommendations = data.recommendations || [];
                setRecommendations(recommendations);
                // Store in session storage with timestamp
                sessionStorage.setItem('aiRecommendations', JSON.stringify({
                    data: recommendations,
                    timestamp: Date.now(),
                    userId: user.sub
                }));
            } else {
                setRecommendationsError(data.errorMessage || 'Failed to get AI recommendations');
            }
        } catch (error) {
            console.error("Error fetching AI recommendations:", error);
            setRecommendationsError('Unable to fetch AI recommendations. Please try again.');
        } finally {
            setRecommendationsLoading(false);
        }
    };

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
                            <button className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded cursor-pointer">
                                About
                            </button>
                            <button 
                                className="bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded cursor-pointer"
                                onClick={() => loginWithRedirect()}
                            >
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

                    {/* Two-column layout for cards - responsive */}
                    <div className="flex flex-col lg:flex-row space-y-4 lg:space-y-0 lg:space-x-4 max-w-7xl mx-auto">
                        {/* Players Card */}
                        <div className="w-full lg:w-1/2 bg-gray-700 rounded-lg p-4 lg:p-6">
                            <h2 className="text-xl lg:text-2xl font-bold text-white mb-4">Your Players</h2>
                            {players.length > 0 ? (
                                <div className="space-y-2 max-h-96 overflow-y-auto">
                                    {players.map((player, idx) => (
                                        <Link key={idx} to={`/player/${player.playerId}`} className="block bg-gray-600 rounded p-2.5 hover:bg-gray-500 transition-colors cursor-pointer">
                                            <div className="flex justify-between items-center">
                                                <div className="flex-1 min-w-0">
                                                    <h3 className="text-white font-medium text-sm truncate">{player.position || 'N/A'} • {player.name || 'Unknown Player'}</h3>
                                                    <p className="text-gray-300 text-xs truncate">{player.teamName || 'N/A'}</p>
                                                </div>
                                                <div className="text-right flex-shrink-0 ml-3">
                                                    <p className="text-gray-300 font-medium text-xs">Rank {player.rankEcr || 'N/A'} | Proj. {player.projectedFantasyPoints || 'N/A'}</p>
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

                        {/* AI Recommendations Card */}
                        <div className="w-full lg:w-1/2 bg-gray-700 rounded-lg p-4 lg:p-6">
                            <h2 className="text-xl lg:text-2xl font-bold text-white mb-4">AI Draft Recommendations</h2>
                            
                            {recommendations.length === 0 && !recommendationsError && (
                                <div className="flex flex-col items-center justify-center h-64">
                                    <div className="text-center mb-4">
                                        <p className="text-gray-400 text-lg mb-2">Get AI-powered draft recommendations</p>
                                        <p className="text-gray-500 text-sm">Discover top players that could strengthen your team</p>
                                    </div>
                                    <button 
                                        onClick={fetchAiRecommendations}
                                        disabled={recommendationsLoading}
                                        className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-2 px-4 rounded flex items-center space-x-2 cursor-pointer disabled:cursor-not-allowed"
                                    >
                                        {recommendationsLoading ? (
                                            <>
                                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                                                <span>Getting recommendations...</span>
                                            </>
                                        ) : (
                                            <span>Get AI Recommendations</span>
                                        )}
                                    </button>
                                </div>
                            )}

                            {recommendationsError && (
                                <div className="flex flex-col items-center justify-center h-64">
                                    <div className="text-center">
                                        <p className="text-red-400 text-lg mb-2">Oops! Something went wrong</p>
                                        <p className="text-gray-500 text-sm mb-4">{recommendationsError}</p>
                                        <button 
                                            onClick={fetchAiRecommendations}
                                            disabled={recommendationsLoading}
                                            className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-2 px-4 rounded cursor-pointer disabled:cursor-not-allowed"
                                        >
                                            Try Again
                                        </button>
                                    </div>
                                </div>
                            )}

                            {recommendations.length > 0 && (
                                <div>
                                    <div className="flex justify-between items-center mb-4">
                                        <p className="text-gray-300 text-sm">Based on your current roster</p>
                                        <button 
                                            onClick={fetchAiRecommendations}
                                            disabled={recommendationsLoading}
                                            className="bg-gray-600 hover:bg-gray-500 disabled:bg-gray-400 text-white text-sm py-1 px-3 rounded cursor-pointer disabled:cursor-not-allowed"
                                        >
                                            Refresh
                                        </button>
                                    </div>
                                    <div className="space-y-2 max-h-96 overflow-y-auto">
                                        {recommendations.map((rec, idx) => (
                                            <Link key={idx} to={`/player/${rec.playerId}`} className="block bg-gray-600 rounded p-3 hover:bg-gray-500 transition-colors cursor-pointer">
                                                <div className="space-y-2">
                                                    <div className="flex justify-between items-start">
                                                        <div className="flex-1 min-w-0">
                                                            <h3 className="text-white font-medium text-sm">{rec.playerName || 'Unknown Player'}</h3>
                                                            <p className="text-blue-400 text-xs">Risk Level: {rec.riskLevel || 'Unknown'}</p>
                                                        </div>
                                                        <div className="text-right flex-shrink-0 ml-3">
                                                            <span className="inline-block bg-blue-600 text-white text-xs px-2 py-1 rounded">
                                                                AI Pick #{idx + 1}
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div className="text-gray-300 text-xs">
                                                        <p className="mb-1"><strong>Why:</strong> {rec.reason}</p>
                                                        {rec.matchupStrength && (
                                                            <p><strong>Matchups:</strong> {rec.matchupStrength}</p>
                                                        )}
                                                    </div>
                                                </div>
                                            </Link>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}