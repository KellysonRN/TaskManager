import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { TaskService } from './task.service';

describe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('calls GET /api/tasks in getAll', () => {
    // Ensures task list retrieval hits the expected endpoint.
    service.getAll().subscribe((tasks) => {
      expect(tasks).toEqual([]);
    });

    const req = httpMock.expectOne('http://localhost:5000/api/tasks');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('calls POST /api/tasks in create', () => {
    // Ensures task creation posts payload to the backend contract.
    const payload = { title: 'New task' };

    service.create(payload).subscribe();

    const req = httpMock.expectOne('http://localhost:5000/api/tasks');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush({ id: '1' });
  });

  it('calls DELETE /api/tasks/{id} in delete', () => {
    // Ensures delete operation targets the correct resource URL.
    service.delete('abc-123').subscribe();

    const req = httpMock.expectOne('http://localhost:5000/api/tasks/abc-123');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('calls PUT /api/tasks/{id} in update', () => {
    // Ensures update operation targets the correct resource URL and payload.
    const payload = { title: 'Updated task', status: 'InProgress' };

    service.update('abc-123', payload).subscribe();

    const req = httpMock.expectOne('http://localhost:5000/api/tasks/abc-123');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(payload);
    req.flush({ id: 'abc-123', ...payload });
  });
});
