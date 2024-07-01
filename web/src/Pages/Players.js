import { useState, useEffect } from 'react';
import Header from '../Components/Header'
import Navbar from '../Components/Navbar'
import PlayerRow from '../Components/PlayerRow';

import Dropdown from '../Components/Dropdown';
import DropdownItem from '../Components/DropdownItem';
import DropdownDivider from '../Components/DropdownDivider';

export default function Players()
{

    const [data, setData] = useState(null);
    const [includeLosers, setIncludeLosers] = useState(false);

    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/fantasyplayers")
        .then((response) => response.json())
        .then((data) => setData(data));
    }, []);


    const filterLosers = (players) => {
        if(data == null) return;
        if (includeLosers) {
            return players;
        }
        else {
            return players.filter(player => player.rankEcr > 0);
        }
    };

    function HandleLosers(e) {
        setIncludeLosers(!includeLosers);
    }

    const updateData = (playerItem) => {
        const updatedData = data.map((item) => {
            if (item.sleeperId === playerItem.sleeperId) {
              return { ...item, player: playerItem };
            }
            return item;
        });
        setData(updatedData);
    };

    function addFilter(position) {
        if (position == "ALL" || position == null) {
            fetch(process.env.REACT_APP_API_URL + "/fantasyplayers")
            .then((response) => response.json())
            .then((data) => setData(data));
            return;
        }
        else {
            fetch(process.env.REACT_APP_API_URL + "/fantasyplayers/search/Position:" + position)
            .then((response) => response.json())
            .then((data) => setData(data));
        }

    };

    return (
        <div class="page">
            <Header />
            <Navbar />
            <div class="page-wrapper">
                <div class="page-body">
                    <div class="container-xl">
                        <div class="col-12">
                            <div class="card">
                                <div class="card-header">
                                    <h3 class="card-title">Players <span class="badge bg-green">{data != null ? filterLosers(data).length : 0}</span></h3>
                                </div>
                                <div class="card-body border-bottom py-3">
                                    <div class="d-flex">
                                        <Dropdown dropdownName="Position">
                                            <DropdownItem title="ALL" addFilter={addFilter} />
                                            <DropdownDivider />
                                            <DropdownItem title="QB" addFilter={addFilter} />
                                            <DropdownItem title="RB" addFilter={addFilter} />
                                            <DropdownItem title="WR" addFilter={addFilter} />
                                            <DropdownItem title="TE" addFilter={addFilter} />
                                            <DropdownItem title="K" addFilter={addFilter} />
                                        </Dropdown>
                                        <div class="mb-3 mt-2 px-3">
                                            <div class="form-label">
                                            <label class="form-check pl-3">
                                                <input class="form-check-input" type="checkbox" checked={!includeLosers} onClick={HandleLosers} />
                                                <span class="form-check-label">Remove Losers</span>
                                            </label>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="table-responsive">
                                    <table class="table card-table table-vcenter text-nowrap datatable">
                                        <thead>
                                            <tr>
                                                <th class="w-1">Sleeper Id</th>
                                                <th></th>
                                                <th>Name</th>
                                                <th>Position</th>
                                                <th>Depth</th>
                                                <th>Team</th>
                                                <th>Bye</th>
                                                <th>Rank</th>
                                                <th>ADP/PPR</th>
                                                <th>Proj. Points</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {filterLosers(data) && filterLosers(data.sort(function(a, b) {return a.rankEcr - b.rankEcr} )).map((item) => (
                                                
                                                <PlayerRow player={item} updateData={updateData} />
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                                <div class="card-footer d-flex align-items-center">
                                    <ul class="pagination m-0 ms-auto"> 
                                        <li class="page-item"><a class="page-link" href="#">1</a></li>
                                        <li class="page-item active"><a class="page-link" href="#">2</a></li>
                                        <li class="page-item"><a class="page-link" href="#">3</a></li>
                                        <li class="page-item"><a class="page-link" href="#">4</a></li>
                                        <li class="page-item"><a class="page-link" href="#">5</a></li>
                                        <li class="page-item">
                                            <a class="page-link" href="#">
                                                next
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 6l6 6l-6 6" /></svg>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}