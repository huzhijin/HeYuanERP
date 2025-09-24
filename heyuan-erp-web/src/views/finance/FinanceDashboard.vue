<template>
  <a-row :gutter="16">
    <a-col :span="8">
      <a-card title="应收仪表板" :loading="loadingA">
        <pre style="white-space: pre-wrap">{{ JSON.stringify(ar, null, 2) }}</pre>
      </a-card>
    </a-col>
    <a-col :span="8">
      <a-card title="应付仪表板" :loading="loadingP">
        <pre style="white-space: pre-wrap">{{ JSON.stringify(ap, null, 2) }}</pre>
      </a-card>
    </a-col>
    <a-col :span="8">
      <a-card title="现金流概览" :loading="loadingC">
        <pre style="white-space: pre-wrap">{{ JSON.stringify(cf, null, 2) }}</pre>
      </a-card>
    </a-col>
  </a-row>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { getReceivableDashboard, getPayableDashboard, getCashFlowDashboard } from '@/api/modules/finance'

const ar = ref<any>(null)
const ap = ref<any>(null)
const cf = ref<any>(null)
const loadingA = ref(false)
const loadingP = ref(false)
const loadingC = ref(false)

async function load(){
  loadingA.value = loadingP.value = loadingC.value = true
  try{ ar.value = await getReceivableDashboard() }catch(e:any){ message.error(e?.message||'AR加载失败') } finally { loadingA.value=false }
  try{ ap.value = await getPayableDashboard() }catch(e:any){ message.error(e?.message||'AP加载失败') } finally { loadingP.value=false }
  try{ cf.value = await getCashFlowDashboard() }catch(e:any){ message.error(e?.message||'现金流加载失败') } finally { loadingC.value=false }
}

onMounted(load)
</script>

<style scoped></style>

