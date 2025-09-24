<template>
  <!-- 收款创建页：表单 + 附件上传 -->
  <div class="page-payment-create">
    <AForm :model="form" :label-col="{ span: 4 }" :wrapper-col="{ span: 14 }">
      <AFormItem label="收款方式" required>
        <ASelect v-model:value="form.method" placeholder="请选择">
          <ASelectOption value="现金">现金</ASelectOption>
          <ASelectOption value="银行转账">银行转账</ASelectOption>
          <ASelectOption value="支付宝">支付宝</ASelectOption>
          <ASelectOption value="微信">微信</ASelectOption>
        </ASelect>
      </AFormItem>

      <AFormItem label="收款金额" required>
        <AInputNumber v-model:value="form.amount" :min="0.01" :precision="2" style="width: 240px" />
      </AFormItem>

      <AFormItem label="收款日期" required>
        <ADatePicker v-model:value="dateVal" format="YYYY-MM-DD" style="width: 240px" />
      </AFormItem>

      <AFormItem label="备注">
        <AInputTextArea v-model:value="form.remark" :rows="3" placeholder="可选" />
      </AFormItem>

      <AFormItem label="附件">
        <UploadAttachment v-model:files="attachments" />
      </AFormItem>

      <AFormItem :wrapper-col="{ span: 14, offset: 4 }">
        <AButton type="primary" :loading="creating" @click="onSubmit">提交</AButton>
        <AButton style="margin-left: 12px" @click="onReset">重置</AButton>
        <AButton style="margin-left: 12px" @click="goBack">返回列表</AButton>
      </AFormItem>
    </AForm>
  </div>
</template>

<script setup lang="ts">
// 中文说明：
// 收款创建表单页面，提交后调用后端 API 创建记录，并返回列表。

import { reactive, ref } from 'vue';
import { usePaymentStore } from '../../store/payments';
import { useRouter } from 'vue-router';
import dayjs, { Dayjs } from 'dayjs';
import { message } from 'ant-design-vue';
import UploadAttachment from '../../components/UploadAttachment.vue';
import {
  Form as AForm,
  FormItem as AFormItem,
  Input as AInput,
  InputNumber as AInputNumber,
  DatePicker as ADatePicker,
  Select as ASelect,
  Button as AButton,
} from 'ant-design-vue';

const ASelectOption = ASelect.Option;
const AInputTextArea = AInput.TextArea;

const router = useRouter();
const store = usePaymentStore();

const form = reactive({
  method: '现金',
  amount: 0 as number,
  paymentDate: '',
  remark: '' as string | undefined,
});

const attachments = ref<File[]>([]);
const dateVal = ref<Dayjs | null>(dayjs());

const creating = store.$state.creating;

function validate(): string | null {
  if (!form.method) return '请选择收款方式';
  if (!form.amount || form.amount <= 0) return '请输入有效的收款金额';
  if (!dateVal.value) return '请选择收款日期';
  return null;
}

async function onSubmit() {
  const err = validate();
  if (err) {
    message.warning(err);
    return;
  }
  try {
    const payload = {
      method: form.method,
      amount: Number(form.amount),
      paymentDate: dateVal.value!.format('YYYY-MM-DD'),
      remark: form.remark && form.remark.trim() ? form.remark.trim() : undefined,
      attachments: attachments.value,
    };
    await store.create(payload);
    router.push({ path: '/payments' });
  } catch (e: any) {
    // 错误提示由 store 处理
  }
}

function onReset() {
  form.method = '现金';
  form.amount = 0;
  form.remark = '';
  attachments.value = [];
  dateVal.value = dayjs();
}

function goBack() {
  router.push({ path: '/payments' });
}

</script>

<style scoped>
.page-payment-create {
  padding: 16px;
  max-width: 900px;
}
</style>

