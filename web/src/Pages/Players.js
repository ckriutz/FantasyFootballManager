import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar';
import { Link } from 'react-router-dom';
import Breadcrumb from '../Components/Breadcrumb';

export default function Players() {
    const [players, setPlayers] = useState([]);
    const [filteredPlayers, setFilteredPlayers] = useState([]);
    const [selectedPosition, setSelectedPosition] = useState('All');

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
                setPlayers(data.players); // Set the players array from the JSON file
                setFilteredPlayers(data.players); // Initialize filtered players
            })
            .catch((error) => console.error('Error fetching player data:', error));
    }, []);

    const handleFilterChange = (event) => {
        const position = event.target.value;
        setSelectedPosition(position);

        if (position === 'All') {
            setFilteredPlayers(players);
        } else {
            setFilteredPlayers(players.filter(player => player.player_positions.includes(position)));
        }
    };

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
                                <th className="border border-gray-300 px-4 py-2 text-left">Name</th>
                                <th className="border border-gray-300 px-4 py-2 text-left">Position</th>
                                <th className="border border-gray-300 px-4 py-2 text-left">Team</th>
                                <th className="border border-gray-300 px-4 py-2 text-left">Rank</th>
                                <th className="border border-gray-300 px-4 py-2 text-left">Bye</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredPlayers.map((player) => (
                                <tr key={player.player_id} className="odd:bg-white even:bg-gray-100">
                                    <td className="border border-gray-300 px-4 py-2 text-blue-600 hover:text-blue-700"><Link to={`/player/${player.player_id}`}>{player.player_name}</Link></td>
                                    <td className="border border-gray-300 px-4 py-2">{player.player_positions}</td>
                                    <td className="border border-gray-300 px-4 py-2">{player.player_team_id}</td>
                                    <td className="border border-gray-300 px-4 py-2">{player.rank_ecr}</td>
                                    <td className="border border-gray-300 px-4 py-2">{player.player_bye_week}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}