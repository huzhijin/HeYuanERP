<template>
  <!-- 共享/转移通用弹窗：根据 mode 展示不同表单并调用对应服务 -->
  <a-modal :open="open" :title="title" :confirm-loading="loading" @ok="onOk" @cancel="onCancel" destroy-on-close>
    <a-form :model="model" layout="vertical">
      <template v-if="mode === 'share'">
        <a-form-item label="目标用户" name="targetUserId" required>
          <a-input v-model:value="(model as any).targetUserId" placeholder="请输入用户" />
        </a-form-item>
        <a-form-item label="权限" name="permission" required>
          <a-radio-group v-model:value="(model as any).permission">
            <a-radio value="read">只读</a-radio>
            <a-radio value="edit">读写</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="过期时间" name="expireAt">
          <a-date-picker v-model:value="(model as any).expireAt" show-time value-format="YYYY-MM-DDTHH:mm:ss[Z]" style="width: 100%" />
        </a-form-item>
      </template>

      <template v-else>
        <a-form-item label="新负责人" name="newOwnerId" required>
          <a-input v-model:value="(model as any).newOwnerId" placeholder="请输入用户" />
        </a-form-item>
      </template>
    </a-form>
  </a-modal>
</template>

<script setup lang="ts">
// 复用的共享/转移弹窗组件
import { computed, ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import { shareAccount, transferAccount } from '@/services/accounts';

interface Props {
  open: boolean;
  mode: 'share' | 'transfer';
  accountId: string;
}
const props = defineProps<Props>();
const emit = defineEmits<{ (e: 'update:open', v: boolean): void; (e: 'done'): void }>();

const title = computed(() => (props.mode === 'share' ? '共享客户' : '转移客户'));

const loading = ref(false);
const model = ref<any>({ permission: 'read' });

watch(
  () => props.open,
  (v) => {
    if (v) {
      model.value = props.mode === 'share' ? { permission: 'read' } : { newOwnerId: '' };
    }
  }
);

function onCancel() {
  emit('update:open', false);
}

async function onOk() {
  if (!props.accountId) return;
  loading.value = true;
  try {
    if (props.mode === 'share') {
      const { targetUserId, permission, expireAt } = model.value;
      if (!targetUserId) {
        message.error('请填写目标用户');
        return;
      }
      await shareAccount(props.accountId, { targetUserId, permission, expireAt });
      message.success('共享成功');
    } else {
      const { newOwnerId } = model.value;
      if (!newOwnerId) {
        message.error('请填写新负责人');
        return;
      }
      await transferAccount(props.accountId, { newOwnerId });
      message.success('转移成功');
    }
    emit('update:open', false);
    emit('done');
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>
