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

    useEffect(() => {
        console.log('User is authenticated:', user);
        if (isAuthenticated) {
            console.log('http://127.0.0.1:8000/user-players/' + user.name);
            fetch('http://127.0.0.1:8000/user-players/' + user.name)
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
    }, [isAuthenticated]);

    const handleFilterChange = (event) => {
        const position = event.target.value;
        setSelectedPosition(position);

        if (position === 'All') {
            setFilteredPlayers(players);
        } else {
            setFilteredPlayers(players.filter(player => player.PlayerPositionId.includes(position)));
        }
    };

    const handleMarkAsMine = (playerId) => {
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }
        console.log(`http://127.0.0.1:8000/player/claim/${playerId}?user=${user.name}`);
        fetch(`http://127.0.0.1:8000/player/claim/${playerId}?user=${user.name}`, {

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
                    console.error('Failed to mark player as mine:', response.statusText);
                    throw new Error('Failed to mark player as mine');

                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as mine:', data);

                // Update the players array to set IsDraftedOnMyTeam to true
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnMyTeam: true }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnMyTeam: true }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error marking player as mine:', error));
    };

    const handleMarkAsTaken = (playerId) => {
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }

        fetch(`http://127.0.0.1:8000/player/notclaimed/${playerId}?user=${user.name}`, {
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
                    console.error('Failed to mark player as taken:', response.statusText);
                    throw new Error('Failed to mark player as taken');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as taken:', data);

                // Update the players array to set IsDraftedOnOtherTeam to true
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnOtherTeam: true }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnOtherTeam: true }
                            : player
                    )
                );
            })
            .catch((error) => console.error('Error marking player as taken:', error));
    };

    const handleResetStatus = (playerId) => {
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }

        fetch(`http://127.0.0.1:8000/player/resetstatus/${playerId}?user=${user.name}`, {
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
                    console.error('Failed to reset player status:', response.statusText);
                    throw new Error('Failed to reset player status');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player status reset:', data);

                // Update the players array to reset IsDraftedOnMyTeam and IsDraftedOnOtherTeam
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnMyTeam: false, IsDraftedOnOtherTeam: false }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsDraftedOnMyTeam: false, IsDraftedOnOtherTeam: false }
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

        fetch(`http://127.0.0.1:8000/player/thumbsup/${playerId}?user=${user.name}`, {
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

                // Update the players array to reset IsThumbsUp and IsThumbsDown
                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsThumbsUp: data.IsThumbsUp, IsThumbsDown: data.IsThumbsDown  }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsThumbsUp: data.IsThumbsUp, IsThumbsDown: data.IsThumbsDown  }
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

        fetch(`http://127.0.0.1:8000/player/thumbsdown/${playerId}?user=${user.name}`, {
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

                // Update the players array to reset IsThumbsUp and IsThumbsDown\
                // Use thre values from the response

                setPlayers((prevPlayers) =>
                    prevPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsThumbsUp: data.IsThumbsUp, IsThumbsDown: data.IsThumbsDown }
                            : player
                    )
                );

                // Optionally update filteredPlayers if needed
                setFilteredPlayers((prevFilteredPlayers) =>
                    prevFilteredPlayers.map((player) =>
                        player.PlayerId === playerId
                            ? { ...player, IsThumbsUp: data.IsThumbsUp, IsThumbsDown: data.IsThumbsDown  }
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
            <div className="flex items-center justify-center h-screen">
                <p className="text-gray-500">Loading...</p>
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
                <div className="container mx-auto px-4 py-8">
                    <h1 className="text-2xl font-bold text-gray-800 mb-4">Players</h1>

                    {/* Filter Dropdown */}
                    <div className="mb-4">
                        <label htmlFor="position-filter" className="block text-gray-700 font-medium mb-2">Filter by Position:</label>
                        <select
                            id="position-filter"
                            value={selectedPosition}
                            onChange={handleFilterChange}
                            className="border border-gray-300 rounded px-4 py-2 w-full md:w-1/3"
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

                    <div className="overflow-x-auto">
                        <table className="min-w-full border-collapse border border-gray-300">
                            <thead>
                                <tr className="bg-gray-800 text-white">
                                    <th></th>
                                    <th className="border border-gray-300 px-4 py-2 text-left">Name</th>
                                    <th className="border border-gray-300 px-4 py-2 text-left">Position</th>
                                    <th className="border border-gray-300 px-4 py-2 text-left">Team</th>
                                    <th className="border border-gray-300 px-4 py-2 text-left">Rank</th>
                                    <th className="border border-gray-300 px-4 py-2 text-left">Bye</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredPlayers.map((player) => (
                                    <tr
                                        key={player.PlayerId}
                                        className={`${player.IsDraftedOnMyTeam
                                                ? 'bg-green-100' // Light green for players drafted on my team
                                                : player.IsDraftedOnOtherTeam
                                                    ? 'bg-red-100' // Light gray for players drafted on other teams
                                                    : 'bg-white' // Default alternating row colors
                                            }`}
                                    >
                                        <td className="border border-gray-300 px-4 py-2">
    <div className="flex space-x-2">
        {/* Show Thumbs Up unless IsThumbsDown is true */}
        {!player.IsThumbsDown && (
            <button
                className="text-green-600 hover:text-green-700"
                title="Thumbs Up"
                onClick={() => handleThumbsUp(player.PlayerId)}
            >
                <span className="sr-only">Thumbs Up</span>
                <TiThumbsUp className="text-xl" />
            </button>
        )}
        {/* Show Thumbs Down unless IsThumbsUp is true */}
        {!player.IsThumbsUp && (
            <button
                className="text-red-600 hover:text-red-700"
                title="Thumbs Down"
                onClick={() => handleThumbsDown(player.PlayerId)}
            >
                <span className="sr-only">Thumbs Down</span>
                <TiThumbsDown className="text-xl" />
            </button>
        )}
    </div>
</td>
                                        <td className="border border-gray-300 px-4 py-2 text-blue-600 hover:text-blue-700">
                                            <Link to={`/player/${player.PlayerId}`}>{player.PlayerName}</Link>
                                        </td>
                                        <td className="border border-gray-300 px-4 py-2">{player.PlayerPositionId}</td>
                                        <td className="border border-gray-300 px-4 py-2">{player.Name}</td>
                                        <td className="border border-gray-300 px-4 py-2">{player.RankEcr}</td>
                                        <td className="border border-gray-300 px-4 py-2">{player.PlayerByeWeek}</td>
                                        <td className="border border-gray-300 px-4 py-2">
                                            <div className="flex space-x-2">
                                                {((player.IsDraftedOnMyTeam || player.IsDraftedOnOtherTeam) && (
                                                    <button
                                                        className="bg-green-600 hover:bg-green-700 text-white font-medium rounded px-3 py-1"
                                                        title="Reset player status"
                                                        onClick={() => handleResetStatus(player.PlayerId)}
                                                    >
                                                        Reset
                                                    </button>
                                                )) || (
                                                        <>
                                                            <button
                                                                className="bg-blue-600 hover:bg-blue-700 text-white font-medium rounded px-3 py-1"
                                                                title="Mark as drafted by you"
                                                                onClick={() => handleMarkAsMine(player.PlayerId)}
                                                            >
                                                                Mine
                                                            </button>
                                                            <button
                                                                className="bg-gray-600 hover:bg-gray-700 text-white font-medium rounded px-3 py-1"
                                                                title="Mark as drafted by someone else"
                                                                onClick={() => handleMarkAsTaken(player.PlayerId)}
                                                            >
                                                                Taken
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
        );
    }
}