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

import Teams from '../Components/teams.json';

export default function Players()
{
    const { id } = useParams();

    const url = process.env.REACT_APP_API_URL + "/fantasyplayer/" + id;

    const [data, setData] = useState(null);
    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/fantasyplayer/" + id)
        .then((response) => response.json())
        .then((data) => setData(data));
    }, [])

    //const updateData = (playerItem) => {
    //    const updatedData = { ...data }
    //    updatedData = playerItem;
    //    setData(updatedData);
    //};


    function onClickThumbsUp(e) {
        e.preventDefault();
        
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/thumbsup/' + data.sleeperId, {
            method: 'POST'
        });

        if(data.isThumbsUp)
        {
            console.log("Setting player " + data.fullName + "to nutral...");
            data.isThumbsUp = false;
        }
        else
        {
            console.log("Liking player " + data.fullName + "...");
            data.isThumbsUp = true;
        }

        const updatedData = { ...data }
        updatedData.isThumbsUp = data.isThumbsUp;
        setData(updatedData);

    }

    function thumbsDown(e)
    {
        e.preventDefault();
        
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/thumbsdown/' + data.sleeperId, {
            method: 'POST'
        });

        if(data.isThumbsDown)
        {
            console.log("Setting player " + data.fullName + "to nutral...");
            data.isThumbsDown = false;
        }
        else
        {
            console.log("Disliking player " + data.fullName + "...");
            data.isThumbsDown = true;
        }

        const updatedData = { ...data }
        updatedData.isThumbsDown = data.isThumbsDown;
        setData(updatedData);
    }

    function claimPlayer(e) {
        console.log("Claiming player " + data.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/claim/' + data.sleeperId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.isTaken = false;
        updatedData.isOnMyTeam = true;
        setData(updatedData);

    }

    function assignPlayer(e) {       
        console.log("Assigning player " + data.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/assign/' + data.sleeperId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.isTaken = true;
        updatedData.isOnMyTeam = false;
        setData(updatedData);
    }
    
    function releasePlayer(e) {
        console.log("Resetting player " + data.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/release/' + data.sleeperId, {
            method: 'POST'
        });

        const updatedData = { ...data }
        updatedData.isTaken = false;
        updatedData.isOnMyTeam = false;
        setData(updatedData);
    }

    function getTeamAbbr(teamId) {
        if (teamId == null) return "";
        var team = Teams.find(t=> t.Id === teamId);
        return team.Name;

    }

    if (data != null) {
    return (
        <div class="page">
            <Header />
            <Navbar />
            <div class="page-wrapper">
                <div class="page-body">
                    <div class="container-xl">
                        <div class="card">
                            <div class="row g-0">
                                <div class="col-12 col-md-9 d-flex flex-column">
                                    <div class="card-body">
                                        <h2 class="mb-4">{data.fullName} <span class="badge bg-secondary">{data.position}</span></h2>
                                        
                                        <div class="row align-items-center">
                                            <div class="col-auto">
                                                <span class="avatar avatar-xl" style={{backgroundImage: "url(" + data.playerImageUrl + ")"}}></span>
                                            </div>
                                        </div>
                                        <div class="row align-items-center mt-3">
                                            <td class="text-end">
                                                <div class="btn-list">
                                                    <ThumbsUpButton isThumbsUp={data.isThumbsUp} onClickThumbsUp={onClickThumbsUp} />
                                                    <ThumbsDownButton isThumbsDown={data.isThumbsDown} onClickThumbsDown={thumbsDown} />
                                                    <ClaimPlayerButton isOnMyTeam={data.isOnMyTeam} isTaken={data.isTaken} claimPlayer={claimPlayer} />
                                                    <ReleasePlayerButton isOnMyTeam={data.isOnMyTeam} isTaken={data.isTaken} releasePlayer={releasePlayer} />
                                                    <AssignPlayerButton isOnMyTeam={data.isOnMyTeam} isTaken={data.isTaken} assignPlayer={assignPlayer} />
                                                </div>
                                            </td>
                                        </div>
                                        <h2 className="mt-4">Player Data</h2>
                                        <div class="row g-3">


                                            <div class="col-md-6 col-xl-4">
                                                <div class="card card-sm">
                                                    <div class="card-body">
                                                        <div class="row align-items-center">
                                                            <div class="col-auto">
                                                                <span class="bg-red text-white avatar">
                                                                    <MoodSick />
                                                                </span>
                                                            </div>
                                                            <div class="col">
                                                                <div class="font-weight-medium">{data.injuryStatus == null ? "Healthy" : data.injuryStatus}</div>
                                                                <div class="text-secondary">{data.injuryBodyPart}</div>
                                                            </div>
                                                            <div class="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6 col-xl-4">
                                                <div class="card card-sm">
                                                    <div class="card-body">
                                                        <div class="row align-items-center">
                                                            <div class="col-auto">
                                                                <span class="bg-orange text-white avatar">ADP</span>
                                                            </div>
                                                            <div class="col">
                                                                <div class="font-weight-medium">{data.averageDraftPositionPPR}</div>
                                                                <div class="text-secondary">Average Draft Position</div>
                                                            </div>
                                                            <div class="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6 col-xl-4">
                                                <div class="card card-sm">
                                                    <div class="card-body">
                                                        <div class="row align-items-center">
                                                            <div class="col-auto">
                                                                <span class="bg-green text-white avatar"><CurrencyDollar /></span>
                                                            </div>
                                                            <div class="col">
                                                                <div class="font-weight-medium">{data.auctionValuePPR}</div>
                                                                <div class="text-secondary">Aucton Value</div>
                                                            </div>
                                                            <div class="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6 col-xl-4">
                                                <div class="card card-sm">
                                                    <div class="card-body">
                                                        <div class="row align-items-center">
                                                            <div class="col-auto">
                                                                <span class="bg-gray-500 text-dark avatar"><ListNumbers /></span>
                                                            </div>
                                                            <div class="col">
                                                                <div class="font-weight-medium">{data.rankEcr}</div>
                                                                <div class="text-secondary">Rank (ECR)</div>
                                                            </div>
                                                            <div class="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6 col-xl-4">
                                                <div class="card card-sm">
                                                    <div class="card-body">
                                                        <div class="row align-items-center">
                                                            <div class="col-auto">
                                                                <span class="bg-gray-500 text-dark avatar"><Trophy /></span>
                                                            </div>
                                                            <div class="col">
                                                                <div class="font-weight-medium">{data.tier}</div>
                                                                <div class="text-secondary">Tier</div>
                                                            </div>
                                                            <div class="col-auto"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card">
                            <div class="card-header">
                                <h3 class="card-title">Raw Player Data</h3>
                            </div>
                            <div class="card-body">
                                <div class="datagrid">
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Position</div>
                                        <div class="datagrid-content">{data.position}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Position Rank</div>
                                        <div class="datagrid-content">{data.posRank}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Depth Chart Order</div>
                                        <div class="datagrid-content">{data.depthChartOrder}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Age</div>
                                        <div class="datagrid-content">{data.age}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Status</div>
                                        <div class="datagrid-content">{data.status}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Injury Status</div>
                                        <div class="datagrid-content">{data.injuryStatus}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Injury Body Part</div>
                                        <div class="datagrid-content">{data.injuryBodyPart}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Team Id</div>
                                        <div class="datagrid-content">{data.teamId}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Team Name</div>
                                        <div class="datagrid-content">{getTeamAbbr(data.teamId)}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">College</div>
                                        <div class="datagrid-content">{data.college}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Search Rank</div>
                                        <div class="datagrid-content">{data.searchRank}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position</div>
                                        <div class="datagrid-content">{data.averageDraftPosition}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position PPR</div>
                                        <div class="datagrid-content">{data.averageDraftPositionPPR}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position IDP</div>
                                        <div class="datagrid-content">{data.averageDraftPositionIDP}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position Rookie</div>
                                        <div class="datagrid-content">{data.averageDraftPositionRookie}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position Dynasty</div>
                                        <div class="datagrid-content">{data.averageDraftPositionDynasty}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Average Draft Position 2QB</div>
                                        <div class="datagrid-content">{data.averageDraftPosition2QB}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Bye Week</div>
                                        <div class="datagrid-content">{data.byeWeek}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Last Season Fantasy Points</div>
                                        <div class="datagrid-content">{data.lastSeasonFantasyPoints}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Projected Fantasy Points</div>
                                        <div class="datagrid-content">{data.projectedFantasyPoints}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Auction Value</div>
                                        <div class="datagrid-content">{data.auctionValue}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Auction Value PPR</div>
                                        <div class="datagrid-content">{data.auctionValuePPR}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Player Owned Average</div>
                                        <div class="datagrid-content">{data.playerOwnedAvg}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Player Owned ESPN</div>
                                        <div class="datagrid-content">{data.playerOwnedEspn}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Player Owned Yahoo</div>
                                        <div class="datagrid-content">{data.playerOwnedYahoo}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Rank ECR</div>
                                        <div class="datagrid-content">{data.rankEcr}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Rank Min</div>
                                        <div class="datagrid-content">{data.rankMin}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Rank Max</div>
                                        <div class="datagrid-content">{data.rankMax}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Rank Ave</div>
                                        <div class="datagrid-content">{data.rankAve}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Rank Std</div>
                                        <div class="datagrid-content">{data.rankStd}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Tier</div>
                                        <div class="datagrid-content">{data.tier}</div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card">
                            <div class="card-header">
                                <h3 class="card-title">API Data</h3>
                            </div>
                            <div class="card-body">
                                <div class="datagrid">
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Sleeper Id</div>
                                        <div class="datagrid-content">{data.sleeperId}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">SportRadar Id</div>
                                        <div class="datagrid-content">{data.sportRadarId}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Yahoo Id</div>
                                        <div class="datagrid-content">{data.yahooId}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">SportsData.io Key</div>
                                        <div class="datagrid-content">{data.sportsDataIoKey}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">FantasyPros Player Id</div>
                                        <div class="datagrid-content">{data.fantasyProsPlayerId}</div>
                                    </div>

                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Last Updated Sleeper</div>
                                        <div class="datagrid-content">
                                            <ClockCog></ClockCog> {data.lastUpdatedSleeper}
                                        </div>
                                    </div>

                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Last Updated SportsData.io</div>
                                        <div class="datagrid-content">
                                            <ClockCog></ClockCog> {data.lastUpdatedSportsDataIo}
                                        </div>
                                    </div>

                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Last Updated FantasyPros</div>
                                        <div class="datagrid-content">
                                            <ClockCog></ClockCog> {data.lastUpdatedSportsDataIo}
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="card">
                            <div class="card-header">
                                <h3 class="card-title">Draft Data</h3>
                            </div>
                            <div class="card-body">
                                <div class="datagrid">
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Picked By</div>
                                        <div class="datagrid-content">{data.pickedBy}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Pick Number</div>
                                        <div class="datagrid-content">{data.pickNumber}</div>
                                    </div>
                                    <div class="datagrid-item">
                                        <div class="datagrid-title">Pick Round</div>
                                        <div class="datagrid-content">{data.pickRound}</div>
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
            <div class="page">
                <header class="navbar navbar-expand-sm navbar-light d-print-none">
                    <div class="container-xl">
                        <h1 class="navbar-brand navbar-brand-autodark d-none-navbar-horizontal pe-0 pe-md-3">🏈 Fantasy Football Manager</h1>
                        <div class="navbar-nav flex-row order-md-last">
                        <div class="nav-item d-none d-md-flex me-3">
                            <div class="btn-list">
                            <a href="https://github.com/tabler/tabler" class="btn" target="_blank" rel="noreferrer">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 19c-4.3 1.4 -4.3 -2.5 -6 -3m12 5v-3.5c0 -1 .1 -1.4 -.5 -2c2.8 -.3 5.5 -1.4 5.5 -6a4.6 4.6 0 0 0 -1.3 -3.2a4.2 4.2 0 0 0 -.1 -3.2s-1.1 -.3 -3.5 1.3a12.3 12.3 0 0 0 -6.2 0c-2.4 -1.6 -3.5 -1.3 -3.5 -1.3a4.2 4.2 0 0 0 -.1 3.2a4.6 4.6 0 0 0 -1.3 3.2c0 4.6 2.7 5.7 5.5 6c-.6 .6 -.6 1.2 -.5 2v3.5" /></svg>
                                Source code
                            </a>
                            <a href="https://github.com/sponsors/codecalm" class="btn" target="_blank" rel="noreferrer">
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon text-pink" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M19.5 12.572l-7.5 7.428l-7.5 -7.428a5 5 0 1 1 7.5 -6.566a5 5 0 1 1 7.5 6.572" /></svg>
                                Sponsor
                            </a>
                            </div>
                        </div>
                        </div>
                    </div>
                </header>
                <Navbar />
                <div class="page-wrapper">
                    <div class="page-body">
                        <div class="container-xl d-flex flex-column justify-content-center">
                            <div class="empty">
                                <div class="empty-img"><img src="./static/illustrations/undraw_printing_invoices_5r4r.svg" height="128" alt="" /></div>
                                <p class="empty-title">For some reason, no player was found.</p>
                                <p class="empty-subtitle text-secondary">Try making sure shit isn't broken.</p>
                                <div class="empty-action">
                                    <a href="./." class="btn btn-primary">
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 5l0 14" /><path d="M5 12l14 0" /></svg>
                                        Add your first client
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}