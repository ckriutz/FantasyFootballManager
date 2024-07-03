import { Link } from "react-router-dom";

export default function Navbar(props)
{
    const activeClass = "nav-link active";

    return (
        <header className="navbar navbar-expand-sm navbar-light d-print-none">
            <div className="collapse navbar-collapse" id="navbar-menu">
                <div className="navbar">
                    <div className="container-xl">
                        <ul className="navbar-nav">
                            <li className={activeClass}>
                                <a className="nav-link" href="./#" >
                                    <span className="nav-link-icon d-md-none d-lg-inline-block">
                                        <svg xmlns="http://www.w3.org/2000/svg" className="icon icon-tabler icon-tabler-home-2" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M5 12l-2 0l9 -9l9 9l-2 0"></path><path d="M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-7"></path><path d="M10 12h4v4h-4z"></path></svg>
                                    </span>
                                    <span className="nav-link-title">Home</span>
                                </a>
                            </li>
                            <li className="nav-item">
                                <Link className="nav-link" to="/players">
                                    <span className="nav-link-icon d-md-none d-lg-inline-block">
                                        <svg xmlns="http://www.w3.org/2000/svg" className="icon icon-tabler icon-tabler-users" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M9 7m-4 0a4 4 0 1 0 8 0a4 4 0 1 0 -8 0"></path><path d="M3 21v-2a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v2"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path><path d="M21 21v-2a4 4 0 0 0 -3 -3.85"></path></svg>
                                    </span>
                                    <span className="nav-link-title">Players</span>
                                </Link>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </header>
    );
}