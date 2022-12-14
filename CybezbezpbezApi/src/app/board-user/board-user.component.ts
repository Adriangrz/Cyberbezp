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
  isPasswordChange:boolean=false;
  nameForm: any = {
    name: '',
  };

  passwordForm: any = {
    password: '',
  };
  oneTimePasswordForm: any = {
    oneTimePassword: '',
  };
  oneTimePassword: number | undefined;
  userEmail:string|undefined;

  a: number|undefined;
  x = Math.floor(Math.random() * 100);

  constructor(private userService: UserService) {
    console.log(this.userEmail)
  }

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
        console.log(err)
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
  password(id:string){
    this.selectId=id;
    this.isPasswordChange=true;
  }

  changeUserName(id:string) {
    this.userService
    .changeUserName(id,this.nameForm.name)
    .subscribe({
      next: (data) => {
        alert("Zmieniono nazwę użytkownika");
      },
      error: (err) => {
      },
    });
  }


  changePassword() {
    this.userService
    .changePassword(this.selectId,this.passwordForm.password.toString())
    .subscribe({
      next: (data) => {
        alert("Zmieniono hasło użytkownika");
      },
      error: (err) => {
      },
    });
  }

  generatePassword(id:string) {
    this.userEmail = this.users.find(x=> x.id === id)?.email;
    this.a = this.userEmail?.length;
    this.oneTimePassword = Math.log(this.a!) / Math.log(this.x);
    this.userService
    .generatePassword(id, this.oneTimePasswordForm.oneTimePassword.toString())
    .subscribe({
      next: (data) => {
        alert("Ustawiono hasło jednorazowe");
      },
      error: (err) => {
      },
    });
    console.log('a', this.a)
    console.log('x', this.x)
    console.log('wynik', this.oneTimePassword)
  }
}
