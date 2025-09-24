<template>
  <!-- 通用订单行编辑器：以表格编辑产品、数量、单价、折扣、税率、交期 -->
  <a-table :data-source="inner" :pagination="false" row-key="rowKey">
    <a-table-column title="#" :customRender="({index}) => index! + 1" width="60" />
    <a-table-column title="产品" width="280">
      <template #default="{ record }">
        <ProductSelect v-model="record.productId" placeholder="按编码/名称选择产品" />
      </template>
    </a-table-column>
    <a-table-column title="数量" width="140">
      <template #default="{ record }">
        <a-input-number v-model:value="record.qty" :min="0" :step="1" style="width: 100%" />
      </template>
    </a-table-column>
    <a-table-column title="单价" width="160">
      <template #default="{ record }">
        <a-input-number v-model:value="record.unitPrice" :min="0" :step="0.01" style="width: 100%" />
      </template>
    </a-table-column>
    <a-table-column title="折扣" width="140">
      <template #default="{ record }">
        <a-input-number v-model:value="record.discount" :min="0" :max="1" :step="0.01" style="width: 100%" />
      </template>
    </a-table-column>
    <a-table-column title="税率" width="140">
      <template #default="{ record }">
        <a-input-number v-model:value="record.taxRate" :min="0" :max="1" :step="0.01" style="width: 100%" />
      </template>
    </a-table-column>
    <a-table-column title="交期" width="200">
      <template #default="{ record }">
        <a-date-picker v-model:value="record.deliveryDate" value-format="YYYY-MM-DD" style="width: 100%" />
      </template>
    </a-table-column>
    <a-table-column title="操作" width="120">
      <template #default="{ record }">
        <a-button type="link" danger @click="remove(record)">删除</a-button>
      </template>
    </a-table-column>
  </a-table>
  <div style="margin-top:8px">
    <a-button type="dashed" @click="add">新增行</a-button>
  </div>
</template>

<script setup lang="ts">
// 通用订单行编辑组件：通过 v-model 绑定行数组，内部负责增删与字段编辑
import { computed } from 'vue';
import ProductSelect from '@/components/select/ProductSelect.vue'

type Line = {
  id?: string;
  productId: string;
  qty: number;
  unitPrice: number;
  discount: number;
  taxRate: number;
  deliveryDate?: string;
  _deleted?: boolean;
}

const props = defineProps<{ modelValue: Line[] }>();
const emit = defineEmits<{ (e: 'update:modelValue', v: Line[]): void }>();

const inner = computed({
  get: () => props.modelValue,
  set: (v: Line[]) => emit('update:modelValue', v),
});

function rowKey(row: any) { return row.id || `${row.productId}-${row.deliveryDate || ''}-${Math.random()}`; }

function add() {
  inner.value.push({ productId: '', qty: 1, unitPrice: 0, discount: 0, taxRate: 0.13 });
}
function remove(rec: Line) {
  const idx = inner.value.indexOf(rec);
  if (idx >= 0) inner.value.splice(idx, 1);
}
</script>

<style scoped>
</style>
