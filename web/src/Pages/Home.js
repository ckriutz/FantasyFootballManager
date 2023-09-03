import { useState, useEffect } from 'react';
import Navbar from '../Components/Navbar'
import Header from '../Components/Header'
import DataStatusCard from '../Components/DataStatusCard';
import { set } from 'date-fns';

export default function Home()
{
    const [data, setData] = useState(null);
    useEffect(() => {
        fetch(process.env.REACT_APP_API_URL + "/DataStatus")
        .then((response) => response.json())
        .then((data) => setData(data));
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
                                <div class="col-4">
                                    <DataStatusCard dataSource={item.dataSource} lastUpdated={item.lastUpdated} />
                                </div>
                            ))} 
                        </div>
                        
                        <div class="col-12">
                            <div class="card">
                                <div class="card-body" style={{height: "10rem"}}></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}