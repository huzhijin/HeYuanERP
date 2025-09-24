<template>
  <a-card title="产品列表" :bordered="false">
    <template #extra>
      <a-space>
        <a-input v-model:value="kw" placeholder="搜索代码/名称（前端过滤）" style="width: 220px" />
        <a-button @click="load">刷新</a-button>
        <a-button type="primary" @click="openCreate">新建产品</a-button>
      </a-space>
    </template>
    <a-table row-key="id" :data-source="filtered" :loading="loading" :pagination="false">
      <a-table-column title="编码" data-index="code" width="160" />
      <a-table-column title="名称" data-index="name" />
      <a-table-column title="分类" data-index="categoryId" width="120" />
      <a-table-column title="单位" data-index="unit" width="100" />
      <a-table-column title="安全库存" data-index="safetyStock" width="120" />
      <a-table-column title="操作" width="280">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确定删除？" @confirm="() => doDelete(record.id)"><a-button danger size="small">删除</a-button></a-popconfirm>
            <a-button size="small" @click="() => openStock(record)">调整库存</a-button>
          </a-space>
        </template>
      </a-table-column>
    </a-table>

    <a-modal v-model:open="modalOpen" :title="isEdit ? '编辑产品' : '新建产品'" @ok="submit" :confirmLoading="saving">
      <a-form layout="vertical">
        <a-form-item label="编码" required><a-input v-model:value="form.code" /></a-form-item>
        <a-form-item label="名称" required><a-input v-model:value="form.name" /></a-form-item>
        <a-form-item label="分类"><CategorySelect v-model="(form as any).categoryId" /></a-form-item>
        <a-form-item label="单位"><a-input v-model:value="form.unit" /></a-form-item>
        <a-form-item label="安全库存"><a-input-number v-model:value="form.safetyStock" :min="0" style="width:100%" /></a-form-item>
        <a-form-item label="启用"><a-switch v-model:checked="form.active" /></a-form-item>
      </a-form>
    </a-modal>

    <a-modal v-model:open="stockOpen" title="调整库存" @ok="submitStock" :confirmLoading="savingStock">
      <a-form layout="vertical">
        <a-form-item label="新库存" required><a-input-number v-model:value="newStock" :min="0" style="width:100%" /></a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { listProducts, createProduct, updateProduct, deleteProduct, updateStock, type ProductItem, type Pagination } from '@/api/modules/productPrice'
import CategorySelect from '@/components/select/CategorySelect.vue'

const rows = ref<ProductItem[]>([])
const loading = ref(false)
const kw = ref('')
const modalOpen = ref(false)
const isEdit = ref(false)
const saving = ref(false)
const form = ref<any>({ code: '', name: '', categoryId: null, unit: '', safetyStock: 0, active: true })
const editingId = ref<string | null>(null)

const stockOpen = ref(false)
const savingStock = ref(false)
const stockProductId = ref<string | null>(null)
const newStock = ref<number | null>(null)

const filtered = computed(() => {
  if (!kw.value) return rows.value
  const k = kw.value.toLowerCase()
  return rows.value.filter(x => (x.code || '').toLowerCase().includes(k) || (x.name || '').toLowerCase().includes(k))
})

async function load() {
  loading.value = true
  try {
    const page: Pagination<ProductItem> = await listProducts(1, 200)
    rows.value = page.items
  } catch (e: any) {
    message.error(e?.message || '加载失败')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  isEdit.value = false
  editingId.value = null
  form.value = { code: '', name: '', categoryId: null, unit: '', safetyStock: 0, active: true }
  modalOpen.value = true
}

function openEdit(rec: ProductItem) {
  isEdit.value = true
  editingId.value = rec.id
  form.value = { code: rec.code, name: rec.name, categoryId: rec.categoryId ?? null, unit: rec.unit, safetyStock: rec.safetyStock ?? 0, active: rec.active ?? true }
  modalOpen.value = true
}

async function submit() {
  saving.value = true
  try {
    if (!form.value.code || !form.value.name) { message.warning('请填写编码与名称'); saving.value=false; return }
    if (isEdit.value && editingId.value) await updateProduct(editingId.value, form.value)
    else await createProduct(form.value)
    message.success('保存成功')
    modalOpen.value = false
    await load()
  } catch (e: any) { message.error(e?.message || '保存失败') } finally { saving.value=false }
}

async function doDelete(id: string) {
  try { await deleteProduct(id); message.success('已删除'); await load() } catch (e:any) { message.error(e?.message||'删除失败') }
}

function openStock(rec: ProductItem) {
  stockProductId.value = rec.id
  newStock.value = null
  stockOpen.value = true
}

async function submitStock() {
  if (!stockProductId.value || newStock.value==null) { message.warning('请输入新库存'); return }
  savingStock.value = true
  try { await updateStock(stockProductId.value, newStock.value); message.success('已更新'); stockOpen.value=false; await load() } catch(e:any){ message.error(e?.message||'更新失败') } finally { savingStock.value=false }
}

onMounted(load)
</script>

<style scoped></style>
