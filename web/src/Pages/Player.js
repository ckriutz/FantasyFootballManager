import { useParams } from 'react-router-dom';
import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar';
import Breadcrumb from '../Components/Breadcrumb';
import { useAuth0 } from "@auth0/auth0-react";
import { FaThumbsUp, FaThumbsDown } from 'react-icons/fa';

export default function Player() {
    const { id } = useParams();
    const [player, setPlayer] = useState();
    const { isLoading, user, isAuthenticated } = useAuth0();

    useEffect(() => {
        // Don't fetch anything while authentication is loading
        if (isLoading) {
            return;
        }

        // Fetch the player data based on authentication status
        if (isAuthenticated && user) {
            // Fetch the player data using the user ID (authenticated)
            console.log(`http://localhost:5180/players/${id}/activity/${user.sub}`);
            fetch(`http://localhost:5180/players/${id}/activity/${user.sub}`)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error('Failed to fetch data');
                    }
                    return response.json();
                })
                .then((data) => {
                    setPlayer(data); // Set the specific player
                    console.log('Player data fetched:', data);
                })
                .catch((error) => console.error('Error fetching player data:', error));
        } else {
            // Fetch without user data (not authenticated)
            console.log(`http://localhost:5180/players/${id}`);
            fetch(`http://localhost:5180/players/${id}`)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error('Failed to fetch data');
                    }
                    return response.json();
                })
                .then((data) => {
                    setPlayer(data); // Set the specific player
                    console.log('Player data fetched:', data);
                })
                .catch((error) => console.error('Error fetching player data:', error));
        }
    }, [id, isAuthenticated, isLoading, user]);

    const handleThumbsUp = (playerId) => {
        console.log(`Thumbs up for player ${playerId}`);
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }
        console.log(`http://localhost:5180/players/${playerId}/thumbsup/${user.sub}`);
        fetch(`http://localhost:5180/players/${playerId}/thumbsup/${user.sub}`, {
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

                // Update the player object to reset isThumbsUp and isThumbsDown
                setPlayer((prevPlayer) => ({
                    ...prevPlayer,
                    isThumbsUp: data.isThumbsUp,
                    isThumbsDown: data.isThumbsDown
                }));

            })
            .catch((error) => console.error('Error thumbs-upping player:', error));
    };

    const handleThumbsDown = (playerId) => {
        console.log(`Thumbs down for player ${playerId}`);
        if (!user || !user.name) {
            console.error("User information is missing.");
            return;
        }

        fetch(`http://localhost:5180/players/${playerId}/thumbsdown/${user.sub}`, {
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

                setPlayer((prevPlayer) => ({
                    ...prevPlayer,
                    isThumbsUp: data.isThumbsUp,
                    isThumbsDown: data.isThumbsDown
                }));
            })
            .catch((error) => console.error('Error thumbs-upping player:', error));
    };

    const handleDrafted = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }
        console.log(`/players/${playerId}/draft/${user.sub}`);
        fetch(`http://localhost:5180/players/${playerId}/draft/${user.sub}`, {

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
                    console.error('Failed to mark player as drafted:', response.statusText);
                    throw new Error('Failed to mark player as drafted');

                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as drafted:', data);

                // Update the players array to set isDraftedOnMyTeam to true
                setPlayer((prevPlayer) => ({
                    ...prevPlayer,
                    isDraftedOnMyTeam: true
                }));


            })
            .catch((error) => console.error('Error marking player as drafted:', error));
    };
    const handleAssigned = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }

        fetch(`http://localhost:5180/players/${playerId}/assign/${user.sub}`, {
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
                    console.error('Failed to mark player as assigned:', response.statusText);
                    throw new Error('Failed to mark player as assigned');
                }
                return response.json();
            })
            .then((data) => {
                console.log('Player marked as assigned:', data);

                // Update the players array to set isDraftedOnOtherTeam to true
                setPlayer((prevPlayer) => ({
                    ...prevPlayer,
                    isDraftedOnOtherTeam: true
                }));
            })
            .catch((error) => console.error('Error marking player as assigned:', error));
    };

    const handleResetStatus = (playerId) => {
        if (!user || !user.sub) {
            console.error("User information is missing.");
            return;
        }

        fetch(`http://localhost:5180/players/${playerId}/reset/${user.sub}`, {
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

                // Update the player to reset isDraftedOnMyTeam and isDraftedOnOtherTeam
                setPlayer((prevPlayer) => ({
                    ...prevPlayer,
                    isDraftedOnMyTeam: false,
                    isDraftedOnOtherTeam: false
                }));
            })
            .catch((error) => console.error('Error resetting player status:', error));
    };

    if (player != null) {
        return (
            <div className="min-h-screen bg-gray-800">
                <Navbar />
                <Breadcrumb
                    items={[
                        { label: 'Home', href: '/' },
                        { label: 'Players', href: '/players' },
                        { label: player.fantasyPros?.player_name || player.sportsDataIo?.Name || 'Player', href: '/player/' + id }
                    ]}
                />
                <div className="container mx-auto p-4">
                    {/* Top Section - Profile and Key Stats */}
                    <div className="flex flex-col lg:flex-row gap-6 mb-6">
                        {/* Player Profile Card */}
                        <div className="w-lg bg-gray-500 rounded-lg shadow-md overflow-hidden">
                            <div className="p-6 text-center">
                                {/* Player Image */}
                                <img
                                    src={player.fantasyPros?.player_image_url || '/placeholder-image.png'}
                                    alt={player.fantasyPros?.player_name || player.sportsDataIo?.Name}
                                    className="w-24 h-24 rounded-full mx-auto mb-4 object-cover shadow-md"
                                />

                                {/* Player Name */}
                                <h1 className="text-xl font-bold text-white mb-2">
                                    {player.fantasyPros?.player_name || player.sportsDataIo?.Name}
                                </h1>

                                {/* Position */}
                                <p className="text-gray-300 mb-1">
                                    {player.fantasyPros?.player_position_id || player.sleeperData?.position}
                                </p>

                                {/* Team */}
                                <p className="text-gray-300 mb-1">
                                    {player.team?.name || 'N/A'} | 🎓 {player.sleeperData?.college || 'N/A'}
                                </p>

                                {/* Action Buttons - Only show when authenticated */}
                                {isAuthenticated && (
                                    <>
                                        <div className="mb-4">
                                            {(!player.isDraftedOnMyTeam && !player.isDraftedOnOtherTeam) ? (
                                                <div className="flex space-x-2">
                                                    <button onClick={() => handleDrafted(id)} className="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded cursor-pointer">
                                                        DRAFT
                                                    </button>
                                                    <button onClick={() => handleAssigned(id)} className="flex-1 bg-gray-600 hover:bg-gray-500 text-white font-medium py-2 px-4 rounded cursor-pointer">
                                                        ASSIGNED
                                                    </button>
                                                </div>
                                            ) : player.isDraftedOnMyTeam ? (
                                                <div className="text-center">
                                                    <p className="text-green-400 font-medium mb-2">Drafted on My Team</p>
                                                    <button onClick={() => handleResetStatus(id)} className="bg-gray-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded cursor-pointer">
                                                        RESET
                                                    </button>
                                                </div>
                                            ) : player.isDraftedOnOtherTeam ? (
                                                <div className="text-center">
                                                    <p className="text-red-400 font-medium mb-2">Drafted on Other Team</p>
                                                    <button onClick={() => handleResetStatus(id)} className="bg-gray-600 hover:bg-red-700 text-white font-medium py-2 px-4 rounded cursor-pointer">
                                                        RESET
                                                    </button>
                                                </div>
                                            ) : null}
                                        </div>

                                        {/* Like/Dislike Buttons */}
                                        <div className="flex justify-center space-x-4">
                                            {player.isThumbsUp && !player.isThumbsDown ? (
                                                <button
                                                    className="p-2 text-green-400 bg-gray-600 rounded-full cursor-pointer"
                                                    onClick={() => handleThumbsUp(id)}
                                                >
                                                    <FaThumbsUp size={20} />
                                                </button>
                                            ) : player.isThumbsDown && !player.isThumbsUp ? (
                                                <button
                                                    className="p-2 text-red-400 bg-gray-600 rounded-full cursor-pointer"
                                                    onClick={() => handleThumbsDown(id)}
                                                >
                                                    <FaThumbsDown size={20} />
                                                </button>
                                            ) : (
                                                <>
                                                    <button
                                                        className="p-2 text-green-400 hover:bg-gray-600 rounded-full cursor-pointer"
                                                        onClick={() => handleThumbsUp(id)}
                                                    >
                                                        <FaThumbsUp size={20} />
                                                    </button>
                                                    <button
                                                        className="p-2 text-red-400 hover:bg-gray-600 rounded-full cursor-pointer"
                                                        onClick={() => handleThumbsDown(id)}
                                                    >
                                                        <FaThumbsDown size={20} />
                                                    </button>
                                                </>
                                            )}
                                        </div>
                                    </>
                                )}
                            </div>
                        </div>

                        {/* Key Stats Cards */}
                        <div className="flex-1">
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
                                {/* Fantasy Points */}
                                <div className="bg-gradient-to-br from-gray-700 to-gray-800 rounded-xl shadow-lg p-6 text-center border border-gray-600 hover:border-yellow-400 transition-colors">
                                    <div className="flex flex-col justify-center items-center h-20">
                                        <p className="text-4xl font-bold text-yellow-400 mb-2">
                                            {player.sportsDataIo?.FantasyPoints || 'N/A'}
                                        </p>
                                        <h3 className="text-sm font-medium text-gray-300 uppercase tracking-wide">Fantasy Points</h3>
                                    </div>
                                </div>

                                {/* Average Draft Position */}
                                <div className="bg-gradient-to-br from-gray-700 to-gray-800 rounded-xl shadow-lg p-6 text-center border border-gray-600 hover:border-blue-400 transition-colors">
                                    <div className="flex flex-col justify-center items-center h-20">
                                        <p className="text-4xl font-bold text-blue-400 mb-2">
                                            {player.sportsDataIo?.AverageDraftPosition || 'N/A'}
                                        </p>
                                        <h3 className="text-sm font-medium text-gray-300 uppercase tracking-wide">Average Draft Position</h3>
                                    </div>
                                </div>

                                {/* Ownership Percentage */}
                                <div className="bg-gradient-to-br from-gray-700 to-gray-800 rounded-xl shadow-lg p-6 text-center border border-gray-600 hover:border-green-400 transition-colors">
                                    <div className="flex flex-col justify-center items-center h-20">
                                        <p className="text-4xl font-bold text-green-400 mb-2">
                                            {player.fantasyPros?.player_owned_avg ? `${player.fantasyPros.player_owned_avg}%` : 'N/A'}
                                        </p>
                                        <h3 className="text-sm font-medium text-gray-300 uppercase tracking-wide">Ownership Percentage</h3>
                                    </div>
                                </div>
                            </div>

                            {/* Full Width Test Card */}
                            <div className="bg-gray-700 rounded-lg shadow-md p-6">
                                <h3 className="text-lg font-bold text-white mb-4">Test Card</h3>
                                <p className="text-gray-300">
                                    This is a test card that spans the full width of all three stat cards above.
                                    You can replace this content with any information you'd like to display here.
                                </p>
                            </div>
                        </div>
                    </div>

                    {/* Player Stats Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-6">
                        {/* Fantasy Rankings */}
                        <div className="bg-gray-700 rounded-lg shadow-md p-6">
                            <h3 className="text-lg font-bold text-white mb-4">Fantasy Rankings</h3>
                            <div className="space-y-3">
                                <div className="flex justify-between">
                                    <span className="text-gray-300">ECR Rank:</span>
                                    <span className="font-medium text-white">{player.fantasyPros?.rank_ecr || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Position Rank:</span>
                                    <span className="font-medium text-white">{player.fantasyPros?.pos_rank || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Tier:</span>
                                    <span className="font-medium text-white">{player.fantasyPros?.tier || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">ADP:</span>
                                    <span className="font-medium text-white">{player.sportsDataIo?.AverageDraftPosition || 'N/A'}</span>
                                </div>
                            </div>
                        </div>

                        {/* Player Info */}
                        <div className="bg-gray-700 rounded-lg shadow-md p-6">
                            <h3 className="text-lg font-bold text-white mb-4">Player Info</h3>
                            <div className="space-y-3">
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Age:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.age || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Height:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.height ? `${Math.floor(player.sleeperData.height / 12)}'${player.sleeperData.height % 12}"` : 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Weight:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.weight ? `${player.sleeperData.weight} lbs` : 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Experience:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.yearsExp ? `${player.sleeperData.yearsExp} years` : 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">College:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.college || 'N/A'}</span>
                                </div>
                            </div>
                        </div>

                        {/* Season Stats */}
                        <div className="bg-gray-700 rounded-lg shadow-md p-6">
                            <h3 className="text-lg font-bold text-white mb-4">Season Info</h3>
                            <div className="space-y-3">
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Bye Week:</span>
                                    <span className="font-medium text-white">{player.sportsDataIo?.ByeWeek || player.fantasyPros?.player_bye_week || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Projected Points:</span>
                                    <span className="font-medium text-white">{player.sportsDataIo?.ProjectedFantasyPoints || 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Auction Value:</span>
                                    <span className="font-medium text-white">{player.sportsDataIo?.AuctionValue ? `$${player.sportsDataIo.AuctionValue}` : 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Owned %:</span>
                                    <span className="font-medium text-white">{player.fantasyPros?.player_owned_avg ? `${player.fantasyPros.player_owned_avg}%` : 'N/A'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-gray-300">Status:</span>
                                    <span className="font-medium text-white">{player.sleeperData?.status || 'N/A'}</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Additional Stats - Dynasty and 2QB */}
                    {(player.sportsDataIo?.AverageDraftPositionDynasty || player.sportsDataIo?.AverageDraftPosition2QB) && (
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            {player.sportsDataIo?.AverageDraftPositionDynasty && (
                                <div className="bg-gray-700 rounded-lg shadow-md p-6">
                                    <h3 className="text-lg font-bold text-white mb-4">Dynasty League</h3>
                                    <div className="flex justify-between">
                                        <span className="text-gray-300">Dynasty ADP:</span>
                                        <span className="font-medium text-white">{player.sportsDataIo.AverageDraftPositionDynasty}</span>
                                    </div>
                                </div>
                            )}

                            {player.sportsDataIo?.AverageDraftPosition2QB && (
                                <div className="bg-gray-700 rounded-lg shadow-md p-6">
                                    <h3 className="text-lg font-bold text-white mb-4">2QB League</h3>
                                    <div className="flex justify-between">
                                        <span className="text-gray-300">2QB ADP:</span>
                                        <span className="font-medium text-white">{player.sportsDataIo.AverageDraftPosition2QB}</span>
                                    </div>
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </div>
        );
    } else {
        return (
            <div className="min-h-screen bg-gray-800">
                <Navbar />
                <div className="container mx-auto p-4">
                    <div className="bg-gray-700 shadow-md rounded-lg p-6 text-center">
                        <h1 className="text-xl font-bold text-white">Player not found</h1>
                    </div>
                </div>
            </div>
        );
    }
}