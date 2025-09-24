<template>
  <!-- 客户列表页：搜索 + 分页表格 + 操作（编辑/共享/转移/附件/拜访） -->
  <div class="page accounts-list">
    <!-- 搜索工具条 -->
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="关键字">
          <a-input v-model:value="query.keyword" allow-clear placeholder="编码/名称" style="width: 220px" @pressEnter="load" />
        </a-form-item>
        <a-form-item label="状态">
          <a-select v-model:value="status" style="width: 120px" allow-clear placeholder="全部">
            <a-select-option :value="true">启用</a-select-option>
            <a-select-option :value="false">停用</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
          <a-button type="dashed" @click="toCreate">新建客户</a-button>
        </a-space>
      </a-form>
    </a-card>

    <!-- 列表 -->
    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="code" data-index="code" title="编码" width="140" />
        <a-table-column key="name" data-index="name" title="名称" />
        <a-table-column key="ownerId" data-index="ownerId" title="负责人" width="160" />
        <a-table-column key="active" title="状态" width="100">
          <template #default="{ record }">
            <a-tag :color="record.active ? 'green' : 'red'">{{ record.active ? '启用' : '停用' }}</a-tag>
          </template>
        </a-table-column>
        <a-table-column key="lastEventDate" data-index="lastEventDate" title="最近业务日期" width="180">
          <template #default="{ text }">{{ text ? dayjs(text).format('YYYY-MM-DD HH:mm') : '-' }}</template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="360">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="toEdit(record.id)">编辑</a-button>
              <a-button type="link" @click="openShare(record)">共享</a-button>
              <a-button type="link" @click="openTransfer(record)">转移</a-button>
              <a-button type="link" @click="openAttachments(record)">附件</a-button>
              <a-button type="link" @click="openVisits(record)">拜访</a-button>
            </a-space>
          </template>
        </a-table-column>
      </a-table>

      <!-- 分页 -->
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

    <!-- 共享/转移弹窗复用组件 -->
    <ShareTransferModal v-model:open="shareOpen" mode="share" :account-id="current?.id || ''" @done="load" />
    <ShareTransferModal v-model:open="transferOpen" mode="transfer" :account-id="current?.id || ''" @done="load" />

    <!-- 附件抽屉：仅预览列表，上传在详情页提供 -->
    <a-drawer v-model:open="attOpen" :title="`附件 - ${current?.name || ''}`" placement="right" width="520">
      <AttachmentPreview :items="attachments" />
    </a-drawer>

    <!-- 拜访抽屉：显示分页列表（简版） -->
    <a-drawer v-model:open="visitOpen" :title="`拜访记录 - ${current?.name || ''}`" placement="right" width="720">
      <a-table :data-source="visits.items" :pagination="false" :loading="visitLoading" size="small" row-key="id">
        <a-table-column key="visitDate" title="时间" width="180">
          <template #default="{ record }">{{ dayjs(record.visitDate).format('YYYY-MM-DD HH:mm') }}</template>
        </a-table-column>
        <a-table-column key="subject" data-index="subject" title="主题" />
        <a-table-column key="visitorId" data-index="visitorId" title="拜访人" width="140" />
        <a-table-column key="location" data-index="location" title="地点" width="160" />
      </a-table>
      <div class="pager">
        <a-pagination
          :current="visitPage"
          :page-size="visitSize"
          :total="visits.total"
          @change="(p,s)=>{visitPage=p;visitSize=s;loadVisits();}"
          show-size-changer
          :page-size-options="['10','20','50']"
        />
      </div>
    </a-drawer>
  </div>
  
</template>

<script setup lang="ts">
// 页面逻辑：对接 accounts 服务层，处理搜索/分页与操作弹窗
import { reactive, ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import { listAccounts, listAccountAttachments, listAccountVisits } from '@/services/accounts';
import type { AccountListQuery, AccountListItem, AttachmentItem, Pagination, AccountVisit } from '@/types/account';
import ShareTransferModal from './ShareTransferModal.vue';
import AttachmentPreview from '@/components/attachments/AttachmentPreview.vue';

const router = useRouter();

const query = reactive<AccountListQuery>({ page: 1, size: 20, keyword: '', active: undefined });
const status = ref<boolean | undefined>(undefined);
const loading = ref(false);
const rows = ref<AccountListItem[]>([]);
const total = ref(0);

async function load() {
  loading.value = true;
  try {
    query.active = status.value;
    const resp = await listAccounts(query);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() {
  query.page = 1; // 重置到第一页
  load();
}

function onReset() {
  query.page = 1;
  query.size = 20;
  query.keyword = '';
  status.value = undefined;
  load();
}

function onPageChange(p: number, s?: number) {
  query.page = p;
  if (s) query.size = s;
  load();
}

function onSizeChange(p: number, s: number) {
  query.page = p;
  query.size = s;
  load();
}

function toCreate() {
  router.push('/accounts/new');
}

function toEdit(id: string) {
  router.push(`/accounts/${id}`);
}

// 共享/转移
const shareOpen = ref(false);
const transferOpen = ref(false);
const current = ref<AccountListItem | null>(null);
function openShare(row: AccountListItem) {
  current.value = row;
  shareOpen.value = true;
}
function openTransfer(row: AccountListItem) {
  current.value = row;
  transferOpen.value = true;
}

// 附件预览
const attOpen = ref(false);
const attachments = ref<AttachmentItem[]>([]);
async function openAttachments(row: AccountListItem) {
  current.value = row;
  attOpen.value = true;
  attachments.value = await listAccountAttachments(row.id);
}

// 拜访记录
const visitOpen = ref(false);
const visitLoading = ref(false);
const visits = ref<Pagination<AccountVisit>>({ items: [], page: 1, size: 20, total: 0 });
const visitPage = ref(1);
const visitSize = ref(20);
async function openVisits(row: AccountListItem) {
  current.value = row;
  visitOpen.value = true;
  visitPage.value = 1;
  await loadVisits();
}
async function loadVisits() {
  if (!current.value) return;
  visitLoading.value = true;
  try {
    visits.value = await listAccountVisits(current.value.id, visitPage.value, visitSize.value);
  } finally {
    visitLoading.value = false;
  }
}

onMounted(load);
</script>

<style scoped>
.pager {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
}
</style>
