import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Deal } from '../models';

const API = 'http://localhost:5241/api/deal';

@Injectable({ providedIn: 'root' })
export class DealService {
  private http = inject(HttpClient);

  getDeals() { return this.http.get<Deal[]>(API); }

  createDeal(leadId: number, propertyId: number, agentId: number, amount: number) {
    const params = new HttpParams()
      .set('leadId', leadId)
      .set('propertyId', propertyId)
      .set('agentId', agentId)
      .set('amount', amount);
    return this.http.post<Deal>(API, null, { params });
  }

  updateStatus(dealId: number, status: string) {
    const params = new HttpParams().set('dealId', dealId).set('status', status);
    return this.http.post(`${API}/status`, null, { params, responseType: 'text' });
  }
}
