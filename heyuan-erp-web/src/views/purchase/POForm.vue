<template>
  <!-- 采购单表单：新建/编辑（行编辑支持增删改） -->
  <div class="page po-form">
    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-space>
        <a-button @click="goList">返回列表</a-button>
        <a-button type="primary" @click="onSubmit" :loading="submitting">保存</a-button>
      </a-space>
    </a-card>

    <a-card :bordered="false" title="采购单信息">
      <a-form layout="vertical" :model="form">
        <a-row :gutter="16">
          <a-col :span="6">
            <a-form-item label="供应商" required>
              <a-input v-model:value="form.vendorId" placeholder="输入供应商" />
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="采购日期">
              <a-date-picker v-model:value="poDate" value-format="YYYY-MM-DD" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="备注">
              <a-input v-model:value="form.remark" placeholder="备注" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <a-card :bordered="false" title="行明细" style="margin-top: 12px">
      <a-space style="margin-bottom: 8px">
        <a-button type="dashed" @click="addLine">新增行</a-button>
      </a-space>
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
        <a-table-column key="unitPrice" title="单价" width="150">
          <template #default="{ record }">
            <a-input-number v-model:value="record.unitPrice" :min="0" :precision="4" style="width: 100%" />
          </template>
        </a-table-column>
        <a-table-column key="amount" title="金额" width="160">
          <template #default="{ record }">{{ (Number(record.qty||0) * Number(record.unitPrice||0)).toFixed(2) }}</template>
        </a-table-column>
        <a-table-column key="action" title="操作" width="120">
          <template #default="{ record, index }">
            <a-space>
              <a-button type="link" danger @click="removeLine(index)">删除</a-button>
            </a-space>
          </template>
        </a-table-column>
      </a-table>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 表单页面：复用同一组件用于新建与编辑
import { computed, onMounted, reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { getPO, createPO, updatePO } from '@/services/pos';
import type { POCreateInput, POUpdateInput, PODetail, POLineUpdateInput } from '@/types/purchase';
import ProductSelect from '@/components/select/ProductSelect.vue'

const router = useRouter();
const route = useRoute();
const id = computed(() => route.params.id as string | undefined);

// 表单状态
const form = reactive<POCreateInput>({ vendorId: '', remark: '', lines: [] });
const poDate = ref<string | undefined>();
const lines = ref<(POLineUpdateInput & { _key: string })[]>([]);
const submitting = ref(false);

function addLine() {
  lines.value.push({ _key: Math.random().toString(36).slice(2), productId: '', qty: 0, unitPrice: 0 });
}
function removeLine(index: number) {
  const item = lines.value[index];
  if (item.id) {
    item._deleted = true; // 编辑模式：标记删除
  }
  lines.value.splice(index, 1);
}

async function loadDetail(poId: string) {
  const detail = await getPO(poId);
  form.vendorId = detail.vendorId;
  form.remark = detail.remark || '';
  poDate.value = detail.poDate?.slice(0, 10);
  lines.value = detail.lines.map(l => ({ _key: l.id, id: l.id, productId: l.productId, qty: l.qty, unitPrice: l.unitPrice }));
}

async function onSubmit() {
  if (!form.vendorId) { message.error('请填写供应商'); return; }
  if (lines.value.length === 0) { message.error('至少需要一条行'); return; }
  submitting.value = true;
  try {
    const bodyLines = lines.value.map(l => ({ id: l.id, productId: (l.productId||'').trim(), qty: Number(l.qty||0), unitPrice: Number(l.unitPrice||0), _deleted: l._deleted }));
    if (!id.value) {
      const body: POCreateInput = { vendorId: form.vendorId.trim(), poDate: poDate.value, remark: form.remark?.trim() || undefined, lines: bodyLines.map(({ id, _deleted, ...x }) => x) };
      await createPO(body);
      message.success('创建成功');
    } else {
      const body: POUpdateInput = { vendorId: form.vendorId.trim(), poDate: poDate.value, remark: form.remark?.trim() || undefined, lines: bodyLines };
      await updatePO(id.value, body);
      message.success('保存成功');
    }
    goList();
  } finally {
    submitting.value = false;
  }
}

function goList() { router.push('/purchase/po'); }

onMounted(() => { if (id.value) loadDetail(id.value); else addLine(); });
</script>

<style scoped>
.po-form { }
</style>
