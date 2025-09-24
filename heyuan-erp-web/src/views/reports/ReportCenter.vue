<template>
  <a-card title="报表中心" :bordered="false">
    <a-row :gutter="16">
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/advanced')" title="高级报表">
          <p>产品销售分析、报价单状态分布等综合分析</p>
        </a-card>
      </a-col>
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/sales-stat')" title="销售统计">
          <p>按客户/产品/业务员维度的销售统计</p>
        </a-card>
      </a-col>
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/invoice-stat')" title="发票统计">
          <p>按日期范围统计发票数量与金额</p>
        </a-card>
      </a-col>
    </a-row>
    <a-row :gutter="16" style="margin-top: 16px">
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/po-query')" title="采购订单查询">
          <p>采购订单条件查询与打印导出</p>
        </a-card>
      </a-col>
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/inventory')" title="库存报表">
          <p>库存汇总与交易明细报表</p>
        </a-card>
      </a-col>
      <a-col :span="8">
        <a-card hoverable @click="$router.push('/reports/snapshots')" title="快照历史">
          <p>查看历史报表快照与导出记录</p>
        </a-card>
      </a-col>
    </a-row>
  </a-card>
</template>

<script setup lang="ts">
</script>

<style scoped></style>
