import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { ResponseToken } from './response-token.interface';
import { LoginData } from './login-data.interface';
import { RegistrationData } from './registration-data.interface';

const AUTH_API = 'https://localhost:7222/api/Authentication/';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly JWT_EXPIRATION_TIME = 'JWT_EXPIRATION_TIME';
  private readonly Logged_In_User = 'Logged_In_User';
  private readonly Role = 'Role';
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

  register(registrationData: RegistrationData) {
    return this.http
      .post<RegistrationData>('/api/Authentication/Register', {
        ...registrationData,
      })
      .pipe(catchError(this.handleError));
  }

  login(loginData: LoginData) {
    return this.http
      .post<ResponseToken>('/api/Authentication/Login', {
        ...loginData,
      })
      .pipe(
        tap((responseToken) =>
          this.doLoginUser(loginData.email, responseToken)
        ),
        catchError(this.handleError)
      );
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

  private doLoginUser(email: string, responseToken: ResponseToken) {
    localStorage.setItem(this.Logged_In_User, email);
    localStorage.setItem(this.Role, responseToken.role);
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
}
