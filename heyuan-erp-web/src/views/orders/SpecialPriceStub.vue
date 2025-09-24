<template>
  <!-- 特价/项目申请（留桩）页面：提交后返回申请号与固定状态 -->
  <div class="page special-apply">
    <a-card title="特价/项目申请（占位）" :bordered="false">
      <a-alert type="warning" show-icon style="margin-bottom: 8px"
               message="该功能为留桩实现：仅回传占位申请号，实际审批流程由 OA/AI 在后续接入" />

      <a-form :model="form" layout="vertical" @submit.prevent>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="关联订单" name="orderId" required>
              <OrderSelect v-model="form.orderId" placeholder="按订单号检索选择" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="项目名称" name="projectName" required>
              <a-input v-model:value="form.projectName" placeholder="如：XX项目-特价申请" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="申请原因" name="reason">
          <a-textarea v-model:value="form.reason" :rows="3" />
        </a-form-item>

        <a-divider>申请明细</a-divider>
        <a-table :data-source="form.lines" :pagination="false" row-key="idx">
          <a-table-column title="#" :customRender="({index}) => index! + 1" width="60" />
          <a-table-column title="产品" width="280">
            <template #default="{ record }">
              <ProductSelect v-model="record.productId" placeholder="按编码/名称选择产品" />
            </template>
          </a-table-column>
          <a-table-column title="目标单价" width="200">
            <template #default="{ record }">
              <a-input-number v-model:value="record.targetUnitPrice" :min="0" :step="0.01" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="备注">
            <template #default="{ record }">
              <a-input v-model:value="record.remark" placeholder="可选" />
            </template>
          </a-table-column>
          <a-table-column title="操作" width="120">
            <template #default="{ record }">
              <a-button type="link" danger @click="removeLine(record)">删除</a-button>
            </template>
          </a-table-column>
        </a-table>
        <div style="margin-top:8px">
          <a-button type="dashed" @click="addLine">新增明细</a-button>
        </div>

        <a-divider />
        <a-space>
          <a-button type="primary" :loading="loading" @click="onSubmit">提 交</a-button>
          <a-button @click="$router.back()">返 回</a-button>
        </a-space>
      </a-form>

      <a-result v-if="result" status="info" title="申请已提交（占位）" style="margin-top: 16px">
        <template #subTitle>
          申请号：{{ result.applyId }}，状态：{{ result.status }}，提示：{{ result.message }}
        </template>
      </a-result>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 特价/项目申请留桩前端页
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { applySpecial, type SpecialApplyInput, type SpecialApplyResult } from '@/services/specials';
import ProductSelect from '@/components/select/ProductSelect.vue'
import OrderSelect from '@/components/select/OrderSelect.vue'

const form = ref<SpecialApplyInput>({ orderId: '', projectName: '', reason: '', lines: [] });
const loading = ref(false);
const result = ref<SpecialApplyResult>();

function addLine() { form.value.lines.push({ productId: '', targetUnitPrice: 0 }); }
function removeLine(row: any) {
  const idx = form.value.lines.indexOf(row);
  if (idx >= 0) form.value.lines.splice(idx, 1);
}

async function onSubmit() {
  if (!form.value.orderId || !form.value.projectName || form.value.lines.length === 0) {
    message.error('请完善订单/项目与至少一条明细');
    return;
  }
  loading.value = true;
  try {
    result.value = await applySpecial(form.value);
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.special-apply { max-width: 1200px; margin: 0 auto; }
</style>
