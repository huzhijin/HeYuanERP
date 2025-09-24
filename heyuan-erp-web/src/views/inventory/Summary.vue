<template>
  <!-- 库存汇总：按 产品/仓库/库位 查询与分页展示 -->
  <div class="page inv-summary">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
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
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="rowKey">
        <a-table-column key="product" title="产品" width="240">
          <template #default="{ record }">
            <div>
              <div>{{ record.productCode || record.productId }}</div>
              <div class="sub">{{ record.productName }}</div>
            </div>
          </template>
        </a-table-column>
        <a-table-column key="whse" data-index="whse" title="仓库" width="120" />
        <a-table-column key="loc" data-index="loc" title="库位" width="120" />
        <a-table-column key="onHand" data-index="onHand" title="现存量" width="120">
          <template #default="{ text }">{{ fmtQty(text) }}</template>
        </a-table-column>
        <a-table-column key="reserved" data-index="reserved" title="已预留" width="120">
          <template #default="{ text }">{{ fmtQty(text) }}</template>
        </a-table-column>
        <a-table-column key="available" data-index="available" title="可用量" width="120">
          <template #default="{ text }">{{ fmtQty(text) }}</template>
        </a-table-column>
        <a-table-column key="avgCost" data-index="avgCost" title="平均成本" width="140">
          <template #default="{ text }">{{ text == null ? '-' : Number(text).toFixed(4) }}</template>
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
// 库存汇总页面：对接库存服务，展示产品在各仓库/库位的现存/预留/可用
import { reactive, ref, onMounted } from 'vue';
import { listInventorySummary } from '@/services/inventory';
import type { InventorySummaryQuery, InventorySummaryItem } from '@/types/inventory';
import ProductSelect from '@/components/select/ProductSelect.vue'

const query = reactive<InventorySummaryQuery>({});
const loading = ref(false);
const rows = ref<InventorySummaryItem[]>([]);
const total = ref(0);
const page = ref(1);
const size = ref(20);

function rowKey(r: InventorySummaryItem) { return `${r.productId}|${r.whse}|${r.loc||''}`; }
function fmtQty(v: number) { return Number(v || 0).toFixed(4); }

async function load() {
  loading.value = true;
  try {
    const params = { ...query, page: page.value, size: size.value } as InventorySummaryQuery;
    const resp = await listInventorySummary(params);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() { page.value = 1; load(); }
function onReset() { Object.assign(query, { productId: '', whse: '', loc: '' }); page.value = 1; size.value = 20; load(); }
function onPageChange(p: number, s?: number) { page.value = p; if (s) size.value = s; load(); }
function onSizeChange(p: number, s: number) { page.value = p; size.value = s; load(); }

onMounted(load);
</script>

<style scoped>
.sub { color: #8c8c8c; font-size: 12px; }
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>
