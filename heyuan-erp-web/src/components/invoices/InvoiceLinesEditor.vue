<template>
  <!-- 发票行编辑器：支持增删行与数量/单价/税率编辑，实时计算金额（仅前端显示） -->
  <div class="invoice-lines-editor">
    <a-space style="margin-bottom: 8px">
      <a-button type="dashed" @click="addRow">新增行</a-button>
    </a-space>

    <a-table :data-source="rows" :pagination="false" row-key="_rowKey">
      <a-table-column title="#" width="60">
        <template #default="{ index }">{{ index + 1 }}</template>
      </a-table-column>
      <a-table-column title="商品编码" width="160">
        <template #default="{ record, index }">
          <a-input v-model:value="record.productCode" @change="emitChange(index)" placeholder="必填" />
        </template>
      </a-table-column>
      <a-table-column title="商品名称" width="200">
        <template #default="{ record, index }">
          <a-input v-model:value="record.productName" @change="emitChange(index)" placeholder="必填" />
        </template>
      </a-table-column>
      <a-table-column title="规格" width="140">
        <template #default="{ record, index }">
          <a-input v-model:value="record.specification" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="单位" width="100">
        <template #default="{ record, index }">
          <a-input v-model:value="record.unit" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="数量" width="120">
        <template #default="{ record, index }">
          <a-input-number v-model:value="record.quantity" :min="0" :precision="4" style="width: 100%" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="单价" width="120">
        <template #default="{ record, index }">
          <a-input-number v-model:value="record.unitPrice" :min="0" :precision="2" style="width: 100%" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="税率" width="120">
        <template #default="{ record, index }">
          <a-input-number v-model:value="record.taxRate" :min="0" :max="1" :step="0.01" :precision="4" style="width: 100%" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="不含税金额" width="140">
        <template #default="{ record }">
          {{ amountExcludingTax(record).toFixed(2) }}
        </template>
      </a-table-column>
      <a-table-column title="税额" width="120">
        <template #default="{ record }">
          {{ taxAmount(record).toFixed(2) }}
        </template>
      </a-table-column>
      <a-table-column title="含税金额" width="140">
        <template #default="{ record }">
          {{ amountIncludingTax(record).toFixed(2) }}
        </template>
      </a-table-column>
      <a-table-column title="备注" width="200">
        <template #default="{ record, index }">
          <a-input v-model:value="record.remark" @change="emitChange(index)" />
        </template>
      </a-table-column>
      <a-table-column title="操作" width="100">
        <template #default="{ index }">
          <a-button type="link" danger @click="removeRow(index)">删除</a-button>
        </template>
      </a-table-column>
    </a-table>

    <div class="totals">
      <span>合计（不含税）：{{ sumExcludingTax.toFixed(2) }}</span>
      <span>税额合计：{{ sumTax.toFixed(2) }}</span>
      <span>含税合计：{{ sumIncludingTax.toFixed(2) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
// 发票行编辑器
import { computed, reactive, watch } from 'vue';

export interface InvoiceItemEdit {
  _rowKey: string;
  productCode: string;
  productName: string;
  specification?: string;
  unit?: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  sortOrder: number;
  remark?: string;
}

const props = defineProps<{ modelValue: InvoiceItemEdit[] }>();
const emits = defineEmits<{ (e: 'update:modelValue', v: InvoiceItemEdit[]): void }>();

const rows = reactive<InvoiceItemEdit[]>(props.modelValue?.map(clone) ?? []);

watch(
  () => props.modelValue,
  (v) => {
    if (!v) return;
    rows.splice(0, rows.length, ...v.map(clone));
  }
);

function clone(x: InvoiceItemEdit): InvoiceItemEdit {
  return { ...x };
}

function nextKey() {
  return Math.random().toString(36).slice(2);
}

function addRow() {
  const idx = rows.length;
  rows.push({
    _rowKey: nextKey(),
    productCode: '',
    productName: '',
    specification: '',
    unit: '',
    quantity: 0,
    unitPrice: 0,
    taxRate: 0.13,
    sortOrder: idx + 1,
    remark: '',
  });
  emits('update:modelValue', rows.map(clone));
}

function removeRow(index: number) {
  rows.splice(index, 1);
  // 重排 sortOrder
  rows.forEach((r, i) => (r.sortOrder = i + 1));
  emits('update:modelValue', rows.map(clone));
}

function emitChange(_index: number) {
  emits('update:modelValue', rows.map(clone));
}

function amountExcludingTax(r: InvoiceItemEdit) {
  const qty = Number(r.quantity || 0);
  const price = Number(r.unitPrice || 0);
  return qty * price;
}

function taxAmount(r: InvoiceItemEdit) {
  const rate = Number(r.taxRate || 0);
  return amountExcludingTax(r) * rate;
}

function amountIncludingTax(r: InvoiceItemEdit) {
  return amountExcludingTax(r) + taxAmount(r);
}

const sumExcludingTax = computed(() => rows.reduce((acc, r) => acc + amountExcludingTax(r), 0));
const sumTax = computed(() => rows.reduce((acc, r) => acc + taxAmount(r), 0));
const sumIncludingTax = computed(() => rows.reduce((acc, r) => acc + amountIncludingTax(r), 0));
</script>

<style scoped>
.totals { display: flex; gap: 16px; justify-content: flex-end; margin-top: 10px; font-weight: 500; }
</style>

