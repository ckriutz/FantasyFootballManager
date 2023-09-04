import { formatDistanceToNowStrict } from 'date-fns';

export default function DataStatusCard(props)
{
    return (
        <div class="card">
            <div class="card-body">
                <div class="row align-items-center">
                    <div class="col-auto">
                        <span class="bg-blue text-white avatar">
                            <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-timeline-event" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                            <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                            <path d="M12 20m-2 0a2 2 0 1 0 4 0a2 2 0 1 0 -4 0"></path>
                            <path d="M10 20h-6"></path>
                            <path d="M14 20h6"></path>
                            <path d="M12 15l-2 -2h-3a1 1 0 0 1 -1 -1v-8a1 1 0 0 1 1 -1h10a1 1 0 0 1 1 1v8a1 1 0 0 1 -1 1h-3l-2 2z"></path>
                            </svg>
                        </span>
                    </div>
                    <div class="col">
                      <h3>{props.dataSource}</h3>
                      <div class="text-secondary">Updated {formatDistanceToNowStrict(new Date(props.lastUpdated))} ago</div>
                    </div>
                    <div class="col-auto"></div>
                </div>
            </div>
        </div>
    )
}