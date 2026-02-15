import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-list-container',
  imports: [ MaterialModule],
  templateUrl: './list-container.component.html'
})
export class ListContainerComponent {
  @Input({ required: true }) columns!: string[];
  
  @Input() showPagination = false;
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() pageSize = 10;
  @Input() hasPreviousPage = false;
  @Input() hasNextPage = false;

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  onPreviousPage() {
    if (this.hasPreviousPage) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  onNextPage() {
    if (this.hasNextPage) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.pageSizeChange.emit(Number(select.value));
  }
}