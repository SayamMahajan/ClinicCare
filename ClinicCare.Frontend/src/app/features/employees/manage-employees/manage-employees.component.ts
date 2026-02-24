import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { MaterialModule } from '../../../shared/ui/material.module';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { DropdownFilterComponent } from '../../../shared/components/dropdown-filter/dropdown-filter.component';
import { Subject, takeUntil } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { EmployeeDetailsDialogComponent } from '../../../shared/components/employee-details-dialog/employee-details-dialog.component';
import { Router } from '@angular/router';
import { EmployeeResponseDto, Gender } from '../../../shared/models/employee.model';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSearchParams } from '../../../shared/models/pagination.model';

type RoleFilter = 'Doctor';
type GenderFilter = 'All' | Gender;

@Component({
  selector: 'app-manage-employees',
  imports: [
    MaterialModule,
    ListFilterBarComponent,
    ListContainerComponent,
    ListRowComponent,
    PaginationComponent,
    DropdownFilterComponent,
  ],
  templateUrl: './manage-employees.component.html',
})
export class ManageEmployeesComponent implements OnInit, OnDestroy {
  private employeeService = inject(EmployeeService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  employees = signal<EmployeeResponseDto[]>([]);
  loading = signal(false);

  search = signal('');
  selectedRole = signal<RoleFilter>('Doctor');
  selectedGender = signal<GenderFilter>('All');

  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPrevious = signal(false);
  hasNext = signal(false);

  ngOnInit() {
    this.loadEmployees();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees() {
    this.loading.set(true);

    const role =  this.selectedRole();
    const genderFilter = this.selectedGender();

    const params: EmployeeSearchParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.search() || undefined,
      role,
      gender: genderFilter === 'All' ? undefined : genderFilter,
    };

    this.employeeService
      .getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.employees.set(result.items);
          this.totalPages.set(result.totalPages);
          this.hasPrevious.set(result.hasPreviousPage);
          this.hasNext.set(result.hasNextPage);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Failed to load employees:', err);
          this.employees.set([]);
          this.loading.set(false);
        },
      });
  }

  columns = computed(() => ['Name', 'Email', 'Role', 'Phone', 'Gender', 'Action']);

  onRoleChange(value: string) {
    this.selectedRole.set(value as RoleFilter);
    this.currentPage.set(1);
    this.loadEmployees();
  }

  onGenderChange(value: string) {
    this.selectedGender.set(value as 'All' | 'Male' | 'Female' | 'Others');
    this.currentPage.set(1);
    this.loadEmployees();
  }

  onSearchChange(value: string) {
    this.search.set(value);
    this.currentPage.set(1); 
    this.loadEmployees();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadEmployees();
  }

  onInfo(id: string) {
    const employee = this.employees().find((e) => e.id === id);
    if (!employee) return;

    this.dialog
      .open(EmployeeDetailsDialogComponent, {
        width: '450px',
        data: { employee },
      })
      .afterClosed()
      .subscribe((action) => {
        if (!action) return;

        switch (action) {
          case 'update':
            this.updateEmployee(employee);
            break;
          case 'delete':
            this.deleteEmployee(employee.id);
            break;
        }
      });
  }

  updateEmployee(employee: EmployeeResponseDto) {
    this.router.navigate(['/employees/edit', employee.id]);
  }

  deleteEmployee(id: string) {
    this.employeeService.delete(id).subscribe(() => {
      this.loadEmployees();
    });
  }

  mapRow(employee: EmployeeResponseDto): string[] {
    return [
      `${employee.firstName} ${employee.lastName}`,
      employee.email,
      employee.role,
      employee.phone,
      employee.gender,
    ];
  }
}