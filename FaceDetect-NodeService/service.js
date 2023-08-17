const express = require('express');
const cors = require('cors');
const app = express();
const http = require('http').createServer(app);

const io = require('socket.io')(http , {
    cors: {
        origins: ['http://localhost:4200']
    }
});

const { EventHubConsumerClient } = require('@azure/event-hubs');

const connectionString = 'Endpoint=sb://everbridge-face-hub.servicebus.windows.net/;SharedAccessKeyName=receiver;SharedAccessKey=+07kXDiYQgP3kXMtsgqvm6wSAxDF3H79e+AEhA5C1Zw=;EntityPath=face-detector-hub';
const eventHubName = 'face-detector-hub';
const consumerGroup = 'web'; 

const consumerClient = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName);


io.on('connection', (socket) => {
    console.log('A client connected');
    const subscription = consumerClient.subscribe({
        processEvents: async (events, context) => {
            socket.emit('events', events);
        },
        processError: async (err, context) => {
            console.error(err);
        }
    });

    socket.on('disconnect', () => {
        console.log('A client disconnected');
        subscription.close();
    });
});

http.listen(3000, () => {
    console.log('Listening on port 3000');
});

