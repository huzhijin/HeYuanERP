// 发票列表状态仓库：保存查询条件与分页结果
import { defineStore } from 'pinia';
import { listInvoices, type InvoiceListItem, type InvoiceListQuery, type InvoiceStatus } from '@/services/invoices';

export interface InvoiceState {
  query: InvoiceListQuery;
  items: InvoiceListItem[];
  total: number;
  loading: boolean;
}

export const useInvoiceStore = defineStore('invoices', {
  state: (): InvoiceState => ({
    query: { page: 1, size: 20 },
    items: [],
    total: 0,
    loading: false,
  }),
  actions: {
    setStatus(status?: InvoiceStatus) {
      this.query.status = status;
    },
    setPage(page: number) {
      this.query.page = page;
    },
    setSize(size: number) {
      this.query.size = size;
    },
    async fetchList() {
      this.loading = true;
      try {
        const resp = await listInvoices(this.query);
        this.items = resp.items;
        this.total = resp.total;
      } finally {
        this.loading = false;
      }
    },
  },
});

