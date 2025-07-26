import Navbar from '../Components/Navbar';
import { useEffect, useState } from 'react';

export default function Status() {
    const [sources, setSources] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Replace with your actual fetch URL if needed
        fetch('http://localhost:8000/datastatuses')
            .then(res => res.json())
            .then(data => {
                setSources(data);
                setLoading(false);
            })
            .catch(() => setLoading(false));
    }, []);

    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <main className="container mx-auto px-4 py-8">
                <h2 className="text-2xl font-bold mb-4 text-gray-800">Status</h2>
                {loading ? (
                    <div className="text-gray-600">Loading...</div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                        {sources.map(source => (
                            <div key={source.Id} className="bg-white rounded shadow p-6 flex flex-col items-start">
                                <div className="text-lg font-bold text-gray-800 mb-2">{source.DataSource}</div>
                                <div className="text-gray-600 text-sm">
                                    <span className="font-medium">Last Updated:</span>
                                    <span> {new Date(source.LastUpdated).toLocaleString()}</span>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </main>
        </div>
    );
}
