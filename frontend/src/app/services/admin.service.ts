import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User, Role, Permission } from '../models';

const API = 'http://localhost:5241/api/admin';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);

  getUsers() { return this.http.get<User[]>(`${API}/users`); }
  getRoles() { return this.http.get<Role[]>(`${API}/roles`); }
  getPermissions() { return this.http.get<Permission[]>(`${API}/permissions`); }

  assignRole(userId: number, roleId: number) {
    return this.http.post(`${API}/assign-role`, { userId, roleId }, { responseType: 'text' });
  }

  assignPermission(roleId: number, permissionId: number) {
    return this.http.post(`${API}/assign-permission`, { roleId, permissionId }, { responseType: 'text' });
  }
}
