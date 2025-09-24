<template>
  <!-- 打印预览页：展示打印模型 JSON，并提供 PDF 预览/下载按钮 -->
  <div class="page print-preview">
    <a-card :title="'订单打印预览 - ' + (id || '')" :bordered="false">
      <a-space style="margin-bottom: 12px">
        <a-button type="primary" @click="openPdf">打开 PDF</a-button>
        <a-button @click="$router.back()">返 回</a-button>
      </a-space>
      <a-alert type="info" show-icon message="以下为打印模型 JSON（用于模板调试）" style="margin-bottom: 8px" />
      <a-typography-paragraph>
        <pre class="json-box">{{ modelText }}</pre>
      </a-typography-paragraph>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 打印预览：调用 /api/print/order/{id}/model 获取 JSON；点击按钮获取 PDF（二进制）在新窗口打开
import { onMounted, ref, computed } from 'vue';
import { useRoute } from 'vue-router';
import { getOrderPrintModel, getOrderPdf } from '@/services/print';

const route = useRoute();
const id = computed(() => route.params.id as string | undefined);
const model = ref<any>({});
const modelText = computed(() => JSON.stringify(model.value, null, 2));

async function load() {
  if (!id.value) return;
  model.value = await getOrderPrintModel(id.value);
}

async function openPdf() {
  if (!id.value) return;
  const data = await getOrderPdf(id.value);
  const blob = new Blob([data], { type: 'application/pdf' });
  const url = URL.createObjectURL(blob);
  window.open(url, '_blank');
  setTimeout(() => URL.revokeObjectURL(url), 60_000);
}

onMounted(load);
</script>

<style scoped>
.json-box { background: #0b1020; color: #c9d1d9; padding: 12px; border-radius: 6px; white-space: pre-wrap; }
</style>

