import Navbar from '../Components/Navbar';
import Breadcrumb from '../Components/Breadcrumb';
import { useAuth0 } from "@auth0/auth0-react";
import React from "react";

const Profile = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();
  console.log(user);
  console.log(isAuthenticated);
  console.log(isLoading);
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
                  <form className="space-y-4">
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        Yahoo League ID
                      </label>
                      <input
                        type="text"
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500"
                        placeholder="Enter your Yahoo league ID"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        ESPN League ID
                      </label>
                      <input
                        type="text"
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500"
                        placeholder="Enter your ESPN league ID"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-gray-300 text-sm font-medium mb-2">
                        Sleeper League ID
                      </label>
                      <input
                        type="text"
                        className="w-full px-3 py-2 bg-gray-600 border border-gray-500 rounded text-white placeholder-gray-400 focus:outline-none focus:border-blue-500"
                        placeholder="Enter your Sleeper league ID"
                      />
                    </div>
                    
                    <button 
                      type="submit"
                      className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded w-full transition-colors"
                    >
                      Save League Numbers
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