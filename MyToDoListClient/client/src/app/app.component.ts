import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'packt-app-service';

  constructor(public authService: AuthService) {
      this.authService.accessToken$.subscribe(token => {
      });
  }
}
