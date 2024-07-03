import { Link } from "react-router-dom";
import {ReactComponent as TrendingUp} from '../Components/Icons/TrendingUp.svg';
import {ReactComponent as TrendingDown} from '../Components/Icons/TrendingDown.svg';

export default function PlayerCard(props)
{
    const TrendingUpData = (proj, diff) => {
        return ( diff > 0 ? <span>{proj}<TrendingUp class="text-success" /></span> : <span>{proj}<TrendingDown class="text-danger" /></span> );
    };

    return (
        <div className="list-group-item active">
            <div className="row align-items-center">
                <div className="col-auto">
                    <a href="#"><span className="avatar">{props.player.pros.playerPositionId}</span></a>
                </div>
                <div className="col text-truncate">
                    <a href="#" class="text-reset d-block"><b><Link class="text-reset" to={`/player/${props.player.sleeper.sleeperId}`}>{props.player.sleeper.fullName}</Link></b></a>
                    <div className="d-block text-secondary text-truncate mt-n1">{props.player.sleeper.team.abbreviation} | Depth Chart Order: {props.player.sleeper.depthChartOrder} | Projected Fantasy Points: {TrendingUpData(props.player.sportsdata.projectedFantasyPoints, props.player.sportsdata.projectedFantasyPoints - props.player.sportsdata.lastSeasonFantasyPoints)}</div>
                </div>
            </div>
        </div>
    )
}   