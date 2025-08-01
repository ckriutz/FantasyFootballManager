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
                    <span className="text-gray-400">📍 New York, NY</span>
                    <div className="flex items-center space-x-1">
                      <span className="text-yellow-400">⭐</span>
                      <span className="text-white font-medium">8.6</span>
                      <div className="flex space-x-1 ml-2">
                        <span className="text-yellow-400">⭐⭐⭐⭐</span>
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex space-x-4">
                    <button className="bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded">
                      📨 Send message
                    </button>
                    <button className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded">
                      👥 Contacts
                    </button>
                    <button className="bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded">
                      🚨 Report user
                    </button>
                  </div>
                </div>
              </div>
            </div>
            
            {/* Profile Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              {/* Left Column - Work & Skills */}
              <div className="space-y-6">
                {/* Work Section */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">WORK</h3>
                  
                  <div className="space-y-4">
                    <div>
                      <div className="flex items-center space-x-2 mb-2">
                        <h4 className="text-white font-medium">Spotify New York</h4>
                        <span className="bg-blue-600 text-white px-2 py-1 rounded text-xs">Primary</span>
                      </div>
                      <p className="text-gray-400 text-sm">170 William Street</p>
                      <p className="text-gray-400 text-sm">New York, NY 10038-78 212 812-51</p>
                    </div>
                    
                    <div>
                      <div className="flex items-center space-x-2 mb-2">
                        <h4 className="text-white font-medium">Metropolitan Museum</h4>
                        <span className="bg-gray-600 text-white px-2 py-1 rounded text-xs">Secondary</span>
                      </div>
                      <p className="text-gray-400 text-sm">525 E 68th Street</p>
                      <p className="text-gray-400 text-sm">New York, NY 10021-78 156-187-60</p>
                    </div>
                  </div>
                </div>
                
                {/* Skills Section */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">SKILLS</h3>
                  <div className="space-y-2">
                    <div className="text-gray-300">Branding</div>
                    <div className="text-gray-300">UI/UX</div>
                    <div className="text-gray-300">Web + Design</div>
                    <div className="text-gray-300">Packaging</div>
                    <div className="text-gray-300">Print & Editorial</div>
                  </div>
                </div>
              </div>
              
              {/* Right Column - Contact & Basic Info */}
              <div className="lg:col-span-2 space-y-6">
                {/* Contact Information */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">CONTACT INFORMATION</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="text-gray-400 text-sm">sub:</label>
                      <p className="text-white">{user.sub}</p>
                    </div>
                    <div>
                      <label className="text-gray-400 text-sm">Address:</label>
                      <p className="text-white">525 E 68th Street</p>
                      <p className="text-gray-300 text-sm">New York, NY 10021-78 156-187-60</p>
                    </div>
                    <div>
                      <label className="text-gray-400 text-sm">E-mail:</label>
                      <p className="text-blue-400">{user.email}</p>
                    </div>
                    <div>
                      <label className="text-gray-400 text-sm">Site:</label>
                      <p className="text-blue-400">www.jeremyrose.com</p>
                    </div>
                  </div>
                </div>
                
                {/* Basic Information */}
                <div className="bg-gray-700 rounded-lg p-6">
                  <h3 className="text-xl font-bold text-white mb-4">BASIC INFORMATION</h3>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="text-gray-400 text-sm">Birthday:</label>
                      <p className="text-white">June 5, 1992</p>
                    </div>
                    <div>
                      <label className="text-gray-400 text-sm">Gender:</label>
                      <p className="text-white">Male</p>
                    </div>
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