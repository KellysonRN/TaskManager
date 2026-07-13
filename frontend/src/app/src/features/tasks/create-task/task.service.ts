import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly apiUrl = `${environment.apiUrl}/api/tasks`;

  constructor(private readonly http: HttpClient) {}

  create(payload: unknown): Observable<unknown> {
    return this.http.post<unknown>(this.apiUrl, payload);
  }

  getAll(): Observable<unknown[]> {
    return this.http.get<unknown[]>(this.apiUrl);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  update(id: string, payload: unknown): Observable<unknown> {
    return this.http.put<unknown>(`${this.apiUrl}/${id}`, payload);
  }
}
