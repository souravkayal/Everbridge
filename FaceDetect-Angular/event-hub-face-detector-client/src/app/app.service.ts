import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { io } from 'socket.io-client';

@Injectable({
    providedIn: 'root'
})
export class EventHubService {
    private socket;

    constructor() {
        this.socket = io('http://localhost:3000'); // Your backend server URL
    }

    getEventStream(): Observable<any> {
        return new Observable<any>(observer => {
            this.socket.on('events', (data) => {
                observer.next(data);
            });
        });
    }
}
