import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { TaskService } from './task.service';

@Component({
  selector: 'create-task',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-task.html',
  styleUrl: './create-task.css',
})
export class CreateTask {
  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly router = inject(Router);

  form: FormGroup<any>;
  isSaving = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor() {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(1000)],
      dueDate: ['', this.dueDateValidator],
      status: ['', this.statusValidator],
    });
  }

  get isFormValid(): boolean {
    return this.form.valid;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;
    this.successMessage = null;

    const payload = {
      title: this.form.value.title,
      description: this.form.value.description,
      dueDate: this.form.value.dueDate,
      status: this.form.value.status,
    };

    this.taskService.create(payload).subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage = 'Task created successfully!';
        this.router.navigate(['/tasks']);
      },
      error: (error) => {
        this.isSaving = false;
        if (error?.error?.errors) {
          this.errorMessage = 'Please review the form and try again.';
        } else {
          this.errorMessage = 'Something went wrong. Please try again later.';
        }
      },
    });
  }

  private dueDateValidator(control: { value: string | null }): { [key: string]: boolean } | null {
    if (!control.value) {
      return null;
    }

    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    return selectedDate < today ? { pastDate: true } : null;
  }

  private statusValidator(control: { value: string | null }): { [key: string]: boolean } | null {
    const allowedStatuses = ['Pending', 'InProgress', 'Completed'];
    if (!control.value) {
      return null;
    }

    return allowedStatuses.includes(control.value) ? null : { invalidStatus: true };
  }
}
