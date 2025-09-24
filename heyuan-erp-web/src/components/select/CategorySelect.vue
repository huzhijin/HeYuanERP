<template>
  <a-select
    :value="modelValue"
    :options="options"
    allow-clear
    show-search
    :filter-option="(input, option) => (option?.label as string).toLowerCase().includes(input.toLowerCase())"
    :placeholder="placeholder || '选择分类'"
    style="width: 100%"
    @change="onChange"
  />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { listCategories, type ProductCategory } from '@/api/modules/productPrice'

interface Props { modelValue?: number | null; placeholder?: string }
const props = defineProps<Props>()
const emit = defineEmits<{ (e: 'update:modelValue', v?: number | null): void; (e: 'select', row?: ProductCategory): void }>()

const options = ref<{ label: string; value: number; row: ProductCategory }[]>([])

async function load(){
  const list = await listCategories()
  options.value = (list||[]).map(c => ({ label: `${c.name} (${c.code})`, value: c.id, row: c }))
}
function onChange(v?: number | null){ emit('update:modelValue', v ?? null); const row = options.value.find(o=>o.value===v)?.row; emit('select', row) }
onMounted(load)
</script>

<style scoped></style>

