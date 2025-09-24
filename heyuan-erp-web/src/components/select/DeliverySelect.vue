<template>
  <a-select
    :value="modelValue"
    :options="options"
    :loading="loading"
    show-search
    allow-clear
    :filter-option="false"
    :placeholder="placeholder || '选择送货单（按单号搜索）'"
    style="width: 100%"
    @search="onSearch"
    @change="onChange"
    @dropdown-visible-change="onOpen"
  />
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { listDeliveries } from '@/services/deliveries'
import type { DeliveryListItem } from '@/types/order'
import type { Pagination } from '@/types/account'

interface Props {
  modelValue?: string
  placeholder?: string
  orderNo?: string // 可选：限定来源订单号
}
const props = defineProps<Props>()
const emit = defineEmits<{ (e: 'update:modelValue', v?: string): void; (e: 'select', row: DeliveryListItem | undefined): void }>()

const loading = ref(false)
const options = ref<{ label: string; value: string; row: DeliveryListItem }[]>([])
let lastQuery = ''

async function search(keyword: string) {
  loading.value = true
  try {
    const resp: Pagination<DeliveryListItem> = await listDeliveries({ deliveryNo: keyword, orderNo: props.orderNo, page: 1, size: 20 })
    options.value = (resp.items || []).map((d) => ({
      label: `${d.deliveryNo}（${String(d.deliveryDate).slice(0, 10)}，${d.status}）`,
      value: d.id,
      row: d,
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

watch(() => props.orderNo, () => { options.value = []; if (lastQuery) search(lastQuery) })
</script>

<style scoped></style>

