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
    this.userService
    .blockUser(id,true)
    .subscribe({
      next: (data) => {
        alert("Zablokowano użytkownika");
      },
      error: (err) => {
      },
    });
  }

  changeUserName(id:string, newName: string) {
    this.userService
    .changeUserName(id,newName)
    .subscribe({
      next: (data) => {
        alert("Zmieniono nazwę użytkownika");
      },
      error: (err) => {
      },
    });
  }

  changePassword(id:string, newPassword: string) {
    this.userService
    .changeUserName(id,newPassword)
    .subscribe({
      next: (data) => {
        alert("Zmieniono hasło użytkownika");
      },
      error: (err) => {
      },
    });
  }
}
