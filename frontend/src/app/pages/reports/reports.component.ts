import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ReportService } from '../../services/report.service';
import { AuthService } from '../../services/auth.service';
import { ReportSummary, AgentPerformance, ConversionRate } from '../../models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './reports.component.html',
})
export class ReportsComponent implements OnInit {
  private service = inject(ReportService);
  auth = inject(AuthService);

  summary = signal<ReportSummary | null>(null);
  conversion = signal<ConversionRate | null>(null);
  agentPerf = signal<AgentPerformance[]>([]);
  loading = signal(true);

  ngOnInit() {
    const calls: Promise<void>[] = [];

    if (this.auth.hasPermission('view_reports')) {
      this.service.getSummary().subscribe({ next: s => this.summary.set(s), error: () => {} });
      this.service.getConversionRate().subscribe({ next: c => this.conversion.set(c), error: () => {} });
    }

    if (this.auth.hasPermission('view_agent_performance')) {
      this.service.getAgentPerformance().subscribe({ next: a => this.agentPerf.set(a), error: () => {} });
    }

    this.loading.set(false);
  }
}
