import Navbar from '../Components/Navbar';
import Breadcrumb from '../Components/Breadcrumb';

export default function About() {
  return (
    <div className="page">
      <Navbar />
      <Breadcrumb
        items={[
          { label: 'Home', href: '/' },
          { label: 'About', href: '/about' }
        ]}
      />
      <div className="flex items-center justify-center h-screen bg-gray-800">
        <div className="text-center text-white space-y-4">
          <h1 className="text-4xl font-bold">About Us</h1>
          <p className="text-lg text-gray-300">
            We are dedicated to helping you manage your fantasy football team with ease.
          </p>
        </div>
      </div>
    </div>
  );
}