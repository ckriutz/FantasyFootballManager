import Navbar from '../Components/Navbar';
import Breadcrumb from '../Components/Breadcrumb';
import { useAuth0 } from "@auth0/auth0-react";
import React, { useState, useEffect } from "react";

const Profile = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();
    // Use environment variable or relative URL for API endpoint
    const apiUrl = process.env.REACT_APP_API_URL || 'http://127.0.0.1:5180';

  // State for league IDs
  const [yahooLeagueId, setYahooLeagueId] = useState('');
  const [espnLeagueId, setEspnLeagueId] = useState('');
  const [sleeperLeagueId, setSleeperLeagueId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingUserData, setIsLoadingUserData] = useState(false);

  // Fetch user data when component mounts
  useEffect(() => {
    const fetchUserData = async () => {
      if (!user?.sub) return;
      
      setIsLoadingUserData(true);
      try {
        const response = await fetch(`${apiUrl}/users/${user.sub}`);
        if (response.ok) {
          
          const userData = await response.json();
          console.log('User data fetched successfully:', userData);
          // Populate the form fields with existing data
          setYahooLeagueId(userData.yahooLeagueId || '');
          setEspnLeagueId(userData.espnLeagueId || '');
          setSleeperLeagueId(userData.sleeperLeagueId || '');
        } else if (response.status !== 404) {
          // Only log error if it's not a 404 (user not found is expected for new users)
          console.error('Error fetching user data:', response.statusText);
        }
      } catch (error) {
        console.error('Error fetching user data:', error);
      } finally {
        setIsLoadingUserData(false);
      }
    };

    fetchUserData();
  }, [user?.sub, apiUrl]);

  // Handle form submission
  const handleSubmitLeagueNumbers = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);

    try {
      const response = await fetch(`${apiUrl}/users`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Auth0Id: user.sub,
          yahooLeagueId,
          espnLeagueId,
          sleeperLeagueId,
        }),
      });

      if (response.ok) {
        alert('League numbers saved successfully!');
      } else {
        throw new Error('Failed to save league numbers');
      }
    } catch (error) {
      console.error('Error saving league numbers:', error);
      alert('Error saving league numbers. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };


  if (isLoading) {
    return <div>
        <Navbar />
        <Breadcrumb
          items={[
            { label: 'Home', href: '/' },
            { label: 'Profile', href: '/profile' },
          ]}
        />
        <div className="flex items-center justify-center h-screen">
          <p className="text-gray-500">Loading...</p>
        </div>
    </div>; 
  }

  return (
    isAuthenticated && (
      <div className="page">
        <Navbar />
        <div className="min-h-screen bg-gray-800 p-6">
          <Breadcrumb
            items={[
              { label: 'Home', href: '/' },
              { label: 'Profile', href: '/profile' },
            ]}
          />
          
          {/* Profile Header */}
          <div className="max-w-4xl mx-auto mt-6">
            <div className="bg-gray-700 rounded-lg p-6 mb-6">
              <div className="flex items-start space-x-6">
                {/* Profile Picture */}
                <div className="flex-shrink-0">
                  <img 
                    src={user.picture} 
                    alt={user.name} 
                    className="w-32 h-32 rounded-lg object-cover border-2 border-gray-600"
                  />
                </div>
                
                {/* Basic Info */}
                <div className="flex-1">
                  <div className="flex items-center space-x-4 mb-4">
                    <h1 className="text-3xl font-bold text-white">{user.name}</h1>
                    <span className="bg-blue-600 text-white px-3 py-1 rounded text-sm font-medium">
                      Product Designer
                    </span>
                  </div>
                  
                  <div className="flex items-center space-x-4 mb-4">
                    <span className="text-gray-400">🆔 {user.sub}</span>
                    <div className="flex items-center space-x-1">
                      <span className="text-yellow-400">⭐</span>
                      <span className="text-white font-medium">8.6</span>
                      <div className="flex space-x-1 ml-2">
                        <span className="text-yellow-400">⭐⭐⭐⭐</span>
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex space-x-4">
                    
                  </div>
                </div>
              </div>
            </div>
            
            {/* Profile Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              {/* Left Column - Work & Skills */}
              <div className="space-y-6">
                {/* Work Section */}
                
                
                {/* League Numbers Form */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">LEAGUE NUMBERS</h3>
                  {isLoadingUserData && (
                    <div className="text-center text-gray-400 mb-4">
                      Loading your league data...
                    </div>
                  )}
                  <form className="space-y-4" onSubmit={handleSubmitLeagueNumbers}>
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        Yahoo League ID
                      </label>
                      <input
                        type="text"
                        value={yahooLeagueId}
                        onChange={(e) => setYahooLeagueId(e.target.value)}
                        disabled={isLoadingUserData}
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                        placeholder="Enter your Yahoo league ID"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        ESPN League ID
                      </label>
                      <input
                        type="text"
                        value={espnLeagueId}
                        onChange={(e) => setEspnLeagueId(e.target.value)}
                        disabled={isLoadingUserData}
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                        placeholder="Enter your ESPN league ID"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        Sleeper League ID
                      </label>
                      <input
                        type="text"
                        value={sleeperLeagueId}
                        onChange={(e) => setSleeperLeagueId(e.target.value)}
                        disabled={isLoadingUserData}
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                        placeholder="Enter your Sleeper league ID"
                      />
                    </div>
                    
                    <button 
                      type="submit"
                      disabled={isSubmitting || isLoadingUserData}
                      className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-2 px-4 rounded w-full transition-colors"
                    >
                      {isSubmitting ? 'Saving...' : 'Save League Numbers'}
                    </button>
                  </form>
                </div>
              </div>
              
              {/* Right Column - Contact & Basic Info */}
              <div className="lg:col-span-2 space-y-6">
                {/* Contact Information */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">CONTACT INFORMATION</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                   
                  </div>
                </div>
                
                {/* Basic Information */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">SOME INFORMATION</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  );
};

export default Profile;