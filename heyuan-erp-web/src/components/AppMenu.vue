<template>
  <!-- 通用侧边菜单组件：根据传入的菜单项渲染，抛出选择事件 -->
  <a-menu :selectedKeys="selectedKeys" theme="dark" mode="inline" @click="onClick">
    <template v-for="item in items" :key="item.key">
      <a-menu-item v-if="!item.children || item.children.length === 0" :key="item.key">
        <template #icon>
          <slot name="icon" :item="item" />
        </template>
        {{ item.title }}
      </a-menu-item>
      <a-sub-menu v-else :key="item.key">
        <template #title>
          <span>{{ item.title }}</span>
        </template>
        <a-menu-item v-for="c in item.children" :key="c.key">{{ c.title }}</a-menu-item>
      </a-sub-menu>
    </template>
  </a-menu>
</template>

<script setup lang="ts">
export interface MenuItem {
  key: string;
  title: string;
  children?: MenuItem[];
}

const props = defineProps<{ items: MenuItem[]; selectedKeys?: string[] }>();
const emit = defineEmits<{ (e: 'select', key: string): void }>();

const selectedKeys = computed(() => props.selectedKeys || []);

function onClick(info: { key: string }) {
  emit('select', info.key);
}
</script>

