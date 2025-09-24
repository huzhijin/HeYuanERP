<template>
  <!-- 通用报表查询表单容器（中文注释）
       用法：
       <ReportFilterForm @search="..." @reset="...">
         <template #fields>...你的表单项...</template>
         <template #actions>...你的按钮...</template>
       </ReportFilterForm>
  -->
  <a-form layout="inline" @submit.prevent>
    <slot name="fields"></slot>
    <a-form-item>
      <a-space>
        <a-button type="primary" @click="$emit('search')" :loading="loading">查询</a-button>
        <a-button @click="$emit('reset')" :disabled="loading">重置</a-button>
        <slot name="actions"></slot>
      </a-space>
    </a-form-item>
  </a-form>
</template>

<script setup lang="ts">
// 说明：仅提供布局与交互骨架，实际表单项由父组件通过插槽传入。
defineProps<{ loading?: boolean }>()
defineEmits<{ (e: 'search'): void; (e: 'reset'): void }>()
</script>

<style scoped>
</style>

