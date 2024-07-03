export default function AssignPlayerButton(props)
{
    if(props.isOnMyTeam === false && props.isTaken === false)
    {
        return (<a className="btn btn-primary" onClick={props.assignPlayer}>Assign</a>);
    }
    
}