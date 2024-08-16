
import {ReactComponent as VersionIcon} from '../Components/Icons/AarowIteration.svg';

export default function Version(props)
{
    return (
        <div className="col-6 col-sm-4 col-md-2 col-xl-auto py-3">
            <a href="#" className="btn w-100">
                <VersionIcon className="text-success" /> 1.1.0
            </a>
      </div>
    )
}