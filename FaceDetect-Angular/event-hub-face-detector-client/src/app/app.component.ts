import { Component, OnInit } from '@angular/core';
import { EventHubService } from './app.service';

interface EventData {
  body: {
      gateId: number;
      FaceCount: number;
      Message: string;
  };
  properties: {
      'Diagnostic-Id': string;
  };
  offset: string;
  sequenceNumber: number;
  enqueuedTimeUtc: string;
  systemProperties: {};
}

@Component({
  selector: 'app-root',
  template: `
    <div *ngFor="let event of events">
        <span> ATTENTION : {{ event.body.FaceCount }} Face(s) detected at gate {{ event.body.gateId }} </span>
        <hr />
    </div>
    `
})
export class AppComponent implements OnInit {
  title = 'event-hub-face-detector-client';

    events: EventData[] =[];

    constructor(private eventHubService: EventHubService) {}

    ngOnInit() {

        this.eventHubService.getEventStream().subscribe(data => {
          console.log(JSON.stringify(data))
          this.events = data;
      });

    }
}
