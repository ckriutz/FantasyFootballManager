import { useAuth0 } from "@auth0/auth0-react";
import { FaSignInAlt } from 'react-icons/fa';
import { Link } from 'react-router-dom';

const LoginButton = () => {
  const { loginWithRedirect } = useAuth0();

  return (
    <Link
      to="/login"
      className="inline-flex items-center space-x-2 bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded cursor-pointer text-sm sm:text-base"
      onClick={e => {
        e.preventDefault();
        loginWithRedirect();
      }}
    >
      <FaSignInAlt />
      <span>Log In</span>
    </Link>
  );
};

export default LoginButton;