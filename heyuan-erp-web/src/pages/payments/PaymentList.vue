<template>
  <!-- 收款列表页：筛选 + 表格 + 导出/新建 -->
  <div class="page-payments">
    <AForm layout="inline" class="filter-bar">
      <AFormItem label="方式">
        <ASelect v-model:value="filters.method" allow-clear placeholder="全部" style="width: 140px">
          <ASelectOption value="现金">现金</ASelectOption>
          <ASelectOption value="银行转账">银行转账</ASelectOption>
          <ASelectOption value="支付宝">支付宝</ASelectOption>
          <ASelectOption value="微信">微信</ASelectOption>
        </ASelect>
      </AFormItem>

      <AFormItem label="金额范围">
        <AInputNumber v-model:value="filters.minAmount" :min="0" :precision="2" placeholder="最小" style="width: 120px" />
        <span style="margin: 0 8px">-</span>
        <AInputNumber v-model:value="filters.maxAmount" :min="0" :precision="2" placeholder="最大" style="width: 120px" />
      </AFormItem>

      <AFormItem label="日期范围">
        <ADatePicker.RangePicker v-model:value="dateRange" format="YYYY-MM-DD" style="width: 260px" />
      </AFormItem>

      <AFormItem label="关键字">
        <AInput v-model:value="filters.keyword" allow-clear placeholder="备注模糊" style="width: 180px" />
      </AFormItem>

      <AFormItem>
        <AButton type="primary" @click="onSearch" :loading="loading">查询</AButton>
      </AFormItem>
      <AFormItem>
        <AButton @click="onReset" :disabled="loading">重置</AButton>
      </AFormItem>
      <AFormItem>
        <AButton @click="onExport" :disabled="loading">导出对账单</AButton>
      </AFormItem>
      <AFormItem style="margin-left: auto">
        <AButton type="primary" ghost @click="toCreate">新增收款</AButton>
      </AFormItem>
    </AForm>

    <ATable
      row-key="id"
      :columns="columns"
      :data-source="items"
      :loading="loading"
      :pagination="paginationOpts"
      @change="handleTableChange"
    />
  </div>
  
</template>

<script setup lang="ts">
// 中文说明：
// 收款列表页：包含筛选、分页、排序、导出。

import { computed, onMounted, reactive, ref, watch } from 'vue';
import dayjs, { Dayjs } from 'dayjs';
import { usePaymentStore } from '../../store/payments';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import {
  Form as AForm,
  FormItem as AFormItem,
  Input as AInput,
  Select as ASelect,
  DatePicker as ADatePicker,
  Button as AButton,
  Table as ATable,
  InputNumber as AInputNumber,
} from 'ant-design-vue';

const ASelectOption = ASelect.Option;

const store = usePaymentStore();
const router = useRouter();

// 本地筛选状态与日期范围
const filters = reactive({
  method: store.filters.method as string | undefined,
  minAmount: store.filters.minAmount as number | undefined,
  maxAmount: store.filters.maxAmount as number | undefined,
  keyword: store.filters.keyword as string | undefined,
});

const dateRange = ref<[Dayjs, Dayjs] | null>(
  store.filters.dateFrom && store.filters.dateTo
    ? [dayjs(store.filters.dateFrom), dayjs(store.filters.dateTo)]
    : null
);

const loading = computed(() => store.loading);
const items = computed(() => store.items);

const columns = [
  { title: '日期', dataIndex: 'paymentDate', sorter: true },
  { title: '方式', dataIndex: 'method' },
  { title: '金额', dataIndex: 'amount', sorter: true },
  { title: '备注', dataIndex: 'remark' },
  { title: '附件数', dataIndex: 'attachmentCount', width: 100 },
];

const paginationOpts = computed(() => ({
  current: store.page,
  pageSize: store.pageSize,
  total: store.totalCount,
  showSizeChanger: true,
  showTotal: (t: number) => `共 ${t} 条`,
}));

function syncFiltersToStore() {
  store.setFilters({
    method: filters.method || undefined,
    minAmount: filters.minAmount,
    maxAmount: filters.maxAmount,
    keyword: filters.keyword || undefined,
    dateFrom: dateRange.value ? dateRange.value[0].format('YYYY-MM-DD') : undefined,
    dateTo: dateRange.value ? dateRange.value[1].format('YYYY-MM-DD') : undefined,
  });
}

async function onSearch() {
  store.setPage(1);
  syncFiltersToStore();
  await store.fetchList();
}

function onReset() {
  filters.method = undefined;
  filters.minAmount = undefined;
  filters.maxAmount = undefined;
  filters.keyword = undefined;
  dateRange.value = null;
  onSearch();
}

async function onExport() {
  syncFiltersToStore();
  try {
    await store.exportCsv();
  } catch (e: any) {
    message.error(e?.message || '导出失败');
  }
}

function toCreate() {
  router.push({ path: '/payments/create' });
}

async function handleTableChange(pag: any, _filters: any, sorter: any) {
  if (pag) {
    store.setPage(pag.current || 1);
    store.setPageSize(pag.pageSize || 20);
  }
  if (sorter && sorter.field) {
    const field = sorter.field as string;
    store.setFilters({ sortBy: field, sortOrder: sorter.order === 'ascend' ? 'asc' : 'desc' });
  }
  syncFiltersToStore();
  await store.fetchList();
}

onMounted(async () => {
  await store.fetchList();
});

</script>

<style scoped>
.page-payments {
  padding: 16px;
}
.filter-bar {
  margin-bottom: 12px;
}
</style>

