import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <main>
      <h1>Task Manager</h1>
      <router-outlet></router-outlet>
    </main>
  `,
  styles: []
})
export class AppComponent {}
