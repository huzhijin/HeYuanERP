// 中文说明：
// 收款 Pinia 状态仓库：包含列表查询、创建收款、对账单导出等操作。

import { defineStore } from 'pinia';
import { message } from 'ant-design-vue';
import type { PaymentListItemDto, PaymentListQuery, PaymentCreateForm, PaymentDetailDto } from '../types/payments';
import { listPayments, createPayment, getPayment, exportReconciliation } from '../api/payments';
import { downloadBlob } from '../utils/download';

export const usePaymentStore = defineStore('payments', {
  state: () => ({
    loading: false as boolean,
    creating: false as boolean,
    items: [] as PaymentListItemDto[],
    page: 1 as number,
    pageSize: 20 as number,
    totalCount: 0 as number,
    filters: {
      method: undefined as string | undefined,
      minAmount: undefined as number | undefined,
      maxAmount: undefined as number | undefined,
      dateFrom: undefined as string | undefined, // yyyy-MM-dd
      dateTo: undefined as string | undefined,
      keyword: undefined as string | undefined,
      sortBy: 'paymentDate' as string,
      sortOrder: 'desc' as 'asc' | 'desc',
    },
    detail: null as PaymentDetailDto | null,
    lastError: null as string | null,
  }),

  actions: {
    // 设置分页
    setPage(page: number) {
      this.page = page > 0 ? page : 1;
    },
    setPageSize(size: number) {
      this.pageSize = size > 0 ? size : 20;
    },
    setFilters(partial: Partial<PaymentListQuery>) {
      this.filters = { ...this.filters, ...partial } as any;
    },

    // 列表查询
    async fetchList() {
      this.loading = true;
      this.lastError = null;
      try {
        const result = await listPayments({
          page: this.page,
          pageSize: this.pageSize,
          sortBy: this.filters.sortBy,
          sortOrder: this.filters.sortOrder,
          method: this.filters.method,
          minAmount: this.filters.minAmount,
          maxAmount: this.filters.maxAmount,
          dateFrom: this.filters.dateFrom,
          dateTo: this.filters.dateTo,
          keyword: this.filters.keyword,
        });
        this.items = result.items;
        this.totalCount = result.totalCount;
        this.page = result.page;
        this.pageSize = result.pageSize;
      } catch (e: any) {
        this.lastError = e?.message || '查询失败';
        message.error(this.lastError);
      } finally {
        this.loading = false;
      }
    },

    // 创建收款
    async create(form: PaymentCreateForm) {
      this.creating = true;
      this.lastError = null;
      try {
        const detail = await createPayment(form);
        this.detail = detail;
        message.success('创建收款成功');
        // 刷新列表
        await this.fetchList();
        return detail;
      } catch (e: any) {
        this.lastError = e?.message || '创建失败';
        message.error(this.lastError);
        throw e;
      } finally {
        this.creating = false;
      }
    },

    // 获取详情
    async loadDetail(id: string) {
      try {
        const detail = await getPayment(id);
        this.detail = detail;
        return detail;
      } catch (e: any) {
        this.lastError = e?.message || '加载详情失败';
        message.error(this.lastError);
        throw e;
      }
    },

    // 导出对账单（CSV）
    async exportCsv() {
      try {
        const { blob, fileName } = await exportReconciliation({
          dateFrom: this.filters.dateFrom,
          dateTo: this.filters.dateTo,
          method: this.filters.method,
        });
        downloadBlob(blob, fileName, 'text/csv;charset=utf-8');
        message.success('对账单已开始下载');
      } catch (e: any) {
        this.lastError = e?.message || '导出失败';
        message.error(this.lastError);
        throw e;
      }
    },
  },
});

