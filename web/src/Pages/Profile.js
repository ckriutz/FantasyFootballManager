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
      <div>
        <Navbar />
        <Breadcrumb
          items={[
            { label: 'Home', href: '/' },
            { label: 'Profile', href: '/profile' },
          ]}
        />
        <img src={user.picture} alt={user.name} />
        <h2>{user.name}</h2>
        <p>{user.email}</p>
      </div>
    )
  );
};

export default Profile;