<template>
  <!-- 附件上传组件：使用 Ant Design Vue a-upload + 自定义上传逻辑（http 封装） -->
  <a-upload
    :multiple="multiple"
    :show-upload-list="true"
    :disabled="disabled || !action"
    :custom-request="onCustomRequest"
    :accept="accept"
  >
    <a-button :disabled="disabled || !action">选择文件</a-button>
  </a-upload>
  <div v-if="!action" class="tip">后端上传接口尚未提供，当前仅支持预览已上传文件。</div>
</template>

<script setup lang="ts">
// 说明：
// - 优先通过 props.action 指定上传地址，以 multipart/form-data 方式提交字段：file、refType、refId；
// - 可选传入 extraFields 合并到表单；上传成功期望返回附件对象（含 id/fileName/storageUri 等）。
// - 上传完成后触发 done 事件，传出后端返回的附件对象。
import { http } from '@/utils/request';
import type { UploadRequestOption } from 'ant-design-vue/es/vc-upload/interface';

interface Props {
  action?: string; // 上传地址，如 /api/Accounts/{id}/attachments
  refType?: string;
  refId?: string;
  multiple?: boolean;
  disabled?: boolean;
  accept?: string;
  extraFields?: Record<string, any>;
}

const props = withDefaults(defineProps<Props>(), {
  multiple: false,
  disabled: false,
  accept: '',
});

const emit = defineEmits<{ (e: 'done', file: any): void }>();

async function onCustomRequest(o: UploadRequestOption) {
  const { file, onProgress, onError, onSuccess } = o;
  if (!props.action) {
    onError?.(new Error('未配置上传地址'));
    return;
  }
  try {
    const fd = new FormData();
    fd.append('file', file as File);
    if (props.refType) fd.append('refType', props.refType);
    if (props.refId) fd.append('refId', props.refId);
    if (props.extraFields) {
      Object.entries(props.extraFields).forEach(([k, v]) => fd.append(k, String(v ?? '')));
    }
    const data = await http.post<any>(props.action, fd, { headers: { 'Content-Type': 'multipart/form-data' }, onUploadProgress: (ev) => {
      const percent = ev.total ? Math.round((ev.loaded / ev.total) * 100) : 0;
      onProgress?.({ percent }, file as any);
    }});
    onSuccess?.(data, file as any);
    emit('done', data);
  } catch (e: any) {
    onError?.(e);
  }
}
</script>

<style scoped>
.tip {
  color: #999;
  font-size: 12px;
  margin-top: 8px;
}
</style>

