<template>
  <a-card title="报价单" :bordered="false">
    <template #extra>
      <a-space>
        <a-input v-model:value="qno" placeholder="报价单号" style="width: 200px" />
        <a-button type="primary" @click="search">按编号查询</a-button>
        <a-select v-model:value="status" style="width: 180px" placeholder="按状态加载">
          <a-select-option value="Draft">Draft</a-select-option>
          <a-select-option value="Sent">Sent</a-select-option>
          <a-select-option value="Accepted">Accepted</a-select-option>
          <a-select-option value="Rejected">Rejected</a-select-option>
          <a-select-option value="Expired">Expired</a-select-option>
        </a-select>
        <a-button @click="loadByStatus">加载列表</a-button>
      </a-space>
    </template>
    <a-table v-if="rows.length" row-key="id" :data-source="rows" :pagination="false">
      <a-table-column title="编号" data-index="quotationNumber" />
      <a-table-column title="标题" data-index="quotationTitle" />
      <!-- 隐藏客户ID列，避免前台展示内部标识 -->
      <a-table-column title="日期" data-index="quoteDate" width="140" />
      <a-table-column title="有效期" data-index="validUntil" width="140" />
      <a-table-column title="状态" data-index="status" width="120" />
      <a-table-column title="金额" data-index="totalAmount" width="140" />
      <a-table-column title="操作" width="200">
        <template #default="{ record }">
          <a-space>
            <a-button size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确定删除？" @confirm="() => doDelete(record.id)"><a-button danger size="small">删除</a-button></a-popconfirm>
          </a-space>
        </template>
      </a-table-column>
    </a-table>
    <a-empty v-else description="请输入报价单号查询" />

    <a-divider />
    <a-button type="primary" @click="openCreate">新建报价</a-button>

    <a-modal v-model:open="modalOpen" :title="isEdit?'编辑报价':'新建报价'" @ok="submit" :confirmLoading="saving">
      <a-form layout="vertical">
        <a-form-item label="编号"><a-input v-model:value="form.quotationNumber"/></a-form-item>
        <a-form-item label="标题"><a-input v-model:value="form.quotationTitle"/></a-form-item>
        <a-form-item label="客户" required><AccountSelect v-model="(form as any).accountId" /></a-form-item>
        <a-form-item label="报价日期" required><a-date-picker v-model:value="form.quoteDate" valueFormat="YYYY-MM-DD" style="width:100%"/></a-form-item>
        <a-form-item label="有效期至"><a-date-picker v-model:value="form.validUntil" valueFormat="YYYY-MM-DD" style="width:100%"/></a-form-item>
        <a-form-item label="状态"><a-input v-model:value="form.status" placeholder="Draft/Sent/Accepted..."/></a-form-item>
        <a-form-item label="金额"><a-input-number v-model:value="form.totalAmount" :min="0" :step="100" style="width:100%"/></a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { getQuotationByNumber, createQuotation, updateQuotation, deleteQuotation, type Quotation } from '@/api/modules/productPrice'
import AccountSelect from '@/components/select/AccountSelect.vue'

const qno = ref('')
const status = ref<string>('')
const rows = ref<Quotation[]>([])
const modalOpen = ref(false)
const isEdit = ref(false)
const saving = ref(false)
const editingId = ref<number | null>(null)
const form = ref<any>({ quotationNumber: '', quotationTitle: '', accountId: undefined, quoteDate: undefined, validUntil: undefined, status: 'Draft', totalAmount: 0 })

async function search() {
  if (!qno.value) { rows.value = []; return }
  try {
    const q = await getQuotationByNumber(qno.value)
    rows.value = q ? [q] : []
    if (!q) message.info('未找到该报价单')
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  }
}

async function loadByStatus() {
  if (!status.value) { rows.value = []; return }
  try {
    const list: any = await http.get(`/api/ProductPrice/quotations/by-status/${encodeURIComponent(status.value)}`)
    rows.value = (list?.data || list || []) as Quotation[]
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  }
}

function openCreate(){ isEdit.value=false; editingId.value=null; form.value={ quotationNumber:'', quotationTitle:'', accountId:undefined, quoteDate:undefined, status:'Draft', totalAmount:0 }; modalOpen.value=true }
function openEdit(rec: Quotation){ isEdit.value=true; editingId.value=rec.id; form.value={ ...rec }; modalOpen.value=true }
async function submit(){
  saving.value=true
  try{
    if(!form.value.accountId || !form.value.quoteDate){ message.warning('请填写客户与报价日期'); saving.value=false; return }
    if(isEdit.value && editingId.value!=null) await updateQuotation(editingId.value, form.value)
    else await createQuotation(form.value)
    message.success('保存成功'); modalOpen.value=false; if(form.value.quotationNumber) { qno.value=form.value.quotationNumber; await search() } else rows.value=[]
  }catch(e:any){ message.error(e?.message||'保存失败') }finally{ saving.value=false }
}
async function doDelete(id:number){ try{ await deleteQuotation(id); message.success('已删除'); rows.value=[] }catch(e:any){ message.error(e?.message||'删除失败') } }
</script>

<style scoped></style>
