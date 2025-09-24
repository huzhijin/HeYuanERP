<template>
  <!-- 库存事务：按时间/产品/仓库/库位/类型 查询明细流水 -->
  <div class="page inv-txns">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="日期">
          <a-range-picker v-model:value="range" value-format="YYYY-MM-DD" />
        </a-form-item>
        <a-form-item label="类型">
          <a-select v-model:value="txnCode" allow-clear style="width: 180px" placeholder="全部">
            <a-select-option value="IN">其他入库</a-select-option>
            <a-select-option value="OUT">其他出库</a-select-option>
            <a-select-option value="DELIVERY">发货</a-select-option>
            <a-select-option value="RETURN">退货</a-select-option>
            <a-select-option value="PORECEIVE">采购收货</a-select-option>
            <a-select-option value="ADJ">调整</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="产品">
          <ProductSelect v-model="(query as any).productId" placeholder="按编码/名称选择产品" />
        </a-form-item>
        <a-form-item label="仓库">
          <a-input v-model:value="query.whse" allow-clear placeholder="仓库编码" style="width: 160px" />
        </a-form-item>
        <a-form-item label="库位">
          <a-input v-model:value="query.loc" allow-clear placeholder="库位" style="width: 160px" />
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="txnDate" title="交易日期" width="160">
          <template #default="{ record }">{{ dayjs(record.txnDate).format('YYYY-MM-DD HH:mm') }}</template>
        </a-table-column>
        <a-table-column key="txnCode" title="类型" width="140">
          <template #default="{ record }">
            <a-tag :color="codeColor(record.txnCode)">{{ codeText(record.txnCode) }}</a-tag>
          </template>
        </a-table-column>
        <a-table-column key="product" title="产品" width="240">
          <template #default="{ record }">
            <div>
              <div>{{ record.productCode || record.productId }}</div>
              <div class="sub">{{ record.productName }}</div>
            </div>
          </template>
        </a-table-column>
        <a-table-column key="qty" data-index="qty" title="数量" width="120">
          <template #default="{ text }">{{ Number(text || 0).toFixed(4) }}</template>
        </a-table-column>
        <a-table-column key="whse" data-index="whse" title="仓库" width="120" />
        <a-table-column key="loc" data-index="loc" title="库位" width="120" />
        <a-table-column key="ref" title="来源" width="160">
          <template #default="{ record }">{{ record.refType }}</template>
        </a-table-column>
        <a-table-column key="createdAt" title="创建时间" width="160">
          <template #default="{ record }">{{ dayjs(record.createdAt).format('YYYY-MM-DD HH:mm') }}</template>
        </a-table-column>
      </a-table>

      <div class="pager">
        <a-pagination
          :current="page"
          :page-size="size"
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
// 库存事务页面：查询并分页展示库存流水
import { reactive, ref, onMounted } from 'vue';
import dayjs from 'dayjs';
import { listInventoryTransactions } from '@/services/inventory';
import type { InventoryTxnQuery, InventoryTxnItem } from '@/types/inventory';
import ProductSelect from '@/components/select/ProductSelect.vue'

const query = reactive<InventoryTxnQuery>({});
const loading = ref(false);
const rows = ref<InventoryTxnItem[]>([]);
const total = ref(0);
const page = ref(1);
const size = ref(20);
const range = ref<[string, string] | null>(null);
const txnCode = ref<string | undefined>();

function codeText(code?: string) {
  switch (code) {
    case 'IN': return '其他入库';
    case 'OUT': return '其他出库';
    case 'DELIVERY': return '发货';
    case 'RETURN': return '退货';
    case 'PORECEIVE': return '采购收货';
    case 'ADJ': return '调整';
    default: return code || '-';
  }
}
function codeColor(code?: string) {
  switch (code) {
    case 'IN': return 'green';
    case 'OUT': return 'red';
    case 'DELIVERY': return 'blue';
    case 'RETURN': return 'orange';
    case 'PORECEIVE': return 'purple';
    case 'ADJ': return 'default';
    default: return 'default';
  }
}

async function load() {
  loading.value = true;
  try {
    const params: InventoryTxnQuery = { ...query, page: page.value, size: size.value };
    if (range.value) {
      params.from = range.value[0];
      params.to = range.value[1];
    } else {
      params.from = undefined;
      params.to = undefined;
    }
    params.txnCode = txnCode.value || undefined;
    const resp = await listInventoryTransactions(params);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() { page.value = 1; load(); }
function onReset() {
  Object.assign(query, { productId: '', whse: '', loc: '' });
  range.value = null; txnCode.value = undefined; page.value = 1; size.value = 20; load();
}
function onPageChange(p: number, s?: number) { page.value = p; if (s) size.value = s; load(); }
function onSizeChange(p: number, s: number) { page.value = p; size.value = s; load(); }

onMounted(load);
</script>

<style scoped>
.sub { color: #8c8c8c; font-size: 12px; }
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>
