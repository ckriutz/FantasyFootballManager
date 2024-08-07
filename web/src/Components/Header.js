import React, { useState } from 'react';
import ShowApiHealth from './ShowApiHealth';

export default function Navbar()
{
    const [health, setHealth] = useState(false);
    fetch(process.env.REACT_APP_API_URL +'/health').then(response => {
        if (response.ok) {
            setHealth(true);
        } else {
            setHealth(false);
        }
    }).catch(error => setHealth(false));

    return (
        <header className="navbar navbar-expand-sm navbar-light d-print-none">
            <div className="container-xl">
                <h1 className="navbar-brand navbar-brand-autodark d-none-navbar-horizontal pe-0 pe-md-3">🏈 Fantasy Football Manager</h1>
                <div className="navbar-nav flex-row order-md-last">
                    <div className="nav-item d-none d-md-flex me-3">
                        <div className="btn-list">
                            <ShowApiHealth Health={health}></ShowApiHealth>
                        </div>
                    </div>
                </div>
            </div>
        </header>
    );
}