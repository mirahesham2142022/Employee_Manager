import { Component, signal } from '@angular/core';
import { DepartmentsPage } from './departments/departments-page';
import { EmployeesPage } from './employees/employees-page';

type Tab = 'employees' | 'departments';

@Component({
  selector: 'app-root',
  imports: [EmployeesPage, DepartmentsPage],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  readonly activeTab = signal<Tab>('employees');
}
