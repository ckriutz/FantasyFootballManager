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
        fetch(process.env.REACT_APP_API_URL + "/simplefantasyplayers", {
            headers: {
                "Content-Type": "application/json",
              },
        })
        .then(function(response) {
            if (response.ok) {
                return response.json();
            }
        })
        .then((data) => setData(data))
    }, []);

    if(data === undefined) return (<div>Loading...</div>);

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
        console.log(playerItem);
        const updatedData = data.map((item) => {
            if (item.sleeperId === playerItem.sleeperId) {
              return { ...item, player: playerItem };
            }
            return item;
        });
        setData(updatedData);
    };

    function addFilter(position) {
        if (position === "ALL" || position == null) {
            fetch(process.env.REACT_APP_API_URL + "/simplefantasyplayers")
            .then((response) => response.json())
            .then((data) => setData(data));
            return;
        }
        else {
            fetch(process.env.REACT_APP_API_URL + "/fantasyplayers/search/position/" + position)
            .then((response) => response.json())
            .then((data) => setData(data));
        }

    };

    return (
        <div className="page">
            <Header />
            <Navbar />
            <div className="page-wrapper">
                <div className="page-body">
                    <div className="container-xl">
                        <div className="col-12">
                            <div className="card">
                                <div className="card-header">
                                    <h3 className="card-title">Players <span className="badge bg-green">{data != null ? data.length : 0}</span></h3>
                                </div>
                                <div className="card-body border-bottom py-3">
                                    <div className="d-flex">
                                        <Dropdown dropdownName="Position">
                                            <DropdownItem title="ALL" addFilter={addFilter} />
                                            <DropdownDivider />
                                            <DropdownItem title="QB" addFilter={addFilter} />
                                            <DropdownItem title="RB" addFilter={addFilter} />
                                            <DropdownItem title="WR" addFilter={addFilter} />
                                            <DropdownItem title="TE" addFilter={addFilter} />
                                            <DropdownItem title="K" addFilter={addFilter} />
                                        </Dropdown>
                                        <div className="mb-3 mt-2 px-3">
                                            <div className="form-label">
                                            <label className="form-check pl-3">
                                                {/*<input className="form-check-input" type="checkbox" checked={!includeLosers} onClick={HandleLosers} />
                                                <span className="form-check-label">Remove Losers</span>*/}
                                            </label>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                                <div className="table-responsive">
                                    <table className="table card-table table-vcenter text-nowrap datatable">
                                        <thead>
                                            <tr>
                                                <th className="w-1">Sleeper Id</th>
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
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}