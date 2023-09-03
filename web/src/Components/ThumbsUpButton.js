import {ReactComponent as ThumbsUp} from '../Components/Icons/ThumbsUp.svg'
import {ReactComponent as ThumbsUpFilled} from '../Components/Icons/ThumbsUpFilled.svg'

export default function ThumbsUpButton(props)
{
    if(props.isThumbsUp)
    {
        return (
            <button type="button" class="btn btn-icon" onClick={props.onClickThumbsUp}>
                <ThumbsUpFilled  /> 
            </button>
            
        )
    }
    else
    {
        return (
            <button type="button" class="btn btn-icon" onClick={props.onClickThumbsUp}>
                <ThumbsUp  />
            </button>
        )
    }
    
}