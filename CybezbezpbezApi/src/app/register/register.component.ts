import { ChangeDetectorRef, Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReCaptcha2Component } from 'ngx-captcha';
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
  public aFormGroup: FormGroup;
  @ViewChild('captchaElem')
  captchaElem!: ReCaptcha2Component;
  @ViewChild('langInput')
  langInput!: ElementRef;
  public captchaIsLoaded = false;
  public captchaSuccess = false;
  public captchaIsExpired = false;
  public captchaResponse?: string;
  public theme: 'light' | 'dark' = 'light'
  public size: 'compact' | 'normal' = 'normal'
  public lang = 'en';
  public type: 'image' | 'audio' = 'image'
  siteKey = "6Le83QojAAAAAPSORRCgKko6t7-Rfy3ZCfHiFGCj";

  constructor(private authService: AuthService, private formBuilder: FormBuilder, private cdr: ChangeDetectorRef) {
    this.aFormGroup = this.formBuilder.group({
      recaptcha: ['', Validators.required]
    });
  }
 handleSuccess(data: any) {
  console.log(data);
 }
 handleError(){

 }
 handleExpire(){

 }
 handleReset(): void {
  this.captchaSuccess = false;
  this.captchaResponse = undefined;
  this.captchaIsExpired = false;
  this.cdr.detectChanges();
}
 handleLoad(){

 }


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
