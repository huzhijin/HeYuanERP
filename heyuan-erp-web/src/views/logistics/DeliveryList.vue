<template>
  <!-- 送货单列表：按单号/订单号/日期/状态筛选。前台不显示 Id，不使用 Id 筛选。 -->
  <div class="page delivery-list">
    <a-card :bordered="false" style="margin-bottom: 12px" title="送货单">
      <a-form layout="inline">
        <a-form-item label="送货单号">
          <a-input v-model:value="filters.deliveryNo" allow-clear placeholder="模糊查询" style="width: 200px" />
        </a-form-item>
        <a-form-item label="订单号">
          <a-input v-model:value="filters.orderNo" allow-clear placeholder="模糊查询" style="width: 200px" />
        </a-form-item>
        <a-form-item label="日期">
          <a-range-picker v-model:value="range" value-format="YYYY-MM-DD" />
        </a-form-item>
        <a-form-item label="状态">
          <a-select v-model:value="filters.status" style="width: 140px" allow-clear placeholder="全部">
            <a-select-option value="draft">草稿</a-select-option>
            <a-select-option value="confirmed">已确认</a-select-option>
            <a-select-option value="reversed">已冲销</a-select-option>
          </a-select>
        </a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch">查询</a-button>
          <a-button @click="onReset">重置</a-button>
          <a-button type="dashed" @click="toCreate">新建送货单</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false" title="列表">
      <a-table :data-source="rows" :loading="loading" :pagination="false" row-key="id">
        <a-table-column key="deliveryNo" data-index="deliveryNo" title="送货单号" width="200" />
        <a-table-column key="orderNo" data-index="orderNo" title="订单号" width="200" />
        <a-table-column key="deliveryDate" data-index="deliveryDate" title="送货日期" width="160">
          <template #default="{ text }">{{ formatDate(text) }}</template>
        </a-table-column>
        <a-table-column key="status" data-index="status" title="状态" width="120">
          <template #default="{ text }"><a-tag>{{ text }}</a-tag></template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="220">
          <template #default="{ record }">
            <a-space>
              <a-button type="link" @click="openDetail(record.id)">查看</a-button>
              <a-button type="link" @click="toEdit(record.id)">编辑</a-button>
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

    <a-modal v-model:open="detailVisible" :title="`送货单 ${detail?.deliveryNo || ''}`" width="720px" :footer="null">
      <a-descriptions bordered size="small" :column="2">
        <a-descriptions-item label="送货单号">{{ detail?.deliveryNo }}</a-descriptions-item>
        <a-descriptions-item label="送货日期">{{ formatDate(detail?.deliveryDate) }}</a-descriptions-item>
        <a-descriptions-item label="状态">{{ detail?.status }}</a-descriptions-item>
        <a-descriptions-item label="明细行数" :span="2">{{ detail?.lines?.length || 0 }}</a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { useRouter } from 'vue-router'
import { getDelivery, listDeliveries } from '@/services/deliveries'
import type { DeliveryDetail, DeliveryListItem, DeliveryListQuery } from '@/types/order'
import type { Pagination } from '@/types/account'

const router = useRouter()
const loading = ref(false)
const rows = ref<DeliveryListItem[]>([])
const page = ref(1)
const size = ref(20)
const total = ref(0)
const filters = reactive<DeliveryListQuery>({})
const range = ref<[string, string] | null>(null)
const detailVisible = ref(false)
const detail = ref<DeliveryDetail | null>(null)

function formatDate(s?: string){
  if (!s) return ''
  return String(s).slice(0,10)
}

async function load(){
  loading.value = true
  try {
    const params: DeliveryListQuery = { ...filters, page: page.value, size: size.value }
    if (range.value) { params.from = range.value[0]; params.to = range.value[1] } else { params.from = undefined; params.to = undefined }
    const resp: Pagination<DeliveryListItem> = await listDeliveries(params)
    rows.value = resp.items
    page.value = resp.page
    size.value = resp.size
    total.value = resp.total
  } finally { loading.value = false }
}

function onSearch(){ page.value = 1; load() }
function onReset(){ Object.assign(filters, { deliveryNo: '', orderNo: '', status: undefined }); range.value = null; page.value = 1; size.value = 20; load() }
function onPageChange(p:number, s?:number){ page.value = p; if (s) size.value = s; load() }
function onSizeChange(p:number, s:number){ page.value = p; size.value = s; load() }

function openDetail(id: string){ getDelivery(id).then(showDetail) }
function showDetail(d: DeliveryDetail){ detail.value = d; detailVisible.value = true }

function toCreate(){ router.push('/deliveries/new') }
function toEdit(id: string){ router.push(`/deliveries/${id}`) }

onMounted(load)
</script>

<style scoped>
.delivery-list { max-width: 1200px; margin: 0 auto; }
.pager { display: flex; justify-content: flex-end; margin-top: 12px; }
</style>
