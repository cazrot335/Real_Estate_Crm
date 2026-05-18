import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Lead } from '../models';

const API = 'http://localhost:5241/api/lead';

@Injectable({ providedIn: 'root' })
export class LeadService {
  private http = inject(HttpClient);

  getLeads() { return this.http.get<Lead[]>(API); }

  createLead(name: string, phone: string) {
    const params = new HttpParams().set('name', name).set('phone', phone);
    return this.http.post<Lead>(API, null, { params });
  }

  assignLead(leadId: number, agentId: number) {
    const params = new HttpParams().set('leadId', leadId).set('agentId', agentId);
    return this.http.post(`${API}/assign`, null, { params, responseType: 'text' });
  }

  updateStatus(leadId: number, status: string) {
    const params = new HttpParams().set('leadId', leadId).set('status', status);
    return this.http.post(`${API}/status`, null, { params, responseType: 'text' });
  }
}
