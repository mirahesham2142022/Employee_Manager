import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { DepartmentDto, DepartmentInputDto } from '../models/department.model';

@Component({
  selector: 'app-department-form',
  imports: [FormsModule],
  styleUrl: './department-form.scss',
  templateUrl: './department-form.html',
})
export class DepartmentForm implements OnChanges {
  @Input() department: DepartmentDto | null = null;
  @Input() saving = false;
  @Output() saved = new EventEmitter<DepartmentInputDto>();
  @Output() cancelled = new EventEmitter<void>();

  name = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['department']) {
      this.name = this.department?.name ?? '';
    }
  }

  submit(form: NgForm): void {
    if (form.invalid || !this.name.trim()) {
      form.control.markAllAsTouched();
      return;
    }
    this.saved.emit({ name: this.name.trim() });
  }
}
