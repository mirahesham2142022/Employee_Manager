import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error;
    if (typeof body === 'string' && body) return body;
    if (typeof body?.error === 'string') return body.error;
    if (typeof body?.message === 'string') return body.message;
    if (typeof body?.title === 'string') return body.title;
  }
  return fallback;
}
