import { Injectable, NgZone, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';

declare const google: any;

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly zone = inject(NgZone);
  private readonly tokenKey = 'id_token';

  readonly isLoggedIn = signal(this.hasValidToken());

  initGoogleButton(container: HTMLElement): void {
    // The Google script loads async, so wait until it is ready.
    if (typeof google === 'undefined') {
      setTimeout(() => this.initGoogleButton(container), 100);
      return;
    }

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: { credential: string }) =>
        this.zone.run(() => this.saveToken(response.credential)),
    });
    google.accounts.id.renderButton(container, { theme: 'outline', size: 'large' });
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    this.isLoggedIn.set(false);
  }

  private saveToken(token: string): void {
    sessionStorage.setItem(this.tokenKey, token);
    this.isLoggedIn.set(true);
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }
}