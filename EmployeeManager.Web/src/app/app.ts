import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { DepartmentsPage } from './departments/departments-page';
import { EmployeesPage } from './employees/employees-page';
import { AuthService } from './services/auth.service';

type Tab = 'employees' | 'departments';

@Component({
  selector: 'app-root',
  imports: [EmployeesPage, DepartmentsPage],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  readonly auth = inject(AuthService);
  readonly activeTab = signal<Tab>('employees');

  @ViewChild('googleBtn')
  set googleBtn(element: ElementRef<HTMLElement> | undefined) {
    if (element) this.auth.initGoogleButton(element.nativeElement);
  }
}