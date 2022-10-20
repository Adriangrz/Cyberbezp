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
  selectId: string = '';

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

  deleteUser(id:string){
    alert("Poprawnie usunięto użytkownika");
    this.users = this.users.filter((t) => t.id !== id);
    this.userService
    .deleteUser(id)
    .subscribe({
      next: (id) => {
      },
      error: (err) => {
      },
    });
  }

  blockUser(id:string){
    this.users = this.users.filter((t) => t.id !== id);
    this.userService
    .blockUser(id)
  }

  changeUserName(id:string) {
    this.users = this.users.filter((t) => t.id !== id);
    this.userService
    .changeUserName(id)
  }

  changePassword(id:string) {
    this.users = this.users.filter((t) => t.id !== id);
    this.userService
    .changePassword(id)
  }
}
