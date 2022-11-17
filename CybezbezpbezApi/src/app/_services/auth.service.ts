import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { ResponseToken } from './response-token.interface';
import { LoginData } from './login-data.interface';
import { RegistrationData } from './registration-data.interface';
import { ChangePassword } from './change-password.interface';
import { Settings } from './settings.interface';

const AUTH_API = 'https://localhost:7222/api/Authentication/';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly JWT_EXPIRATION_TIME = 'JWT_EXPIRATION_TIME';
  private readonly Logged_In_User = 'Logged_In_User';
  private readonly User_Session = 'User_Session';
  private readonly Role = 'Role';
  private readonly Last_Request = 'Last_Request';
  private _isFirstLogin = false;
  private _hasPasswordExpired = false;
  constructor(private http: HttpClient) {}

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0 || error.status >= 500 || !error.error) {
      return throwError(() => 'Coś poszło nie tak');
    }
    return throwError(() => error.error);
  }

  get getLoggedInUser() {
    return localStorage.getItem(this.Logged_In_User);
  }

  get isFirstLogin(){
    return this._isFirstLogin;
  }

  get hasPasswordExpired(){
    return this._hasPasswordExpired;
  }

  register(registrationData: RegistrationData) {
    return this.http
      .post<RegistrationData>('/api/Authentication/Register', {
        ...registrationData,
      })
      .pipe(catchError(this.handleError));
  }

  changePassword(changePassword: ChangePassword) {
    return this.http
      .post<ChangePassword>('/api/Authentication/ChangePassword', {
        ...changePassword,
      })
      .pipe(catchError(this.handleError));
  }

  login(loginData: LoginData) {
    return this.http
      .post<ResponseToken>('/api/Authentication/Login', {
        ...loginData,
      })
      .pipe(
        tap((responseToken) =>{
          this._isFirstLogin= responseToken.isFirstLogin;
          this._hasPasswordExpired= responseToken.hasPasswordExpired;
          this.doLoginUser(loginData.email, responseToken)
        }
        ),
        catchError(this.handleError)
      );
  }

  changeRequirePasswordLength(length:number) {
    return this.http
      .post<number>('/api/Authentication/ChangePasswordMinLength', length)
      .pipe(catchError(this.handleError));
  }

  changeDigitRequirement(requireDigit:boolean){
    return this.http
      .post<boolean>('/api/Authentication/EnableOrDisablePasswordRequirements', requireDigit)
      .pipe(catchError(this.handleError));
  }

  changePasswordValidity(passwordValidity:number){
    return this.http
      .post<boolean>('/api/Authentication/SetPasswordExpirationTime', passwordValidity)
      .pipe(catchError(this.handleError));
  }

  changeMaximumNumberOfAttempts(maximumNumberOfAttempts:number){
    return this.http
      .post<boolean>('/api/Authentication/SetMaximumNumberOfAttempts', maximumNumberOfAttempts)
      .pipe(catchError(this.handleError));
  }

  changeUserSession(userSession:number){
    return this.http
      .post<boolean>('/api/Authentication/SetUserSession', userSession)
      .pipe(catchError(this.handleError));
  }

  getAllSettings(){
    return this.http
      .get<Settings>('/api/Authentication/GetAllSettings')
      .pipe(catchError(this.handleError));
  }

  getLogs(){
    return this.http
      .get<string[]>('/api/Log')
      .pipe(catchError(this.handleError));
  }

  isLoggedIn() {
    return !!this.getJwtToken();
  }

  getJwtToken() {
    return localStorage.getItem(this.JWT_TOKEN);
  }
  getRole() {
    return localStorage.getItem(this.Role);
  }

  getUserSession() {
    return localStorage.getItem(this.User_Session);
  }

  private doLoginUser(email: string, responseToken: ResponseToken) {
    localStorage.setItem(this.Logged_In_User, email);
    localStorage.setItem(this.Role, responseToken.role);
    localStorage.setItem(this.User_Session, responseToken.userSession.toString());
    localStorage.setItem(this.Last_Request, new Date().getTime().toString());
    this.storeTokens(responseToken);
  }

  private storeTokens(responseToken: ResponseToken) {
    localStorage.setItem(
      this.JWT_EXPIRATION_TIME,
      responseToken.expiration.toString()
    );
    localStorage.setItem(this.JWT_TOKEN, responseToken.token);
  }

  logout() {
    this.doLogoutUser();
  }

  private doLogoutUser() {
    localStorage.removeItem(this.Logged_In_User);
    this.removeToken();
  }

  private removeToken() {
    localStorage.removeItem(this.JWT_TOKEN);
  }
  getFiles() {
   return this.http
    .get<string[]>('/api/File', {params: {filename: "private-file.txt"}, responseType: 'text' as 'json'})
      .pipe(catchError(this.handleError));
  }
}
