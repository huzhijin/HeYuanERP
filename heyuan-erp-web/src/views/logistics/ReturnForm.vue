<template>
  <!-- 退货单创建页：选择订单（可选来源送货单），编辑退货明细 -->
  <div class="page return-form">
    <a-card title="新建退货单" :bordered="false">
      <a-form :model="form" layout="vertical" @submit.prevent>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="来源订单" name="orderId" required>
              <OrderSelect v-model="form.orderId" placeholder="按订单号检索选择" @select="onOrderSelected" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="来源送货单（可选）" name="sourceDeliveryId">
              <DeliverySelect v-model="form.sourceDeliveryId" :orderNo="orderNo" placeholder="按送货单号检索选择（可选）" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="退货日期" name="returnDate" required>
              <a-date-picker v-model:value="form.returnDate" value-format="YYYY-MM-DD" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-divider>退货明细</a-divider>
        <a-table :data-source="form.lines" :pagination="false" row-key="id">
          <a-table-column title="#" :customRender="({index}) => index! + 1" width="60" />
          <a-table-column title="产品Id" width="260">
            <template #default="{ record }">
              <a-input v-model:value="record.productId" placeholder="产品 Id" />
            </template>
          </a-table-column>
          <a-table-column title="数量" width="160">
            <template #default="{ record }">
              <a-input-number v-model:value="record.qty" :min="0" :step="1" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="原因" >
            <template #default="{ record }">
              <a-input v-model:value="record.reason" placeholder="可选" />
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
          <a-button type="primary" :loading="saving" @click="onSubmit">提 交</a-button>
          <a-button @click="$router.back()">返 回</a-button>
        </a-space>
      </a-form>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 创建退货单：基础表单 + 明细编辑，提交到 /api/Returns
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { createReturn } from '@/services/returns';
import { getOrder } from '@/services/orders'
import type { ReturnCreateInput, OrderDetail } from '@/types/order';
import OrderSelect from '@/components/select/OrderSelect.vue'
import DeliverySelect from '@/components/select/DeliverySelect.vue'

const form = ref<ReturnCreateInput>({
  orderId: '',
  sourceDeliveryId: '',
  returnDate: undefined,
  lines: [],
});

const saving = ref(false);
const orderNo = ref<string>('')

function addLine() {
  form.value.lines.push({ productId: '', qty: 1 });
}
function removeLine(row: any) {
  const idx = form.value.lines.indexOf(row);
  if (idx >= 0) form.value.lines.splice(idx, 1);
}

async function onSubmit() {
  if (!form.value.orderId || !form.value.lines.length) {
    message.error('请填写订单与至少一条明细');
    return;
  }
  saving.value = true;
  try {
    const created = await createReturn(form.value);
    message.success(`创建成功：${created.returnNo}`);
  } finally {
    saving.value = false;
  }
}

async function onOrderSelected(row?: OrderDetail | any) {
  orderNo.value = ''
  if (!form.value.orderId) return
  try {
    const od: OrderDetail = await getOrder(form.value.orderId)
    orderNo.value = od.orderNo
    // 重置来源送货单选择
    form.value.sourceDeliveryId = ''
  } catch {}
}
</script>

<style scoped>
.return-form { max-width: 1200px; margin: 0 auto; }
</style>
