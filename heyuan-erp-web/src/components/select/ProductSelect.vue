<template>
  <a-select
    :value="modelValue"
    :options="options"
    :loading="loading"
    show-search
    allow-clear
    :filter-option="false"
    :placeholder="placeholder || '选择产品（按编码/名称搜索）'"
    style="width: 100%"
    @search="onSearch"
    @change="onChange"
    @dropdown-visible-change="onOpen"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { searchProducts, type ProductItem } from '@/api/modules/productPrice'

interface Props { modelValue?: string; placeholder?: string }
const props = defineProps<Props>()
const emit = defineEmits<{ (e: 'update:modelValue', v?: string): void; (e: 'select', row?: ProductItem): void }>()

const loading = ref(false)
const options = ref<{ label: string; value: string; row: ProductItem }[]>([])
let last = ''

async function query(q: string){
  loading.value = true
  try {
    const list = await searchProducts(q || '')
    options.value = (list||[]).map(p => ({ label: `${p.code} ${p.name}`, value: p.id, row: p }))
  } finally { loading.value=false }
}

function onSearch(v: string){ last = v; query(v) }
function onOpen(open: boolean){ if(open && options.value.length===0) query(last) }
function onChange(v?: string){ emit('update:modelValue', v); const row = options.value.find(o=>o.value===v)?.row; emit('select', row) }
</script>

<style scoped></style>

