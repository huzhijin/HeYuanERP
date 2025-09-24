<template>
  <!-- 对账单导出页：筛选条件 + 导出按钮 -->
  <div class="page-reconciliation-export">
    <AForm layout="inline" class="filter-bar">
      <AFormItem label="方式">
        <ASelect v-model:value="filters.method" allow-clear placeholder="全部" style="width: 160px">
          <ASelectOption value="现金">现金</ASelectOption>
          <ASelectOption value="银行转账">银行转账</ASelectOption>
          <ASelectOption value="支付宝">支付宝</ASelectOption>
          <ASelectOption value="微信">微信</ASelectOption>
        </ASelect>
      </AFormItem>

      <AFormItem label="日期范围">
        <ADatePicker.RangePicker v-model:value="dateRange" format="YYYY-MM-DD" style="width: 280px" />
      </AFormItem>

      <AFormItem>
        <AButton type="primary" @click="onExport" :loading="loading">导出 CSV</AButton>
      </AFormItem>
      <AFormItem>
        <AButton @click="onReset" :disabled="loading">重置</AButton>
      </AFormItem>
    </AForm>

    <p class="hint">说明：导出包含日期、方式、金额、备注与附件数量。建议选择 30 天内的时间范围以加快导出速度。</p>
  </div>
</template>

<script setup lang="ts">
// 中文说明：
// 对账单导出页面，调用后端 CSV 导出接口并触发浏览器下载。

import { reactive, ref } from 'vue';
import dayjs, { Dayjs } from 'dayjs';
import { message } from 'ant-design-vue';
import { usePaymentStore } from '../../store/payments';
import {
  Form as AForm,
  FormItem as AFormItem,
  Select as ASelect,
  Button as AButton,
  DatePicker as ADatePicker,
} from 'ant-design-vue';

const ASelectOption = ASelect.Option;

const store = usePaymentStore();

const filters = reactive({
  method: store.filters.method as string | undefined,
});
const dateRange = ref<[Dayjs, Dayjs] | null>(
  store.filters.dateFrom && store.filters.dateTo
    ? [dayjs(store.filters.dateFrom), dayjs(store.filters.dateTo)]
    : null
);

const loading = ref(false);

function syncToStore() {
  store.setFilters({
    method: filters.method || undefined,
    dateFrom: dateRange.value ? dateRange.value[0].format('YYYY-MM-DD') : undefined,
    dateTo: dateRange.value ? dateRange.value[1].format('YYYY-MM-DD') : undefined,
  });
}

async function onExport() {
  loading.value = true;
  try {
    syncToStore();
    await store.exportCsv();
  } catch (e: any) {
    message.error(e?.message || '导出失败');
  } finally {
    loading.value = false;
  }
}

function onReset() {
  filters.method = undefined;
  dateRange.value = null;
}

</script>

<style scoped>
.page-reconciliation-export {
  padding: 16px;
}
.filter-bar {
  margin-bottom: 8px;
}
.hint {
  color: #666;
}
</style>

