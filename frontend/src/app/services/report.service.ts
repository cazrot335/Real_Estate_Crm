import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReportSummary, AgentPerformance, ConversionRate } from '../models';

const API = 'http://localhost:5241/api/reports';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private http = inject(HttpClient);

  getSummary() { return this.http.get<ReportSummary>(`${API}/summary`); }
  getAgentPerformance() { return this.http.get<AgentPerformance[]>(`${API}/agent-performance`); }
  getConversionRate() { return this.http.get<ConversionRate>(`${API}/conversion-rate`); }
}
