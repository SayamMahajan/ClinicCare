import { Component, input, output } from '@angular/core';
import { MaterialModule } from '../../ui/material.module';

@Component({
  selector: 'app-pagination',
  imports: [ MaterialModule ],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css',
})
export class PaginationComponent {
  currentPage = input.required<number>();
  totalPages = input.required<number>();
  hasPrevious = input.required<boolean>();
  hasNext = input.required<boolean>();
  pageSize = input<number>(10);

  pageChange = output<number>();
  pageSizeChange = output<number>();

  getDisplayedPages(): (number | string)[] {
    const current = this.currentPage();
    const total = this.totalPages();
    const pages: (number | string)[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      pages.push(1);

      if (current > 3) {
        pages.push('...');
      }
      const start = Math.max(2, current - 1);
      const end = Math.min(total - 1, current + 1);

      for (let i = start; i <= end; i++) {
        pages.push(i);
      }

      if (current < total - 2) {
        pages.push('...');
      }

      pages.push(total);
    }

    return pages;
  }

  onPageClick(page: number | string) {
    if (typeof page === 'number' && page !== this.currentPage()) {
      this.pageChange.emit(page);
    }
  }

  onPrevious() {
    if (this.hasPrevious()) {
      this.pageChange.emit(this.currentPage() - 1);
    }
  }

  onNext() {
    if (this.hasNext()) {
      this.pageChange.emit(this.currentPage() + 1);
    }
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.pageSizeChange.emit(+select.value);
  }

  isNumber(value: number | string): boolean {
    return typeof value === 'number';
  }
}