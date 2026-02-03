import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-list-container',
  imports: [CommonModule, MaterialModule],
  templateUrl: './list-container.component.html'
})
export class ListContainerComponent {
  @Input({ required: true }) columns!: string[];
}
