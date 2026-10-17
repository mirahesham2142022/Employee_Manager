import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DepartmentDto, DepartmentInputDto } from '../models/department.model';

@Injectable({ providedIn: 'root' })
export class DepartmentApiService {
  private readonly http = inject(HttpClient);
  private readonly departmentUrl = `${environment.apiUrl}/Department`;

  getDepartments(): Observable<DepartmentDto[]> {
    return this.http.get<DepartmentDto[]>(`${this.departmentUrl}/GetAll`);
  }

  createDepartment(dto: DepartmentInputDto): Observable<DepartmentDto> {
    return this.http.post<DepartmentDto>(`${this.departmentUrl}/Create`, dto);
  }

  updateDepartment(id: number, dto: DepartmentInputDto): Observable<DepartmentDto> {
    return this.http.put<DepartmentDto>(`${this.departmentUrl}/Update/${id}`, dto);
  }

  deleteDepartment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.departmentUrl}/Delete/${id}`);
  }
}