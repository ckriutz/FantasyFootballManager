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
        console.log("Claiming player " + props.player.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/FantasyPlayer/claim/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.isTaken = false;
        props.player.isOnMyTeam = true;

        props.updateData(props.player);
    }

    function assignPlayer(e) {       
        console.log("Assigning player " + props.player.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/FantasyPlayer/assign/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.isTaken = true;
        props.player.isOnMyTeam = false;

        props.updateData(props.player);
    }
    
    function releasePlayer(e) {
        console.log("Resetting player " + props.player.fullName + "...");
        e.preventDefault();
        fetch(process.env.REACT_APP_API_URL + '/FantasyPlayer/release/' + props.player.sleeperId, {
            method: 'POST'
        });
        props.player.isTaken = false;
        props.player.isOnMyTeam = false;

        props.updateData(props.player);
    }

    const TrendingUpData = (proj, diff) => {
        return ( diff > 0 ? <span>{proj}<TrendingUp class="text-success" /></span> : <span>{proj}<TrendingDown class="text-danger" /></span> );
    };

    return (
        <tr className={props.player.isTaken == true ? "table-secondary" : props.player.isOnMyTeam == true ? "table-success" : ""}>
            <td><span class="text-secondary">{props.player.sleeperId}</span></td>
            <td>
                {props.player.isThumbsUp && <ThumbsUpFilled />}
                {props.player.isThumbsDown && <ThumbsDownFilled />}
            </td>
            
            <td><Link class="text-reset" to={`/player/${props.player.sleeperId}`}>{props.player.fullName}</Link></td>
            <td>{props.player.position}</td>
            <td>{props.player.depthChartOrder}</td>
            <td>{props.player.teamName}</td>
            <td>{props.player.rankEcr}</td>
            <td>{TrendingUpData(props.player.projectedFantasyPoints, props.player.projectedFantasyPoints - props.player.lastSeasonFantasyPoints)}</td>
            <td class="text-end">
                <div class="btn-list">
                    <ClaimPlayerButton isOnMyTeam={props.player.isOnMyTeam} isTaken={props.player.isTaken} claimPlayer={claimPlayer} />
                    <ReleasePlayerButton isOnMyTeam={props.player.isOnMyTeam} isTaken={props.player.isTaken} releasePlayer={releasePlayer} />
                    <AssignPlayerButton isOnMyTeam={props.player.isOnMyTeam} isTaken={props.player.isTaken} assignPlayer={assignPlayer} />
                </div>
            </td>
        </tr>
    );
}


