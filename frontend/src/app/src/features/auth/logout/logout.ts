import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

/**
 * Navigating to /logout programmatically triggers an immediate logout.
 * Useful for links in emails, external redirects, or end-to-end tests.
 */
@Component({
  selector: 'app-logout',
  standalone: true,
  template: ''
})
export class Logout implements OnInit {
  private readonly authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.logout();
  }
}
