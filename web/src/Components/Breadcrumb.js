import React from 'react';
import { Link } from 'react-router-dom';
import { FaChevronRight } from 'react-icons/fa';

const Breadcrumb = ({ items }) => {
    return (
        <nav className="flex items-center space-x-2 text-sm font-medium text-gray-600 bg-white border border-gray-200 p-4 rounded-lg shadow-md">
            {items.map((item, index) => (
                <div key={index} className="flex items-center">
                    {index > 0 && <FaChevronRight className="mx-3 text-gray-400 text-xs" />}
                    {item.href ? (
                        <Link
                            to={item.href}
                            className="text-gray-700 hover:text-blue-600 hover:bg-blue-50 px-2 py-1 rounded transition-all duration-200 font-medium"
                        >
                            {item.label}
                        </Link>
                    ) : (
                        <span className="text-blue-600 font-bold bg-blue-50 px-2 py-1 rounded">{item.label}</span>
                    )}
                </div>
            ))}
        </nav>
    );
};

export default Breadcrumb;