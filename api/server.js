// Import required modules
const Connection = require('tedious').Connection;
var Request = require('tedious').Request;
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const redis = require('redis');
const { Schema } = require('redis-om');
const { Repository } = require('redis-om');

// Create an instance of Express
const app = express();

// Middleware setup
app.use(bodyParser.json());
app.use(cors());

// SQL Server Setup
const config = {
  server: '192.168.0.239', //update with your server name
  authentication: {
    type: 'default',
    options: {
      userName: 'sa', //update with your username
      password: '++password1++' //update with your password
    }
  },
  options: {
    encrypt: true, // for azure
    database: 'ffootball',  //update with your database name
    trustServerCertificate: true
  }
};

// Redis Setup
const client = redis.createClient({
    socket: {
      host: '192.168.0.239',
      port: 6379,
      trustServerCertificate: true
    },
});

client.on('error', function(err) {
  console.log('Redis Client Error', err);
});

client.on('connect', function() {
  console.log('Connected to Redis');
});

// this is for the SQL Server connection.
const connection = new Connection(config);

// Define the FantasyPlayer object
const fantasyPlayerSchema = new Schema('fantasyPlayer', {
    SleeperId: { type: 'string' },
    SportRadarId: { type: 'string' },
    FirstName: { type: 'string' },
    LastName: { type: 'string' },
    FullName: { type: 'string' },
    Status: { type: 'string' },
    Position: { type: 'string' },
    TeamName: { type: 'string' },
    YahooId: { type: 'number' },
    SearchRank: { type: 'number' },
    Age: { type: 'number' },
    College: { type: 'string' },
    LastUpdatedSleeper: { type: 'date' },
    SportsDataIoKey: { type: 'string' },
    LastUpdatedSportsDataIo: { type: 'date' },
    FantasyProsPlayerId: { type: 'number' },
    PlayerOwnedAvg: { type: 'number' },
    PlayerOwnedEspn: { type: 'number' },
    PlayerOwnedYahoo: { type: 'number' },
    RankEcr: { type: 'number' },
    Tier: { type: 'number' },
    LastUpdatedFantasyPros: { type: 'date' },
    IsThumbsUp: { type: 'boolean' },
    IsThumbsDown: { type: 'boolean' },
    IsTaken: { type: 'boolean' },
    IsOnMyTeam: { type: 'boolean' },
    PickedBy: { type: 'string' },
    }, {
    dataStructure: 'JSON', indexName: 'fantasyplayers-idx'
});

const fantasyRepository = new Repository(fantasyPlayerSchema, client)

app.get('/datastatus', (req, res) => {
    // Retrieves all statuses from the database.
    connection.connect((err) => {
        if (err) {
          console.log('Connection Failed');
          throw err;
        } 
        else {
          console.log('Connected to SQL Server');
          executeStatement().then(data => {
            res.json(data);
          }).catch(err => {
            console.log(err);
            res.json(err);
          });
        } 
    });
});

// Get all players
app.get('/fantasyplayers', (req, res) => {
    client.connect();

    console.log('Fetching all players from Redis');

    fantasyRepository.search().return.all().then((players) => {
        res.json(players);
    }).finally(() => {
        client.quit()
    }).catch((err) => {
        console.log('Error fetching players: ' + err);
    });
});

// Get a player by id
app.get('/fantasyplayer/:id', async (req, res) => {
    await client.connect();

    console.log('Fetching player ' + req.params.id + ' from Redis');

    player = await fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first()
    await client.quit();
    res.json(player);
});

// Get my players
app.get('/myplayers', (req, res) => {
    client.connect();

    console.log('Fetching my players from Redis');

    fantasyRepository.search().where("IsOnMyTeam").equals(true).return.all().then((players) => {
        res.json(players);
    }).finally(() => {
        client.quit()
    }).catch((err) => {
        console.log('Error fetching players: ' + err);
    });
});

// Get number of players with an id
app.get('/fantasyplayer/count/:id', (req, res) => {
    client.connect();

    console.log('Fetching player ' + req.params.id + ' from Redis');

    fantasyRepository.search().where("SleeperId").equals(req.params.id).return.count().then((count) => {
        res.json(count);
    }).finally(() => {
        client.quit()
    }).catch((err) => {
        console.log('Error fetching player: ' + err);
    });
});

