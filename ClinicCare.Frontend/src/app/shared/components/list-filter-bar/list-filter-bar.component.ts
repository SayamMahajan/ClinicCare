import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';
import { DropdownFilterComponent } from "../dropdown-filter/dropdown-filter.component";

@Component({
  selector: 'app-list-filter-bar',
  imports: [CommonModule, MaterialModule, DropdownFilterComponent],
  templateUrl: './list-filter-bar.component.html'
})
export class ListFilterBarComponent {
  @Input({ required: true }) searchLabel!: string;
  @Input({ required: true }) label!: string;;
  @Input({ required: true }) options!: string[];

  @Output() searchChange = new EventEmitter<string>();
  @Output() optionChange = new EventEmitter<string>();
}
