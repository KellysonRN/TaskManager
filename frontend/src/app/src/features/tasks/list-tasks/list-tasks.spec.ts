import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { TaskService } from '../create-task/task.service';
import { TaskList } from './list-tasks';

describe('TaskList', () => {
  let component: TaskList;
  let fixture: ComponentFixture<TaskList>;
  let taskServiceSpy: jasmine.SpyObj<Pick<TaskService, 'getAll' | 'delete' | 'update'>>;
  let authServiceSpy: jasmine.SpyObj<Pick<AuthService, 'logout'>>;

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['getAll', 'delete', 'update']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['logout']);

    taskServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [TaskList],
      providers: [
        provideRouter([]),
        { provide: TaskService, useValue: taskServiceSpy as unknown as TaskService },
        { provide: AuthService, useValue: authServiceSpy as unknown as AuthService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('loads tasks on init', () => {
    // Verifies initial render fetches tasks from the API.
    expect(taskServiceSpy.getAll).toHaveBeenCalledTimes(1);
  });

  it('deletes a task and removes it from the rendered list', () => {
    // Ensures delete flow updates UI state after successful API deletion.
    component.tasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: null,
        dueDate: null,
        status: 'Pending',
        ownerId: 'owner-1',
      },
    ];
    taskServiceSpy.delete.and.returnValue(of(void 0));

    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('.btn-delete') as HTMLButtonElement;
    button.click();
    fixture.detectChanges();

    expect(taskServiceSpy.delete).toHaveBeenCalledWith('task-1');
    expect(component.tasks.length).toBe(0);
  });

  it('shows an error when delete fails', () => {
    // Confirms users get feedback when task deletion fails.
    component.tasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: null,
        dueDate: null,
        status: 'Pending',
        ownerId: 'owner-1',
      },
    ];
    taskServiceSpy.delete.and.returnValue(throwError(() => new Error('delete failed')));

    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('.btn-delete') as HTMLButtonElement;
    button.click();
    fixture.detectChanges();

    expect(component.errorMessage).toBe('Unable to delete task right now.');
    expect(component.tasks.length).toBe(1);
  });

  it('triggers logout from the logout button', () => {
    // Ensures logout action delegates to AuthService.
    const button = fixture.nativeElement.querySelector('.btn-logout') as HTMLButtonElement;
    button.click();

    expect(authServiceSpy.logout).toHaveBeenCalledTimes(1);
  });

  it('starts edit mode when clicking Edit', () => {
    // Ensures task edit action enters editable state.
    component.tasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Desc',
        dueDate: null,
        status: 'Pending',
        ownerId: 'owner-1',
      },
    ];
    fixture.detectChanges();

    const editButton = fixture.nativeElement.querySelector('.btn-edit') as HTMLButtonElement;
    editButton.click();
    fixture.detectChanges();

    expect(component.editingTaskId).toBe('task-1');
    expect(component.editModel.title).toBe('Task 1');
  });

  it('updates a task and exits edit mode', () => {
    // Ensures update flow persists changes and refreshes local list state.
    component.tasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Desc',
        dueDate: null,
        status: 'Pending',
        ownerId: 'owner-1',
      },
    ];
    taskServiceSpy.update.and.returnValue(of({ id: 'task-1', title: 'Task updated' }));

    component.startEdit(component.tasks[0]);
    component.editModel.title = 'Task updated';
    component.saveEdit('task-1');

    expect(taskServiceSpy.update).toHaveBeenCalledWith('task-1', jasmine.objectContaining({ title: 'Task updated' }));
    expect(component.tasks[0].title).toBe('Task updated');
    expect(component.editingTaskId).toBeNull();
  });

  it('shows error when update fails', () => {
    // Confirms users receive feedback when update operation fails.
    component.tasks = [
      {
        id: 'task-1',
        title: 'Task 1',
        description: 'Desc',
        dueDate: null,
        status: 'Pending',
        ownerId: 'owner-1',
      },
    ];
    taskServiceSpy.update.and.returnValue(throwError(() => new Error('update failed')));

    component.startEdit(component.tasks[0]);
    component.editModel.title = 'Task updated';
    component.saveEdit('task-1');

    expect(component.errorMessage).toBe('Unable to update task right now.');
    expect(component.editingTaskId).toBe('task-1');
  });
});
