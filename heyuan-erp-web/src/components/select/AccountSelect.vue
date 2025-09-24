<template>
  <a-select
    :value="modelValue"
    :options="options"
    :loading="loading"
    show-search
    allow-clear
    :filter-option="false"
    :placeholder="placeholder || '选择客户（按编码/名称搜索）'"
    style="width: 100%"
    @search="onSearch"
    @change="onChange"
    @dropdown-visible-change="onOpen"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { listAccounts } from '@/services/accounts'
import type { AccountListItem, Pagination } from '@/types/account'

interface Props { modelValue?: string; placeholder?: string }
const props = defineProps<Props>()
const emit = defineEmits<{ (e: 'update:modelValue', v?: string): void; (e: 'select', row?: AccountListItem): void }>()

const loading = ref(false)
const options = ref<{ label: string; value: string; row: AccountListItem }[]>([])
let last = ''

async function query(q: string){
  loading.value = true
  try {
    const page: Pagination<AccountListItem> = await listAccounts({ keyword: q, page: 1, size: 20 })
    options.value = (page.items||[]).map(a => ({ label: `${a.code} ${a.name}`, value: a.id, row: a }))
  } finally { loading.value=false }
}

function onSearch(v: string){ last = v; query(v) }
function onOpen(open: boolean){ if(open && options.value.length===0) query(last) }
function onChange(v?: string){ emit('update:modelValue', v); const row = options.value.find(o=>o.value===v)?.row; emit('select', row) }
</script>

<style scoped></style>

