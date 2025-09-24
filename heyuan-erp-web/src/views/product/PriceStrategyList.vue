<template>
  <a-card title="价格策略" :bordered="false">
    <template #extra>
      <a-button @click="load">刷新</a-button>
    </template>
    <a-table row-key="id" :data-source="rows" :loading="loading" :pagination="false">
      <!-- 隐藏策略ID列，避免前台展示内部标识 -->
      <a-table-column title="名称" data-index="name" />
      <a-table-column title="类型" data-index="type" width="140" />
      <a-table-column title="生效日" data-index="effectiveDate" width="140" />
      <a-table-column title="失效日" data-index="expiryDate" width="140" />
      <a-table-column title="启用" :customRender="r => (r.record.isActive ? '是' : '否')" width="100" />
      <a-table-column title="操作" width="200">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确定删除？" @confirm="() => doDelete(record.id)"><a-button danger size="small">删除</a-button></a-popconfirm>
          </a-space>
        </template>
      </a-table-column>
    </a-table>
    <a-divider />
    <a-button type="primary" @click="openCreate">新建策略</a-button>

    <a-modal v-model:open="modalOpen" :title="isEdit?'编辑策略':'新建策略'" @ok="submit" :confirmLoading="saving">
      <a-form layout="vertical">
        <a-form-item label="名称" required><a-input v-model:value="form.name"/></a-form-item>
        <a-form-item label="类型" required><a-input v-model:value="form.type"/></a-form-item>
        <a-form-item label="生效日"><a-date-picker v-model:value="form.effectiveDate" valueFormat="YYYY-MM-DD" style="width:100%"/></a-form-item>
        <a-form-item label="失效日"><a-date-picker v-model:value="form.expiryDate" valueFormat="YYYY-MM-DD" style="width:100%"/></a-form-item>
        <a-form-item label="启用"><a-switch v-model:checked="form.isActive"/></a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { listStrategies, createStrategy, updateStrategy, deleteStrategy, type PriceStrategy } from '@/api/modules/productPrice'

const rows = ref<PriceStrategy[]>([])
const loading = ref(false)
const modalOpen = ref(false)
const isEdit = ref(false)
const saving = ref(false)
const editingId = ref<number | null>(null)
const form = ref<any>({ name: '', type: 'Standard', effectiveDate: undefined, expiryDate: undefined, isActive: true })

async function load() {
  loading.value = true
  try {
    rows.value = await listStrategies()
  } catch (e: any) {
    message.error(e?.message || '加载失败')
  } finally {
    loading.value = false
  }
}

function openCreate() { isEdit.value=false; editingId.value=null; form.value={ name:'', type:'Standard', isActive:true }; modalOpen.value=true }
function openEdit(rec: PriceStrategy) { isEdit.value=true; editingId.value=rec.id; form.value={ ...rec }; modalOpen.value=true }
async function submit(){
  saving.value=true
  try{
    if(!form.value.name || !form.value.type){ message.warning('请填写名称与类型'); saving.value=false; return }
    if(isEdit.value && editingId.value!=null) await updateStrategy(editingId.value, form.value)
    else await createStrategy(form.value)
    message.success('保存成功'); modalOpen.value=false; await load()
  }catch(e:any){ message.error(e?.message||'保存失败') }finally{ saving.value=false }
}
async function doDelete(id:number){ try{ await deleteStrategy(id); message.success('已删除'); await load() }catch(e:any){ message.error(e?.message||'删除失败') } }

onMounted(load)
</script>

<style scoped></style>
