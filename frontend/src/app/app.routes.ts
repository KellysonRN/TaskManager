import { Routes } from '@angular/router';

import { CreateTask } from './src/features/tasks/create-task/create-task';
import { TaskService } from './src/features/tasks/create-task/task.service';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'tasks/create',
    pathMatch: 'full',
  },
  {
    path: 'tasks/create',
    loadComponent: () => import('./src/features/tasks/create-task/create-task').then((m) => m.CreateTask),
    providers: [TaskService],
  },
];
