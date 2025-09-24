<template>
  <!-- 订单列表页：搜索 + 分页表格 + 操作（编辑/确认/反审/打印） -->
  <div class="page order-list">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="订单号">
          <a-input v-model:value="query.keyword" allow-clear placeholder="模糊查询" style="width: 200px" @pressEnter="onSearch" />
        </a-form-item>
        <a-form-item label="日期">
          <a-range-picker v-model:value="range" value-format="YYYY-MM-DD" />
        </a-form-item>
        <a-form-item label="状态">
          <a-select v-model:value="status" style="width: 140px" allow-clear placeholder="全部">
            <a-select-option value="draft">草稿</a-select-option>
            <a-select-option value="confirmed">已确认</a-select-option>
            <a-select-option value="reversed">已反审</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
          <a-button type="dashed" @click="toCreate">新建订单</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="orderNo" data-index="orderNo" title="订单号" width="160" />
        <a-table-column key="accountId" data-index="accountId" title="客户" width="160" />
        <a-table-column key="orderDate" data-index="orderDate" title="下单日期" width="140">
          <template #default="{ text }">{{ dayjs(text).format('YYYY-MM-DD') }}</template>
        </a-table-column>
        <a-table-column key="status" data-index="status" title="状态" width="110">
          <template #default="{ text }">
            <a-tag :color="statusColor(text)">{{ statusText(text) }}</a-tag>
          </template>
        </a-table-column>
        <a-table-column key="totalWithTax" data-index="totalWithTax" title="含税金额" width="140">
          <template #default="{ text }">{{ Number(text).toFixed(2) }}</template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="360">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="toEdit(record.id)">编辑</a-button>
              <a-button type="link" v-if="record.status==='draft'" @click="confirm(record.id)">确认</a-button>
              <a-button type="link" danger v-if="record.status==='confirmed'" @click="reverse(record.id)">反审</a-button>
              <a-button type="link" @click="printPreview(record.id)">打印</a-button>
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
// 列表页面：对接订单服务，提供确认/反审与打印入口
import { reactive, ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import { listOrders, confirmOrder, reverseOrder } from '@/services/orders';
import type { OrderListQuery, OrderListItem } from '@/types/order';
import { message, Modal } from 'ant-design-vue';

const router = useRouter();

const query = reactive<OrderListQuery>({ page: 1, size: 20 });
const range = ref<[string, string] | null>(null);
const status = ref<OrderListQuery['status']>();
const loading = ref(false);
const rows = ref<OrderListItem[]>([]);
const total = ref(0);

function statusText(s?: string) {
  return s === 'confirmed' ? '已确认' : s === 'reversed' ? '已反审' : '草稿';
}
function statusColor(s?: string) {
  return s === 'confirmed' ? 'green' : s === 'reversed' ? 'orange' : 'default';
}

async function load() {
  loading.value = true;
  try {
    query.status = status.value;
    if (range.value) {
      query.from = range.value[0];
      query.to = range.value[1];
    } else {
      query.from = undefined;
      query.to = undefined;
    }
    const resp = await listOrders(query);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() { query.page = 1; load(); }
function onReset() { query.page = 1; query.size = 20; query.keyword = ''; range.value = null; status.value = undefined; load(); }
function onPageChange(p: number, s?: number) { query.page = p; if (s) query.size = s; load(); }
function onSizeChange(p: number, s: number) { query.page = p; query.size = s; load(); }

function toCreate() { router.push('/orders/new'); }
function toEdit(id: string) { router.push(`/orders/${id}`); }
function printPreview(id: string) { router.push(`/orders/${id}/print`); }

async function confirm(id: string) {
  await confirmOrder(id);
  message.success('确认成功');
  load();
}

async function reverse(id: string) {
  Modal.confirm({
    title: '反审确认',
    content: '确定要反审该订单吗？',
    async onOk() {
      await reverseOrder(id, {});
      message.success('已反审');
      await load();
    },
  });
}

onMounted(load);
</script>

<style scoped>
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>

