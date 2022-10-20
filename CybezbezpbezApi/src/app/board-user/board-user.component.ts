import { Component, OnInit } from '@angular/core';
import { User } from '../_services/user.interface';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-board-user',
  templateUrl: './board-user.component.html',
  styleUrls: ['./board-user.component.scss']
})
export class BoardUserComponent implements OnInit {
  users: User[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService
    .getAll()
    .subscribe({
      next: (data) => {
        this.users=data;
      },
      error: (err) => {
      },
    });
  }
}
