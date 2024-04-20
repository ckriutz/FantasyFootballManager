// Import required modules
const Connection = require('tedious').Connection;
var Request = require('tedious').Request;
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const { DescribeParameterEncryptionResultSet1 } = require('tedious/lib/always-encrypted/types');

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

const connection = new Connection(config);

// Server port
const port = process.env.PORT || 3000;

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

// Start the server
app.listen(port, () => {
  console.log(`Server is running on port ${port}`);
});