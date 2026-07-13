import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';

import { CreateTask } from './create-task';
import { TaskService } from './task.service';

describe('CreateTask', () => {
  let component: CreateTask;
  let fixture: ComponentFixture<CreateTask>;
  let taskService: jasmine.SpyObj<Pick<TaskService, 'create'>>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    taskService = jasmine.createSpyObj('TaskService', ['create']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CreateTask],
      providers: [
        { provide: TaskService, useValue: taskService as unknown as TaskService },
        { provide: Router, useValue: router },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTask);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  function fillValidForm(): void {
    const form = (component as any).form;
    if (!form) {
      return;
    }

    form.patchValue({
      title: 'Write unit tests',
      description: 'Cover create task validation and submission behavior.',
      dueDate: '2030-01-01',
      status: 'Pending',
    });
    fixture.detectChanges();
  }

  // Verifies the component can be instantiated before exercising its behaviors.
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // Confirms the form starts in a neutral state with blank fields.
  it('initializes the form with empty values', () => {
    const form = (component as any).form;

    expect(form).toBeDefined();
    expect(form.get('title').value).toBe('');
    expect(form.get('description').value).toBe('');
    expect(form.get('dueDate').value).toBe('');
    expect(form.get('status').value).toBe('');
  });

  // Protects the core validation rule that every new task must have a title.
  it('marks the title as required', () => {
    const titleControl = (component as any).form.get('title');

    titleControl.setValue('');
    fixture.detectChanges();

    expect(titleControl.valid).toBeFalse();
    expect(titleControl.hasError('required')).toBeTrue();
  });

  // Ensures the title length limit is enforced to match the API contract.
  it('rejects titles longer than 200 characters', () => {
    const titleControl = (component as any).form.get('title');

    titleControl.setValue('a'.repeat(201));
    fixture.detectChanges();

    expect(titleControl.valid).toBeFalse();
    expect(titleControl.hasError('maxlength')).toBeTrue();
  });

  // Guards the description length boundary so oversized input is rejected early.
  it('rejects descriptions longer than 1000 characters', () => {
    const descriptionControl = (component as any).form.get('description');

    descriptionControl.setValue('a'.repeat(1001));
    fixture.detectChanges();

    expect(descriptionControl.valid).toBeFalse();
    expect(descriptionControl.hasError('maxlength')).toBeTrue();
  });

  // Prevents users from submitting a task with an already expired due date.
  it('rejects due dates that are in the past', () => {
    const dueDateControl = (component as any).form.get('dueDate');

    dueDateControl.setValue(new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString());
    fixture.detectChanges();

    expect(dueDateControl.valid).toBeFalse();
    expect(dueDateControl.errors).toBeTruthy();
  });

  // Keeps status values aligned with the allowed domain values.
  it('rejects invalid status values', () => {
    const statusControl = (component as any).form.get('status');

    statusControl.setValue('Invalid');
    fixture.detectChanges();

    expect(statusControl.valid).toBeFalse();
    expect(statusControl.errors).toBeTruthy();
  });

  // Verifies the submit action stays unavailable until the form is valid.
  it('keeps the save button disabled while the form is invalid', () => {
    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;

    expect(saveButton).not.toBeNull();
    expect(saveButton.disabled).toBeTrue();
  });

  // Confirms form validity unlocks the save action as expected.
  it('enables the save button once the form becomes valid', () => {
    fillValidForm();
    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;

    expect(saveButton).not.toBeNull();
    expect(saveButton.disabled).toBeFalse();
  });

  // Ensures the create flow triggers the service exactly once for a valid submission.
  it('calls TaskService.create exactly once when the form is saved', () => {
    taskService.create.and.returnValue(of({ id: 1 }));
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();

    expect(taskService.create).toHaveBeenCalledTimes(1);
  });

  // Captures the UX requirement that a loading state is visible during submission.
  it('shows a loading indicator while the create request is in progress', () => {
    taskService.create.and.returnValue(new Observable(() => undefined));
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();
    fixture.detectChanges();

    const loading = fixture.nativeElement.querySelector('[data-testid="loading"]');
    expect(loading).not.toBeNull();
  });

  // Makes the happy path explicit by requiring navigation after a successful save.
  it('navigates back to the task list after a successful create', () => {
    taskService.create.and.returnValue(of({ id: 1 }));
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();

    expect(router.navigate).toHaveBeenCalledWith(['/tasks']);
  });

  // Protects the user experience for API validation failures with a readable message.
  it('displays a user-friendly validation error from the API', () => {
    taskService.create.and.returnValue(
      throwError(() => ({ error: { errors: { title: ['Title is required'] } } }))
    );
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();
    fixture.detectChanges();

    const errorMessage = fixture.nativeElement.querySelector('[data-testid="error-message"]');
    expect(errorMessage?.textContent).toContain('Please review the form and try again.');
  });

  // Covers non-validation failures so unexpected server issues also surface a friendly message.
  it('displays a user-friendly unexpected error message when the API fails', () => {
    taskService.create.and.returnValue(throwError(() => ({ status: 500 })));
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();
    fixture.detectChanges();

    const errorMessage = fixture.nativeElement.querySelector('[data-testid="error-message"]');
    expect(errorMessage?.textContent).toContain('Something went wrong. Please try again later.');
  });

  // Ensures the submission payload is shaped correctly before it leaves the component.
  it('sends the form values to the service when creating a task', () => {
    taskService.create.and.returnValue(of({ id: 1 }));
    fillValidForm();

    const saveButton = fixture.nativeElement.querySelector('button[type="submit"]') as HTMLButtonElement;
    saveButton.click();

    expect(taskService.create).toHaveBeenCalledWith(jasmine.objectContaining({
      title: 'Write unit tests',
      description: 'Cover create task validation and submission behavior.',
      dueDate: '2030-01-01',
      status: 'Pending',
    }));
  });
});
