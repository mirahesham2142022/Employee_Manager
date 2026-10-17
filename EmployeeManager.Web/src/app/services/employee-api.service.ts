import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CreateEmployeeDto,
  EmployeeDto,
  EmployeePageDto,
  UpdateEmployeeDto,
} from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeeApiService {
  private readonly http = inject(HttpClient);
  private readonly employeeUrl = `${environment.apiUrl}/employees`;

  getEmployees(
    page: number,
    pageSize: number,
    departmentId: number | null,
    searchTerm: string = '',
  ): Observable<EmployeePageDto> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (departmentId !== null) {
      params = params.set('departmentId', departmentId);
    }

    if (searchTerm && searchTerm.trim()) {
      params = params.set('searchTerm', searchTerm.trim());
    }

    return this.http.get<EmployeePageDto>(`${this.employeeUrl}/GetPage`, { params });
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