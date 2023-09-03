import {ReactComponent as Heartbeat} from '../Components/Icons/Heartbeat.svg';

export default function ShowApiHealth(props)
{
    if(props.Health == true)
    {
        return (
            <div class="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                <a href="#" class="btn w-100">
                    <Heartbeat class="text-success" /> Healthy
                </a>
          </div>

        )
    }
    else {
        return (
            <div class="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
                <a href="#" class="btn w-100">
                    <Heartbeat class="text-danger" /> Unhealthy
                </a>
            </div>
        )
    }
}