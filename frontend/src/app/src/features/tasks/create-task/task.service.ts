import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable()
export class TaskService {
  create(_payload: unknown): Observable<unknown> {
    return of({});
  }
}
