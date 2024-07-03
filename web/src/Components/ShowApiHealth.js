import {ReactComponent as Heartbeat} from '../Components/Icons/Heartbeat.svg';

export default function ShowApiHealth(props)
{
    if(props.Health == true)
    {
        return (
            <div className="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                <a href="#" className="btn w-100">
                    <Heartbeat className="text-success" /> Healthy
                </a>
          </div>

        )
    }
    else {
        return (
            <div className="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                <a href="#" className="btn w-100">
                    <Heartbeat className="text-danger" /> Unhealthy
                </a>
            </div>
        )
    }
}