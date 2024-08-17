import { useParams } from 'react-router-dom';
import { useState, useEffect } from 'react';
import Header from '../Components/Header'
import Navbar from '../Components/Navbar'

import ClaimPlayerButton from '../Components/ClaimPlayerButton';
import ReleasePlayerButton from '../Components/ReleasePlayerButton';
import AssignPlayerButton from '../Components/AssignPlayerButton';
import ThumbsUpButton from '../Components/ThumbsUpButton';
import ThumbsDownButton from '../Components/ThumbsDownButton';

import {ReactComponent as ClockCog} from '../Components/Icons/ClockCog.svg'
import {ReactComponent as MoodSick} from '../Components/Icons/MoodSick.svg'
import {ReactComponent as CurrencyDollar} from '../Components/Icons/CurrencyDollar.svg'
import {ReactComponent as ListNumbers} from '../Components/Icons/ListNumbers.svg'
import {ReactComponent as Trophy} from '../Components/Icons/Trophy.svg'
import {ReactComponent as HeartCheck} from '../Components/Icons/HeartCheck.svg'

export default function Players()
{
    const { id } = useParams();
    const [data, setData] = useState(null);

    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/fantasyplayer/" + id, {
            headers: {
                "Content-Type": "application/json",
              },
        })
        .then((response) => {
            if (response.ok) {
                return response.json();
            }
        })
        .then((data) => setData(data))
    }, [])

    if(data != null) { console.log(data); }

    function onClickThumbsUp(e) {
        e.preventDefault();
        
        if(data.fantasy.isThumbsUp)
        {
            // Player is already liked, so we're unliking them
            console.log("Setting player " + data.sleeper.fullName + " to nutral...");
            fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/nothumbs/' + data.sleeper.playerId, {
                method: 'POST'
            });
            data.fantasy.isThumbsUp = false;
        }
        else
        {
            console.log("Liking player " + data.sleeper.fullName + "...");
            fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/thumbsup/' + data.sleeper.playerId, {
                method: 'POST'
            });
            data.fantasy.isThumbsUp = true;
        }

        const updatedData = { ...data }
        updatedData.fantasy.isThumbsUp = data.fantasy.isThumbsUp;
        setData(updatedData);

    }

    function thumbsDown(e)
    {
        e.preventDefault();
        
        if(data.fantasy.isThumbsDown)
        {
            // Player is already unliked, so we're setting them to nutral.
            console.log("Setting player " + data.sleeper.fullName + " to nutral...");
            fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/nothumbs/' + data.sleeper.playerId, {
                method: 'POST'
            });
            data.fantasy.isThumbsDown = false;
        }
        else
        {
            console.log("Disliking player " + data.sleeper.fullName + "...");
            fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/thumbsdown/' + data.sleeper.playerId, {
                method: 'POST'
            });
            data.fantasy.isThumbsDown = true;
        }

        const updatedData = { ...data }
        updatedData.fantasy.isThumbsDown = data.fantasy.isThumbsDown;
        setData(updatedData);
    }

    function claimPlayer(e) {
        console.log("Claiming player " + data.sleeper.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/claim/' + data.sleeper.playerId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.fantasy.isTaken = false;
        updatedData.fantasy.isOnMyTeam = true;
        setData(updatedData);
    }

    function assignPlayer(e) {       
        console.log("Assigning player " + data.sleeper.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/assign/' + data.sleeper.playerId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.fantasy.isTaken = true;
        updatedData.fantasy.isOnMyTeam = false;
        setData(updatedData);
    }
    
    function releasePlayer(e) {
        console.log("Resetting player " + data.sleeper.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/reset/' + data.sleeper.playerId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.fantasy.isTaken = false;
        updatedData.fantasy.isOnMyTeam = false;
        setData(updatedData);
    }

    if (data != null) {
    return (
        <div className="page">
            <Header />
            <Navbar />
            <div className="page-wrapper">
                <div className="page-body">
                    <div className="container-xl">
                        <div className="card">
                            <div className="row g-0">
                                <div className="col-12 col-md-12 d-flex flex-column">
                                    <div className="card-body">
                                        <h2 className="mb-4">{data.sleeper.fullName} <span className="badge bg-secondary">{data.sleeper.position}</span></h2>
                                        
                                        <div className="row align-items-center">
                                            <div className="col-auto">
                                                <span className="avatar avatar-xl" style={{backgroundImage: "url(" + data.pros.playerImageUrl + ")"}}></span>
                                            </div>
                                        </div>
                                        <div className="row align-items-center mt-3">
                                            <td className="text-end">
                                                <div className="btn-list">
                                                    <ThumbsUpButton isThumbsUp={data.fantasy.isThumbsUp} onClickThumbsUp={onClickThumbsUp} />
                                                    <ThumbsDownButton isThumbsDown={data.fantasy.isThumbsDown} onClickThumbsDown={thumbsDown} />
                                                    <ClaimPlayerButton isOnMyTeam={data.fantasy.isOnMyTeam} isTaken={data.fantasy.isTaken} claimPlayer={claimPlayer} />
                                                    <ReleasePlayerButton isOnMyTeam={data.fantasy.isOnMyTeam} isTaken={data.fantasy.isTaken} releasePlayer={releasePlayer} />
                                                    <AssignPlayerButton isOnMyTeam={data.fantasy.isOnMyTeam} isTaken={data.fantasy.isTaken} assignPlayer={assignPlayer} />
                                                </div>
                                            </td>
                                        </div>
                                        <h2 className="mt-4">Player Data</h2>
                                        <div className="row g-3 mb-3">
                                            <div className="col">
                                                <div className="card card-sm">
                                                    <div className="card-body">
                                                        <div className="row align-items-center">
                                                            <div className="col-auto">
                                                                {data.sleeper.injuryStatus == null ? <span className="avatar text-success"><HeartCheck /></span> : <span className="avatar text-danger"><MoodSick /></span>}
                                                            </div>
                                                            <div className="col">
                                                                <div className="font-weight-medium">{data.sleeper.injuryStatus == null ? "Healthy" : data.sleeper.injuryStatus}</div>
                                                                <div className="text-secondary">{data.sleeper.injuryBodyPart}</div>
                                                            </div>
                                                            <div className="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className="col">
                                                <div className="card card-sm">
                                                    <div className="card-body">
                                                        <div className="row align-items-center">
                                                            <div className="col-auto">
                                                                <span className="bg-orange text-white avatar">ADP</span>
                                                            </div>
                                                            <div className="col">
                                                                <div className="font-weight-medium">{data.sportsdata.averageDraftPositionPPR}</div>
                                                                <div className="text-secondary">Average Draft Position</div>
                                                            </div>
                                                            <div className="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className="col">
                                                <div className="card card-sm">
                                                    <div className="card-body">
                                                        <div className="row align-items-center">
                                                            <div className="col-auto">
                                                                <span className="bg-green text-white avatar"><CurrencyDollar /></span>
                                                            </div>
                                                            <div className="col">
                                                                <div className="font-weight-medium">{data.sportsdata.auctionValuePPR}</div>
                                                                <div className="text-secondary">Auction Value</div>
                                                            </div>
                                                            <div className="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="row g-3">
                                            <div className="col-md-6 col-xl-4">
                                                <div className="card card-sm">
                                                    <div className="card-body">
                                                        <div className="row align-items-center">
                                                            <div className="col-auto">
                                                                <span className="bg-gray-500 text-dark avatar"><ListNumbers /></span>
                                                            </div>
                                                            <div className="col">
                                                                <div className="font-weight-medium">{data.pros.rankEcr}</div>
                                                                <div className="text-secondary">Rank (ECR)</div>
                                                            </div>
                                                            <div className="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className="col-md-6 col-xl-4">
                                                <div className="card card-sm">
                                                    <div className="card-body">
                                                        <div className="row align-items-center">
                                                            <div className="col-auto">
                                                                <span className="bg-gray-500 text-dark avatar"><Trophy /></span>
                                                            </div>
                                                            <div className="col">
                                                                <div className="font-weight-medium">{data.pros.tier}</div>
                                                                <div className="text-secondary">Tier</div>
                                                            </div>
                                                            <div className="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="card">
                            <div className="card-header">
                                <h3 className="card-title">Raw Player Data</h3>
                            </div>
                            <div className="card-body">
                                <div className="datagrid">
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Position</div>
                                        <div className="datagrid-content">{data.sleeper.position}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Position Rank</div>
                                        <div className="datagrid-content">{data.sleeper.posRank}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Depth Chart Order</div>
                                        <div className="datagrid-content">{data.sleeper.depthChartOrder}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Age</div>
                                        <div className="datagrid-content">{data.sleeper.age}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Status</div>
                                        <div className="datagrid-content">{data.sleeper.status}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Injury Status</div>
                                        <div className="datagrid-content">{data.sleeper.injuryStatus}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Injury Body Part</div>
                                        <div className="datagrid-content">{data.sleeper.injuryBodyPart}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Team Name</div>
                                        <div className="datagrid-content">{data.sleeper.team == null ? "" : data.sleeper.team.teamName}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">College</div>
                                        <div className="datagrid-content">{data.sleeper.college}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Search Rank</div>
                                        <div className="datagrid-content">{data.sleeper.searchRank}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPosition}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position PPR</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPositionPPR}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position IDP</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPositionIDP}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position Rookie</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPositionRookie}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position Dynasty</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPositionDynasty}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Average Draft Position 2QB</div>
                                        <div className="datagrid-content">{data.sportsdata.averageDraftPosition2QB}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Bye Week</div>
                                        <div className="datagrid-content">{data.pros.playerByeWeek}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Last Season Fantasy Points</div>
                                        <div className="datagrid-content">{data.sportsdata.lastSeasonFantasyPoints}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Projected Fantasy Points</div>
                                        <div className="datagrid-content">{data.sportsdata.projectedFantasyPoints}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Auction Value</div>
                                        <div className="datagrid-content">${data.sportsdata.auctionValue}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Auction Value PPR</div>
                                        <div className="datagrid-content">{data.sportsdata.auctionValuePPR}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Player Owned Average</div>
                                        <div className="datagrid-content">{data.pros.playerOwnedAvg}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Player Owned ESPN</div>
                                        <div className="datagrid-content">{data.pros.playerOwnedEspn}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Player Owned Yahoo</div>
                                        <div className="datagrid-content">{data.pros.playerOwnedYahoo}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Rank ECR</div>
                                        <div className="datagrid-content">{data.pros.rankEcr}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Rank Min</div>
                                        <div className="datagrid-content">{data.pros.rankMin}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Rank Max</div>
                                        <div className="datagrid-content">{data.pros.rankMax}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Rank Ave</div>
                                        <div className="datagrid-content">{data.pros.rankAve}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Rank Std</div>
                                        <div className="datagrid-content">{data.pros.rankStd}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Tier</div>
                                        <div className="datagrid-content">{data.pros.tier}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="card">
                            <div className="card-header">
                                <h3 className="card-title">API Data</h3>
                            </div>
                            <div className="card-body">
                                <div className="datagrid">
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Sleeper Id</div>
                                        <div className="datagrid-content">{data.sleeper.playerId}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">SportRadar Id</div>
                                        <div className="datagrid-content">{data.sleeper.sportRadarId}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Yahoo Id</div>
                                        <div className="datagrid-content">{data.sleeper.yahooId}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">SportsData.io Key</div>
                                        <div className="datagrid-content">{data.sportsdata.playerID}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">FantasyPros Player Id</div>
                                        <div className="datagrid-content">{data.pros.playerId}</div>
                                    </div>

                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Last Updated Sleeper</div>
                                        <div className="datagrid-content">
                                            <ClockCog></ClockCog> {data.sleeper.lastUpdated}
                                        </div>
                                    </div>

                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Last Updated SportsData.io</div>
                                        <div className="datagrid-content">
                                            <ClockCog></ClockCog> {data.sportsdata.lastUpdated}
                                        </div>
                                    </div>

                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Last Updated FantasyPros</div>
                                        <div className="datagrid-content">
                                            <ClockCog></ClockCog> {data.pros.lastUpdated}
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div className="card">
                            <div className="card-header">
                                <h3 className="card-title">Draft Data</h3>
                            </div>
                            <div className="card-body">
                                <div className="datagrid">
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Picked By</div>
                                        <div className="datagrid-content">{data.fantasy.pickedBy}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Pick Number</div>
                                        <div className="datagrid-content">{data.fantasy.pickNumber}</div>
                                    </div>
                                    <div className="datagrid-item">
                                        <div className="datagrid-title">Pick Round</div>
                                        <div className="datagrid-content">{data.fantasy.pickRound}</div>
                                    </div>
                                </div>
                            </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        );
    }
    else {
        return (
            <div className="page">
                <header className="navbar navbar-expand-sm navbar-light d-print-none">
                    <div className="container-xl">
                        <h1 className="navbar-brand navbar-brand-autodark d-none-navbar-horizontal pe-0 pe-md-3">🏈 Fantasy Football Manager</h1>
                        <div className="navbar-nav flex-row order-md-last">
                        <div className="nav-item d-none d-md-flex me-3">
                            <div className="btn-list">
                            <a href="https://github.com/tabler/tabler" className="btn" target="_blank" rel="noreferrer">
                                <svg xmlns="http://www.w3.org/2000/svg" className="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 19c-4.3 1.4 -4.3 -2.5 -6 -3m12 5v-3.5c0 -1 .1 -1.4 -.5 -2c2.8 -.3 5.5 -1.4 5.5 -6a4.6 4.6 0 0 0 -1.3 -3.2a4.2 4.2 0 0 0 -.1 -3.2s-1.1 -.3 -3.5 1.3a12.3 12.3 0 0 0 -6.2 0c-2.4 -1.6 -3.5 -1.3 -3.5 -1.3a4.2 4.2 0 0 0 -.1 3.2a4.6 4.6 0 0 0 -1.3 3.2c0 4.6 2.7 5.7 5.5 6c-.6 .6 -.6 1.2 -.5 2v3.5" /></svg>
                                Source code
                            </a>
                            <a href="https://github.com/sponsors/codecalm" className="btn" target="_blank" rel="noreferrer">
                                <svg xmlns="http://www.w3.org/2000/svg" className="icon text-pink" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M19.5 12.572l-7.5 7.428l-7.5 -7.428a5 5 0 1 1 7.5 -6.566a5 5 0 1 1 7.5 6.572" /></svg>
                                Sponsor
                            </a>
                            </div>
                        </div>
                        </div>
                    </div>
                </header>
                <Navbar />
                <div className="page-wrapper">
                    <div className="page-body">
                        <div className="container-xl d-flex flex-column justify-content-center">
                            <div className="empty">
                                <div className="empty-img"><img src="./static/illustrations/undraw_printing_invoices_5r4r.svg" height="128" alt="" /></div>
                                <p className="empty-title">For some reason, no player was found.</p>
                                <p className="empty-subtitle text-secondary">Try making sure shit isn't broken.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}