import { Component, EventEmitter, Output } from '@angular/core';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-toolbar',
  imports: [MaterialModule],
  templateUrl: './app-toolbar.component.html'
})
export class AppToolbarComponent {
  @Output() profile = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();
}
