<template>
  <!-- 发票列表页：状态筛选 + 分页表格 + 打印操作 -->
  <div class="page invoice-list">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="状态">
          <a-select v-model:value="status" style="width: 160px" allow-clear placeholder="全部">
            <a-select-option value="Draft">草稿</a-select-option>
            <a-select-option value="Pending">待开具</a-select-option>
            <a-select-option value="Issued">已开具</a-select-option>
            <a-select-option value="Canceled">已作废</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="number" data-index="number" title="发票号" width="180" />
        <a-table-column key="customerName" data-index="customerName" title="客户" width="200" />
        <a-table-column key="createdAt" data-index="createdAt" title="创建时间" width="180">
          <template #default="{ text }">{{ dayjs(text).format('YYYY-MM-DD HH:mm') }}</template>
        </a-table-column>
        <a-table-column key="status" data-index="status" title="状态" width="120">
          <template #default="{ text }">
            <InvoiceStatusTag :status="text" />
          </template>
        </a-table-column>
        <a-table-column key="grandTotal" data-index="grandTotal" title="含税合计" width="140">
          <template #default="{ text }">{{ Number(text).toFixed(2) }}</template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="200">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="printPdf(record.id)">打印</a-button>
            </a-space>
          </template>
        </a-table-column>
      </a-table>

      <div class="pager">
        <a-pagination
          :current="query.page"
          :page-size="query.size"
          :total="total"
          :show-total="(t:number) => `共 ${t} 条`"
          @change="onPageChange"
          @showSizeChange="onSizeChange"
          show-size-changer
          :page-size-options="['10','20','50','100']"
        />
      </div>
    </a-card>
  </div>
  
</template>

<script setup lang="ts">
// 发票列表页：使用 Pinia 仓库 + 服务封装
import { ref, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import dayjs from 'dayjs';
import { useInvoiceStore } from '@/store/invoices';
import type { InvoiceStatus } from '@/services/invoices';
import { printInvoicePdf } from '@/services/invoices';

import InvoiceStatusTag from '@/components/invoices/InvoiceStatusTag.vue';

const store = useInvoiceStore();

const status = ref<InvoiceStatus | undefined>(undefined);

const { query, items: rows, total, loading } = storeToRefs(store);

async function load() {
  store.setStatus(status.value);
  await store.fetchList();
}

function onSearch() { query.value.page = 1; load(); }
function onReset() { status.value = undefined; query.value.page = 1; query.value.size = 20; load(); }
function onPageChange(p: number, s?: number) { query.value.page = p; if (s) query.value.size = s; load(); }
function onSizeChange(p: number, s: number) { query.value.page = p; query.value.size = s; load(); }

async function printPdf(id: string) {
  const bytes = await printInvoicePdf(id);
  const blob = new Blob([bytes], { type: 'application/pdf' });
  const url = URL.createObjectURL(blob);
  // 直接新窗口预览，若需下载可改为 a[download]
  window.open(url, '_blank');
}

onMounted(load);
</script>

<style scoped>
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>
