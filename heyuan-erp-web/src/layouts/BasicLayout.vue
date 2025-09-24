<template>
  <!-- 基础布局：头部 + 侧边菜单 + 内容区域 -->
  <a-layout class="layout-root">
    <a-layout-header class="layout-header">
      <div class="brand" @click="$router.push('/')">{{ appName }}</div>
      <div class="spacer" />
      <a-space>
        <a-tooltip title="占位：通知">
          <a-button shape="circle" size="small">🔔</a-button>
        </a-tooltip>
        <a-dropdown>
          <a-avatar style="background:#1677ff; cursor: pointer">U</a-avatar>
          <template #overlay>
            <a-menu @click="onMenuClick">
              <a-menu-item key="profile">个人信息</a-menu-item>
              <a-menu-item key="logout">退出登录</a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
      </a-space>
    </a-layout-header>

    <a-layout>
      <a-layout-sider :collapsed="collapsed" collapsible @collapse="collapsed = $event" class="layout-sider">
        <div class="menu-title" v-if="!collapsed">功能菜单</div>
        <a-menu v-model:selectedKeys="selectedKeys" theme="dark" mode="inline" @click="onMenuSelect">
          <a-menu-item key="dashboard">工作台</a-menu-item>
          <a-menu-item key="accounts">客户</a-menu-item>
          <a-menu-item key="orders">订单</a-menu-item>
          <a-menu-item key="purchase">采购</a-menu-item>
          <a-menu-item key="inventory">库存</a-menu-item>
          <a-menu-item key="invoices">发票</a-menu-item>
          <a-menu-item key="payments">收款</a-menu-item>
          <a-menu-item key="finance">财务</a-menu-item>
          <a-menu-item key="crm">CRM</a-menu-item>
          <a-menu-item key="product">产品价格</a-menu-item>
          <a-menu-item key="reports">报表</a-menu-item>
          <a-sub-menu key="admin">
            <template #title>系统管理</template>
            <a-menu-item key="users">用户</a-menu-item>
            <a-menu-item key="roles">角色</a-menu-item>
          </a-sub-menu>
        </a-menu>
      </a-layout-sider>

      <a-layout-content class="layout-content">
        <router-view />
        <!-- 默认欢迎占位（当无子路由时显示） -->
        <div v-if="$route.path === '/'" class="welcome">
          <a-typography-title :level="3">欢迎使用 HeYuanERP</a-typography-title>
          <a-typography-text type="secondary">本页面为基础布局占位。功能页面将在后续批次补充。</a-typography-text>
        </div>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/store/auth';

const router = useRouter();
const auth = useAuthStore();

const appName = computed(() => import.meta.env.VITE_APP_NAME || 'HeYuanERP');
const collapsed = ref(false);
const selectedKeys = ref<string[]>(['dashboard']);

function onMenuSelect(info: { key: string }) {
  selectedKeys.value = [info.key];
  // 简单路由映射，后续替换为实际页面
  const map: Record<string, string> = {
    dashboard: '/',
    accounts: '/accounts',
    orders: '/orders',
    purchase: '/purchase/po',
    inventory: '/inventory/summary',
    invoices: '/invoices',
    payments: '/payments',
    reports: '/reports',
    crm: '/crm/opportunities',
    product: '/product/products',
    finance: '/finance/dashboard',
  };
  const path = map[info.key];
  if (path) router.push(path);
}

function onMenuClick({ key }: { key: string }) {
  if (key === 'logout') {
    auth.clear();
    router.push('/login');
  }
}
</script>

<style scoped>
.layout-root {
  min-height: 100vh;
}
.layout-header {
  display: flex;
  align-items: center;
  padding: 0 16px;
}
.brand {
  color: #fff;
  font-weight: 600;
  font-size: 16px;
}
.spacer { flex: 1; }
.layout-sider { min-height: calc(100vh - 64px); }
.menu-title {
  color: #9fb3c8;
  padding: 12px 16px;
}
.layout-content { padding: 16px; }
.welcome {
  background: #fff;
  padding: 24px;
  border-radius: 8px;
}
</style>
