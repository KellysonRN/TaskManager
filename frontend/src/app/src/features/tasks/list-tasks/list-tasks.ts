import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { TaskService } from '../create-task/task.service';

export interface TaskSummary {
  id: string;
  title: string | null;
  description: string | null;
  dueDate: string | null;
  status: string | null;
  ownerId: string;
}

@Component({
  selector: 'task-list',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './list-tasks.html',
  styleUrl: './list-tasks.css',
})
export class TaskList implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly authService = inject(AuthService);

  logout(): void {
    this.authService.logout();
  }

  tasks: TaskSummary[] = [];
  isLoading = false;
  deletingTaskId: string | null = null;
  editingTaskId: string | null = null;
  updatingTaskId: string | null = null;
  errorMessage: string | null = null;
  editModel: {
    title: string;
    description: string;
    dueDate: string;
    status: string;
  } = {
    title: '',
    description: '',
    dueDate: '',
    status: 'Pending',
  };

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.taskService.getAll().subscribe({
      next: (items) => {
        this.tasks = items as TaskSummary[];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load tasks right now.';
      },
    });
  }

  deleteTask(taskId: string): void {
    this.deletingTaskId = taskId;
    this.errorMessage = null;

    this.taskService.delete(taskId).subscribe({
      next: () => {
        this.tasks = this.tasks.filter((t) => t.id !== taskId);
        this.deletingTaskId = null;
      },
      error: () => {
        this.deletingTaskId = null;
        this.errorMessage = 'Unable to delete task right now.';
      },
    });
  }

  startEdit(task: TaskSummary): void {
    this.editingTaskId = task.id;
    this.errorMessage = null;
    this.editModel = {
      title: task.title ?? '',
      description: task.description ?? '',
      dueDate: task.dueDate ? task.dueDate.slice(0, 10) : '',
      status: task.status ?? 'Pending',
    };
  }

  cancelEdit(): void {
    this.editingTaskId = null;
    this.updatingTaskId = null;
  }

  saveEdit(taskId: string): void {
    if (!this.editModel.title.trim()) {
      this.errorMessage = 'Title is required.';
      return;
    }

    this.updatingTaskId = taskId;
    this.errorMessage = null;

    const payload = {
      title: this.editModel.title,
      description: this.editModel.description,
      dueDate: this.editModel.dueDate || null,
      status: this.editModel.status,
    };

    this.taskService.update(taskId, payload).subscribe({
      next: () => {
        this.tasks = this.tasks.map((task) =>
          task.id === taskId
            ? {
                ...task,
                title: payload.title,
                description: payload.description,
                dueDate: payload.dueDate,
                status: payload.status,
              }
            : task
        );
        this.updatingTaskId = null;
        this.editingTaskId = null;
      },
      error: () => {
        this.updatingTaskId = null;
        this.errorMessage = 'Unable to update task right now.';
      },
    });
  }
}
