import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { TaskItem } from '../models';

const API = 'http://localhost:5241/api/task';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private http = inject(HttpClient);

  getTasks() { return this.http.get<TaskItem[]>(API); }

  createTask(title: string, desc: string, assignedTo: number, dueDate: string, leadId?: number, dealId?: number) {
    let params = new HttpParams()
      .set('title', title)
      .set('desc', desc)
      .set('assignedTo', assignedTo)
      .set('dueDate', dueDate);
    if (leadId) params = params.set('leadId', leadId);
    if (dealId) params = params.set('dealId', dealId);
    return this.http.post<TaskItem>(API, null, { params });
  }

  assignTask(taskId: number, agentId: number) {
    const params = new HttpParams().set('taskId', taskId).set('agentId', agentId);
    return this.http.post(`${API}/assign`, null, { params, responseType: 'text' });
  }

  updateStatus(taskId: number, status: string) {
    const params = new HttpParams().set('taskId', taskId).set('status', status);
    return this.http.post(`${API}/status`, null, { params, responseType: 'text' });
  }
}
