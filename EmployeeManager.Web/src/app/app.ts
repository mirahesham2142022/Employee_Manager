import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { DepartmentDto } from './models/department.model';
import { CreateEmployeeDto, EmployeeDto } from './models/employee.model';
import { EmployeeApiService } from './services/employee-api.service';

@Component({
  imports: [FormsModule],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  private readonly api = inject(EmployeeApiService);
  readonly employees = signal<EmployeeDto[]>([]);
  readonly departments = signal<DepartmentDto[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly page = signal(1);
  readonly pageSize = 10;
  readonly totalCount = signal(0);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  selectedDepartmentId: number | null = null;
  activeTab: 'employees' | 'departments' = 'employees';
  showForm = false;
  editingEmployeeId: number | null = null;
  form: CreateEmployeeDto = this.emptyForm();
  showDepartmentForm = false;
  editingDepartmentId: number | null = null;
  departmentName = '';

  constructor() {
    void this.loadDepartments();
    void this.loadEmployees();
  }

  async loadDepartments(): Promise<void> {
    try {
      this.departments.set(await firstValueFrom(this.api.getDepartments()));
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not load departments. Check that the API is running.'));
    }
  }

  async loadEmployees(): Promise<void> {
    this.loading.set(true);
    this.errorMessage.set(null);
    try {
      const result = await firstValueFrom(this.api.getEmployees(this.page(), this.pageSize, this.selectedDepartmentId));
      this.employees.set(result.items);
      this.totalCount.set(result.totalCount);
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not load employees. Check that the API is running.'));
    } finally {
      this.loading.set(false);
    }
  }

  async changeDepartment(departmentId: number | null): Promise<void> {
    this.selectedDepartmentId = departmentId;
    this.page.set(1);
    await this.loadEmployees();
  }

  async goToPage(page: number): Promise<void> {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
    await this.loadEmployees();
  }

  startCreate(): void {
    this.editingEmployeeId = null;
    this.form = this.emptyForm();
    this.showForm = true;
    this.errorMessage.set(null);
  }

  startEdit(employee: EmployeeDto): void {
    this.editingEmployeeId = employee.id;
    this.form = {
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      hireDate: employee.hireDate,
      departmentId: employee.departmentId,
    };
    this.showForm = true;
    this.errorMessage.set(null);
  }

  closeForm(): void {
    this.showForm = false;
    this.editingEmployeeId = null;
  }

  startCreateDepartment(): void {
    this.editingDepartmentId = null;
    this.departmentName = '';
    this.showDepartmentForm = true;
    this.errorMessage.set(null);
  }

  startEditDepartment(department: DepartmentDto): void {
    this.editingDepartmentId = department.id;
    this.departmentName = department.name;
    this.showDepartmentForm = true;
    this.errorMessage.set(null);
  }

  closeDepartmentForm(): void {
    this.showDepartmentForm = false;
    this.editingDepartmentId = null;
  }

  async saveDepartment(form: NgForm): Promise<void> {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.errorMessage.set(null);
    try {
      const dto = { name: this.departmentName.trim() };
      if (this.editingDepartmentId === null) {
        await firstValueFrom(this.api.createDepartment(dto));
      } else {
        await firstValueFrom(this.api.updateDepartment(this.editingDepartmentId, dto));
      }
      this.closeDepartmentForm();
      await this.loadDepartments();
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not save the department. Check the name and try again.'));
    } finally {
      this.saving.set(false);
    }
  }

  async deleteDepartment(department: DepartmentDto): Promise<void> {
    if (!window.confirm(`Delete ${department.name}?`)) return;
    this.errorMessage.set(null);
    try {
      await firstValueFrom(this.api.deleteDepartment(department.id));
      if (this.selectedDepartmentId === department.id) {
        this.selectedDepartmentId = null;
        this.page.set(1);
      }
      await this.loadDepartments();
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not delete the department. It may still have employees assigned.'));
    }
  }

  async saveEmployee(form: NgForm): Promise<void> {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.errorMessage.set(null);
    try {
      if (this.editingEmployeeId === null) {
        await firstValueFrom(this.api.createEmployee(this.form));
      } else {
        await firstValueFrom(this.api.updateEmployee(this.editingEmployeeId, this.form));
      }
      this.closeForm();
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not save the employee. Check the values and try again.'));
    } finally {
      this.saving.set(false);
    }
  }

  async deleteEmployee(employee: EmployeeDto): Promise<void> {
    if (!window.confirm(`Delete ${employee.firstName} ${employee.lastName}?`)) return;
    this.errorMessage.set(null);
    try {
      await firstValueFrom(this.api.deleteEmployee(employee.id));
      if (this.employees().length === 1 && this.page() > 1) this.page.update((value) => value - 1);
      await this.loadEmployees();
    } catch (error: unknown) {
      this.errorMessage.set(this.getErrorMessage(error, 'Could not delete the employee. Please try again.'));
    }
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse) {
      const responseBody = error.error;
      if (typeof responseBody === 'string') return responseBody;
      if (typeof responseBody?.error === 'string') return responseBody.error;
      if (typeof responseBody?.message === 'string') return responseBody.message;
      if (typeof responseBody?.title === 'string') return responseBody.title;
    }
    return fallback;
  }

  private emptyForm(): CreateEmployeeDto {
    return { firstName: '', lastName: '', email: '', hireDate: '', departmentId: 0 };
  }
}



