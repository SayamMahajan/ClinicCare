import { Component, Input } from '@angular/core';
import { StatCardComponent } from '../stat-card/stat-card.component';

export interface AnalyticsStat {
  label: string;
  value: string | number;
}

@Component({
  selector: 'app-analytics',
  imports: [StatCardComponent],
  templateUrl: './analytics.component.html'
})

export class AnalyticsComponent {
  @Input({ required: true }) stats!: AnalyticsStat[];
}
