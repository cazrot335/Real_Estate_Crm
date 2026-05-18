import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { PropertyService } from '../../services/property.service';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { Property, User } from '../../models';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './properties.component.html',
})
export class PropertiesComponent implements OnInit {
  private service = inject(PropertyService);
  private admin = inject(AdminService);
  auth = inject(AuthService);

  properties = signal<Property[]>([]);
  users = signal<User[]>([]);
  loading = signal(true);
  showCreate = signal(false);
  toast = signal('');
  toastType = signal<'success' | 'error'>('success');

  newProp = { title: '', location: '', price: 0 };
  assignForm = { propertyId: 0, agentId: 0 };
  statusForm = { propertyId: 0, status: '' };

  readonly statuses = ['Available', 'Sold', 'Rented'];

  get canCreate() { return this.auth.hasPermission('create_property'); }
  get canAssign() { return this.auth.hasPermission('assign_property'); }
  get canStatus() { return this.auth.hasPermission('update_property_status'); }

  ngOnInit() {
    this.load();
    if (this.canAssign) this.admin.getUsers().subscribe({ next: u => this.users.set(u), error: () => {} });
  }

  load() {
    this.loading.set(true);
    this.service.getProperties().subscribe({ next: p => { this.properties.set(p); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  create() {
    if (!this.newProp.title.trim() || !this.newProp.location.trim() || !this.newProp.price) return;
    this.service.createProperty(this.newProp.title, this.newProp.location, this.newProp.price).subscribe({
      next: () => { this.newProp = { title: '', location: '', price: 0 }; this.showCreate.set(false); this.load(); this.notify('Property created.', 'success'); },
      error: () => this.notify('Failed to create property.', 'error'),
    });
  }

  assign() {
    if (!this.assignForm.propertyId || !this.assignForm.agentId) return;
    this.service.assignProperty(this.assignForm.propertyId, this.assignForm.agentId).subscribe({
      next: () => { this.load(); this.notify('Property assigned.', 'success'); },
      error: () => this.notify('Failed to assign.', 'error'),
    });
  }

  updateStatus() {
    if (!this.statusForm.propertyId || !this.statusForm.status) return;
    this.service.updateStatus(this.statusForm.propertyId, this.statusForm.status).subscribe({
      next: () => { this.load(); this.notify('Status updated.', 'success'); },
      error: () => this.notify('Failed to update status.', 'error'),
    });
  }

  statusClass(s: string) {
    const m: Record<string, string> = {
      Available: 'bg-green-100 text-green-700',
      Sold: 'bg-gray-100 text-gray-600',
      Rented: 'bg-blue-100 text-blue-700',
    };
    return m[s] ?? 'bg-gray-100 text-gray-600';
  }

  private notify(msg: string, type: 'success' | 'error') {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
