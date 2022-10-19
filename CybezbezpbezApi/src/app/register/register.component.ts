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

  constructor(private authService: AuthService) { }


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
