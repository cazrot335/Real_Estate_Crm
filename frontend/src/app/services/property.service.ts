import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Property } from '../models';

const API = 'http://localhost:5241/api/property';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private http = inject(HttpClient);

  getProperties() { return this.http.get<Property[]>(API); }

  createProperty(title: string, location: string, price: number) {
    const params = new HttpParams().set('title', title).set('location', location).set('price', price);
    return this.http.post<Property>(API, null, { params });
  }

  assignProperty(propertyId: number, agentId: number) {
    const params = new HttpParams().set('propertyId', propertyId).set('agentId', agentId);
    return this.http.post(`${API}/assign`, null, { params, responseType: 'text' });
  }

  updateStatus(propertyId: number, status: string) {
    const params = new HttpParams().set('propertyId', propertyId).set('status', status);
    return this.http.post(`${API}/status`, null, { params, responseType: 'text' });
  }
}
