import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar';
import { Link } from 'react-router-dom';
import Breadcrumb from '../Components/Breadcrumb';
import { useAuth0 } from "@auth0/auth0-react";
import { TiThumbsUp, TiThumbsDown } from "react-icons/ti";

export default function Players() {
    const [players, setPlayers] = useState([]);
    const [filteredPlayers, setFilteredPlayers] = useState([]);
    const [selectedPosition, setSelectedPosition] = useState('All');
    const { isLoading, user, isAuthenticated } = useAuth0();

    // Use environment variable or relative URL for API endpoint
    const apiUrl = process.env.REACT_APP_API_URL || 'https://ffootball-api.caseyk.dev';

    useEffect(() => {
        console.log('User is authenticated:', user);
        if (isAuthenticated) {
            console.log(`${apiUrl}/players/simple/${user.sub}`);
            fetch(`${apiUrl}/players/simple/${user.sub}`)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error('Failed to fetch data');
                    }
                    return response.json();
                })
                .then((data) => {
                    setPlayers(data); // Set the players array from the JSON file
                    setFilteredPlayers(data); // Initialize filtered players
                })
                .catch((error) => console.error('Error fetching player data:', error));
        }
    }, [isAuthenticated, user]);

    const handleFilterChange = (event) => {
        const position = event.target.value;
        setSelectedPosition(position);

        if (position === 'All') {
            setFilteredPlayers(players);
        } else {
            setFilteredPlayers(players.filter(player => 
                player && player.position && player.position.includes(position)
            ));
        }
    };

    const handleDrafted = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }
        console.log(`${apiUrl}/players/${playerId}/draft/${user.sub}`);
        fetch(`${apiUrl}/players/${playerId}/draft/${user.sub}`, {

            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: user.sub, // Use user.sub for the Auth0 user ID
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    console.error('Failed to mark player as mine:', response.statusText);
                    throw new Error('Failed to mark player as mine');

                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as mine:', data);

                // Update the players array to set isDraftedOnMyTeam to true
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnMyTeam: true }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnMyTeam: true }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error marking player as mine:', error));
    };

    const handleAssigned = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }

        fetch(`${apiUrl}/players/${playerId}/assign/${user.sub}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: user.sub, // Use user.sub for the Auth0 user ID
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    console.error('Failed to mark player as taken:', response.statusText);
                    throw new Error('Failed to mark player as taken');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as taken:', data);

                // Update the players array to set isDraftedOnOtherTeam to true
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnOtherTeam: true }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnOtherTeam: true }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error marking player as taken:', error));
    };

    const handleResetStatus = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }

        fetch(`${apiUrl}/players/${playerId}/reset/${user.sub}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: user.sub,
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    console.error('Failed to reset player status:', response.statusText);
                    throw new Error('Failed to reset player status');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player status reset:', data);

                // Update the players array to reset isDraftedOnMyTeam and isDraftedOnOtherTeam
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnMyTeam: false, isDraftedOnOtherTeam: false }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isDraftedOnMyTeam: false, isDraftedOnOtherTeam: false }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error resetting player status:', error));
    };

    const handleThumbsUp = (playerId) => {
        console.log(`Thumbs up for player ${playerId}`);
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }
        console.log(`${apiUrl}/players/${playerId}/thumbsup/${user.sub}`);
        fetch(`${apiUrl}/players/${playerId}/thumbsup/${user.sub}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: user.name,
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    console.error('Failed to thumbs-up player:', response.statusText);
                    throw new Error('Failed to thumbs-up player');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player thumbs-up:', data);

                // Update the players array to reset isThumbsUp and isThumbsDown
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isThumbsUp: data.isThumbsUp, isThumbsDown: data.isThumbsDown  }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isThumbsUp: data.isThumbsUp, isThumbsDown: data.isThumbsDown  }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error thumbs-upping player:', error));
    };

    const handleThumbsDown = (playerId) => {
        console.log(`Thumbs down for player ${playerId}`);
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }

        fetch(`${apiUrl}/players/${playerId}/thumbsdown/${user.sub}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: user.name,
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    console.error('Failed to thumbs-down player:', response.statusText);
                    throw new Error('Failed to thumbs-down player');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player thumbs-down:', data);

                // Update the players array to reset isThumbsUp and isThumbsDown\
                // Use thre values from the response

                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isThumbsUp: data.isThumbsUp, isThumbsDown: data.isThumbsDown }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.sleeperId === playerId
                            ? { ...player, isThumbsUp: data.isThumbsUp, isThumbsDown: data.isThumbsDown  }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error thumbs-upping player:', error));
    };

    if (!isAuthenticated) {
        return (
            <div className="page">
                <Navbar />
                <div className="flex items-center justify-center h-screen bg-gray-800">
                    <div className="text-center text-white space-y-4">
                        <h1 className="text-4xl font-bold">Access Denied</h1>
                        <p className="text-lg text-gray-300">
                            You must be logged in to view this page.
                        </p>
                        <div className="space-x-4">
                            <Link to="/" className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded">
                                Go to Home
                            </Link>
                            <Link to="/login" className="bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded">
                                Login
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    if (players.length === 0 || isLoading) {
        return (
            <div className="page">
                <Navbar />
                <div className="flex items-center justify-center h-screen bg-gray-800">
                    <div className="text-center space-y-4">
                        <div className="flex justify-center">
                            <div className="w-16 h-16 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                        </div>
                        <p className="text-white font-medium">Loading players...</p>
                    </div>
                </div>
            </div>
        );
    }
    else {
        return (
            <div className="page">
                <Navbar />
                <Breadcrumb
                    items={[
                        { label: 'Home', href: '/' },
                        { label: 'Players', href: '/players' },
                    ]}
                />
                <div className="min-h-screen bg-gray-800 p-6">
                    <div className="container mx-auto">
                        <div className="text-center text-white space-y-4 mb-8">
                            <h1 className="text-4xl font-bold">Players</h1>
                            <p className="text-lg text-gray-300">
                                Browse and manage your fantasy football players.
                            </p>
                        </div>

                        {/* Filter Section */}
                        <div className="bg-gray-700 rounded-lg p-6 mb-6">
                            <div className="mb-4">
                                <label htmlFor="position-filter" className="block text-white font-medium mb-2">Filter by Position:</label>
                                <select
                                    id="position-filter"
                                    value={selectedPosition}
                                    onChange={handleFilterChange}
                                    className="bg-gray-600 border border-gray-500 rounded px-4 py-2 text-white w-full md:w-1/3 focus:outline-none focus:ring-2 focus:ring-blue-600"
                                >
                                    <option value="All">All</option>
                                    <option value="QB">Quarterback (QB)</option>
                                    <option value="RB">Running Back (RB)</option>
                                    <option value="WR">Wide Receiver (WR)</option>
                                    <option value="TE">Tight End (TE)</option>
                                    <option value="K">Kicker (K)</option>
                                    <option value="DEF">Defense (DEF)</option>
                                </select>
                            </div>
                        </div>

                        {/* Players Table */}
                        <div className="bg-white rounded-lg shadow-lg overflow-hidden border border-gray-200">
                            <div className="overflow-x-auto">
                                <table className="min-w-full">
                                    <thead>
                                        <tr className="bg-gray-50 border-b border-gray-200">
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Position</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Team</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rank</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Proj. Points</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Bye</th>
                                            <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                                        </tr>
                                    </thead>
                                    <tbody className="bg-white divide-y divide-gray-200">
                                        {filteredPlayers.map((player, index) => (
                                            <tr
                                                key={player.sleeperId}
                                                className={`${
                                                    player.isDraftedOnMyTeam
                                                        ? 'bg-green-50 border-l-4 border-green-400' // Light green for players drafted on my team
                                                        : player.isDraftedOnOtherTeam
                                                        ? 'bg-red-50 border-l-4 border-red-400' // Light red for players drafted on other teams
                                                        : 'bg-white hover:bg-gray-50' // Clean white background
                                                } transition-colors`}
                                            >
                                                <td className="px-3 py-2 whitespace-nowrap">
                                                    <div className="flex space-x-1">
                                                        {/* Show Thumbs Up unless IsThumbsDown is true */}
                                                        {!player.isThumbsDown && (
                                                            <button
                                                                className="p-1 text-green-600 hover:text-green-700 hover:bg-green-100 rounded transition-colors cursor-pointer"
                                                                title="Thumbs Up"
                                                                onClick={() => handleThumbsUp(player.sleeperId)}
                                                            >
                                                                <span className="sr-only">Thumbs Up</span>
                                                                <TiThumbsUp className="text-lg" />
                                                            </button>
                                                        )}
                                                        {/* Show Thumbs Down unless IsThumbsUp is true */}
                                                        {!player.isThumbsUp && (
                                                            <button
                                                                className="p-1 text-red-600 hover:text-red-700 hover:bg-red-100 rounded transition-colors cursor-pointer"
                                                                title="Thumbs Down"
                                                                onClick={() => handleThumbsDown(player.sleeperId)}
                                                            >
                                                                <span className="sr-only">Thumbs Down</span>
                                                                <TiThumbsDown className="text-lg" />
                                                            </button>
                                                        )}
                                                    </div>
                                                </td>
                                                <td className="px-3 py-2 whitespace-nowrap">
                                                    <Link to={`/player/${player.sleeperId}`} className="text-blue-600 hover:text-blue-800 font-medium transition-colors">
                                                        {player.name}
                                                    </Link>
                                                </td>
                                                <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-600">{player.position}</td>
                                                <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-600">{player.team?.abbreviation || 'N/A'}</td>
                                                <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-600">{player.rankEcr || 'N/A'}</td>
                                                <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-600">{player.projPoints || 'N/A'}</td>
                                                <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-600">{player.byeWeek || 'N/A'}</td>
                                                <td className="px-3 py-2 whitespace-nowrap">
                                                    <div className="flex space-x-1">
                                                        {((player.isDraftedOnMyTeam || player.isDraftedOnOtherTeam) && (
                                                            <button
                                                                className="inline-flex items-center px-2 py-1 border border-green-300 text-xs font-medium rounded text-green-700 bg-white hover:bg-green-50 transition-colors cursor-pointer"
                                                                title="Reset player status"
                                                                onClick={() => handleResetStatus(player.sleeperId)}
                                                            >
                                                                Reset
                                                            </button>
                                                        )) || (
                                                                <>
                                                                    <button
                                                                        className="inline-flex items-center px-2 py-1 border border-blue-300 text-xs font-medium rounded text-blue-700 bg-white hover:bg-blue-50 transition-colors cursor-pointer"
                                                                        title="Mark as drafted by you"
                                                                        onClick={() => handleDrafted(player.sleeperId)}
                                                                    >
                                                                        Draft
                                                                    </button>
                                                                    <button
                                                                        className="inline-flex items-center px-2 py-1 border border-gray-300 text-xs font-medium rounded text-gray-700 bg-white hover:bg-gray-50 transition-colors cursor-pointer"
                                                                        title="Mark as drafted by someone else"
                                                                        onClick={() => handleAssigned(player.sleeperId)}
                                                                    >
                                                                        Assigned
                                                                    </button>
                                                                </>
                                                            )}
                                                    </div>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}