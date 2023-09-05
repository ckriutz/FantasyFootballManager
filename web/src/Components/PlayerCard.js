import { Link } from "react-router-dom";
import Teams from './teams.json';
import {ReactComponent as TrendingUp} from '../Components/Icons/TrendingUp.svg';
import {ReactComponent as TrendingDown} from '../Components/Icons/TrendingDown.svg';

export default function PlayerCard(props)
{
    function getTeamAbbr(teamId) {
        if (teamId == null) return "";
        var team = Teams.find(t=> t.Id === teamId);
        return team.Name;
    }

    const TrendingUpData = (proj, diff) => {
        return ( diff > 0 ? <span>{proj}<TrendingUp class="text-success" /></span> : <span>{proj}<TrendingDown class="text-danger" /></span> );
    };

    return (
        <div class="list-group-item active">
            <div class="row align-items-center">
                <div class="col-auto">
                    <a href="#"><span class="avatar">{props.player.position}</span></a>
                </div>
                <div class="col text-truncate">
                    <a href="#" class="text-reset d-block"><b><Link class="text-reset" to={`/player/${props.player.sleeperId}`}>{props.player.fullName}</Link></b></a>
                    <div class="d-block text-secondary text-truncate mt-n1">{getTeamAbbr(props.player.teamId)} | Depth Chart Order: {props.player.depthChartOrder} | Projected Fantasy Points: {TrendingUpData(props.player.projectedFantasyPoints, props.player.projectedFantasyPoints - props.player.lastSeasonFantasyPoints)}</div>
                </div>
            </div>
        </div>
    )
}   