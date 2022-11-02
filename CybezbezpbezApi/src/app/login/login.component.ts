import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { ResponseToken } from '../_services/response-token.interface';
import { TokenStorageService } from '../_services/token-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  form: any = {
    email: null,
    password: null,
    oneTimePassword: null
  };
  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  roles: string[] = [];

  constructor(private authService: AuthService, private tokenStorage: TokenStorageService, private router: Router) { }

  ngOnInit(): void {
    if (this.tokenStorage.getToken()) {
      this.isLoggedIn = true;
      this.roles = this.tokenStorage.getUser().roles;
    }
  }

  onSubmit(): void {
    this.authService
    .login({
      ...this.form
    })
    .subscribe({
      next: (responseToken:ResponseToken) => {
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
