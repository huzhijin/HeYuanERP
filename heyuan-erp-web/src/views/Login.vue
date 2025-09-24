<template>
  <!-- 登录页：输入账号与密码，调用后端登录接口，保存 Token 与用户信息到 Pinia，并跳转 -->
  <div class="login-page">
    <a-card class="login-card" :bordered="false">
      <h2 class="title">{{ appName }} 登录</h2>
      <a-form :model="form" @submit.prevent>
        <a-form-item label="账号" required>
          <a-input v-model:value="form.loginId" placeholder="请输入账号，如 admin" allow-clear />
        </a-form-item>
        <a-form-item label="密码" required>
          <a-input-password v-model:value="form.password" placeholder="请输入密码" allow-clear />
        </a-form-item>
        <a-form-item>
          <a-space style="width: 100%" direction="vertical">
            <a-button type="primary" :loading="loading" block @click="onSubmit">登录</a-button>
            <small class="hint">开发环境默认种子：账号 admin，密码 CHANGE_ME（如已修改请用实际值）</small>
          </a-space>
        </a-form-item>
      </a-form>
    </a-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { login } from '@/services/auth';
import { useAuthStore } from '@/store/auth';

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const appName = import.meta.env.VITE_APP_NAME || 'HeYuanERP';

const form = reactive({
  loginId: '',
  password: '',
});
const loading = ref(false);

async function onSubmit() {
  if (!form.loginId || !form.password) {
    message.warning('请输入账号与密码');
    return;
  }
  loading.value = true;
  try {
    const res = await login({ loginId: form.loginId, password: form.password });
    // 仅保存原始 JWT，utils/request 会自动加上 Bearer 前缀
    auth.setToken(res.token);
    auth.setUser(res.user, res.roles);
    message.success('登录成功');
    const redirect = (route.query.redirect as string) || '/';
    router.replace(redirect);
  } catch (err: any) {
    message.error(err?.message || '登录失败');
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f6f7;
}
.login-card {
  width: 360px;
}
.title {
  text-align: center;
  margin-bottom: 12px;
}
.hint { color: #999; }
</style>

