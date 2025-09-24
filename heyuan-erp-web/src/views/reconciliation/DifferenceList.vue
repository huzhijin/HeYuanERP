<template>
  <a-card title="对账差异" :bordered="false">
    <template #extra>
      <a-button @click="load">刷新</a-button>
    </template>
    <a-table row-key="id" :data-source="rows" :loading="loading" :pagination="false">
      <a-table-column title="差异号" data-index="differenceNo" />
      <a-table-column title="类型" data-index="type" width="140" />
      <a-table-column title="数量差" data-index="differenceQuantity" width="120" />
      <a-table-column title="金额差" data-index="differenceAmount" width="120" />
      <a-table-column title="状态" data-index="status" width="140" />
      <a-table-column title="创建时间" data-index="createdAt" width="180" />
      <a-table-column title="操作" width="220">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openResolve(record)">处理</a-button>
          </a-space>
        </template>
      </a-table-column>
    </a-table>

    <a-modal v-model:open="resolveOpen" title="处理差异" @ok="submitResolve" :confirmLoading="resolving">
      <a-form layout="vertical">
        <a-form-item label="处理意见" required>
          <a-textarea v-model:value="resolution" :rows="4" />
        </a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { listUnresolvedDifferences, resolveDifference } from '@/api/modules/reconciliation'

const rows = ref<any[]>([])
const loading = ref(false)
const resolveOpen = ref(false)
const resolving = ref(false)
const currentId = ref<string>('')
const resolution = ref('按公司政策，生成红字发票/或补开发票进行调整。')

async function load(){
  loading.value = true
  try{ rows.value = await listUnresolvedDifferences() }catch(e:any){ message.error(e?.message||'加载失败') } finally { loading.value=false }
}

function openResolve(rec:any){ currentId.value = rec.id; resolution.value=''; resolveOpen.value=true }

async function submitResolve(){
  if(!currentId.value || !resolution.value){ message.warning('请输入处理意见'); return }
  resolving.value = true
  try{
    await resolveDifference(currentId.value, resolution.value, 'webuser')
    message.success('已处理')
    resolveOpen.value = false
    await load()
  }catch(e:any){ message.error(e?.message||'处理失败') } finally { resolving.value=false }
}

onMounted(load)
</script>

<style scoped></style>

