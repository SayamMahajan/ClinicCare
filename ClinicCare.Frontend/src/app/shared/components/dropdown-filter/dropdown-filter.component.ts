import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-dropdown-filter',
  imports: [CommonModule, MaterialModule],
  templateUrl: './dropdown-filter.component.html'
})
export class DropdownFilterComponent {
  @Input({ required: true }) label!: string;
  @Input({ required: true }) options!: string[];

  @Output() valueChange = new EventEmitter<string>();
}
