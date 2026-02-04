import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../shared/ui/material.module';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { Employee, EmployeeService } from '../../../shared/services/employee.service';
import { Subject, takeUntil } from 'rxjs';

type Role = 'Admin' | 'Doctor';
type RoleFilter = 'All' | Role;

@Component({
  selector: 'app-manage-employees',
  imports: [CommonModule, MaterialModule, ListFilterBarComponent, ListContainerComponent, ListRowComponent],
  templateUrl: './manage-employees.component.html',
})

export class ManageEmployeesComponent implements OnInit, OnDestroy {
  private employeeService = inject(EmployeeService);
  private destroy$ = new Subject<void>();

  employees: Employee[] = [];
  loading = false;
  search = '';
  selectedRole: RoleFilter = 'All';

  ngOnInit() {
    this.loadEmployees();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees(role?: RoleFilter) {
    this.loading = true;
    const apiRole = role === 'All' ? undefined : role;
    
    this.employeeService.getEmployees(apiRole).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (data: Employee[]) => {
        this.employees = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error:', err);
        this.employees = [];
        this.loading = false;
      }
    });
  }

  onRoleChange(value: string) {
    this.selectedRole = value as RoleFilter;
    this.loadEmployees(this.selectedRole);
  }

  onSearchChange(value: string) {
    this.search = value;
  }

  filteredEmployees(): Employee[] {
    return this.employees.filter(emp => {
      const matchesSearch = 
        `${emp.firstName} ${emp.lastName} ${emp.email}`
          .toLowerCase()
          .includes(this.search.toLowerCase());

      const matchesRole = 
        this.selectedRole === 'All' || emp.role === this.selectedRole;

      return matchesSearch && matchesRole;
    });
  }
}
