<template>
  <!-- 报表快照历史/查询（中文注释） -->
  <a-card title="报表快照查询" :bordered="false">
    <a-alert
      type="info"
      show-icon
      message="当前后端仅提供按编号或按 类型+参数哈希 查询单条快照，暂不支持分页列表。"
      style="margin-bottom: 12px"
    />

    <a-tabs>
      <a-tab-pane key="byId" tab="按编号查询">
        <a-space>
          <a-input v-model:value="byId" placeholder="输入快照编号" style="width: 360px" />
          <a-button type="primary" @click="onGetById" :loading="loading">查询</a-button>
        </a-space>
      </a-tab-pane>
      <a-tab-pane key="byHash" tab="按类型+参数哈希查询">
        <a-space>
          <a-select v-model:value="type" placeholder="选择报表类型" style="width: 160px">
            <a-select-option value="sales-stat">销售统计</a-select-option>
            <a-select-option value="invoice-stat">发票统计</a-select-option>
            <a-select-option value="po-query">采购订单查询</a-select-option>
            <a-select-option value="inventory">库存报表</a-select-option>
          </a-select>
          <a-input v-model:value="paramHash" placeholder="参数哈希(HEX)" style="width: 240px" />
          <a-button type="primary" @click="onGetByHash" :loading="loading">查询</a-button>
        </a-space>
      </a-tab-pane>
    </a-tabs>

    <a-card v-if="snapshot" size="small" style="margin-top: 12px">
      <p><b>编号：</b>{{ snapshot.id }}</p>
      <p><b>类型：</b>{{ snapshot.type }}</p>
      <p><b>创建时间：</b>{{ snapshot.createdAtUtc }}</p>
      <p><b>文件：</b><a :href="snapshot.fileUri" target="_blank">{{ snapshot.fileUri }}</a></p>
      <p><b>参数 JSON：</b></p>
      <a-textarea :value="snapshot.parametersJson" :rows="6" readonly />
    </a-card>
  </a-card>
</template>

<script setup lang="ts">
// 说明：快照查询逻辑
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import http from '../../api/http'
import { getSnapshotById } from '../../api/modules/reports'

const loading = ref(false)
const byId = ref('')
const type = ref<'sales-stat' | 'invoice-stat' | 'po-query' | 'inventory' | ''>('')
const paramHash = ref('')
const snapshot = ref<any | null>(null)

async function onGetById() {
  if (!byId.value) return
  loading.value = true
  try {
    snapshot.value = await getSnapshotById(byId.value)
  } catch (e: any) {
    snapshot.value = null
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

async function onGetByHash() {
  if (!type.value || !paramHash.value) return
  loading.value = true
  try {
    // 直接调用 Minimal API：/api/reports/snapshots?type=xxx&paramHash=yyy
    snapshot.value = await http.get('/api/reports/snapshots', { params: { type: type.value, paramHash: paramHash.value } }).then((r) => (r as any))
  } catch (e: any) {
    snapshot.value = null
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
</style>
