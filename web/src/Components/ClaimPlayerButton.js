export default function ClaimPlayerButton(props)
{
    if(props.isOnMyTeam == false && props.isTaken == false)
    {
        return (<a class="btn btn-success" onClick={props.claimPlayer}>Claim</a>);
    }
    
}