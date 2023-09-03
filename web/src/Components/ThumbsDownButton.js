import {ReactComponent as ThumbsDown} from '../Components/Icons/ThumbsDown.svg'
import {ReactComponent as ThumbsDownFilled} from '../Components/Icons/ThumbsDownFilled.svg'

export default function ThumbsDownButton(props)
{
    if(props.isThumbsDown)
    {
        return (
            <button type="button" class="btn btn-icon" onClick={props.onClickThumbsDown}>
                <ThumbsDownFilled  /> 
            </button>
            
        )
    }
    else
    {
        return (
            <button type="button" class="btn btn-icon" onClick={props.onClickThumbsDown}>
                <ThumbsDown  />
            </button>
        )
    }
    
}