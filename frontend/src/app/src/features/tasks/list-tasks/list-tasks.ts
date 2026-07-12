import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';

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
  imports: [CommonModule, RouterLink],
  templateUrl: './list-tasks.html',
  styleUrl: './list-tasks.css',
})
export class TaskList implements OnInit {
  private readonly taskService = inject(TaskService);

  tasks: TaskSummary[] = [];
  isLoading = false;
  errorMessage: string | null = null;

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
}
