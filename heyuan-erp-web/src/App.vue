<template>
  <!-- 应用根组件：放置全局配置与路由出口 -->
  <a-config-provider :locale="zhCN">
    <router-view />
  </a-config-provider>
  <!-- 说明：页面布局由路由对应的视图组件（如 BasicLayout）提供 -->
  <!-- 后续可在此放置全局反馈组件（消息/通知/全局 Spin）等 -->
  
</template>

<script setup lang="ts">
// Ant Design Vue 中文本地化
import zhCN from 'ant-design-vue/es/locale/zh_CN';
</script>

<style>
html, body, #app {
  height: 100%;
  margin: 0;
}
</style>

