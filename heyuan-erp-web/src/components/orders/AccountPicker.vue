<template>
  <!-- 客户选择器：远程搜索 Accounts 列表，绑定返回 Id -->
  <a-select
    v-model:value="inner"
    show-search
    allow-clear
    :filter-option="false"
    :options="options"
    placeholder="搜索客户：输入名称或编码"
    @search="onSearch"
    :loading="loading"
    style="width: 100%"
  />
</template>

<script setup lang="ts">
// 客户选择器：调用 /api/Accounts 分页接口进行远程搜索
import { ref, computed } from 'vue';
import { listAccounts } from '@/services/accounts';

const props = defineProps<{ modelValue?: string }>();
const emit = defineEmits<{ (e: 'update:modelValue', v?: string): void }>();

const inner = computed({
  get: () => props.modelValue,
  set: (v?: string) => emit('update:modelValue', v),
});

const loading = ref(false);
const options = ref<{ label: string; value: string }[]>([]);

async function onSearch(kw: string) {
  loading.value = true;
  try {
    const data = await listAccounts({ page: 1, size: 20, keyword: kw });
    options.value = data.items.map(it => ({ label: `${it.name}（${it.code}）`, value: it.id }));
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>

