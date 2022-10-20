import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { User } from './user.interface';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private handleError(error: HttpErrorResponse) {
    if (error.status === 0 || error.status >= 500 || !error.error) {
      return throwError(() => 'Coś poszło nie tak');
    }
    return throwError(() => error.error);
  }
  constructor(private http: HttpClient) { }

  getAll(){
    return this.http
      .get<User[]>('/api/User')
      .pipe(catchError(this.handleError));
  }

  deleteUser(id: string){
    return this.http
    .delete<string>(`/api/User/${id}`)
    .pipe(catchError(this.handleError));
  }

  blockUser(id: string, enabled:boolean) {
    return this.http
      .post(`/api/User/${id}/BlockUser`,enabled)
      .pipe(catchError(this.handleError));
  }

  changeUserName(id: string, userName: string) {
    return this.http
      .post(`/api/User/${id}/UserName`,userName)
      .pipe(catchError(this.handleError));
  }

  changePassword(id: string, password: string) {
    return this.http
      .post(`/api/User/${id}/Password`,password)
      .pipe(catchError(this.handleError));
  }

}
