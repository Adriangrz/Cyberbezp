import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { TokenStorageService } from '../_services/token-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  form: any = {
    email: null,
    password: null
  };
  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  roles: string[] = [];

  constructor(private authService: AuthService, private tokenStorage: TokenStorageService) { }

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
      next: () => {
        this.isLoggedIn=true;
        this.roles.push(this.authService.getRole()!);
      },
      error: (err) => {
        this.isLoginFailed=true;
        this.errorMessage = err;
      },
    });
  }

  reloadPage(): void {
    window.location.reload();
  }
}
