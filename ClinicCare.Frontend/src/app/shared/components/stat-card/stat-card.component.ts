import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-stat-card',
  imports: [MaterialModule],
  templateUrl: './stat-card.component.html'
})
export class StatCardComponent {
  @Input({ required: true }) label!: string;
  @Input({ required: true }) value!: string | number;
}
