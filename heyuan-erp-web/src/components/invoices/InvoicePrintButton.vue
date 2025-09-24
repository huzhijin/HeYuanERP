<template>
  <a-button :type="type" :loading="loading" @click="onPrint"><slot>打印</slot></a-button>
</template>

<script setup lang="ts">
// 打印发票按钮：调用后端 /api/invoices/{id}/print/pdf 打开新窗口
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { printInvoicePdf } from '@/services/invoices';

const props = withDefaults(defineProps<{ id: string; type?: 'link' | 'default' | 'primary' | 'dashed' | 'text' }>(), {
  type: 'link',
});

const loading = ref(false);

async function onPrint() {
  if (!props.id) return;
  loading.value = true;
  try {
    const bytes = await printInvoicePdf(props.id);
    const blob = new Blob([bytes], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
  } catch (err: any) {
    message.error(err?.message || '打印失败');
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>

