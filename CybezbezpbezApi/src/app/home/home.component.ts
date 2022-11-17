import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  content?: string;

  constructor(private userService: UserService, private authService: AuthService) { }

  ngOnInit(): void {
this.showFiles();
  }

  showFiles(){
     this.authService
    .getFiles()
    .subscribe(data => {
      console.log(data);
    });
  }
}
