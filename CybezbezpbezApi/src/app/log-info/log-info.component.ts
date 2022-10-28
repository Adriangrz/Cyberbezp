import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-log-info',
  templateUrl: './log-info.component.html',
  styleUrls: ['./log-info.component.scss']
})
export class LogInfoComponent implements OnInit {
  logs:string[][] = [];

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.authService
    .getLogs()
    .subscribe({
      next: (data) => {
        for (let index = 0; index < data.length; index++) {
          const element = data[index];
          this.logs.push(element.split(';'))
        }
      },
      error: (err) => {
      },
    });
  }
}
