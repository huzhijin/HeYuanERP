// 应用入口：挂载 Vue 实例，注册 Pinia、Router 与 Ant Design Vue
import { createApp } from 'vue';
import { createPinia } from 'pinia';
import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/reset.css';

import App from './App.vue';
import router from './router';

const app = createApp(App);

// 全局状态管理与路由
app.use(createPinia());
app.use(router);

// UI 组件库
app.use(Antd);

// 挂载
app.mount('#app');

