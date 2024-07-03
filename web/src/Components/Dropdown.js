export default function Dropdown(props)
{
    return (
        <div className="dropdown">
            <a href="#" className="btn dropdown-toggle" data-bs-toggle="dropdown">{props.dropdownName}</a>
            <div className="dropdown-menu">
                {props.children}
            </div>
        </div>
    );
}