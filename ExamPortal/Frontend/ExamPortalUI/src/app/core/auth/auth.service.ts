import { Injectable, signal, computed, effect, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  userId: string;
  role: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  // Ensure this port matches your.NET Backend (check launchSettings.json)
  private readonly apiUrl = 'https://localhost:7178/api/auth'; 

  // Writable Signal initialized from localStorage
  private readonly authState = signal<AuthResponseDto | null>(this.loadState());

  // Computed Signals for UI bindings
  public readonly currentUser = computed(() => this.authState());
  public readonly isAuthenticated = computed(() => this.authState()!== null);
  public readonly isAdmin = computed(() => this.authState()?.role === 'Admin');

  constructor() {
    // Automatically syncs to localStorage whenever authState changes
    effect(() => {
      const state = this.authState();
      if (state) {
        localStorage.setItem('exam_auth', JSON.stringify(state));
      } else {
        localStorage.removeItem('exam_auth');
      }
    });
  }

  private loadState(): AuthResponseDto | null {
    // SSR Check: Prevent 'localStorage is not defined' error during hydration
    if (typeof window!== 'undefined') {
      const saved = localStorage.getItem('exam_auth');
      return saved? JSON.parse(saved) : null;
    }
    return null;
  }

  public login(credentials: LoginRequestDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.authState.set(response);
      })
    );
  }

  public logout(): void {
    this.authState.set(null);
  }
}