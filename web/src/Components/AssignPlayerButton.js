export default function AssignPlayerButton(props)
{
    if(props.isOnMyTeam == false && props.isTaken == false)
    {
        return (<a class="btn btn-primary" onClick={props.assignPlayer}>Assign</a>);
    }
    
}