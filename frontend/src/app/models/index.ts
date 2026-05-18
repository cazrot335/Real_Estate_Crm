export interface Lead {
  id: number;
  name: string;
  phone: string;
  status: string;
  assignedToUserId?: number;
  createdAt: string;
}

export interface Property {
  id: number;
  title: string;
  location: string;
  price: number;
  status: string;
  assignedAgentId?: number;
  createdAt: string;
}

export interface Deal {
  id: number;
  leadId: number;
  propertyId: number;
  agentId: number;
  amount: number;
  status: string;
  createdAt: string;
}

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  assignedToUserId: number;
  status: string;
  dueDate: string;
  leadId?: number;
  dealId?: number;
  createdAt: string;
}

export interface User {
  id: number;
  email: string;
  roles: string[];
}

export interface Role {
  id: number;
  name: string;
}

export interface Permission {
  id: number;
  name: string;
}

export interface ReportSummary {
  totalLeads?: number;
  convertedLeads?: number;
  totalDeals?: number;
  closedDeals?: number;
  totalRevenue?: number;
}

export interface AgentPerformance {
  agent: string;
  dealsClosed: number;
  revenue: number;
}

export interface ConversionRate {
  totalLeads: number;
  convertedLeads: number;
  conversionRate: number;
}
