import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { DealService } from '../../services/deal.service';
import { LeadService } from '../../services/lead.service';
import { PropertyService } from '../../services/property.service';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { Deal, Lead, Property, User } from '../../models';

@Component({
  selector: 'app-deals',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './deals.component.html',
})
export class DealsComponent implements OnInit {
  private service = inject(DealService);
  private leadSvc = inject(LeadService);
  private propSvc = inject(PropertyService);
  private admin = inject(AdminService);
  auth = inject(AuthService);

  deals = signal<Deal[]>([]);
  leads = signal<Lead[]>([]);
  properties = signal<Property[]>([]);
  users = signal<User[]>([]);
  loading = signal(true);
  showCreate = signal(false);
  toast = signal('');
  toastType = signal<'success' | 'error'>('success');

  newDeal = { leadId: 0, propertyId: 0, agentId: 0, amount: 0 };
  statusForm = { dealId: 0, status: '' };

  readonly statuses = ['Open', 'Negotiation', 'Closed', 'Cancelled'];

  get canCreate() { return this.auth.hasPermission('create_deal'); }
  get canStatus() { return this.auth.hasPermission('update_deal_status'); }

  ngOnInit() {
    this.load();
    if (this.canCreate) {
      this.leadSvc.getLeads().subscribe({ next: l => this.leads.set(l), error: () => {} });
      this.propSvc.getProperties().subscribe({ next: p => this.properties.set(p), error: () => {} });
      this.admin.getUsers().subscribe({ next: u => this.users.set(u), error: () => {} });
    }
  }

  load() {
    this.loading.set(true);
    this.service.getDeals().subscribe({ next: d => { this.deals.set(d); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  create() {
    const { leadId, propertyId, agentId, amount } = this.newDeal;
    if (!leadId || !propertyId || !agentId || !amount) return;
    this.service.createDeal(leadId, propertyId, agentId, amount).subscribe({
      next: () => { this.newDeal = { leadId: 0, propertyId: 0, agentId: 0, amount: 0 }; this.showCreate.set(false); this.load(); this.notify('Deal created.', 'success'); },
      error: () => this.notify('Failed to create deal.', 'error'),
    });
  }

  updateStatus() {
    if (!this.statusForm.dealId || !this.statusForm.status) return;
    this.service.updateStatus(this.statusForm.dealId, this.statusForm.status).subscribe({
      next: () => { this.load(); this.notify('Deal status updated.', 'success'); },
      error: () => this.notify('Failed to update status.', 'error'),
    });
  }

  statusClass(s: string) {
    const m: Record<string, string> = {
      Open: 'bg-blue-100 text-blue-700',
      Negotiation: 'bg-yellow-100 text-yellow-700',
      Closed: 'bg-green-100 text-green-700',
      Cancelled: 'bg-red-100 text-red-700',
    };
    return m[s] ?? 'bg-gray-100 text-gray-600';
  }

  private notify(msg: string, type: 'success' | 'error') {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
