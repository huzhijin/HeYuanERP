<template>
  <a-card title="销售机会" :bordered="false">
    <template #extra>
      <a-space>
        <a-select v-model:value="filter.type" style="width: 120px">
          <a-select-option value="status">按状态</a-select-option>
          <a-select-option value="user">按用户</a-select-option>
        </a-select>
        <a-input v-if="filter.type==='status'" v-model:value="filter.status" placeholder="状态（如 Proposal）" style="width: 160px" />
        <a-input-number v-else v-model:value="filter.userId" :min="1" placeholder="用户" style="width: 160px" />
        <a-button type="primary" @click="load">查询</a-button>
        <a-button @click="openCreate">新建机会</a-button>
      </a-space>
    </template>

    <a-table row-key="id" :data-source="rows" :loading="loading" :pagination="false">
      <!-- 隐藏内部ID列，避免在前台展示 -->
      <a-table-column title="机会" data-index="opportunityName" />
      <!-- 隐藏客户ID列，避免在前台展示 -->
      <a-table-column title="阶段" data-index="stage" width="120" />
      <a-table-column title="概率%" data-index="probability" width="100" />
      <a-table-column title="预估金额" data-index="estimatedValue" width="140" />
      <a-table-column title="负责人" data-index="assignedToUserName" width="140" />
      <a-table-column title="创建时间" data-index="createdAt" width="180" />
      <a-table-column title="操作" width="200">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确定删除？" @confirm="() => doDelete(record.id)"><a-button danger size="small">删除</a-button></a-popconfirm>
          </a-space>
        </template>
      </a-table-column>
    </a-table>

    <a-modal v-model:open="createOpen" title="新建机会" @ok="submitCreate" :confirmLoading="creating">
      <a-form layout="vertical">
        <a-form-item label="机会名称" required>
          <a-input v-model:value="form.opportunityName" placeholder="如：XX项目机会" />
        </a-form-item>
        <a-form-item label="客户" required>
          <AccountSelect v-model="(form as any).accountId" />
        </a-form-item>
        <a-form-item label="预估金额" required>
          <a-input-number v-model:value="form.estimatedValue" :min="0" :step="100" style="width: 100%" />
        </a-form-item>
        <a-form-item label="成功概率%" required>
          <a-input-number v-model:value="form.probability" :min="0" :max="100" style="width: 100%" />
        </a-form-item>
        <a-form-item label="阶段">
          <a-input v-model:value="form.stage" placeholder="Proposal/Nego..." />
        </a-form-item>
        <a-form-item label="负责人">
          <a-input-number v-model:value="form.assignedToUserId" :min="1" style="width: 100%" />
        </a-form-item>
      </a-form>
    </a-modal>

    <a-modal v-model:open="editOpen" title="编辑机会" @ok="submitEdit" :confirmLoading="editing">
      <a-form layout="vertical">
        <a-form-item label="机会名称" required><a-input v-model:value="editForm.opportunityName"/></a-form-item>
        <a-form-item label="阶段"><a-input v-model:value="editForm.stage"/></a-form-item>
        <a-form-item label="预估金额"><a-input-number v-model:value="editForm.estimatedValue" :min="0" :step="100" style="width:100%"/></a-form-item>
        <a-form-item label="成功概率%"><a-input-number v-model:value="editForm.probability" :min="0" :max="100" style="width:100%"/></a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import type { SalesOpportunity } from '@/types/crm'
import { getOpportunitiesByStatus, getOpportunitiesByUser, createOpportunity, updateOpportunity, deleteOpportunity } from '@/api/modules/crm'
import AccountSelect from '@/components/select/AccountSelect.vue'
import { http } from '@/lib/http'

const rows = ref<SalesOpportunity[]>([])
const loading = ref(false)
const filter = ref<{ type: 'status'|'user'; status: string; userId?: number }>({ type: 'status', status: 'Proposal' })

async function load() {
  loading.value = true
  try {
    rows.value = filter.value.type === 'status'
      ? await getOpportunitiesByStatus(filter.value.status || 'Proposal')
      : await getOpportunitiesByUser(filter.value.userId || 1)
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

const createOpen = ref(false)
const creating = ref(false)
const form = ref<Partial<SalesOpportunity>>({ opportunityName: '', accountId: undefined, estimatedValue: 0, probability: 10, stage: 'Proposal' })
const editOpen = ref(false)
const editing = ref(false)
const editForm = ref<Partial<SalesOpportunity>>({})

function openCreate() { createOpen.value = true }

function openEdit(rec: SalesOpportunity) {
  editForm.value = { ...rec }
  editOpen.value = true
}

async function submitCreate() {
  if (!form.value.opportunityName || !form.value.accountId) {
    message.warning('请填写机会名称与客户')
    return
  }
  creating.value = true
  try {
    await createOpportunity(form.value)
    message.success('创建成功')
    createOpen.value = false
    await load()
  } catch (e: any) {
    message.error(e?.message || '创建失败')
  } finally {
    creating.value = false
  }
}

async function submitEdit() {
  if (!editForm.value.id) { editOpen.value=false; return }
  editing.value = true
  try {
    await updateOpportunity(editForm.value.id as number, editForm.value)
    message.success('保存成功')
    editOpen.value = false
    await load()
  } catch (e: any) {
    message.error(e?.message || '保存失败')
  } finally { editing.value = false }
}

async function doDelete(id: number) {
  try { await deleteOpportunity(id); message.success('已删除'); await load() } catch (e:any) { message.error(e?.message||'删除失败') }
}

onMounted(load)
</script>

<style scoped></style>
