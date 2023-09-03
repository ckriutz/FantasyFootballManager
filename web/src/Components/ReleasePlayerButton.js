export default function ClaimPlayerButton(props)
{
    if(props.isOnMyTeam == true || props.isTaken == true)
    {
        return (<a class="btn btn-danger" onClick={props.releasePlayer}>Release</a>);
    }
    
}