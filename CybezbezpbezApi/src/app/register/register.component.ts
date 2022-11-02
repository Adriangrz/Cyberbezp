import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  form: any = {
    name: null,
    email: null,
    password: null
  };
  isSuccessful = false;
  isSignUpFailed = false;
  errorMessage = '';
  oneTimePassword: number | undefined;
  a = 3;
  x = 84;

  constructor(private authService: AuthService) { }

  generatePassword() {
    this.oneTimePassword = Math.log(this.a) / Math.log(this.x);
    console.log('a', this.a)
    console.log('x', this.x)

  }
  onSubmit(): void {
    this.authService
    .register({
      ...this.form
    })
    .subscribe({
      next: () => {
        this.isSuccessful=true;
        this.isSignUpFailed=false;
      },
      error: (err) => {
        this.isSuccessful=false;
        this.isSignUpFailed=true;
        this.errorMessage = err;
      },
    });
  }
}
