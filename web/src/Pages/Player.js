import { useParams } from 'react-router-dom';
import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar';
import Breadcrumb from '../Components/Breadcrumb';

export default function Player() {
    const { id } = useParams();
    const [player, setPlayer] = useState();

    useEffect(() => {
        // Fetch the sleeper_data.json file
        fetch('/Data/sleeper_data.json')
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Failed to fetch data');
                }
                return response.json();
            })
            .then((data) => {
                const foundPlayer = data.players.find((p) => p.player_id === Number(id)); // Match player by id
                setPlayer(foundPlayer); // Set the specific player
            })
            .catch((error) => console.error('Error fetching player data:', error));
    }, [id]);

    if (player != null) {
        return (
            <div className="min-h-screen bg-gray-100">
                <Navbar />
                {/* Here is where I want to add tailwindcss breadcumb to go back to player list. */}
                <Breadcrumb
                    items={[
                        { label: 'Home', href: '/' },
                        { label: 'Players', href: '/players' },
                        { label: player.player_name, href: '/player/'+id }
                    ]}
                />
                <div className="container mx-auto p-4">
                    <div className="bg-white shadow-md rounded-lg p-6">
                        <div className="flex items-center space-x-4 mb-6">
                            {/* Player Image */}
                            <img
                                src={player.player_square_image_url || '/placeholder-image.png'}
                                alt={`${player.player_name}`}
                                className="w-24 h-24 rounded-full object-cover shadow-md"
                            />
                            {/* Player Name */}
                            <h1 className="text-2xl font-bold text-gray-800">{player.player_name}</h1>
                            <div className="p-6 ml-4 bg-gray-50 rounded shadow text-center">
                                <p className="text-3xl font-semibold tracking-tight text-balance ">{player.player_positions || 'N/A'}</p>
                                <p className="text-xs font-medium text-gray-700">position</p>
                            </div>
                            <div className="p-6 ml-4 bg-gray-50 rounded shadow text-center">
                                <p className="text-3xl font-semibold tracking-tight text-balance ">{player.player_team_id || 'N/A'}</p>
                                <p className="text-xs font-medium text-gray-700">team</p>
                            </div>
                            <div className="p-6 ml-4 bg-gray-50 rounded shadow text-center">
                                <p className="text-3xl font-semibold tracking-tight text-balance ">{player.rank_ecr || 'N/A'}</p>
                                <p className="text-xs font-medium text-gray-700">rank</p>
                            </div>
                        </div>
                        <div className="w-full p-6 bg-gray-100 rounded-lg shadow-md">
                            <h2 className="text-xl font-semibold text-gray-800 mb-4">Player Information</h2>
                            <div className="grid grid-cols-2 gap-4">
                                {/* Column 1 */}
                                <div className="space-y-4">
                                    <div className="flex items-center">
                                        <label className="w-1/4 text-gray-700 font-medium">Tier:</label>
                                        <div className="flex-1 bg-white border border-gray-300 rounded-md px-3 py-2 text-gray-800">
                                            {player.tier || 'N/A'}
                                        </div>
                                    </div>
                                    <div className="flex items-center">
                                        <label className="w-1/4 text-gray-700 font-medium">Bye Week:</label>
                                        <div className="flex-1 bg-white border border-gray-300 rounded-md px-3 py-2 text-gray-800">
                                            {player.player_bye_week || 'N/A'}
                                        </div>
                                    </div>
                                </div>
                                {/* Column 2 */}
                                <div className="space-y-4">
                                    <div className="flex items-center">
                                        <label className="w-1/4 text-gray-700 font-medium">Owned Avg:</label>
                                        <div className="flex-1 bg-white border border-gray-300 rounded-md px-3 py-2 text-gray-800">
                                            {player.player_owned_avg || 'N/A'}
                                        </div>
                                    </div>
                                    <div className="flex items-center">
                                        <label className="w-1/4 text-gray-700 font-medium">Position Rank:</label>
                                        <div className="flex-1 bg-white border border-gray-300 rounded-md px-3 py-2 text-gray-800">
                                            {player.pos_rank || 'N/A'}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    } else {
        return (
            <div className="min-h-screen bg-gray-100">
                <Navbar />
                <div className="container mx-auto p-4">
                    <div className="bg-white shadow-md rounded-lg p-6 text-center">
                        <h1 className="text-xl font-bold text-gray-800">Player not found</h1>
                    </div>
                </div>
            </div>
        );
    }
}