import Navbar from '../Components/Navbar';
import { useEffect, useState } from 'react';

export default function Status() {
    const [sources, setSources] = useState([]);
    const [loading, setLoading] = useState(true);
    const [apiHealth, setApiHealth] = useState(null);
    const [apiHealthLoading, setApiHealthLoading] = useState(true);

    // Resolve API base URL at runtime via public/config.json with safe fallbacks
    const [apiUrl, setApiUrl] = useState(null);

    // Helper: Check API health
    const checkApiHealth = async () => {
        try {
            if (!apiUrl) return; // wait until apiUrl resolved
            const response = await fetch(`${apiUrl}/health`);
            if (response.ok) {
                const healthData = await response.text();
                console.log('API Health Response:', healthData);
                setApiHealth(healthData === '"API is healthy"' || healthData === 'API is healthy' ? 'healthy' : 'unhealthy');
            } else {
                setApiHealth('unhealthy');
            }
        } catch (error) {
            setApiHealth('unhealthy');
        } finally {
            setApiHealthLoading(false);
        }
    };

    // Helper: Fetch data status
    const fetchDataStatus = async () => {
        try {
            if (!apiUrl) return; // wait until apiUrl resolved
            const response = await fetch(`${apiUrl}/datastatus`);
            if (response.ok) {
                const data = await response.json();
                setSources(data);
            } else {
                console.error('Failed to fetch data status');
            }
        } catch (error) {
            console.error('Error fetching data status:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        // Load runtime config first, then perform requests
        const loadConfig = async () => {
            try {
                const resp = await fetch('/config.json', { cache: 'no-store' });
                if (resp.ok) {
                    const cfg = await resp.json();
                    setApiUrl(cfg?.apiBaseUrl || '');
                } else {
                    console.error('Failed to load /config.json');
                    setApiUrl('');
                }
            } catch (e) {
                console.error('Error loading /config.json', e);
                setApiUrl('');
            }
        };

        // Load config
        loadConfig();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    // Trigger requests once apiUrl is resolved
    useEffect(() => {
        if (!apiUrl) return;
        const run = async () => {
            await Promise.all([checkApiHealth(), fetchDataStatus()]);
        };
        run();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [apiUrl]);

    return (
        <div className="min-h-screen bg-gray-800">
            <Navbar />
            <main className="container mx-auto px-4 py-8">
                <h2 className="text-2xl font-bold mb-6 text-white">System Status</h2>
                
                {/* API Health Status */}
                <div className="mb-8">
                    <h3 className="text-xl font-bold mb-4 text-white">API Health</h3>
                    <div className="bg-gray-700 rounded-lg shadow p-6">
                        {apiHealthLoading ? (
                            <div className="text-gray-300">Checking API health...</div>
                        ) : (
                            <div className="flex items-center space-x-2">
                                <div className={`w-3 h-3 rounded-full ${
                                    apiHealth === 'healthy' ? 'bg-green-500' : 'bg-red-500'
                                }`}></div>
                                <span className={`font-medium ${
                                    apiHealth === 'healthy' ? 'text-green-400' : 'text-red-400'
                                }`}>
                                    API is {apiHealth === 'healthy' ? 'Healthy' : 'Unhealthy'}
                                </span>
                            </div>
                        )}
                    </div>
                </div>

                {/* Data Sources Status */}
                <div>
                    <h3 className="text-xl font-bold mb-4 text-white">Data Sources</h3>
                    {loading ? (
                        <div className="text-gray-300">Loading data sources...</div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                            {sources.map(source => {
                                const lastUpdated = new Date(source.lastUpdated);
                                const now = new Date();
                                const hoursSinceUpdate = (now - lastUpdated) / (1000 * 60 * 60);
                                const isStale = hoursSinceUpdate > 24;
                                
                                return (
                                    <div key={source.id} className={`bg-gray-700 rounded-lg shadow p-6 flex flex-col items-start border-l-4 hover:bg-gray-600 transition-colors ${
                                        isStale ? 'border-red-500' : 'border-green-500'
                                    }`}>
                                        <div className="flex items-center justify-between w-full mb-2">
                                            <div className="text-lg font-bold text-white">{source.dataSource}</div>
                                            <div className={`w-3 h-3 rounded-full ${
                                                isStale ? 'bg-red-500' : 'bg-green-500'
                                            }`}></div>
                                        </div>
                                        <div className="text-gray-300 text-sm mb-2">
                                            <span className="font-medium">Last Updated:</span>
                                            <span> {lastUpdated.toLocaleString()}</span>
                                        </div>
                                        {isStale && (
                                            <div className="text-red-400 text-sm font-medium">
                                                ⚠️ Data is over 24 hours old
                                            </div>
                                        )}
                                        {!isStale && (
                                            <div className="text-green-400 text-sm font-medium">
                                                ✅ Data is current
                                            </div>
                                        )}
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
}
