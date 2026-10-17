import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DepartmentDto } from '../models/department.model';
import { CreateEmployeeDto, EmployeeDto, EmployeePageDto, UpdateEmployeeDto } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeeApiService {
  private readonly http = inject(HttpClient);
  private readonly employeeUrl = 'https://localhost:7025/api/employees';
  private readonly departmentUrl = 'https://localhost:7025/api/Department';

  getEmployees(page: number, pageSize: number, departmentId: number | null): Observable<EmployeePageDto> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (departmentId !== null) params = params.set('departmentId', departmentId);
    return this.http.get<EmployeePageDto>(`${this.employeeUrl}/GetPage`, { params });
  }

  getDepartments(): Observable<DepartmentDto[]> {
    return this.http.get<DepartmentDto[]>(`${this.departmentUrl}/GetAll`);
  }

  createEmployee(dto: CreateEmployeeDto): Observable<EmployeeDto> {
    return this.http.post<EmployeeDto>(`${this.employeeUrl}/Create`, dto);
  }

  updateEmployee(id: number, dto: UpdateEmployeeDto): Observable<EmployeeDto> {
    return this.http.put<EmployeeDto>(`${this.employeeUrl}/Update/${id}`, dto);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.employeeUrl}/Delete/${id}`);
  }
}
