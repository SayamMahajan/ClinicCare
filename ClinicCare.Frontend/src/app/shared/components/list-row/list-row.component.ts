import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-list-row',
  imports: [CommonModule, MaterialModule],
  templateUrl: './list-row.component.html'
})
export class ListRowComponent {
  @Input({ required: true }) values!: string[];
  @Input() actionType: 'info' | 'select' = 'info';

  @Input() selected = false;
  @Input() disabled = false;
  
  @Output() infoClick = new EventEmitter<void>();
  @Output() selectClick = new EventEmitter<void>();
}
