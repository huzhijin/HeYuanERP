<template>
  <!-- 采购导入：上传 CSV/Excel（占位），预检并提交导入生成采购单 -->
  <div class="page po-import">
    <a-card :bordered="false" title="导入文件">
      <a-form layout="inline">
        <a-form-item label="供应商" required>
          <a-input v-model:value="vendorId" placeholder="输入供应商" style="width: 260px" />
        </a-form-item>
        <a-form-item>
          <a-upload-dragger :before-upload="beforeUpload" :file-list="fileList" :multiple="false" accept=".csv,.xlsx,.xls">
            <p class="ant-upload-drag-icon">📄</p>
            <p class="ant-upload-text">点击或拖拽文件到此区域</p>
            <p class="ant-upload-hint">当前支持 CSV 占位格式（标题：productId,qty,unitPrice）</p>
          </a-upload-dragger>
        </a-form-item>
        <a-space>
          <a-button type="primary" :disabled="!canPrecheck" @click="onPrecheck" :loading="loading">预检</a-button>
          <a-button type="dashed" :disabled="!canCommit" @click="onCommit" :loading="loading">提交导入</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false" title="预检结果" style="margin-top: 12px" v-if="precheck">
      <a-descriptions bordered size="small">
        <a-descriptions-item label="供应商">{{ precheck.vendorId }}</a-descriptions-item>
        <a-descriptions-item label="总记录">{{ precheck.totalRecords }}</a-descriptions-item>
        <a-descriptions-item label="有效">{{ precheck.validRecords }}</a-descriptions-item>
        <a-descriptions-item label="无效">{{ precheck.invalidRecords }}</a-descriptions-item>
      </a-descriptions>

      <a-row :gutter="12" style="margin-top: 12px">
        <a-col :span="12">
          <a-typography-title :level="5">预览（前10条）</a-typography-title>
          <a-table :data-source="precheck.preview" :pagination="false" row-key="productId">
            <a-table-column key="productId" data-index="productId" title="产品" />
            <a-table-column key="qty" data-index="qty" title="数量" />
            <a-table-column key="unitPrice" data-index="unitPrice" title="单价" />
          </a-table>
        </a-col>
        <a-col :span="12">
          <a-typography-title :level="5">错误列表</a-typography-title>
          <a-table :data-source="precheck.errors" :pagination="false" row-key="rowNo">
            <a-table-column key="rowNo" data-index="rowNo" title="行号" width="100" />
            <a-table-column key="field" data-index="field" title="字段" width="160" />
            <a-table-column key="message" data-index="message" title="说明" />
          </a-table>
        </a-col>
      </a-row>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 导入页面：先预检再提交，成功后跳转到新创建的采购单
import { ref, computed } from 'vue';
import type { UploadProps } from 'ant-design-vue';
import { message } from 'ant-design-vue';
import { precheckPOImport, commitPOImport } from '@/services/poImport';
import type { POImportPrecheckResult } from '@/types/purchase';
import { useRouter } from 'vue-router';

const router = useRouter();

const vendorId = ref('');
const file = ref<File | null>(null);
const fileList = ref<UploadProps['fileList']>([]);
const precheck = ref<POImportPrecheckResult | null>(null);
const loading = ref(false);

const canPrecheck = computed(() => !!vendorId.value && !!file.value);
const canCommit = computed(() => !!precheck.value && (precheck.value?.invalidRecords ?? 0) === 0);

const beforeUpload: UploadProps['beforeUpload'] = (f) => {
  file.value = f as File;
  fileList.value = [{ uid: f.uid as string, name: f.name, status: 'done', size: f.size, type: f.type } as any];
  return false; // 阻止自动上传
};

async function onPrecheck() {
  if (!canPrecheck.value) { message.error('请先选择供应商与文件'); return; }
  loading.value = true;
  try {
    precheck.value = await precheckPOImport(vendorId.value.trim(), file.value!);
    if ((precheck.value?.invalidRecords ?? 0) > 0) {
      message.warning('存在无效记录，请修正后再提交');
    } else {
      message.success('预检通过');
    }
  } finally {
    loading.value = false;
  }
}

async function onCommit() {
  if (!canCommit.value) { message.error('请先通过预检'); return; }
  loading.value = true;
  try {
    const receipt = await commitPOImport(vendorId.value.trim(), file.value!);
    message.success(`导入成功，采购单号：${receipt.poNo}`);
    router.push(`/purchase/po/${receipt.poId}`);
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.po-import { }
</style>
