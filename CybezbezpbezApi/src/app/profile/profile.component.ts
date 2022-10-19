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
  passwordLengthForm: any = {
    length: 1,
  };
  passwordComplexityForm: any = {
    requireDigit: true,
  };
  passwordValidityForm: any = {
    passwordValidity: 1,
  };
  currentUserMail: any;
  currentUserRole: any;
  currentUserRoleAdmin = false;
  roles: string[] = []
  email? : string;
  passwordVisible = false;
  passwordChange: ChangePassword | undefined;
  isLoggedIn = false;
  passwordError: string| undefined;
  passwordMessage: string| undefined;
  lengthError: string| undefined;
  lengthMessage: string| undefined;
  passwordComplexityError: string| undefined;
  passwordComplexityMessage: string| undefined;
  passwordValidityError: string| undefined;


  constructor(private tokenStorage: TokenStorageService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.currentUserMail = this.authService.getLoggedInUser;
    this.currentUserRole = this.authService.getRole();
    if (this.currentUserRole === 'Admin')
    {this.currentUserRoleAdmin = true
    }else{
    this.currentUserRoleAdmin = false;
    }
    this.authService
    .getAllSettings()
    .subscribe({
      next: (data) => {
        console.log(data);
        this.passwordComplexityForm.requireDigit = data.isEnabledPasswordRequirements;
        this.passwordValidityForm.passwordValidity = data.passwordExpirationTime;
        this.passwordLengthForm.length = data.passwordMinLength;
      },
      error: (err) => {
      },
    });
  }

  save(): void {
    console.log(this.form);
    this.authService
    .changePassword({
      ...this.form,
      email:this.authService.getLoggedInUser
    })
    .subscribe({
      next: () => {
        this.passwordMessage="Hasło zostało zmienione";
      },
      error: (err) => {
        this.passwordError = err;
      },
    });
  }

  changeRequirePasswordLength(){
    console.log(this.passwordLengthForm);
    if(this.passwordLengthForm.length<1){
      this.lengthError = "Za mała długość"
      return;
    }
    this.authService
    .changeRequirePasswordLength(this.passwordLengthForm.length)
    .subscribe({
      next: () => {
        this.lengthMessage="Długość została zmienione";
      },
      error: (err) => {
        this.lengthError = err;
      },
    });
  }

  changeDigitRequirement(){
    console.log(this.passwordComplexityForm);
    this.authService
    .changeDigitRequirement(this.passwordComplexityForm.requireDigit)
    .subscribe({
      next: () => {
      },
      error: (err) => {
        this.passwordComplexityError = err;
      },
    });
  }

  changePasswordValidity(){
    this.authService
    .changePasswordValidity(this.passwordValidityForm.passwordValidity)
    .subscribe({
      next: () => {
      },
      error: (err) => {
        this.passwordValidityError = err;
      },
    });
  }
}
