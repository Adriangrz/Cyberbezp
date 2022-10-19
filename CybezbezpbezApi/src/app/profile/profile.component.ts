import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { TokenStorageService } from '../_services/token-storage.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  currentUserMail: any;
  currentUserRole: any;
  roles: string[] = []
  email? : string;


  constructor(private tokenStorage: TokenStorageService, private authService: AuthService,) { }

  ngOnInit(): void {
    this.currentUserMail = this.authService.getLoggedInUser;
    this.currentUserRole = this.authService.getRole();
  }
}
