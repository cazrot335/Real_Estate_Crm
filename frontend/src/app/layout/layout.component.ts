import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './layout.component.html',
})
export class LayoutComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  get email() { return this.auth.getEmail(); }
  get role() { return this.auth.getRole(); }

  can(permission: string) { return this.auth.hasPermission(permission); }
  isAdmin() { return this.auth.isAdmin(); }

  get roleBadge() {
    const map: Record<string, string> = {
      Admin: 'bg-red-500',
      Agent: 'bg-blue-500',
      Viewer: 'bg-green-500',
    };
    return map[this.role] ?? 'bg-gray-500';
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
