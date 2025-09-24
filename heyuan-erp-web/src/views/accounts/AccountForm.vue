<template>
  <!-- 客户表单页：新建/编辑；附带附件预览（上传在后续批次对接） -->
  <div class="page account-form">
    <a-card :title="pageTitle" :loading="loading" :bordered="false">
      <a-form :model="form" :rules="rules" layout="vertical" @submit.prevent>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="客户编码" name="code">
              <a-input v-model:value="form.code" :disabled="isEdit" placeholder="唯一编码，如 CUST001" @blur="onCodeBlur" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="客户名称" name="name">
              <a-input v-model:value="form.name" placeholder="客户显示名称" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="负责人" name="ownerId">
              <a-input v-model:value="form.ownerId" allow-clear placeholder="可留空" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="税号" name="taxNo">
              <a-input v-model:value="form.taxNo" allow-clear />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="状态" name="active">
              <a-switch v-model:checked="form.active" checked-children="启用" un-checked-children="停用" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="备注" name="description">
              <a-textarea v-model:value="form.description" :rows="3" allow-clear />
            </a-form-item>
          </a-col>
        </a-row>

        <a-space>
          <a-button type="primary" :loading="saving" @click="onSubmit">保 存</a-button>
          <a-button @click="$router.back()">返 回</a-button>
        </a-space>
      </a-form>
    </a-card>

    <!-- 附件区域：仅预览；当为编辑模式时展示 -->
    <a-card v-if="isEdit" title="附件" style="margin-top: 12px" :bordered="false">
      <AttachmentPreview :items="attachments" />
      <a-alert type="info" show-icon style="margin-top: 8px" :message="'上传接口将在后续批次提供，此处暂仅展示已有关联附件'" />
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 页面逻辑：加载详情（编辑模式），唯一性预校验，保存提交，附件列表预览
import { onMounted, ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { getAccount, createAccount, updateAccount, existsCode, listAccountAttachments } from '@/services/accounts';
import type { AccountCreateInput, AccountUpdateInput, AccountDetail, AttachmentItem } from '@/types/account';
import AttachmentPreview from '@/components/attachments/AttachmentPreview.vue';

const route = useRoute();
const router = useRouter();

const id = computed(() => route.params.id as string | undefined);
const isEdit = computed(() => !!id.value);
const pageTitle = computed(() => (isEdit.value ? '编辑客户' : '新建客户'));

const loading = ref(false);
const saving = ref(false);

// 表单模型（根据新建/编辑复用）
const form = ref<AccountCreateInput | AccountUpdateInput>({
  code: '',
  name: '',
  ownerId: '',
  taxNo: '',
  active: true,
  description: '',
});

const rules = {
  code: [{ required: true, message: '请输入客户编码' }],
  name: [{ required: true, message: '请输入客户名称' }],
};

async function load() {
  if (!isEdit.value) return;
  loading.value = true;
  try {
    const detail: AccountDetail = await getAccount(id.value!);
    form.value = {
      code: detail.code,
      name: detail.name,
      ownerId: detail.ownerId,
      taxNo: detail.taxNo,
      active: detail.active,
      description: detail.description,
    };
    attachments.value = await listAccountAttachments(detail.id);
  } finally {
    loading.value = false;
  }
}

async function onCodeBlur() {
  if (!form.value.code || !form.value.code.trim()) return;
  const exist = await existsCode(form.value.code, isEdit.value ? id.value : undefined);
  if (exist.exists) {
    message.warning('该客户编码已存在，请更换');
  }
}

async function onSubmit() {
  // 简化校验：仅校验必填
  if (!form.value.code || !form.value.name) {
    message.error('请完善必填项');
    return;
  }
  saving.value = true;
  try {
    if (isEdit.value) {
      // 编辑：允许变更编码
      await updateAccount(id.value!, form.value as AccountUpdateInput);
      message.success('保存成功');
    } else {
      const created = await createAccount(form.value as AccountCreateInput);
      message.success('创建成功');
      router.replace(`/accounts/${created.id}`);
    }
  } finally {
    saving.value = false;
  }
}

// 附件列表
const attachments = ref<AttachmentItem[]>([]);

onMounted(load);
</script>

<style scoped>
.account-form {
  max-width: 1200px;
  margin: 0 auto;
}
</style>
