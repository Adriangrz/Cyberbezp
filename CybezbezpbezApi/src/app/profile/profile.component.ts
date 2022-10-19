import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { ChangePassword } from '../_services/change-password.interface';
import { TokenStorageService } from '../_services/token-storage.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  form: any = {
    email: null,
    oldPassword: null,
    newPassword: null,
  };
  currentUserMail: any;
  currentUserRole: any;
  currentUserRoleAdmin = false;
  roles: string[] = []
  email? : string;
  passwordVisible = false;
  passwordChange: ChangePassword | undefined;
  isLoggedIn = false;


  constructor(private tokenStorage: TokenStorageService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.currentUserMail = this.authService.getLoggedInUser;
    this.currentUserRole = this.authService.getRole();
    if (this.currentUserRole === 'Admin')
    {this.currentUserRoleAdmin = true
      return;
    }
    this.currentUserRoleAdmin = false;
  }
  save(): void {
    this.authService
    .login({
      ...this.form
    })
    .subscribe({
      next: () => {
        this.isLoggedIn=true;
        this.roles.push(this.authService.getRole()!);
        this.router.navigateByUrl('/profile')
      },
      error: (err) => {
        this.isLoginFailed=true;
        this.errorMessage = err;
      },
    });
  }


}
