import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { LeadService } from '../../services/lead.service';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { Lead, User } from '../../models';

@Component({
  selector: 'app-leads',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './leads.component.html',
})
export class LeadsComponent implements OnInit {
  private service = inject(LeadService);
  private admin = inject(AdminService);
  auth = inject(AuthService);

  leads = signal<Lead[]>([]);
  users = signal<User[]>([]);
  loading = signal(true);
  showCreate = signal(false);
  toast = signal('');
  toastType = signal<'success' | 'error'>('success');

  newLead = { name: '', phone: '' };
  assignForm = { leadId: 0, agentId: 0 };
  statusForm = { leadId: 0, status: '' };

  readonly statuses = ['New', 'Contacted', 'Qualified', 'Converted'];

  get canCreate() { return this.auth.hasPermission('create_lead'); }
  get canAssign() { return this.auth.hasPermission('assign_lead'); }
  get canStatus() { return this.auth.hasPermission('update_lead_status'); }

  ngOnInit() {
    this.load();
    if (this.canAssign) this.admin.getUsers().subscribe({ next: u => this.users.set(u), error: () => {} });
  }

  load() {
    this.loading.set(true);
    this.service.getLeads().subscribe({ next: l => { this.leads.set(l); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  create() {
    if (!this.newLead.name.trim() || !this.newLead.phone.trim()) return;
    this.service.createLead(this.newLead.name, this.newLead.phone).subscribe({
      next: () => { this.newLead = { name: '', phone: '' }; this.showCreate.set(false); this.load(); this.notify('Lead created successfully.', 'success'); },
      error: () => this.notify('Failed to create lead.', 'error'),
    });
  }

  assign() {
    if (!this.assignForm.leadId || !this.assignForm.agentId) return;
    this.service.assignLead(this.assignForm.leadId, this.assignForm.agentId).subscribe({
      next: () => { this.load(); this.notify('Lead assigned.', 'success'); },
      error: () => this.notify('Failed to assign lead.', 'error'),
    });
  }

  updateStatus() {
    if (!this.statusForm.leadId || !this.statusForm.status) return;
    this.service.updateStatus(this.statusForm.leadId, this.statusForm.status).subscribe({
      next: () => { this.load(); this.notify('Status updated.', 'success'); },
      error: () => this.notify('Failed to update status.', 'error'),
    });
  }

  statusClass(s: string) {
    const m: Record<string, string> = {
      New: 'bg-gray-100 text-gray-600',
      Contacted: 'bg-blue-100 text-blue-700',
      Qualified: 'bg-yellow-100 text-yellow-700',
      Converted: 'bg-green-100 text-green-700',
    };
    return m[s] ?? 'bg-gray-100 text-gray-600';
  }

  private notify(msg: string, type: 'success' | 'error') {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
