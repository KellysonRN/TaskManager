import { Routes } from '@angular/router';
import { authGuard } from './src/core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./src/features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'logout',
    loadComponent: () => import('./src/features/auth/logout/logout').then((m) => m.Logout),
  },
  {
    path: 'tasks',
    loadComponent: () => import('./src/features/tasks/list-tasks/list-tasks').then((m) => m.TaskList),
    canActivate: [authGuard],
  },
  {
    path: 'tasks/create',
    loadComponent: () => import('./src/features/tasks/create-task/create-task').then((m) => m.CreateTask),
    canActivate: [authGuard],
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
