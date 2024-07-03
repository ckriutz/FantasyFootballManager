export default function DropdownItem(props)
{
    return (
        <a className="dropdown-item" onClick={() => props.addFilter(props.title)}>{props.title}</a>
    );
}