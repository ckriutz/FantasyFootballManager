import React, { useState } from 'react';
import { Link } from "react-router-dom";

import ClaimPlayerButton from './ClaimPlayerButton';
import ReleasePlayerButton from './ReleasePlayerButton';
import AssignPlayerButton from './AssignPlayerButton';

import {ReactComponent as TrendingUp} from '../Components/Icons/TrendingUp.svg';
import {ReactComponent as TrendingDown} from '../Components/Icons/TrendingDown.svg';
import {ReactComponent as ThumbsUpFilled} from '../Components/Icons/ThumbsUpFilled.svg'
import {ReactComponent as ThumbsDownFilled} from '../Components/Icons/ThumbsDownFilled.svg'

export default function PlayerRow(props)
{
    function claimPlayer(e) {
        console.log("Claiming player " + props.player.name + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/claim/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.fantasyPlayer.isTaken = false;
        props.player.fantasyPlayer.isOnMyTeam = true;

        props.updateData(props.player);
    }

    function assignPlayer(e) {       
        console.log("Assigning player " + props.player.name + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/assign/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.fantasyPlayer.isTaken = true;
        props.player.fantasyPlayer.isOnMyTeam = false;

        props.updateData(props.player);
    }
    
    function releasePlayer(e) {
        console.log("Resetting player " + props.player.name + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/fantasyplayer/reset/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.fantasyPlayer.isTaken = false;
        props.player.fantasyPlayer.isOnMyTeam = false;

        props.updateData(props.player);
    }

    const TrendingUpData = (proj, diff) => {
        return ( diff > 0 ? <span>{proj}<TrendingUp class="text-success" /></span> : <span>{proj}<TrendingDown class="text-danger" /></span> );
    };

    return (
        <tr className={props.player.fantasyPlayer.isTaken === true ? "table-secondary" : props.player.fantasyPlayer.isOnMyTeam === true ? "table-success" : ""}>
            <td><span className="text-secondary">{props.player.sleeperId}</span></td>
            <td>
                {props.player.fantasyPlayer.isThumbsUp && <ThumbsUpFilled />}
                {props.player.fantasyPlayer.isThumbsDown && <ThumbsDownFilled />}
            </td>
            
            <td><Link className="text-reset" to={`/player/${props.player.sleeperId}`}>{props.player.name}</Link></td>
            <td>{props.player.position}</td>
            <td>{props.player.depth}</td>
            <td>{props.player.team == null ? <span></span> : props.player.team.abbreviation}</td>
            <td>{props.player.byeWeek}</td>
            <td>{props.player.rankEcr}</td>
            <td>{props.player.adpPpr}</td>
            <td>{TrendingUpData(props.player.projPoints, props.player.projPoints - props.player.lastSeasonProjPoints)}</td>
            <td className="text-end">
                <div className="btn-list">
                    <ClaimPlayerButton isOnMyTeam={props.player.fantasyPlayer.isOnMyTeam} isTaken={props.player.fantasyPlayer.isTaken} claimPlayer={claimPlayer} />
                    <ReleasePlayerButton isOnMyTeam={props.player.fantasyPlayer.isOnMyTeam} isTaken={props.player.fantasyPlayer.isTaken} releasePlayer={releasePlayer} />
                    <AssignPlayerButton isOnMyTeam={props.player.fantasyPlayer.isOnMyTeam} isTaken={props.player.fantasyPlayer.isTaken} assignPlayer={assignPlayer} />
                </div>
            </td>
        </tr>
    );
}


