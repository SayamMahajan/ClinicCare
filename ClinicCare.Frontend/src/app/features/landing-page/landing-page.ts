import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from "@angular/router";

@Component({
  selector: 'app-landing-page',
  imports: [RouterLink, FormsModule],
  templateUrl: './landing-page.html'
})
export class LandingPage {
  private router = inject(Router);
  imagePath = signal(`url('bgimg.png')`);
  selectedLoginType = signal('login');

  onLogin(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (!value) return;

    if (value === 'patient') {
      this.router.navigateByUrl('auth/login/patient');
    } else if (value === 'employee') {
      this.router.navigateByUrl('auth/login/employee');
    }
  }

}
