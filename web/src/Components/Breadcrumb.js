import React from 'react';
import { Link } from 'react-router-dom';
import { FaChevronRight } from 'react-icons/fa';

const Breadcrumb = ({ items }) => {
    return (
        <nav className="flex items-center space-x-2 text-sm font-medium text-gray-500 bg-gray-100 p-3 rounded shadow">
            {items.map((item, index) => (
                <div key={index} className="flex items-center">
                    {index > 0 && <FaChevronRight className="mx-2 text-gray-400" />}
                    {item.href ? (
                        <Link
                            to={item.href}
                            className="text-gray-600 hover:text-blue-600 transition-colors"
                        >
                            {item.label}
                        </Link>
                    ) : (
                        <span className="text-gray-800 font-bold">{item.label}</span>
                    )}
                </div>
            ))}
        </nav>
    );
};

export default Breadcrumb;