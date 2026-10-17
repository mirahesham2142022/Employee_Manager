import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { DepartmentDto } from '../models/department.model';
import { CreateEmployeeDto, EmployeeDto } from '../models/employee.model';

@Component({
  selector: 'app-employee-form',
  imports: [FormsModule],
  styleUrl: './employee-form.scss',
  templateUrl: './employee-form.html',
})
export class EmployeeForm implements OnChanges {
  @Input() employee: EmployeeDto | null = null;
  @Input() departments: DepartmentDto[] = [];
  @Input() saving = false;
  @Output() saved = new EventEmitter<CreateEmployeeDto>();
  @Output() cancelled = new EventEmitter<void>();

  model: CreateEmployeeDto = this.emptyModel();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employee']) {
      this.model = this.employee ? this.toModel(this.employee) : this.emptyModel();
    }
  }

  submit(form: NgForm): void {
    if (form.invalid || this.model.departmentId < 1) {
      form.control.markAllAsTouched();
      return;
    }
    this.saved.emit({ ...this.model });
  }

  private toModel(employee: EmployeeDto): CreateEmployeeDto {
    return {
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      hireDate: employee.hireDate,
      departmentId: employee.departmentId,
    };
  }

  private emptyModel(): CreateEmployeeDto {
    return { firstName: '', lastName: '', email: '', hireDate: '', departmentId: 0 };
  }
}
