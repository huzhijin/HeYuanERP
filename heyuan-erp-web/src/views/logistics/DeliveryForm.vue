<template>
  <!-- 送货单创建页：选择来源订单，编辑送货明细，提交创建 -->
  <div class="page delivery-form">
    <a-card title="新建送货单" :bordered="false">
      <a-form :model="form" layout="vertical" @submit.prevent>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="来源订单" name="orderId" required>
              <OrderSelect v-model="form.orderId" placeholder="按订单号检索选择" @select="onOrderSelected" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="送货日期" name="deliveryDate" required>
              <a-date-picker v-model:value="form.deliveryDate" value-format="YYYY-MM-DD" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-divider>送货明细</a-divider>
        <a-table :data-source="form.lines" :pagination="false" row-key="id">
          <a-table-column title="#" :customRender="({index}) => index! + 1" width="60" />
          <a-table-column title="产品Id" width="220">
            <template #default="{ record }">
              <a-input v-model:value="record.productId" placeholder="产品 Id" />
            </template>
          </a-table-column>
          <a-table-column title="对应订单行（可选）" width="260">
            <template #default="{ record }">
              <a-select
                v-model:value="record.orderLineId"
                :options="orderLineOptions"
                :disabled="orderLineOptions.length===0"
                allow-clear
                placeholder="选择订单行（可选）"
                style="width: 100%"
              />
            </template>
          </a-table-column>
          <a-table-column title="数量" width="160">
            <template #default="{ record }">
              <a-input-number v-model:value="record.qty" :min="0" :step="1" style="width: 100%" />
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
// 创建送货单：基础表单 + 明细编辑，提交到 /api/Deliveries
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { createDelivery } from '@/services/deliveries';
import { getOrder } from '@/services/orders'
import type { DeliveryCreateInput, OrderDetail } from '@/types/order';
import OrderSelect from '@/components/select/OrderSelect.vue'

const form = ref<DeliveryCreateInput>({
  orderId: '',
  deliveryDate: undefined,
  lines: [],
});

const saving = ref(false);
const orderLineOptions = ref<{ label: string; value: string }[]>([])

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
    const created = await createDelivery(form.value);
    message.success(`创建成功：${created.deliveryNo}`);
  } finally {
    saving.value = false;
  }
}

async function onOrderSelected(row?: OrderDetail | any) {
  orderLineOptions.value = []
  if (!form.value.orderId) return
  try {
    const od: OrderDetail = await getOrder(form.value.orderId)
    orderLineOptions.value = (od.lines || []).map((l:any, idx:number) => ({
      label: `#${idx+1} ${l.productId} x ${l.qty}`,
      value: l.id,
    }))
  } catch {}
}
</script>

<style scoped>
.delivery-form { max-width: 1200px; margin: 0 auto; }
</style>