// Update the player to give a thumbs up
app.post('/fantasyplayer/thumbsup/:id', async (req, res) => {
    await client.connect();

    console.log('Updating player ' + req.params.id + ' to thumbs up');
    player = await fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first();
    player.IsThumbsUp = true;
    player.IsThumbsDown = false;
    upPlayer = await fantasyRepository.save(player);
    console.log('Saved player player: ' + JSON.stringify(upPlayer));
    await client.quit();
    res.json(upPlayer);

    //fantasyRepository.save(data).then((player) => { res.json(player)
    //}).catch((err) => {
    //  console.log('Error updating player: ' + err);
    //}).finally(() => {
    //  client.quit()
    //});
});

// Update the player to give a thumbs down
app.post('/fantasyplayer/thumbsdown/:id', (req, res) => {
    client.connect();

    console.log('Updating player ' + req.params.id + ' to thumbs down');
    const player = fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first()
    .then((data) => {
      data.IsThumbsUp = false;
      data.IsThumbsDown = true;
      return data;
    }).then((data) => {
      fantasyRepository.save(data).then((player) => { res.json(player); })
    }).catch((err) => {
      console.log('Error updating player: ' + err);
    }).finally(() => {
      client.quit()
    });
});

/// Claim the player for my team
app.post('/fantasyplayer/claim/:id', (req, res) => {
    client.connect();

    console.log('Updating player ' + req.params.id + ' to my team');
    const player = fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first()
    .then((data) => {
      data.IsOnMyTeam = true;
      data.IsTaken = false;
      return data;
    }).then((data) => {
      fantasyRepository.save(data).then((player) => { res.json(player); })
    }).catch((err) => {
      console.log('Error updating player: ' + err);
    }).finally(() => {
      client.quit()
    });
});

// Assign the player to someone else
app.post('/fantasyplayer/assign/:id', (req, res) => {
    client.connect();

    console.log('Updating player ' + req.params.id + ' to someone else');
    const player = fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first()
    .then((data) => {
      data.IsOnMyTeam = false;
      data.IsTaken = true;
      return data;
    }).then((data) => {
      fantasyRepository.save(data).then((player) => { res.json(player); })
    }).catch((err) => {
      console.log('Error updating player: ' + err);
    }).finally(() => {
      client.quit()
    });
});

// Release player from everyone
app.post('/fantasyplayer/release/:id', (req, res) => {
    client.connect();

    console.log('Updating player ' + req.params.id + ' to be released');
    const player = fantasyRepository.search().where("SleeperId").equals(req.params.id).return.first()
    .then((data) => {
      data.IsOnMyTeam = false;
      data.IsTaken = false;
      return data;
    }).then((data) => {
      fantasyRepository.save(data).then((player) => { res.json(player); })
    }).catch((err) => {
      console.log('Error updating player: ' + err);
    }).finally(() => {
      client.quit()
    });
});

// Execute the SQL statement
function executeStatement() {

    return new Promise((resolve, reject) => { 
        const query = "SELECT [Id],[DataSource],[LastUpdated] FROM [ffootball].[dbo].[DataStatus]";
        var request = new Request(query, function(err) {  
            if (err) {  
                console.log(err);
            }  
        });

        let results = [];
        // this is what gets executed when the query is completed, and we have row data.
        request.on('row', (columns) => {
            let result = {};
            columns.forEach((column) => {
                result[column.metadata.colName] = column.value;
            });
            results.push(result);
        });

        // this is what gets called when the query is completed, and we have no more rows.
        request.on('doneInProc', (rowCount, more) => {
            //console.log(JSON.stringify(results, null, 2));  // Pretty print the JSON
            console.log(rowCount + ' rows returned');
            resolve(results);
        });

        // this is what gets called when the query is completed, and we have an error.
        request.on('error', function(err) {
            console.log("Error occurred:", err);
            reject(err);
        });

        // Close the connection after the final event emitted by the request, after the callback passes
        request.on("requestCompleted", function (rowCount, more) {
            console.log("closing connection.");
            connection.close();
        });

        connection.execSql(request);
    });
}

// Server port
const port = process.env.PORT || 3000;

// Start the server
app.listen(port, () => {
  console.log(`Server is running on port ${port}`);
});