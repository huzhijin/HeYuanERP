<template>
  <div class="dashboard">
    <a-row :gutter="16">
      <a-col :span="8">
        <a-card title="快捷入口" size="small">
          <a-space wrap>
            <a-button type="primary">新建订单</a-button>
            <a-button>客户列表</a-button>
            <a-button>库存查询</a-button>
          </a-space>
        </a-card>
      </a-col>
      <a-col :span="16">
        <a-card title="公告与提醒" size="small">
          <a-empty description="暂无公告" />
        </a-card>
      </a-col>
    </a-row>
  </div>
  
</template>

<script setup lang="ts">
// 工作台占位页
</script>

<style scoped>
.dashboard :deep(.ant-card) { margin-bottom: 16px; }
</style>

