<template>
  <!-- 通用附件上传组件（不直传，读取为 File 列表） -->
  <div>
    <AUpload
      multiple
      :before-upload="beforeUpload"
      :file-list="fileList"
      :show-upload-list="true"
      @remove="onRemove"
      @change="onChange"
      :accept="accept"
      list-type="text"
    >
      <AButton>
        <template #icon>
          <UploadOutlined />
        </template>
        选择文件
      </AButton>
    </AUpload>
    <div class="tip">支持多文件，最大单文件大小建议 ≤ 20MB。</div>
  </div>
</template>

<script setup lang="ts">
// 中文说明：
// 该组件不进行实际上传，仅收集文件，外部通过 v-model:files 获取 File[]。

import { ref, watch, computed } from 'vue';
import { Upload as AUpload, Button as AButton } from 'ant-design-vue';
import { UploadOutlined } from '@ant-design/icons-vue';

const props = defineProps<{
  files?: File[];
  accept?: string;
}>();

const emit = defineEmits<{
  (e: 'update:files', v: File[]): void;
}>();

const accept = computed(() => props.accept || '');

type UploadFileItem = any; // 避免引入内部类型定义
const fileList = ref<UploadFileItem[]>([]);

watch(
  () => props.files,
  (val) => {
    // 将外部 files 同步到展示列表（仅初始化/外部变更时）
    const list = (val || []).map((f, idx) => ({
      uid: `${idx}-${f.name}-${f.size}`,
      name: f.name,
      status: 'done',
      size: (f as any).size,
      originFileObj: f,
    }));
    fileList.value = list as any;
  },
  { immediate: true }
);

function beforeUpload() {
  // 阻止 AntD 自动上传
  return false;
}

function onChange(info: any) {
  // 提取所有 originFileObj 作为 File[]
  const files: File[] = (info.fileList || [])
    .map((x: any) => x.originFileObj)
    .filter(Boolean);
  emit('update:files', files);
}

function onRemove(file: any) {
  const next = (fileList.value || []).filter((x: any) => x.uid !== file.uid);
  fileList.value = next as any;
  const files: File[] = next.map((x: any) => x.originFileObj).filter(Boolean);
  emit('update:files', files);
}

</script>

<style scoped>
.tip {
  font-size: 12px;
  color: #888;
  margin-top: 6px;
}
</style>

