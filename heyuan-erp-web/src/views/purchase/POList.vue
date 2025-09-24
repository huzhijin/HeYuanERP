<template>
  <!-- 采购单列表页：搜索 + 分页表格 + 操作（编辑/确认/收货/删除） -->
  <div class="page po-list">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="采购单号">
          <a-input v-model:value="query.keyword" allow-clear placeholder="模糊查询" style="width: 200px" @pressEnter="onSearch" />
        </a-form-item>
        <a-form-item label="供应商">
          <a-input v-model:value="query.vendorId" allow-clear placeholder="供应商" style="width: 200px" />
        </a-form-item>
        <a-form-item label="日期">
          <a-range-picker v-model:value="range" value-format="YYYY-MM-DD" />
        </a-form-item>
        <a-form-item label="状态">
          <a-select v-model:value="status" style="width: 140px" allow-clear placeholder="全部">
            <a-select-option value="draft">草稿</a-select-option>
            <a-select-option value="confirmed">已确认</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
          <a-button type="dashed" @click="toCreate">新建采购单</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="poNo" data-index="poNo" title="采购单号" width="160" />
        <a-table-column key="vendorId" data-index="vendorId" title="供应商" width="160" />
        <a-table-column key="poDate" data-index="poDate" title="下单日期" width="140">
          <template #default="{ text }">{{ dayjs(text).format('YYYY-MM-DD') }}</template>
        </a-table-column>
        <a-table-column key="status" data-index="status" title="状态" width="110">
          <template #default="{ text }">
            <a-tag :color="statusColor(text)">{{ statusText(text) }}</a-tag>
          </template>
        </a-table-column>
        <a-table-column key="totalAmount" data-index="totalAmount" title="总金额" width="140">
          <template #default="{ text }">{{ Number(text).toFixed(2) }}</template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="400">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="toEdit(record.id)">编辑</a-button>
              <a-button type="link" v-if="record.status==='draft'" @click="confirm(record.id)">确认</a-button>
              <a-button type="link" v-if="record.status==='confirmed'" @click="toReceive(record.id)">收货</a-button>
              <a-button type="link" danger v-if="record.status==='draft'" @click="remove(record.id)">删除</a-button>
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
// 列表页面：对接采购服务，提供确认/收货入口
import { reactive, ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import { listPOs, confirmPO, deletePO } from '@/services/pos';
import type { POListQuery, POListItem } from '@/types/purchase';
import { message, Modal } from 'ant-design-vue';

const router = useRouter();

const query = reactive<POListQuery>({ page: 1, size: 20 });
const range = ref<[string, string] | null>(null);
const status = ref<POListQuery['status']>();
const loading = ref(false);
const rows = ref<POListItem[]>([]);
const total = ref(0);

function statusText(s?: string) {
  return s === 'confirmed' ? '已确认' : '草稿';
}
function statusColor(s?: string) {
  return s === 'confirmed' ? 'green' : 'default';
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
    const resp = await listPOs(query);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() { query.page = 1; load(); }
function onReset() { query.page = 1; query.size = 20; query.keyword = ''; query.vendorId = ''; range.value = null; status.value = undefined; load(); }
function onPageChange(p: number, s?: number) { query.page = p; if (s) query.size = s; load(); }
function onSizeChange(p: number, s: number) { query.page = p; query.size = s; load(); }

function toCreate() { router.push('/purchase/po/new'); }
function toEdit(id: string) { router.push(`/purchase/po/${id}`); }
function toReceive(id: string) { router.push(`/purchase/po/${id}/receive`); }

async function confirm(id: string) {
  await confirmPO(id);
  message.success('确认成功');
  load();
}

async function remove(id: string) {
  Modal.confirm({
    title: '删除确认',
    content: '仅草稿可删除，是否继续？',
    async onOk() {
      await deletePO(id);
      message.success('已删除');
      await load();
    },
  });
}

onMounted(load);
</script>

<style scoped>
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>
