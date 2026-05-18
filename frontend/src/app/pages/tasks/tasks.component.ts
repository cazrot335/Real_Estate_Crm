import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { TaskItem, User } from '../../models';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './tasks.component.html',
})
export class TasksComponent implements OnInit {
  private service = inject(TaskService);
  private admin = inject(AdminService);
  auth = inject(AuthService);

  tasks = signal<TaskItem[]>([]);
  users = signal<User[]>([]);
  loading = signal(true);
  showCreate = signal(false);
  toast = signal('');
  toastType = signal<'success' | 'error'>('success');

  newTask = { title: '', desc: '', assignedTo: 0, dueDate: '', leadId: 0, dealId: 0 };
  assignForm = { taskId: 0, agentId: 0 };
  statusForm = { taskId: 0, status: '' };

  readonly statuses = ['Pending', 'Completed'];

  get canCreate() { return this.auth.hasPermission('create_task'); }
  get canAssign() { return this.auth.hasPermission('assign_task'); }
  get canStatus() { return this.auth.hasPermission('update_task_status'); }

  ngOnInit() {
    this.load();
    this.admin.getUsers().subscribe({ next: u => this.users.set(u), error: () => {} });
  }

  load() {
    this.loading.set(true);
    this.service.getTasks().subscribe({ next: t => { this.tasks.set(t); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  create() {
    const { title, desc, assignedTo, dueDate } = this.newTask;
    if (!title.trim() || !desc.trim() || !assignedTo || !dueDate) return;
    const leadId = this.newTask.leadId || undefined;
    const dealId = this.newTask.dealId || undefined;
    this.service.createTask(title, desc, assignedTo, dueDate, leadId, dealId).subscribe({
      next: () => { this.newTask = { title: '', desc: '', assignedTo: 0, dueDate: '', leadId: 0, dealId: 0 }; this.showCreate.set(false); this.load(); this.notify('Task created.', 'success'); },
      error: () => this.notify('Failed to create task.', 'error'),
    });
  }

  assign() {
    if (!this.assignForm.taskId || !this.assignForm.agentId) return;
    this.service.assignTask(this.assignForm.taskId, this.assignForm.agentId).subscribe({
      next: () => { this.load(); this.notify('Task assigned.', 'success'); },
      error: () => this.notify('Failed to assign task.', 'error'),
    });
  }

  updateStatus() {
    if (!this.statusForm.taskId || !this.statusForm.status) return;
    this.service.updateStatus(this.statusForm.taskId, this.statusForm.status).subscribe({
      next: () => { this.load(); this.notify('Status updated.', 'success'); },
      error: () => this.notify('Failed to update status.', 'error'),
    });
  }

  statusClass(s: string) {
    return s === 'Completed' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700';
  }

  private notify(msg: string, type: 'success' | 'error') {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
