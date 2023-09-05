export default function Dropdown(props)
{
    return (
        <div class="dropdown">
            <a href="#" class="btn dropdown-toggle" data-bs-toggle="dropdown">{props.dropdownName}</a>
            <div class="dropdown-menu">
                {props.children}
            </div>
        </div>
    );
}