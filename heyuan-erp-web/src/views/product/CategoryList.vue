<template>
  <a-card title="产品分类" :bordered="false">
    <template #extra>
      <a-button @click="load">刷新</a-button>
    </template>
    <a-table row-key="id" :data-source="rows" :loading="loading" :pagination="false">
      <!-- 隐藏内部ID列，不在前台展示 -->
      <a-table-column title="编码" data-index="code" width="160" />
      <a-table-column title="名称" data-index="name" />
      <a-table-column title="上级分类" data-index="parentId" width="160" />
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
    <a-button type="primary" @click="openCreate">新建分类</a-button>

    <a-modal v-model:open="modalOpen" :title="isEdit ? '编辑分类' : '新建分类'" @ok="submit" :confirmLoading="saving">
      <a-form layout="vertical">
        <a-form-item label="编码" required>
          <a-input v-model:value="form.code" />
        </a-form-item>
        <a-form-item label="名称" required>
          <a-input v-model:value="form.name" />
        </a-form-item>
        <a-form-item label="上级分类">
          <CategorySelect v-model="(form as any).parentId" />
        </a-form-item>
        <a-form-item label="启用">
          <a-switch v-model:checked="form.isActive" />
        </a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { listCategories, createCategory, updateCategory, deleteCategory, type ProductCategory } from '@/api/modules/productPrice'
import CategorySelect from '@/components/select/CategorySelect.vue'

const rows = ref<ProductCategory[]>([])
const loading = ref(false)
const modalOpen = ref(false)
const isEdit = ref(false)
const saving = ref(false)
const editingId = ref<number | null>(null)
const form = ref<any>({ code: '', name: '', parentId: null, isActive: true })

async function load() {
  loading.value = true
  try {
    rows.value = await listCategories()
  } catch (e: any) {
    message.error(e?.message || '加载失败')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  isEdit.value = false
  editingId.value = null
  form.value = { code: '', name: '', parentId: null, isActive: true }
  modalOpen.value = true
}

function openEdit(rec: ProductCategory) {
  isEdit.value = true
  editingId.value = rec.id
  form.value = { code: rec.code, name: rec.name, parentId: rec.parentId ?? null, isActive: rec.isActive }
  modalOpen.value = true
}

async function submit() {
  saving.value = true
  try {
    if (!form.value.code || !form.value.name) { message.warning('请填写编码与名称'); saving.value = false; return }
    if (isEdit.value && editingId.value != null) await updateCategory(editingId.value, form.value)
    else await createCategory(form.value)
    message.success('保存成功')
    modalOpen.value = false
    await load()
  } catch (e: any) {
    message.error(e?.message || '保存失败')
  } finally {
    saving.value = false
  }
}

async function doDelete(id: number) {
  try {
    await deleteCategory(id)
    message.success('已删除')
    await load()
  } catch (e: any) {
    message.error(e?.message || '删除失败')
  }
}

onMounted(load)
</script>

<style scoped></style>
