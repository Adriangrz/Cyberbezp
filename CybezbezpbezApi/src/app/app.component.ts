import { Component } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { TokenStorageService } from './_services/token-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private roles: string[] = [];
  isLoggedIn = false;
  isAdmin= false;
  showAdminBoard = false;
  username?: string;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.isLoggedIn = !!this.authService.getJwtToken();
    this.isAdmin = this.authService.getRole() === 'Admin';

    if (this.isLoggedIn) {
      const user = this.authService.getLoggedInUser;
      this.roles.push(this.authService.getRole()!);
      this.showAdminBoard = this.roles.includes('ROLE_ADMIN');
      this.username = this.authService.getLoggedInUser!;
    }
  }

  logout(): void {
    this.authService.logout();
    window.location.reload();
  }
}
