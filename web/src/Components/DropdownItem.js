export default function DropdownItem(props)
{
    return (
        <a class="dropdown-item" onClick={() => props.addFilter(props.title)}>{props.title}</a>
    );
}