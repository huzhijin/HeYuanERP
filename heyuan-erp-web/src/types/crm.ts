// CRM 前端类型：销售机会、客户拜访

export interface SalesOpportunity {
  id: number
  opportunityName: string
  accountId: number
  account?: any
  status: string
  estimatedValue: number
  probability: number
  stage: string
  source?: string
  expectedCloseDate?: string
  assignedToUserId: number
  assignedToUserName?: string
  createdAt?: string
}

export interface CustomerVisit {
  id: number
  accountId: number
  contactId?: number
  salesOpportunityId?: number
  visitDate: string
  type?: string
  purpose?: string
  notes?: string
  nextAction?: string
  nextActionDate?: string
  visitorId?: number
}

