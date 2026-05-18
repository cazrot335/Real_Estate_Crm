import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { User, Role, Permission } from '../../models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin.component.html',
})
export class AdminComponent implements OnInit {
  private admin = inject(AdminService);

  users = signal<User[]>([]);
  roles = signal<Role[]>([]);
  permissions = signal<Permission[]>([]);
  toast = signal('');
  toastType = signal<'success' | 'error'>('success');

  activeTab = signal<'users' | 'roles' | 'permissions'>('users');
  assignRoleForm = { userId: 0, roleId: 0 };
  assignPermForm = { roleId: 0, permissionId: 0 };

  ngOnInit() {
    this.admin.getUsers().subscribe({ next: u => this.users.set(u), error: () => {} });
    this.admin.getRoles().subscribe({ next: r => this.roles.set(r), error: () => {} });
    this.admin.getPermissions().subscribe({ next: p => this.permissions.set(p), error: () => {} });
  }

  assignRole() {
    if (!this.assignRoleForm.userId || !this.assignRoleForm.roleId) return;
    this.admin.assignRole(this.assignRoleForm.userId, this.assignRoleForm.roleId).subscribe({
      next: () => { this.admin.getUsers().subscribe(u => this.users.set(u)); this.notify('Role assigned.', 'success'); },
      error: () => this.notify('Failed to assign role.', 'error'),
    });
  }

  assignPermission() {
    if (!this.assignPermForm.roleId || !this.assignPermForm.permissionId) return;
    this.admin.assignPermission(this.assignPermForm.roleId, this.assignPermForm.permissionId).subscribe({
      next: () => { this.notify('Permission assigned to role.', 'success'); },
      error: () => this.notify('Failed to assign permission.', 'error'),
    });
  }

  private notify(msg: string, type: 'success' | 'error') {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
