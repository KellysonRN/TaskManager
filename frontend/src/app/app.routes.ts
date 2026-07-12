import { Routes } from '@angular/router';

import { TaskService } from './src/features/tasks/create-task/task.service';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'tasks',
    pathMatch: 'full',
  },
  {
    path: 'tasks',
    loadComponent: () => import('./src/features/tasks/list-tasks/list-tasks').then((m) => m.TaskList),
    providers: [TaskService],
  },
  {
    path: 'tasks/create',
    loadComponent: () => import('./src/features/tasks/create-task/create-task').then((m) => m.CreateTask),
    providers: [TaskService],
  },
];
