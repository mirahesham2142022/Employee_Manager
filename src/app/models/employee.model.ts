export interface EmployeeDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  hireDate: string;
  departmentId: number;
  departmentName: string;
  createdAt: string;
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  hireDate: string;
  departmentId: number;
}

export type UpdateEmployeeDto = CreateEmployeeDto;

export interface EmployeePageDto {
  items: EmployeeDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
