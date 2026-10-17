import { Component, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DepartmentDto, DepartmentInputDto } from '../models/department.model';
import { DepartmentApiService } from '../services/department-api.service';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { getErrorMessage } from '../shared/http-error';
import { DepartmentForm } from './department-form';

@Component({
  selector: 'app-departments-page',
  imports: [ConfirmDialogComponent, DepartmentForm],
  templateUrl: './departments-page.html',
})
export class DepartmentsPage {
  private readonly api = inject(DepartmentApiService);

  readonly departments = signal<DepartmentDto[]>([]);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly formOpen = signal(false);
  readonly editingDepartment = signal<DepartmentDto | null>(null);
  readonly departmentToDelete = signal<DepartmentDto | null>(null);

  constructor() {
    void this.loadDepartments();
  }

  async loadDepartments(): Promise<void> {
    try {
      this.departments.set(await firstValueFrom(this.api.getDepartments()));
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not load departments. Check that the API is running.'));
    }
  }

  openCreateForm(): void {
    this.editingDepartment.set(null);
    this.formOpen.set(true);
    this.errorMessage.set(null);
  }

  openEditForm(department: DepartmentDto): void {
    this.editingDepartment.set(department);
    this.formOpen.set(true);
    this.errorMessage.set(null);
  }

  closeForm(): void {
    this.formOpen.set(false);
    this.editingDepartment.set(null);
  }

  async saveDepartment(dto: DepartmentInputDto): Promise<void> {
    this.saving.set(true);
    this.errorMessage.set(null);
    try {
      const editing = this.editingDepartment();
      if (editing) {
        await firstValueFrom(this.api.updateDepartment(editing.id, dto));
      } else {
        await firstValueFrom(this.api.createDepartment(dto));
      }
      this.closeForm();
      await this.loadDepartments();
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not save the department. Check the name and try again.'));
    } finally {
      this.saving.set(false);
    }
  }

  askToDelete(department: DepartmentDto): void {
    this.departmentToDelete.set(department);
  }

  cancelDelete(): void {
    this.departmentToDelete.set(null);
  }

  async confirmDelete(): Promise<void> {
    const department = this.departmentToDelete();
    if (!department) return;

    this.departmentToDelete.set(null);
    this.errorMessage.set(null);
    try {
      await firstValueFrom(this.api.deleteDepartment(department.id));
      await this.loadDepartments();
    } catch (error: unknown) {
      this.errorMessage.set(getErrorMessage(error, 'Could not delete the department. It may still have employees assigned.'));
    }
  }
}
