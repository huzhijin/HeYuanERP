<template>
  <a-select
    :value="modelValue"
    :options="options"
    :loading="loading"
    show-search
    allow-clear
    :filter-option="false"
    :placeholder="placeholder || '选择订单（按单号搜索）'"
    style="width: 100%"
    @search="onSearch"
    @change="onChange"
    @dropdown-visible-change="onOpen"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { listOrders } from '@/services/orders'
import type { OrderListItem } from '@/types/order'
import type { Pagination } from '@/types/account'

interface Props {
  modelValue?: string
  placeholder?: string
}
const props = defineProps<Props>()
const emit = defineEmits<{ (e: 'update:modelValue', v?: string): void; (e: 'select', row: OrderListItem | undefined): void }>()

const loading = ref(false)
const options = ref<{ label: string; value: string; row: OrderListItem }[]>([])
let lastQuery = ''

async function search(keyword: string) {
  loading.value = true
  try {
    const resp: Pagination<OrderListItem> = await listOrders({ keyword, page: 1, size: 20 })
    options.value = (resp.items || []).map((o) => ({
      label: `${o.orderNo}（${String(o.orderDate).slice(0, 10)}，${o.status}）`,
      value: o.id,
      row: o,
    }))
  } finally { loading.value = false }
}

function onSearch(v: string) {
  lastQuery = v
  search(v)
}

function onOpen(open: boolean) {
  if (open && options.value.length === 0) search(lastQuery)
}

function onChange(v?: string) {
  emit('update:modelValue', v)
  const row = options.value.find((x) => x.value === v)?.row
  emit('select', row)
}
</script>

<style scoped></style>

