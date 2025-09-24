<template>
  <!-- 发票状态标签：Draft/Pending/Issued/Canceled 显示不同颜色与文本 -->
  <a-tag :color="color">{{ text }}</a-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue';

type InvoiceStatus = 'Draft' | 'Pending' | 'Issued' | 'Canceled';

const props = defineProps<{ status?: InvoiceStatus }>();

const text = computed(() => {
  switch (props.status) {
    case 'Issued': return '已开具';
    case 'Pending': return '待开具';
    case 'Canceled': return '已作废';
    case 'Draft':
    default: return '草稿';
  }
});

const color = computed(() => {
  switch (props.status) {
    case 'Issued': return 'green';
    case 'Pending': return 'blue';
    case 'Canceled': return 'red';
    case 'Draft':
    default: return 'default';
  }
});
</script>

<style scoped>
</style>

