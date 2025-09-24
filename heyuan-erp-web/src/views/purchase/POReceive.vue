<template>
  <!-- 采购收货：选择收货日期、备注与收货行（产品/数量/仓库/库位） -->
  <div class="page po-receive">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-space>
        <a-button @click="goPO">返回采购单</a-button>
        <a-button type="primary" @click="onSubmit" :loading="submitting">提交收货</a-button>
      </a-space>
    </a-card>

    <a-card :bordered="false" title="收货信息">
      <a-descriptions bordered size="small">
        <a-descriptions-item label="采购单号" :span="1">{{ header.poNo || '-' }}</a-descriptions-item>
        <a-descriptions-item label="供应商" :span="1">{{ header.vendorId || '-' }}</a-descriptions-item>
        <a-descriptions-item label="状态" :span="1">{{ header.status || '-' }}</a-descriptions-item>
      </a-descriptions>

      <a-form layout="inline" style="margin-top: 12px">
        <a-form-item label="收货日期">
          <a-date-picker v-model:value="receiveDate" value-format="YYYY-MM-DD" />
        </a-form-item>
        <a-form-item label="备注">
          <a-input v-model:value="remark" style="width: 360px" />
        </a-form-item>
        <a-form-item label="默认仓库">
          <a-input v-model:value="defaultWhse" placeholder="应用到新行" style="width: 200px" />
        </a-form-item>
        <a-space>
          <a-button @click="addLine">新增行</a-button>
          <a-button @click="fillFromPO">按PO行生成</a-button>
        </a-space>
      </a-form>
    </a-card>

    <a-card :bordered="false" title="收货行" style="margin-top: 12px">
      <a-table :data-source="lines" row-key="_key" :pagination="false">
        <a-table-column key="productId" title="产品" width="280">
          <template #default="{ record }">
            <ProductSelect v-model="record.productId" placeholder="按编码/名称选择产品" />
          </template>
        </a-table-column>
        <a-table-column key="qty" title="数量" width="150">
          <template #default="{ record }">
            <a-input-number v-model:value="record.qty" :min="0" :precision="4" style="width: 100%" />
          </template>
        </a-table-column>
        <a-table-column key="whse" title="仓库" width="160">
          <template #default="{ record }">
            <a-input v-model:value="record.whse" placeholder="仓库" />
          </template>
        </a-table-column>
        <a-table-column key="loc" title="库位" width="160">
          <template #default="{ record }">
            <a-input v-model:value="record.loc" placeholder="库位(可选)" />
          </template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="120">
          <template #default="{ index }">
            <a-button type="link" danger @click="removeLine(index)">删除</a-button>
          </template>
        </a-table-column>
      </a-table>
    </a-card>

    <a-card v-if="resultId" :bordered="false" style="margin-top: 12px" title="收货结果">
      <a-space>
        <span>收货单已创建：{{ resultId }}</span>
        <a-button type="link" @click="openPrintModel">查看打印模型</a-button>
      </a-space>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 收货表单：根据 PO 创建收货入库，提交后展示打印模型入口
import { computed, onMounted, reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { getPO, receivePO } from '@/services/pos';
import type { PODetail, POReceiveCreateInput, POReceiveLineCreateInput } from '@/types/purchase';
import ProductSelect from '@/components/select/ProductSelect.vue'

const route = useRoute();
const router = useRouter();
const poId = computed(() => route.params.id as string);

const header = reactive<Partial<PODetail>>({});
const receiveDate = ref<string | undefined>();
const remark = ref<string | undefined>('');
const defaultWhse = ref<string>('');
const lines = ref<(POReceiveLineCreateInput & { _key: string })[]>([]);
const submitting = ref(false);
const resultId = ref<string>('');

function addLine() {
  lines.value.push({ _key: Math.random().toString(36).slice(2), productId: '', qty: 0, whse: defaultWhse.value || '', loc: '' });
}
function removeLine(index: number) { lines.value.splice(index, 1); }

async function loadPO() {
  const detail = await getPO(poId.value);
  Object.assign(header, detail);
}

function fillFromPO() {
  if (!header.lines || header.lines.length === 0) { message.warning('PO 无行可复制'); return; }
  lines.value = header.lines.map(l => ({ _key: l.id, productId: l.productId, qty: l.qty, whse: defaultWhse.value || '', loc: '' }));
}

async function onSubmit() {
  if (lines.value.length === 0) { message.error('至少需要一条收货行'); return; }
  if (lines.value.some(l => !l.productId || !l.whse || !Number(l.qty))) { message.error('请完整填写产品/数量/仓库'); return; }
  submitting.value = true;
  try {
    const body: POReceiveCreateInput = {
      receiveDate: receiveDate.value,
      remark: remark.value || undefined,
      lines: lines.value.map(l => ({ productId: l.productId.trim(), qty: Number(l.qty||0), whse: (l.whse||'').trim(), loc: l.loc?.trim() || undefined }))
    };
    const resp = await receivePO(poId.value, body);
    resultId.value = resp.id;
    message.success('收货成功');
  } finally {
    submitting.value = false;
  }
}

function goPO() { router.push(`/purchase/po/${poId.value}`); }
function openPrintModel() {
  const base = (import.meta as any).env.VITE_API_BASE || '';
  const rid = resultId.value;
  if (!rid) return;
  window.open(`${base}/api/print/po-receive/${rid}/model`, '_blank');
}

onMounted(loadPO);
</script>

<style scoped>
.po-receive { }
</style>
