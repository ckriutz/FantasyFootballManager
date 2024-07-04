import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar'
import Header from '../Components/Header'
import DataStatusCard from '../Components/DataStatusCard';
import PlayerCard from '../Components/PlayerCard';

export default function Home()
{
    const [data, setData] = useState(null);
    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/datastatus")
        .then(response => response.json())
        .then((data) => setData(data)).catch((error) => console.log(error));
    }, []);

    const [myPlayerData, setMyPLayerData] = useState(null);
    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/myplayers", {
            headers: {
                "Content-Type": "application/json",
              },
        })
        .then((response) => response.json())
        .then((myPlayerData) => setMyPLayerData(myPlayerData)).catch((error) => console.log(error));
    }, []);

    return (
        <div class="page">
            <Header />
            <Navbar />

            <div class="page-wrapper">
                <div class="page-body">
                    <div class="container-xl">
                        <div class="row row-deck row-cards">
                            {data && data.map((item) => (
                                <div class="col-3">
                                    <DataStatusCard dataSource={item.dataSource} lastUpdated={item.lastUpdated} />
                                </div>
                            ))} 
                        </div>
                        
                        <div class="col-12 mt-3">
                            <div class="card">
                                <div class="card-body">
                                    <div class="col-12">
                                        <div class="card">
                                            <div class="card-header">
                                                <h3 class="card-title">My Team</h3>
                                            </div>
                                            <div class="list-group list-group-flush">
                                                {myPlayerData && myPlayerData.map((item) => (
                                                        <PlayerCard player={item} />
                                                ))} 
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}