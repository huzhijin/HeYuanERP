<template>
  <!-- 库位管理：查询 + 分页表格 + 新建/编辑/删除 -->
  <div class="page locations">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-form layout="inline">
        <a-form-item label="仓库">
          <a-input v-model:value="query.warehouseId" allow-clear placeholder="所属仓库" style="width: 220px" />
        </a-form-item>
        <a-form-item label="关键词">
          <a-input v-model:value="query.keyword" allow-clear placeholder="编码/名称模糊" style="width: 220px" @pressEnter="onSearch" />
        </a-form-item>
        <a-form-item label="启用">
          <a-select v-model:value="query.active" allow-clear style="width: 140px" placeholder="全部">
            <a-select-option :value="true">是</a-select-option>
            <a-select-option :value="false">否</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
          <a-button type="dashed" @click="openCreate">新建库位</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="warehouseId" data-index="warehouseId" title="仓库" width="200" />
        <a-table-column key="code" data-index="code" title="编码" width="180" />
        <a-table-column key="name" data-index="name" title="名称" width="220" />
        <a-table-column key="active" title="启用" width="100">
          <template #default="{ record }"><a-tag :color="record.active ? 'green' : 'default'">{{ record.active ? '是' : '否' }}</a-tag></template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="200">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="openEdit(record.id)">编辑</a-button>
              <a-button type="link" danger @click="remove(record.id)">删除</a-button>
            </a-space>
          </template>
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

    <!-- 新建/编辑弹窗 -->
    <a-modal v-model:open="modalOpen" :title="currentId ? '编辑库位' : '新建库位'" :confirm-loading="saving" @ok="onSubmit">
      <a-form layout="vertical">
        <a-form-item label="仓库" required>
          <a-input v-model:value="form.warehouseId" placeholder="所属仓库" />
        </a-form-item>
        <a-form-item label="编码" required>
          <a-input v-model:value="form.code" placeholder="唯一编码（同仓库内）" />
        </a-form-item>
        <a-form-item label="名称" required>
          <a-input v-model:value="form.name" placeholder="库位名称" />
        </a-form-item>
        <a-form-item label="启用">
          <a-switch v-model:checked="form.active" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
// 库位管理页面：对接库位服务，实现查询与 CRUD
import { reactive, ref, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';
import { listLocations, getLocation, createLocation, updateLocation, deleteLocation } from '@/services/locations';
import type { LocationListQuery, LocationListItem, LocationCreateInput, LocationUpdateInput } from '@/types/inventory';

const query = reactive<LocationListQuery>({});
const loading = ref(false);
const rows = ref<LocationListItem[]>([]);
const total = ref(0);
const page = ref(1);
const size = ref(20);

const modalOpen = ref(false);
const saving = ref(false);
const currentId = ref<string | null>(null);
const form = reactive<LocationCreateInput>({ warehouseId: '', code: '', name: '', active: true });

async function load() {
  loading.value = true;
  try {
    const params = { ...query, page: page.value, size: size.value } as LocationListQuery;
    const resp = await listLocations(params);
    rows.value = resp.items;
    total.value = resp.total;
  } finally {
    loading.value = false;
  }
}

function onSearch() { page.value = 1; load(); }
function onReset() { Object.assign(query, { warehouseId: '', keyword: '', active: undefined }); page.value = 1; size.value = 20; load(); }
function onPageChange(p: number, s?: number) { page.value = p; if (s) size.value = s; load(); }
function onSizeChange(p: number, s: number) { page.value = p; size.value = s; load(); }

function openCreate() {
  currentId.value = null;
  Object.assign(form, { warehouseId: '', code: '', name: '', active: true });
  modalOpen.value = true;
}

async function openEdit(id: string) {
  currentId.value = id;
  const detail = await getLocation(id);
  Object.assign(form, { warehouseId: detail.warehouseId, code: detail.code, name: detail.name, active: detail.active });
  modalOpen.value = true;
}

async function onSubmit() {
  if (!form.warehouseId || !form.code || !form.name) { message.error('请填写仓库、编码与名称'); return; }
  saving.value = true;
  try {
    const body: LocationUpdateInput = { warehouseId: form.warehouseId.trim(), code: form.code.trim(), name: form.name.trim(), active: !!form.active };
    if (currentId.value) {
      await updateLocation(currentId.value, body);
      message.success('保存成功');
    } else {
      await createLocation(body);
      message.success('创建成功');
    }
    modalOpen.value = false;
    await load();
  } finally {
    saving.value = false;
  }
}

function remove(id: string) {
  Modal.confirm({
    title: '删除确认',
    content: '确定要删除该库位吗？',
    async onOk() {
      await deleteLocation(id);
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
