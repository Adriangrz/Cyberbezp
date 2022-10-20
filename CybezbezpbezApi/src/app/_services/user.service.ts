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
}
