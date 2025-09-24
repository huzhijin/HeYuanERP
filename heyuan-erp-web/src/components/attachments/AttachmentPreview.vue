<template>
  <!-- 附件预览列表：文件名/大小/上传者/时间；支持图片与 PDF 的内联预览 -->
  <a-list :data-source="items" :locale="{ emptyText: '暂无附件' }">
    <template #renderItem="{ item }">
      <a-list-item>
        <a-space style="width: 100%; justify-content: space-between; align-items: center;">
          <div class="meta">
            <div class="name">
              <a @click="open(item)">{{ item.fileName }}</a>
            </div>
            <div class="desc">
              <span>{{ humanSize(item.size) }}</span>
              <span v-if="item.uploadedBy"> · {{ item.uploadedBy }}</span>
              <span> · {{ dayjs(item.uploadedAt).format('YYYY-MM-DD HH:mm') }}</span>
            </div>
          </div>
          <div>
            <a-button type="link" @click="open(item)">预览/下载</a-button>
          </div>
        </a-space>
      </a-list-item>
    </template>
  </a-list>

  <!-- 预览弹窗（图片/PDF） -->
  <a-modal v-model:open="previewOpen" :title="previewTitle" width="80vw" :footer="null">
    <div v-if="isImage(previewUrl)" class="preview-box">
      <img :src="previewUrl" alt="preview" />
    </div>
    <div v-else-if="isPdf(previewUrl)" class="preview-box">
      <iframe :src="previewUrl" width="100%" height="80vh" frameborder="0"></iframe>
    </div>
    <div v-else>
      <a-result status="info" title="暂不支持内联预览的文件类型">
        <template #extra>
          <a-button type="primary" @click="download(previewUrl)">下载文件</a-button>
        </template>
      </a-result>
    </div>
  </a-modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import dayjs from 'dayjs';
import type { AttachmentItem } from '@/types/account';

defineProps<{ items: AttachmentItem[] }>();

const previewOpen = ref(false);
const previewUrl = ref('');
const previewTitle = ref('预览');

function humanSize(n: number): string {
  if (n < 1024) return `${n} B`;
  if (n < 1024 * 1024) return `${(n / 1024).toFixed(1)} KB`;
  if (n < 1024 * 1024 * 1024) return `${(n / (1024 * 1024)).toFixed(1)} MB`;
  return `${(n / (1024 * 1024 * 1024)).toFixed(1)} GB`;
}

function isImage(url: string): boolean {
  return /\.(png|jpg|jpeg|gif|bmp|webp)$/i.test(url);
}
function isPdf(url: string): boolean {
  return /\.pdf$/i.test(url);
}
function open(item: AttachmentItem) {
  const url = item.storageUri;
  previewUrl.value = url;
  previewTitle.value = item.fileName;
  if (isImage(url) || isPdf(url)) {
    previewOpen.value = true;
  } else {
    download(url);
  }
}
function download(url: string) {
  if (!url) return;
  window.open(url, '_blank');
}
</script>

<style scoped>
.meta .name { font-weight: 600; }
.meta .desc { color: #999; font-size: 12px; }
.preview-box { text-align: center; }
.preview-box img { max-width: 100%; max-height: 80vh; }
</style>
