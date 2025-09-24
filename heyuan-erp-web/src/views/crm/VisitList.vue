<template>
  <a-card title="客户拜访" :bordered="false">
    <template #extra>
      <a-space>
        <AccountSelect v-model="(query as any).accountId" placeholder="选择客户" style="width: 240px" />
        <a-button type="primary" @click="load">查询</a-button>
        <a-button @click="openCreate">新增拜访</a-button>
      </a-space>
    </template>
    <a-table row-key="id" :data-source="rows" :loading="loading" :pagination="false">
      <a-table-column title="时间" data-index="visitDate" width="180" />
      <!-- 隐藏客户/机会的内部ID列，避免在前台展示 -->
      <a-table-column title="类型" data-index="type" width="120" />
      <a-table-column title="目的" data-index="purpose" />
      <a-table-column title="备注" data-index="notes" />
      <a-table-column title="操作" width="200">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确定删除？" @confirm="() => doDelete(record.id)"><a-button danger size="small">删除</a-button></a-popconfirm>
          </a-space>
        </template>
      </a-table-column>
    </a-table>

    <a-modal v-model:open="createOpen" title="新增拜访" @ok="submitCreate" :confirmLoading="creating">
      <a-form layout="vertical">
        <a-form-item label="客户" required>
          <AccountSelect v-model="(form as any).accountId" />
        </a-form-item>
        <a-form-item label="机会">
          <a-input v-model:value="form.salesOpportunityId" placeholder="可选" />
        </a-form-item>
        <a-form-item label="拜访日期" required>
          <a-date-picker v-model:value="form.visitDate" valueFormat="YYYY-MM-DD" style="width: 100%" />
        </a-form-item>
        <a-form-item label="类型">
          <a-input v-model:value="form.type" placeholder="上门/电话/视频" />
        </a-form-item>
        <a-form-item label="目的">
          <a-input v-model:value="form.purpose" />
        </a-form-item>
        <a-form-item label="备注">
          <a-textarea v-model:value="form.notes" :rows="3" />
        </a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import type { CustomerVisit } from '@/types/crm'
import { getVisitsByAccount, getVisitsByUser, createVisit, updateVisit, deleteVisit } from '@/api/modules/crm'
import AccountSelect from '@/components/select/AccountSelect.vue'

const rows = ref<CustomerVisit[]>([])
const loading = ref(false)
const query = ref<{ accountId?: number; userId?: number }>({})
const editOpen = ref(false)
const editing = ref(false)
const editForm = ref<any>({})

async function load() {
  loading.value = true
  try {
    if (query.value.accountId) rows.value = await getVisitsByAccount(query.value.accountId)
    else if (query.value.userId) rows.value = await getVisitsByUser(query.value.userId)
    else rows.value = []
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

const createOpen = ref(false)
const creating = ref(false)
const form = ref<any>({ accountId: undefined, salesOpportunityId: undefined, visitDate: undefined, type: '', purpose: '', notes: '' })

function openCreate() { createOpen.value = true }

async function submitCreate() {
  if (!form.value.accountId || !form.value.visitDate) {
    message.warning('请填写客户与拜访日期')
    return
  }
  creating.value = true
  try {
    await createVisit(form.value)
    message.success('创建成功')
    createOpen.value = false
    await load()
  } catch (e: any) {
    message.error(e?.message || '创建失败')
  } finally {
    creating.value = false
  }
}
function openEdit(rec: CustomerVisit) { editForm.value = { ...rec }; editOpen.value = true }
async function doDelete(id: number){ try{ await deleteVisit(id); message.success('已删除'); await load() }catch(e:any){ message.error(e?.message||'删除失败') } }
async function submitEdit(){
  if(!editForm.value.id){ editOpen.value=false; return }
  editing.value=true
  try{ await updateVisit(editForm.value.id as number, editForm.value); message.success('保存成功'); editOpen.value=false; await load() }catch(e:any){ message.error(e?.message||'保存失败') } finally { editing.value=false }
}
</script>

<style scoped></style>
