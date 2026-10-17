import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { DepartmentDto } from '../models/department.model';
import { CreateEmployeeDto, EmployeeDto } from '../models/employee.model';
import { DepartmentApiService } from '../services/department-api.service';
import { EmployeeApiService } from '../services/employee-api.service';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { getErrorMessage } from '../shared/http-error';
import { EmployeeForm } from './employee-form';

@Component({
  selector: 'app-employees-page',
  imports: [FormsModule, DatePipe, ConfirmDialogComponent, EmployeeForm],
  styleUrl: './employees-page.scss',
  templateUrl: './employees-page.html',
})
export class EmployeesPage {
  private readonly employeeApi = inject(EmployeeApiService);
  private readonly departmentApi = inject(DepartmentApiService);
  private readonly pageSize = 10;

  readonly employees = signal<EmployeeDto[]>([]);
  readonly departments = signal<DepartmentDto[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly page = signal(1);
  readonly totalCount = signal(0);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));
  readonly selectedDepartmentId = signal<number | null>(null);
  readonly formOpen = signal(false);
  readonly editingEmployee = signal<EmployeeDto | null>(null);
  readonly employeeToDelete = signal<EmployeeDto | null>(null);

  searchTerm = '';

  constructor() {
    void this.loadDepartments();
    void this.loadEmployees();
  }

  async loadDepartments(): Promise<void> {
    try {
      this.departments.set(await firstValueFrom(this.departmentApi.getDepartments()));
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not load departments. Check that the API is running.'));
    }
  }

  async loadEmployees(): Promise<void> {
    this.loading.set(true);
    this.errorMessage.set(null);
    try {
      const result = await firstValueFrom(
        this.employeeApi.getEmployees(this.page(), this.pageSize, this.selectedDepartmentId(), this.searchTerm),
      );
      this.employees.set(result.items);
      this.totalCount.set(result.totalCount);
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not load employees. Check that the API is running.'));
    } finally {
      this.loading.set(false);
    }
  }

  async changeDepartment(departmentId: number | null): Promise<void> {
    this.selectedDepartmentId.set(departmentId);
    this.page.set(1);
    await this.loadEmployees();
  }

  async searchEmployees(): Promise<void> {
    this.page.set(1);
    await this.loadEmployees();
  }

  async goToPage(page: number): Promise<void> {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
    await this.loadEmployees();
  }

  openCreateForm(): void {
    this.editingEmployee.set(null);
    this.formOpen.set(true);
    this.errorMessage.set(null);
  }

  openEditForm(employee: EmployeeDto): void {
    this.editingEmployee.set(employee);
    this.formOpen.set(true);
    this.errorMessage.set(null);
  }

  closeForm(): void {
    this.formOpen.set(false);
    this.editingEmployee.set(null);
  }

  async saveEmployee(dto: CreateEmployeeDto): Promise<void> {
    this.saving.set(true);
    this.errorMessage.set(null);
    try {
      const editing = this.editingEmployee();
      if (editing) {
        await firstValueFrom(this.employeeApi.updateEmployee(editing.id, dto));
      } else {
        await firstValueFrom(this.employeeApi.createEmployee(dto));
      }
      this.closeForm();
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not save the employee. Check the values and try again.'));
    } finally {
      this.saving.set(false);
    }
  }

  askToDelete(employee: EmployeeDto): void {
    this.employeeToDelete.set(employee);
  }

  cancelDelete(): void {
    this.employeeToDelete.set(null);
  }

  async confirmDelete(): Promise<void> {
    const employee = this.employeeToDelete();
    if (!employee) return;

    this.employeeToDelete.set(null);
    this.errorMessage.set(null);
    try {
      await firstValueFrom(this.employeeApi.deleteEmployee(employee.id));
      if (this.employees().length === 1 && this.page() > 1) this.page.update((value) => value - 1);
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not delete the employee. Please try again.'));
    }
  }
}
