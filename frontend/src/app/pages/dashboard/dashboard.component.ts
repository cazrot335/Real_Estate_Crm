import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ReportService } from '../../services/report.service';
import { AuthService } from '../../services/auth.service';
import { ReportSummary, AgentPerformance, ConversionRate } from '../../models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private report = inject(ReportService);
  auth = inject(AuthService);

  summary = signal<ReportSummary | null>(null);
  conversion = signal<ConversionRate | null>(null);
  agentPerf = signal<AgentPerformance[]>([]);

  get role() { return this.auth.getRole(); }
  get email() { return this.auth.getEmail(); }

  ngOnInit() {
    if (this.auth.hasPermission('view_reports')) {
      this.report.getSummary().subscribe({ next: s => this.summary.set(s), error: () => {} });
      this.report.getConversionRate().subscribe({ next: c => this.conversion.set(c), error: () => {} });
    }
    if (this.auth.hasPermission('view_agent_performance')) {
      this.report.getAgentPerformance().subscribe({ next: a => this.agentPerf.set(a), error: () => {} });
    }
  }
}
