import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  error = signal('');
  loading = signal(false);
  mode = signal<'login' | 'register'>('login');
  successMsg = signal('');

  submit() {
    this.error.set('');
    this.successMsg.set('');
    this.loading.set(true);

    if (this.mode() === 'login') {
      this.auth.login(this.email, this.password).subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: () => { this.error.set('Invalid credentials. Please try again.'); this.loading.set(false); },
      });
    } else {
      this.auth.register(this.email, this.password).subscribe({
        next: () => { this.successMsg.set('Account created! You can now sign in.'); this.mode.set('login'); this.loading.set(false); },
        error: (err) => { this.error.set(err.error || 'Registration failed.'); this.loading.set(false); },
      });
    }
  }
}
